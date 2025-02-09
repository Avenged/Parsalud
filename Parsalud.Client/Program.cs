using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Parsalud.Client;
using Radzen;
using VENative.Blazor.ServiceGenerator.Extensions.BlazorWebAssembly;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddGeneratedServices(typeof(Parsalud.BusinessLayer.Abstractions.IAssemblyMarker).Assembly);
builder.Services.AddRadzenComponents();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

await builder.Build().RunAsync();
