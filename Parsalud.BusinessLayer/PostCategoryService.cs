using Microsoft.EntityFrameworkCore;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess.Models;
using Parsalud.DataAccess;
using VENative.Blazor.ServiceGenerator.Attributes;
using Microsoft.EntityFrameworkCore.Internal;
using Azure.Core;

namespace Parsalud.BusinessLayer;

[GenerateHub]
public class PostCategoryService(
    IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService) : IPostCategoryService
{
    private readonly IDbContextFactory<ParsaludDbContext> _dbContextFactory = dbContextFactory;
    private readonly IUserService _userService = userService;

    public async Task<BusinessResponse> CreateAsync(ManagePostCategoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            dbContext.Add(new PostCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CreatedAt = DateTime.Now,
                CreatedById = _userService.Id,
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success();
        }
        catch (Exception)
        {
            return BusinessResponse.Error("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse> UpdateAsync(Guid id, ManagePostCategoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var post = await dbContext.PostCategories.FirstAsync(x => x.Id == id && !x.Deleted, cancellationToken);

            post.Name = request.Name;
            post.UpdatedAt = DateTime.Now;
            post.UpatedById = _userService.Id;

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success();
        }
        catch (Exception)
        {
            return BusinessResponse.Error("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var post = await dbContext.PostCategories.FirstAsync(x => x.Id == id && !x.Deleted, cancellationToken);

            post.Deleted = true;
            post.UpdatedAt = DateTime.Now;
            post.UpatedById = _userService.Id;

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success();
        }
        catch (Exception)
        {
            return BusinessResponse.Error("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludPostCategory>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.PostCategories.Where(x => x.Id == id && !x.Deleted)
                .Select(x => new ParsaludPostCategory
                {
                    Id = x.Id,
                    Name = x.Name,
                    PostsCount = x.Posts.Count,
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

    public async Task<BusinessResponse<Paginated<ParsaludPostCategory[]>>> GetByCriteriaAsync(PostCategorySearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = dbContext.PostCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Name))
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{criteria.Name}%"));

            query = query.Where(x => !x.Deleted);

            var entities = await query.Select(x => new ParsaludPostCategory
            {
                Id = x.Id,
                Name = x.Name,
                PostsCount = x.Posts.Count,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(new Paginated<ParsaludPostCategory[]>
            {
                Data = entities,
                Page = 0,
                PageSize = entities.Length,
                TotalItems = entities.Length
            });
        }
        catch
        {
            return BusinessResponse.Error<Paginated<ParsaludPostCategory[]>>("Ocurrió un error inesperado");
        }
    }
}
