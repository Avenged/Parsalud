using Microsoft.EntityFrameworkCore;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;
using Parsalud.DataAccess.Models;
using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer;

[GenerateHub]
public class FaqService(
    IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService) : IFaqService
{
    private readonly IDbContextFactory<ParsaludDbContext> _dbContextFactory = dbContextFactory;
    private readonly IUserService _userService = userService;

    public async Task<BusinessResponse<ParsaludFaq>> CreateAsync(ManageFaqRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = new Faq
            {
                Id = Guid.NewGuid(),
                Question = request.Question,
                Answer = request.Answer,
                Hidden = request.Hidden,
                CreatedAt = DateTime.Now,
                CreatedById = _userService.Id,
            };
            dbContext.Add(entity);

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success(new ParsaludFaq
            {
                Id = entity.Id,
                Answer = entity.Answer,
                Question = entity.Question,
                Hidden = entity.Hidden,
                CreatedAt = entity.CreatedAt
            });
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludFaq>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludFaq>> UpdateAsync(Guid id, ManageFaqRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Faqs.FirstAsync(x => x.Id == id && !x.Deleted, cancellationToken);

            entity.Answer = request.Answer;
            entity.Question = request.Question;
            entity.Hidden = request.Hidden;
            entity.UpdatedAt = DateTime.Now;
            entity.UpatedById = _userService.Id;

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success(new ParsaludFaq
            {
                Id = entity.Id,
                Answer = entity.Answer,
                Question = entity.Question,
                Hidden = entity.Hidden,
                CreatedAt = entity.CreatedAt
            });
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludFaq>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var post = await dbContext.Faqs.FirstAsync(x => x.Id == id && !x.Deleted, cancellationToken);

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

    public async Task<BusinessResponse<ParsaludFaq>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Faqs.Where(x => x.Id == id && !x.Deleted)
                .Select(x => new ParsaludFaq
                {
                    Id = x.Id,
                    Answer = x.Answer,
                    Question = x.Question,
                    Hidden = x.Hidden,
                    CreatedAt = x.CreatedAt
                }).FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                return BusinessResponse.Error<ParsaludFaq>("Faq inexistente");
            }

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludFaq>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<Paginated<ParsaludFaq[]>>> GetByCriteriaAsync(FaqSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = dbContext.Faqs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Question))
                query = query.Where(x => EF.Functions.Like(x.Question, $"%{criteria.Question}%"));

            if (!string.IsNullOrWhiteSpace(criteria.Answer))
                query = query.Where(x => EF.Functions.Like(x.Answer, $"%{criteria.Answer}%"));

            if (criteria.MaxCount.HasValue)
                query = query.Take(criteria.MaxCount.Value);

            query = query.Where(x => !x.Deleted);

            query = query.OrderByDescending(x => x.CreatedAt);

            var entities = await query.Select(x => new ParsaludFaq
            {
                Id = x.Id,
                Answer = x.Answer,
                Question = x.Question,
                Hidden = x.Hidden,
                CreatedAt = x.CreatedAt
            }).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(new Paginated<ParsaludFaq[]>
            {
                Data = entities,
                Page = 0,
                PageSize = entities.Length,
                TotalItems = entities.Length
            });
        }
        catch
        {
            return BusinessResponse.Error<Paginated<ParsaludFaq[]>>("Ocurrió un error inesperado");
        }
    }
}