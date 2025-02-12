using Microsoft.EntityFrameworkCore;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;
using Parsalud.DataAccess.Models;
using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer;

[GenerateHub]
public class PostService(IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService) : IPostService
{
    private readonly IDbContextFactory<ParsaludDbContext> dbContextFactory = dbContextFactory;
    private readonly IUserService userService = userService;

    public async Task<BusinessResponse> CreateAsync(ManagePostRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            dbContext.Add(new Post
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                Hidden = request.Hidden,
                PostCategoryId = request.PostCategoryId,
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

    public Task<BusinessResponse> UpdateAsync(Guid id, ManagePostRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BusinessResponse<ParsaludPost>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Posts.Where(x => x.Id == id)
                .Select(x => new ParsaludPost
                {
                    Id = x.Id,
                    Content = x.Content,
                    Title = x.Title,
                    PostCategory = x.PostCategory.Name,
                    Hidden = x.Hidden,
                    PostCategoryId = x.PostCategoryId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                return BusinessResponse.Error<ParsaludPost>("Post inexistente");
            }

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludPost>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludPost[]>> GetByCriteriaAsync(PostSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = dbContext.Posts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Title))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{criteria.Title}%"));

            var entity = await query.Select(x => new ParsaludPost
            {
                Id = x.Id,
                Content = x.Content,
                PostCategory = x.PostCategory.Name,
                Title = x.Title,
                Hidden = x.Hidden,
                PostCategoryId = x.PostCategoryId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludPost[]>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludPost[]>> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            
            var query = dbContext.Posts.Take(4)
                .OrderByDescending(x => x.CreatedById);

            var entity = await query.Select(x => new ParsaludPost
            {
                Id = x.Id,
                Content = x.Content,
                PostCategory = x.PostCategory.Name,
                Title = x.Title,
                Hidden = x.Hidden,
                PostCategoryId = x.PostCategoryId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludPost[]>("Ocurrió un error inesperado");
        }
    }
}
