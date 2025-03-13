using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;
using Parsalud.DataAccess.Models;
using System.Text.RegularExpressions;
using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer;

[GenerateHub(useAuthentication: true)]
public class ServiceService(
    IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService) : IServiceService
{
    private readonly IDbContextFactory<ParsaludDbContext> _dbContextFactory = dbContextFactory;
    private readonly IUserService _userService = userService;

    private static ParsaludService EntityToDTO(Service entity)
    {
        return new ParsaludService
        (
            Id: entity.Id,
            Name: entity.Name,
            Code: entity.Code
        );
    }

    public async Task<BusinessResponse<ParsaludService>> CreateAsync(ManageServiceRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var exists = await dbContext.Services.AnyAsync(x => EF.Functions.Like(x.Code, request.Code), cancellationToken);

            if (exists)
            {
                return BusinessResponse.Error<ParsaludService>("Ya existe un servicio con el mismo código");
            }

            if (!Regex.IsMatch(request.Code, @"^[a-zA-Z0-9_-]+$"))
            {
                return BusinessResponse.Error<ParsaludService>("El código solo puede contener letras, números, guiones (-) y guiones bajos (_). No se permiten espacios ni caracteres especiales.");
            }

            Service entity = new()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code.ToUpper(),
                CreatedAt = DateTime.Now,
                CreatedById = _userService.Id,
            };

            dbContext.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return BusinessResponse.Success(EntityToDTO(entity));
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludService>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Services.Include(x => x.Faqs)
                .FirstAsync(x => x.Id == id && !x.Deleted, cancellationToken);

            if (entity.Faqs.Count > 0)
            {
                return BusinessResponse.Error<ParsaludService>("El servicio está siendo usado en una o mas FAQ's");
            }

            entity.Deleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.UpatedById = _userService.Id;

            dbContext.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return BusinessResponse.Success(EntityToDTO(entity));
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludSection>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<Paginated<ParsaludService[]>>> GetByCriteriaAsync(ServiceSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var query = dbContext.Services.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Name))
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{criteria.Name}%"));

            if (!string.IsNullOrWhiteSpace(criteria.Code))
                query = query.Where(x => EF.Functions.Like(x.Code, $"%{criteria.Code}%"));

            query = query.Where(x => !x.Deleted);

            var entities = await query.Select(x => new ParsaludService
            (
                x.Id,
                x.Name,
                x.Code.ToUpper()
            )).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(new Paginated<ParsaludService[]>
            {
                Data = entities,
                Page = 0,
                PageSize = entities.Length,
                TotalItems = entities.Length
            });
        }
        catch
        {
            return BusinessResponse.Error<Paginated<ParsaludService[]>>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludService>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Services.Where(x => x.Id == id && !x.Deleted)
                .Select(x => new ParsaludService
                (
                    x.Id,
                    x.Name,
                    x.Code.ToUpper()
                )).FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                return BusinessResponse.Error<ParsaludService>("Servicio inexistente");
            }

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludService>("Ocurrió un error inesperado");
        }
    }

    public void OnInitialized(HubCallerContext context)
    {
        _userService.SetUser(context.User);
    }

    public async Task<BusinessResponse<ParsaludService>> UpdateAsync(Guid id, ManageServiceRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var exists = await dbContext.Services.AnyAsync(x => EF.Functions.Like(x.Code, request.Code) && x.Id != id, cancellationToken);

            if (exists)
            {
                return BusinessResponse.Error<ParsaludService>("Ya existe un servicio con el mismo código");
            }

            if (!Regex.IsMatch(request.Code, @"^[a-zA-Z0-9_-]+$"))
            {
                return BusinessResponse.Error<ParsaludService>("El código solo puede contener letras, números, guiones (-) y guiones bajos (_). No se permiten espacios ni caracteres especiales.");
            }

            var entity = await dbContext.Services.FirstAsync(x => x.Id == id && !x.Deleted, cancellationToken);

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.UpdatedAt = DateTime.Now;
            entity.UpatedById = _userService.Id;

            dbContext.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return BusinessResponse.Success(EntityToDTO(entity));
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludService>("Ocurrió un error inesperado");
        }
    }
}
