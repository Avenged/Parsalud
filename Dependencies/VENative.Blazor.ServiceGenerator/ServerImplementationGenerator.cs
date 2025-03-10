﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VENative.Blazor.ServiceGenerator.Extensions;
using VENative.Blazor.ServiceGenerator.Helpers;

namespace VENative.Blazor.ServiceGenerator;

[Generator]
public class ServerImplementationGenerator : IIncrementalGenerator
{
    const string TASK = "System.Threading.Tasks.Task";
    const string CANCELLATION_TOKEN = "System.Threading.CancellationToken";
    const string SERVER_ATTRIBUTE = "VENative.Blazor.ServiceGenerator.Attributes.GenerateHubAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DEBUG
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}
#endif

        var compilationProvider = context.CompilationProvider;

        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsClassWithAttributes(node),
                transform: static (context, _) => GetClassWithServerHubAttribute(context))
            .Where(static symbol => symbol is not null)
            .Collect();

        var combined = compilationProvider.Combine(classDeclarations);
        context.RegisterSourceOutput(combined, (spc, source) => Execute(spc, source.Left, source.Right));
    }

    private static bool IsClassWithAttributes(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclaration && classDeclaration.AttributeLists.Count > 0;
    }

    private static INamedTypeSymbol? GetClassWithServerHubAttribute(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var semanticModel = context.SemanticModel;
        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

        if (classSymbol?.GetAttributes().Any(ad => ad.AttributeClass?.ToDisplayString() == SERVER_ATTRIBUTE) == true)
        {
            return classSymbol;
        }
        return null;
    }

    private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<INamedTypeSymbol?> classes)
    {
        if (classes.IsDefaultOrEmpty)
            return;

        var serverHubAttributeSymbol = compilation.GetTypeByMetadataName(SERVER_ATTRIBUTE);
        var hubSymbol = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.SignalR.Hub");

        if (serverHubAttributeSymbol is null)
        {
            ReportError(context, "SG0001", $"Attribute '{SERVER_ATTRIBUTE}' not found.");
            return;
        }

        if (hubSymbol is null)
        {
            ReportError(context, "SG0001", "The class 'Microsoft.AspNetCore.SignalR.Hub' could not be found. Make sure your project references the necessary packages.");
            return;
        }

        foreach (var classSymbol in classes)
        {
            if (classSymbol is null) continue;

            var errors = 0;

            var attributeData = classSymbol.GetAttributes()
                .First(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, serverHubAttributeSymbol));

            var @namespace = (string?)attributeData.ConstructorArguments[0].Value;
            if (string.IsNullOrWhiteSpace(@namespace))
            {
                @namespace = classSymbol.ContainingNamespace.ToDisplayString();
            }

            var @route = (string?)attributeData.ConstructorArguments[1].Value;
            if (string.IsNullOrWhiteSpace(@route))
            {
                @route = $"/I{classSymbol.Name}";
            }

            var @useAuthentication = (bool)attributeData.ConstructorArguments[2].Value!;

            var source = GenerateProxyClass(context, classSymbol, @namespace!, @route!, @useAuthentication, ref errors);

            if (errors == 0)
            {
                context.AddSource($"Proxy{classSymbol.Name}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
    }

    private static string GenerateProxyClass(SourceProductionContext context, INamedTypeSymbol classSymbol, string @namespace, string @route, bool useAuthentication, ref int errors)
    {
        var className = classSymbol.Name;
        var proxyClassName = $"Proxy{className}";
        var classAttributes = string.Join("\n", classSymbol.GetAttributesByNames(["AuthorizeAttribute", "AllowAnonymous"])
            .Select(x => x.GetAttributeInfo()));
        var interfaceDisplayString = GetFirstImplementedInterfaceName(classSymbol);
        var hasInitializeMethod = HasInitializeMethod(classSymbol);

        var source =
        $$"""
        // <auto-generated>
        // This file was generated by the ServerImplementationGenerator.
        // Any changes made to this file will be overwritten.
        // </auto-generated>

        using System;
        using System.Threading.Tasks;
        using Microsoft.AspNetCore.SignalR;
        using Microsoft.AspNetCore.Components;
        using Microsoft.AspNetCore.Authorization;
        using System.Runtime.CompilerServices;
        using Microsoft.AspNetCore.Http;

        #nullable enable

        namespace {{@namespace}};

        {{classAttributes}}
        [Route("{{@route}}")]
        public sealed class {{proxyClassName}} : Hub
        {
            private readonly {{interfaceDisplayString}} _implementation;

            public {{proxyClassName}}({{interfaceDisplayString}} implementation)
            {
                this._implementation = implementation;
            }

            private void OnInitialized(HubCallerContext? context)
            {
                if (context is null) 
                {
                    return;
                }

                {{(hasInitializeMethod ? $"(({classSymbol.ToDisplayString()})_implementation).OnInitialized(context);" : null)}}
            }

            private void EnsureAuthenticated(HubCallerContext? context)
            {
                if (context is null) 
                {
                    return;
                }

                var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
                if (!isAuthenticated)
                {
                    throw new InvalidOperationException("The user is not authenticated.");
                }
            }

            {{GenerateMethods(context, classSymbol, useAuthentication, ref errors)}}
        }
        """;

        return source;
    }

    private static bool HasInitializeMethod(INamedTypeSymbol classSymbol)
    {
        return classSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Any(m => m.Name == "OnInitialized");
    }

    private static string GetFirstImplementedInterfaceName(INamedTypeSymbol typeSymbol)
    {
        var firstInterface = typeSymbol.AllInterfaces.FirstOrDefault();
        return firstInterface?.ToDisplayString() ?? string.Empty;
    }

    private static string GenerateMethods(SourceProductionContext context, INamedTypeSymbol classSymbol, bool useAuthentication, ref int errors)
    {
        HashSet<string> overridenMehodNames = [];
        HashSet<string> usedMehodNames = [];
        var methodsBuilder = new StringBuilder();
        var methods = new List<IMethodSymbol>();

        var currentClass = classSymbol;
        while (currentClass is not null)
        {
            methods.AddRange(currentClass.GetMembers().OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public && m.Name != ".ctor"));

            currentClass = currentClass.BaseType;

            if (currentClass?.ContainingNamespace.Name == "System" && currentClass?.Name == "Object")
            {
                break;
            }
        }

        var classLocation = classSymbol.Locations.First();

        foreach (var method in methods)
        {
            if (method.IsStatic) continue;
            if (method.Name.Equals("OnInitialized"))
            {
                continue;
            }

            var methodError = false;
            var methodLocation = method.Locations.First();

            if (overridenMehodNames.Contains(method.Name.ToLower()))
            {
                continue;
            }

            if (method.IsOverride)
            {
                overridenMehodNames.Add(method.Name.ToLower());
            }

            if (!usedMehodNames.Add(method.Name.ToLower()))
            {
                var diagnostic = Diagnostic.Create(new DiagnosticDescriptor(
                    id: "SG0003",
                    title: "Method overloading is not allowed",
                    messageFormat: "Method '{0}' of class '{1}' has more than one overload. When a class has the attribute 'GenerateHubAttribute' method overloading is not allowed.",
                    category: "SourceGenerator",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true), methodLocation, [classLocation], method.Name, classSymbol.Name);
                context.ReportDiagnostic(diagnostic);
                methodError = true;
                errors++;
            }

            if (methodError)
            {
                continue;
            }

            var parameters = string.Join(", ", method.Parameters.Select(p => $"{p.Type} {p.Name}"));
            var parametersWithoutToken = string.Join(", ", method.Parameters.ExcludeCancellationToken().Select(p => $"{p.Type} {p.Name}"));
            var paramsSeparator = !string.IsNullOrWhiteSpace(parametersWithoutToken) ? ", " : null;
            var arguments = string.Join(", ", method.Parameters.Select(p => p.Name));
            var argumentsWithoutToken = string.Join(", ", method.Parameters.ExcludeCancellationToken().Select(p => p.Name));
            var argsSeparator = !string.IsNullOrWhiteSpace(argumentsWithoutToken) ? ", " : null;
            var returnType = method.ReturnType.ToString();
            var isAsync = returnType.StartsWith(TASK);
            var isVoid = returnType == TASK;
            var isAsyncEnumerable = method.ReturnType.OriginalDefinition.Name == "IAsyncEnumerable";
            var methodAttributes = string.Join("\n", method.GetAttributesByNames(["AuthorizeAttribute", "AllowAnonymous"])
                .Select(x => x.GetAttributeInfo()));

            var hasCancellationToken = method.Parameters
                .Any(p => p.Type.ToDisplayString() == CANCELLATION_TOKEN);

            var hasInitializeMethod = HasInitializeMethod(classSymbol);

            if (isAsync)
            {
                ManageAsyncMethod(
                    useAuthentication,
                    methodsBuilder,
                    method,
                    parameters,
                    parametersWithoutToken,
                    paramsSeparator,
                    arguments,
                    argumentsWithoutToken,
                    argsSeparator,
                    returnType,
                    isVoid,
                    methodAttributes,
                    hasCancellationToken,
                    hasInitializeMethod);
            }
            else
            {
                ManageSyncMethod(
                    context,
                    useAuthentication,
                    methodsBuilder,
                    method,
                    parametersWithoutToken,
                    paramsSeparator,
                    argumentsWithoutToken,
                    argsSeparator,
                    returnType,
                    isAsyncEnumerable,
                    methodAttributes,
                    hasInitializeMethod);
            }
        }

        return methodsBuilder.ToString();
    }

    private static void ManageSyncMethod(SourceProductionContext context, bool useAuthentication, StringBuilder methodsBuilder, IMethodSymbol method, string parametersWithoutToken, string? paramsSeparator, string argumentsWithoutToken, string? argsSeparator, string returnType, bool isAsyncEnumerable, string methodAttributes, bool hasInitializeMethod)
    {
        if (isAsyncEnumerable)
        {
            methodsBuilder.AppendLine(
            $$"""
                {{methodAttributes}}
                public async Task<{{returnType}}> {{method.Name}}({{parametersWithoutToken}}{{paramsSeparator}} CancellationToken cancellationToken = default)
                {
                    {{(useAuthentication ? "EnsureAuthenticated(Context);" : null)}}
                    {{(hasInitializeMethod ? "OnInitialized(Context);" : null)}}
                    return _implementation.{{method.Name}}({{argumentsWithoutToken}}{{argsSeparator}}cancellationToken: cancellationToken);
                }
            """);
        }
        else
        {
            ReportError(context, "SG0001", "Synchronous methods are not supported.");
        }
    }

    private static void ManageAsyncMethod(bool useAuthentication, StringBuilder methodsBuilder, IMethodSymbol method, string parameters, string parametersWithoutToken, string? paramsSeparator, string arguments, string argumentsWithoutToken, string? argsSeparator, string returnType, bool isVoid, string methodAttributes, bool hasCancellationToken, bool hasInitializeMethod)
    {
        if (isVoid)
        {
            ManageVoidMethod(
                useAuthentication,
                methodsBuilder,
                method,
                parameters,
                parametersWithoutToken,
                paramsSeparator,
                arguments,
                argumentsWithoutToken,
                argsSeparator,
                methodAttributes,
                hasCancellationToken,
                hasInitializeMethod);
        }
        else
        {
            ManageReturningMethod(
                useAuthentication,
                methodsBuilder,
                method,
                parameters,
                parametersWithoutToken,
                paramsSeparator,
                arguments,
                argumentsWithoutToken,
                argsSeparator,
                returnType,
                methodAttributes,
                hasCancellationToken,
                hasInitializeMethod);
        }
    }

    private static void ManageReturningMethod(bool useAuthentication, StringBuilder methodsBuilder, IMethodSymbol method, string parameters, string parametersWithoutToken, string? paramsSeparator, string arguments, string argumentsWithoutToken, string? argsSeparator, string returnType, string methodAttributes, bool hasCancellationToken, bool hasInitializeMethod)
    {
        var genericType = returnType.Substring(returnType.IndexOf('<') + 1, returnType.Length - returnType.IndexOf('<') - 2);

        if (hasCancellationToken)
        {
            methodsBuilder.AppendLine(
            $$"""
                {{methodAttributes}}
                public async IAsyncEnumerable<{{genericType}}> {{method.Name}}({{parametersWithoutToken}}{{paramsSeparator}}[EnumeratorCancellation] CancellationToken cancellationToken = default)
                {
                    {{(useAuthentication ? "EnsureAuthenticated(Context);" : null)}}
                    {{(hasInitializeMethod ? "OnInitialized(Context);" : null)}}
                    yield return await _implementation.{{method.Name}}({{argumentsWithoutToken}}{{argsSeparator}}cancellationToken: cancellationToken);
                }
            """);
        }
        else
        {
            methodsBuilder.AppendLine(
            $$"""
                {{methodAttributes}}
                public async Task<{{genericType}}> {{method.Name}}({{parameters}})
                {
                    {{(useAuthentication ? "EnsureAuthenticated(Context);" : null)}}
                    {{(hasInitializeMethod ? "OnInitialized(Context);" : null)}}
                    return await _implementation.{{method.Name}}({{arguments}});
                }
            """);
        }
    }

    private static void ManageVoidMethod(bool useAuthentication, StringBuilder methodsBuilder, IMethodSymbol method, string parameters, string parametersWithoutToken, string? paramsSeparator, string arguments, string argumentsWithoutToken, string? argsSeparator, string methodAttributes, bool hasCancellationToken, bool hasInitializeMethod)
    {
        if (hasCancellationToken)
        {
            methodsBuilder.AppendLine(
            $$"""
                {{methodAttributes}}
                public async IAsyncEnumerable<bool> {{method.Name}}({{parametersWithoutToken}}{{paramsSeparator}}[EnumeratorCancellation] CancellationToken cancellationToken = default)
                {
                    {{(useAuthentication ? "EnsureAuthenticated(Context);" : null)}}
                    {{(hasInitializeMethod ? "OnInitialized(Context);" : null)}}
                    await _implementation.{{method.Name}}({{argumentsWithoutToken}}{{argsSeparator}}cancellationToken: cancellationToken);
                    yield return true;
                }
            """);
        }
        else
        {
            methodsBuilder.AppendLine(
            $$"""
                {{methodAttributes}}
                public async Task {{method.Name}}({{parameters}})
                {
                    {{(useAuthentication ? "EnsureAuthenticated(Context);" : null)}}
                    {{(hasInitializeMethod ? "OnInitialized(Context);" : null)}}
                    await _implementation.{{method.Name}}({{arguments}});
                }
            """);
        }
    }

    private static void ReportError(SourceProductionContext context, string id, string message, INamedTypeSymbol? classSymbol = null)
    {
        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(id, "Validation Error", message, "SourceGenerator", DiagnosticSeverity.Error, true),
            classSymbol?.Locations.FirstOrDefault());
        context.ReportDiagnostic(diagnostic);
    }
}