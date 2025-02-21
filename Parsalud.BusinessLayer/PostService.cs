﻿using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;
using Parsalud.DataAccess.Models;
using VENative.Blazor.ServiceGenerator.Attributes;
using ZiggyCreatures.Caching.Fusion;

namespace Parsalud.BusinessLayer;

[GenerateHub]
public class PostService(
    IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService,
    IFusionCache cache) : IPostService
{
    private readonly IDbContextFactory<ParsaludDbContext> _dbContextFactory = dbContextFactory;
    private readonly IUserService _userService = userService;
    private readonly IFusionCache _cache = cache;

    public async Task<BusinessResponse> CreateAsync(ManagePostRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            dbContext.Add(new Post
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                Hidden = request.Hidden,
                ImgSrc = request.ImgSrc,
                PostCategoryId = request.PostCategoryId,
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

    public async Task<BusinessResponse> UpdateAsync(Guid id, ManagePostRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var post = await dbContext.Posts.FirstAsync(x => x.Id == id && !x.Deleted, cancellationToken);

            post.Title = request.Title;
            post.Content = request.Content;
            post.ImgSrc = request.ImgSrc;
            post.Hidden = request.Hidden;
            post.PostCategoryId = request.PostCategoryId;
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
            var entity = await _cache.GetOrSetAsync($"post|{id}", async token =>
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

                var entity = await dbContext.Posts.Where(x => x.Id == id && !x.Deleted)
                    .Select(x => new ParsaludPost
                    {
                        Id = x.Id,
                        Content = x.Content,
                        Title = x.Title,
                        ImgSrc = x.ImgSrc ?? "",
                        PostCategory = x.PostCategory.Name,
                        Hidden = x.Hidden,
                        PostCategoryId = x.PostCategoryId,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).FirstOrDefaultAsync(cancellationToken);

                return entity;
            }, tags: [CacheTags.Post], token: cancellationToken);

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
                PostCategory = x.PostCategory.Name,
                ImgSrc = x.ImgSrc ?? "",
                Title = x.Title,
                Hidden = x.Hidden,
                PostCategoryId = x.PostCategoryId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            })
                .OrderByDescending(x => x.CreatedAt)
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
