using Microsoft.EntityFrameworkCore;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;
using Parsalud.DataAccess.Models;
using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer;

[GenerateHub]
public class SectionService(IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService) : ISectionService
{
    private readonly IDbContextFactory<ParsaludDbContext> dbContextFactory = dbContextFactory;
    private readonly IUserService userService = userService;

    public async Task<BusinessResponse> CreateAsync(ManageSectionRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            dbContext.Add(new Section
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code.ToUpper(),
                Content = request.Content,
                Hidden = request.Hidden,
                CreatedAt = DateTime.Now,
                CreatedById = userService.Id,
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success();
        }
        catch (Exception)
        {
            return BusinessResponse.Error("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse> UpdateAsync(Guid id, ManageSectionRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            var entity = await dbContext.Sections.FirstAsync(x => x.Id == id, cancellationToken);

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.Content = request.Content;
            entity.Hidden = request.Hidden;
            entity.UpdatedAt = DateTime.Now;
            entity.UpatedById = userService.Id;

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success();
        }
        catch (Exception)
        {
            return BusinessResponse.Error("Ocurrió un error inesperado");
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
                    Hidden = x.Hidden,
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

            var entity = await query.Select(x => new ParsaludSection
            {
                Id = x.Id,
                Content = x.Content,
                Name = x.Name,
                Code = x.Code.ToUpper(),
                Hidden = x.Hidden,
            }).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludSection[]>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludSection>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Sections.Where(x => x.Code.ToUpper() == code.ToUpper())
                .Select(x => new ParsaludSection
                {
                    Id = x.Id,
                    Content = x.Content,
                    Name = x.Name,
                    Code = x.Code,
                    Hidden = x.Hidden,
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
}
