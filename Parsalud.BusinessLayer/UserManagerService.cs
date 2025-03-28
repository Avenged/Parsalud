﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;
using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer;

[GenerateHub(useAuthentication: true)]
public class UserManagerService(
    IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService) : IUserManagerService
{
    private readonly IDbContextFactory<ParsaludDbContext> _dbContextFactory = dbContextFactory;
    private readonly IUserService _userService = userService;

    public void OnInitialized(HubCallerContext context)
    {
        _userService.SetUser(context.User);
    }

    public Task<BusinessResponse<ParsaludUserDto>> CreateAsync(ManageUserRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public async Task<BusinessResponse<Paginated<ParsaludUserDto[]>>> GetByCriteriaAsync(UserSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var query = dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.UserName))
                query = query.Where(x => EF.Functions.Like(x.UserName, $"%{criteria.UserName}%"));

            if (criteria.IsDisabled.HasValue)
                query = query.Where(x => x.IsDisabled == criteria.IsDisabled);

            var entities = await query.Select(x => new ParsaludUserDto
            {
                Id = x.Id,
                UserName = x.UserName!,
                IsDisabled = x.IsDisabled
            }).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(new Paginated<ParsaludUserDto[]>
            {
                Data = entities,
                Page = 0,
                PageSize = entities.Length,
                TotalItems = entities.Length
            });
        }
        catch
        {
            return BusinessResponse.Error<Paginated<ParsaludUserDto[]>>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludUserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Users.Where(x => x.Id == id)
                .Select(x => new ParsaludUserDto
                {
                    Id = x.Id,
                    UserName = x.UserName!,
                    IsDisabled = x.IsDisabled,
                }).FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                return BusinessResponse.Error<ParsaludUserDto>("Usuario inexistente");
            }

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludUserDto>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludUserDto>> UpdateAsync(Guid id, ManageUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.Users
                .FirstAsync(x => x.Id == id, cancellationToken);

            entity.IsDisabled = request.IsDisabled;

            await dbContext.SaveChangesAsync(cancellationToken);
            return BusinessResponse.Success(new ParsaludUserDto
            {
                Id = entity.Id,
                UserName = entity.UserName!,
                IsDisabled = entity.IsDisabled,
            });
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludUserDto>("Ocurrió un error inesperado");
        }
    }
}