using Microsoft.EntityFrameworkCore;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess.Models;
using Parsalud.DataAccess;
using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer;

[GenerateHub]
public class PostCategoryService(
    IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService) : IPostCategoryService
{
    private readonly IDbContextFactory<ParsaludDbContext> dbContextFactory = dbContextFactory;
    private readonly IUserService userService = userService;

    public async Task<BusinessResponse> CreateAsync(ManagePostCategoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            dbContext.Add(new PostCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
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

    public Task<BusinessResponse> UpdateAsync(Guid id, ManagePostCategoryRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BusinessResponse<ParsaludPostCategory>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.PostCategories.Where(x => x.Id == id)
                .Select(x => new ParsaludPostCategory
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                return BusinessResponse.Error<ParsaludPostCategory>("Categoría de artículo inexistente");
            }

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludPostCategory>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludPostCategory[]>> GetByCriteriaAsync(PostCategorySearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = dbContext.PostCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Name))
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{criteria.Name}%"));

            var entity = await query.Select(x => new ParsaludPostCategory
            {
                Id = x.Id,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludPostCategory[]>("Ocurrió un error inesperado");
        }
    }
}
