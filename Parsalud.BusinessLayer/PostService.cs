using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;
using Parsalud.DataAccess.Models;
using VENative.Blazor.ServiceGenerator.Attributes;
using ZiggyCreatures.Caching.Fusion;

namespace Parsalud.BusinessLayer;

[GenerateHub(useAuthentication: true)]
public class PostService(
    IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService) : IPostService
{
    private readonly IDbContextFactory<ParsaludDbContext> _dbContextFactory = dbContextFactory;
    private readonly IUserService _userService = userService;

    public void OnInitialized(HubCallerContext context)
    {
        _userService.SetUser(context.User);
    }

    public async Task<BusinessResponse<ParsaludPost>> CreateAsync(ManagePostRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = new Post
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                Description = request.Description,
                Hidden = request.Hidden,
                ImgSrc = request.ImgSrc,
                PostCategoryId = request.PostCategoryId,
                CreatedAt = DateTime.Now,
                CreatedById = _userService.Id,
            };
            dbContext.Add(entity);

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success(new ParsaludPost
            {
                Id = entity.Id,
                Content = entity.Content,
                Title = entity.Title,
                Description = entity.Description ?? "",
                ImgSrc = entity.ImgSrc ?? "",
                PostCategory = entity.PostCategory?.Name ?? "",
                Hidden = entity.Hidden,
                PostCategoryId = entity.PostCategoryId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            });
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludPost>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludPost>> UpdateAsync(Guid id, ManagePostRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Posts.Include(x => x.PostCategory)
                .FirstAsync(x => x.Id == id && !x.Deleted, cancellationToken);

            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.Content = request.Content;
            entity.ImgSrc = request.ImgSrc;
            entity.Hidden = request.Hidden;
            entity.PostCategoryId = request.PostCategoryId;
            entity.UpdatedAt = DateTime.Now;
            entity.UpatedById = _userService.Id;

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success(new ParsaludPost
            {
                Id = entity.Id,
                Content = entity.Content,
                Title = entity.Title,
                Description = entity.Description ?? "",
                ImgSrc = entity.ImgSrc ?? "",
                PostCategory = entity.PostCategory.Name,
                Hidden = entity.Hidden,
                PostCategoryId = entity.PostCategoryId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            });
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludPost>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var post = await dbContext.Posts.FirstAsync(x => x.Id == id, cancellationToken);

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

    public async Task<BusinessResponse<ParsaludPost>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Posts.Where(x => x.Id == id && !x.Deleted)
                .Select(x => new ParsaludPost
                {
                    Id = x.Id,
                    Content = x.Content,
                    Description = x.Description ?? "",
                    Title = x.Title,
                    ImgSrc = x.ImgSrc ?? "",
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

    public async Task<BusinessResponse<Paginated<ParsaludPost[]>>> GetByCriteriaAsync(PostSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = dbContext.Posts.AsQueryable();

            if (criteria.Ids?.Length > 0)
                query = query.Where(x => criteria.Ids.Contains(x.Id));

            if (!string.IsNullOrWhiteSpace(criteria.Title))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{criteria.Title}%") || EF.Functions.Like(x.Content, $"%{criteria.Content}%"));

            if (criteria.CategoryIds?.Length > 0)
            {
                var cc = criteria.CategoryIds;
                query = query.Where(x => cc.Contains(x.PostCategoryId));
            }

            if (criteria.ExcludedIds?.Length > 0)
            {
                var eIds = criteria.ExcludedIds;
                query = query.Where(x => !eIds.Contains(x.Id));
            }

            query = query.Where(x => !x.Deleted);

            int totalItems = 0;
            if (criteria.Page.HasValue && criteria.Size.HasValue)
            {
                totalItems = await query.CountAsync(cancellationToken);
            }

            query = query.OrderByDescending(x => x.CreatedAt);

            if (criteria.Page.HasValue)
            {
                var size = criteria.Size.GetValueOrDefault(1);
                query = query.Skip(criteria.Page.Value * size);
            }

            if (criteria.Size.HasValue)
                query = query.Take(criteria.Size.Value);

            var ihidden = criteria.IncludeHidden;
            query = query.Where(x => (!ihidden && !x.Hidden) || ihidden);

            var entities = await query.Where(x => !x.Deleted).Select(x => new ParsaludPost
            {
                Id = x.Id,
                Content = x.Content,
                Description = x.Description ?? "",
                PostCategory = x.PostCategory.Name,
                ImgSrc = x.ImgSrc ?? "",
                Title = x.Title,
                Hidden = x.Hidden,
                PostCategoryId = x.PostCategoryId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
                .ToArrayAsync(cancellationToken);

            if (!criteria.Page.HasValue && !criteria.Size.HasValue)
            {
                totalItems = entities.Length;
            }

            return BusinessResponse.Success(new Paginated<ParsaludPost[]>
            {
                Data = entities,
                Page = criteria.Page,
                PageSize = criteria.Size,
                TotalItems = totalItems
            });
        }
        catch
        {
            return BusinessResponse.Error<Paginated<ParsaludPost[]>>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludPost[]>> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var query = dbContext.Posts.Where(x => !x.Deleted && !x.Hidden).Take(4)
                .OrderByDescending(x => x.CreatedAt);

            var entity = await query.Select(x => new ParsaludPost
            {
                Id = x.Id,
                Content = x.Content,
                Description = x.Description ?? "",
                PostCategory = x.PostCategory.Name,
                ImgSrc = x.ImgSrc ?? "",
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
