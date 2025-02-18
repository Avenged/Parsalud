using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;
using Parsalud.DataAccess.Models;
using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer;

[GenerateHub]
public class SectionService(IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService,
    IMemoryCache memoryCache) : ISectionService
{
    private readonly IDbContextFactory<ParsaludDbContext> dbContextFactory = dbContextFactory;
    private readonly IUserService userService = userService;
    private readonly IMemoryCache memoryCache = memoryCache;

    private static string? ConvertEmptyToNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return value;
    }

    public async Task<BusinessResponse<ParsaludSection>> CreateAsync(ManageSectionRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var exists = await dbContext.Sections.AnyAsync(x => x.Code.ToUpper() == request.Code.ToUpper(), cancellationToken);

            if (exists)
            {
                return BusinessResponse.Error<ParsaludSection>("Ya existe una sección con el mismo código");
            }

            Section entity = new()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code.ToUpper(),
                Content = request.Content,
                StyleSheetId = request.StyleSheetId,
                Hidden = request.Hidden,
                Page = request.Page,
                Param1 = ConvertEmptyToNull(request.Param1),
                Param2 = ConvertEmptyToNull(request.Param2),
                Param3 = ConvertEmptyToNull(request.Param3),
                Param4 = ConvertEmptyToNull(request.Param4),
                Param5 = ConvertEmptyToNull(request.Param5),
                Param6 = ConvertEmptyToNull(request.Param6),
                CreatedAt = DateTime.Now,
                CreatedById = userService.Id,
            };

            dbContext.Add(entity);

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success(EntityToDTO(entity));
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludSection>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludSection>> UpdateAsync(Guid id, ManageSectionRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var exists = await dbContext.Sections.AnyAsync(x => x.Code.ToUpper() == request.Code.ToUpper() && x.Id != id, cancellationToken);

            if (exists)
            {
                return BusinessResponse.Error<ParsaludSection>("Ya existe una sección con el mismo código");
            }

            var entity = await dbContext.Sections.FirstAsync(x => x.Id == id, cancellationToken);

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Content = request.Content;
            entity.StyleSheetId = request.StyleSheetId;
            entity.Hidden = request.Hidden;
            entity.Page = ConvertEmptyToNull(request.Page);
            entity.Param1 = ConvertEmptyToNull(request.Param1);
            entity.Param2 = ConvertEmptyToNull(request.Param2);
            entity.Param3 = ConvertEmptyToNull(request.Param3);
            entity.Param4 = ConvertEmptyToNull(request.Param4);
            entity.Param5 = ConvertEmptyToNull(request.Param5);
            entity.Param6 = ConvertEmptyToNull(request.Param6);
            entity.UpdatedAt = DateTime.Now;
            entity.UpatedById = userService.Id;

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success(EntityToDTO(entity));
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludSection>("Ocurrió un error inesperado");
        }
    }

    public Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BusinessResponse<ParsaludSection>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Sections.Where(x => x.Id == id)
                .Select(x => new ParsaludSection
                {
                    Id = x.Id,
                    Content = x.Content,
                    Name = x.Name,
                    Code = x.Code.ToUpper(),
                    StyleSheetId = x.StyleSheetId,
                    Hidden = x.Hidden,
                    Page = x.Page,
                    Param1 = x.Param1,
                    Param2 = x.Param2,
                    Param3 = x.Param3,
                    Param4 = x.Param4,
                    Param5 = x.Param5,
                    Param6 = x.Param6,
                }).FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                return BusinessResponse.Error<ParsaludSection>("Post inexistente");
            }

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludSection>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludSection[]>> GetByCriteriaAsync(SectionSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = dbContext.Sections.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Name))
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{criteria.Name}%"));

            if (!string.IsNullOrWhiteSpace(criteria.Code))
                query = query.Where(x => EF.Functions.Like(x.Code, $"%{criteria.Code}%"));

            if (!string.IsNullOrWhiteSpace(criteria.Content))
                query = query.Where(x => EF.Functions.Like(x.Content, $"%{criteria.Content}%"));

            if (criteria.SectionKind.HasValue && criteria.SectionKind == SectionKind.Page)
                query = query.Where(x => !string.IsNullOrWhiteSpace(x.Page));
            else if (criteria.SectionKind.HasValue && criteria.SectionKind == SectionKind.Component)
                query = query.Where(x => string.IsNullOrWhiteSpace(x.Page));

            var entity = await query.Select(x => new ParsaludSection
            {
                Id = x.Id,
                Content = x.Content,
                Name = x.Name,
                Code = x.Code.ToUpper(),
                StyleSheetId = x.StyleSheetId,
                Hidden = x.Hidden,
                Page = x.Page,
                Param1 = x.Param1,
                Param2 = x.Param2,
                Param3 = x.Param3,
                Param4 = x.Param4,
                Param5 = x.Param5,
                Param6 = x.Param6,
            }).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludSection[]>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludSection>> GetByCodeAsync(
        string code,
        string? param1 = default,
        string? param2 = default,
        string? param3 = default,
        string? param4 = default,
        string? param5 = default,
        string? param6 = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            var codeUpper = code.ToUpper();

            var query = dbContext.Sections.Where(x => 
                (!string.IsNullOrWhiteSpace(x.Page) && x.Page.ToUpper() == codeUpper) ||
                x.Code.ToUpper() == codeUpper);

            if (!string.IsNullOrWhiteSpace(param1))
                query = query.Where(x => x.Param1!.ToUpper() == param1.ToUpper() || string.IsNullOrWhiteSpace(x.Page));
            else
                query = query.Where(x => x.Param1 == null);

            if (!string.IsNullOrWhiteSpace(param2))
                query = query.Where(x => x.Param2!.ToUpper() == param2.ToUpper() || string.IsNullOrWhiteSpace(x.Page));
            else
                query = query.Where(x => x.Param2 == null);

            if (!string.IsNullOrWhiteSpace(param3))
                query = query.Where(x => x.Param3!.ToUpper() == param3.ToUpper() || string.IsNullOrWhiteSpace(x.Page));
            else
                query = query.Where(x => x.Param3 == null);

            if (!string.IsNullOrWhiteSpace(param4))
                query = query.Where(x => x.Param4!.ToUpper() == param4.ToUpper() || string.IsNullOrWhiteSpace(x.Page));
            else
                query = query.Where(x => x.Param4 == null);

            if (!string.IsNullOrWhiteSpace(param5))
                query = query.Where(x => x.Param5!.ToUpper() == param5.ToUpper() || string.IsNullOrWhiteSpace(x.Page));
            else
                query = query.Where(x => x.Param5 == null);

            if (!string.IsNullOrWhiteSpace(param6))
                query = query.Where(x => x.Param6!.ToUpper() == param6.ToUpper() || string.IsNullOrWhiteSpace(x.Page));
            else
                query = query.Where(x => x.Param6 == null);

            var entity = await query.Select(x => new ParsaludSection
            {
                Id = x.Id,
                Content = x.Content,
                Name = x.Name,
                Code = x.Code,
                StyleSheetId = x.StyleSheetId,
                Hidden = x.Hidden,
                Page = x.Page,
                Param1 = x.Param1,
                Param2 = x.Param2,
                Param3 = x.Param3,
                Param4 = x.Param4,
                Param5 = x.Param5,
                Param6 = x.Param6,
            }).FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                return BusinessResponse.Error<ParsaludSection>("Sección inexistente");
            }

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludSection>("Ocurrió un error inesperado");
        }
    }

    private static ParsaludSection EntityToDTO(Section entity)
    {
        return new ParsaludSection
        {
            Id = entity.Id,
            Content = entity.Content,
            Name = entity.Name,
            Code = entity.Code,
            StyleSheetId = entity.StyleSheetId,
            Hidden = entity.Hidden,
            Page = entity.Page,
            Param1 = entity.Param1,
            Param2 = entity.Param2,
            Param3 = entity.Param3,
            Param4 = entity.Param4,
            Param5 = entity.Param5,
            Param6 = entity.Param6,
        };
    }

    public async Task UpdateLiveServerAsync(LiveServerInstance ins, CancellationToken cancellationToken = default)
    {
        var instance = memoryCache.GetOrCreate($"LiveServer-{ins.Id}", entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            return new LiveServerInstance()
            {
                Id = ins.Id,
                Html = ins.Html,
                Css = ins.Css,
            };
        });
        instance!.Id = ins.Id;
        instance!.Html = ins.Html;
        instance!.Css = ins.Css;
        instance!.I++;

        await Task.CompletedTask;
    }
}
