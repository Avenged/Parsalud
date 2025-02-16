using Microsoft.EntityFrameworkCore;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;
using Parsalud.DataAccess.Models;
using System.Linq;
using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer;

[GenerateHub]
public class FaqService(IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService) : IFaqService
{
    private readonly IDbContextFactory<ParsaludDbContext> dbContextFactory = dbContextFactory;
    private readonly IUserService userService = userService;

    public async Task<BusinessResponse> CreateAsync(ManageFaqRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            dbContext.Add(new Faq
            {
                Id = Guid.NewGuid(),
                Question = request.Question,
                Answer = request.Answer,
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

    public Task<BusinessResponse> UpdateAsync(Guid id, ManageFaqRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BusinessResponse<ParsaludFaq>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Faqs.Where(x => x.Id == id)
                .Select(x => new ParsaludFaq
                {
                    Id = x.Id,
                    Answer = x.Answer,
                    Question = x.Question,
                    Hidden = x.Hidden,
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

    public async Task<BusinessResponse<ParsaludFaq[]>> GetByCriteriaAsync(FaqSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = dbContext.Faqs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Question))
                query = query.Where(x => EF.Functions.Like(x.Question, $"%{criteria.Question}%"));

            if (!string.IsNullOrWhiteSpace(criteria.Answer))
                query = query.Where(x => EF.Functions.Like(x.Answer, $"%{criteria.Answer}%"));

            if (criteria.MaxCount.HasValue)
                query = query.Take(criteria.MaxCount.Value);

            var entity = await query.Select(x => new ParsaludFaq
            {
                Id = x.Id,
                Answer = x.Answer,
                Question = x.Question,
                Hidden = x.Hidden,
            }).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludFaq[]>("Ocurrió un error inesperado");
        }
    }
}
