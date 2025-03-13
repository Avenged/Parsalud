using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using NUglify;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.DataAccess;
using Parsalud.DataAccess.Models;
using System.Text;
using VENative.Blazor.ServiceGenerator.Attributes;
using ZiggyCreatures.Caching.Fusion;

namespace Parsalud.BusinessLayer;

[GenerateHub(useAuthentication: true)]
public class StyleSheetService(
    IDbContextFactory<ParsaludDbContext> dbContextFactory,
    IUserService userService) : IStyleSheetService
{
    private readonly IDbContextFactory<ParsaludDbContext> _dbContextFactory = dbContextFactory;
    private readonly IUserService _userService = userService;

    public void OnInitialized(HubCallerContext context)
    {
        _userService.SetUser(context.User);
    }

    public async Task<BusinessResponse<ParsaludStyleSheet>> CreateAsync(ManageStyleSheetRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var exists = await dbContext.StyleSheets.AnyAsync(x => EF.Functions.Like(x.FileName, request.FileName), cancellationToken);

            if (exists)
            {
                return BusinessResponse.Error<ParsaludStyleSheet>("Ya existe una hoja de estilos con el mismo nombre");
            }

            StyleSheet styleSheet = new()
            {
                Id = Guid.NewGuid(),
                FileName = request.FileName,
                Content = request.Content,
                CreatedAt = DateTime.Now,
                CreatedById = _userService.Id,
            };
            dbContext.Add(styleSheet);

            await dbContext.SaveChangesAsync(cancellationToken);

            return BusinessResponse.Success(new ParsaludStyleSheet
            {
                Id = styleSheet.Id,
                Content = styleSheet.Content,
                FileName = styleSheet.FileName,
            });
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludStyleSheet>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludStyleSheet>> UpdateAsync(Guid id, ManageStyleSheetRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var exists = await dbContext.StyleSheets.AnyAsync(x => EF.Functions.Like(x.FileName, request.FileName) && x.Id != id, cancellationToken);

            if (exists)
            {
                return BusinessResponse.Error<ParsaludStyleSheet>("Ya existe una hoja de estilos con el mismo nombre");
            }

            var entity = await dbContext.StyleSheets.FirstAsync(x => x.Id == id && !x.Deleted, cancellationToken);

            entity.FileName = request.FileName;
            entity.Content = request.Content;
            entity.UpdatedAt = DateTime.Now;
            entity.UpatedById = _userService.Id;

            dbContext.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return BusinessResponse.Success(new ParsaludStyleSheet
            {
                Id = entity.Id,
                Content = entity.Content,
                FileName = entity.FileName,
            });
        }
        catch (Exception)
        {
            return BusinessResponse.Error<ParsaludStyleSheet>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.StyleSheets.FirstAsync(x => x.Id == id && !x.Deleted, cancellationToken);

            entity.Deleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.UpatedById = _userService.Id;

            dbContext.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return BusinessResponse.Success();
        }
        catch (Exception)
        {
            return BusinessResponse.Error("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<Paginated<ParsaludStyleSheet[]>>> GetByCriteriaAsync(StyleSheetSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var query = dbContext.StyleSheets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.FileName))
                query = query.Where(x => EF.Functions.Like(x.FileName, $"%{criteria.FileName}%"));

            if (!string.IsNullOrWhiteSpace(criteria.Content))
                query = query.Where(x => EF.Functions.Like(x.Content, $"%{criteria.Content}%"));

            query = query.Where(x => !x.Deleted);

            var results = await query.Select(x => new ParsaludStyleSheet
            {
                Id = x.Id,
                Content = x.Content,
                FileName = x.FileName,
            }).ToArrayAsync(cancellationToken);

            return BusinessResponse.Success(new Paginated<ParsaludStyleSheet[]>
            {
                Data = results,
                Page = 0,
                PageSize = results.Length,
                TotalItems = results.Length
            });
        }
        catch
        {
            return BusinessResponse.Error<Paginated<ParsaludStyleSheet[]>>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludStyleSheet>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var entity = await dbContext.StyleSheets.Where(x => x.Id == id && !x.Deleted)
                .Select(x => new ParsaludStyleSheet
                {
                    Id = x.Id,
                    Content = x.Content,
                    FileName = x.FileName,
                }).FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                return BusinessResponse.Error<ParsaludStyleSheet>("Post inexistente");
            }

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludStyleSheet>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<ParsaludStyleSheet>> GetByFileNameAsync(string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.StyleSheets.Where(x => EF.Functions.Like(x.FileName, fileName) && !x.Deleted)
                .Select(x => new ParsaludStyleSheet
                {
                    Id = x.Id,
                    Content = x.Content,
                    FileName = x.FileName,
                }).FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                return BusinessResponse.Error<ParsaludStyleSheet>("Post inexistente");
            }

            return BusinessResponse.Success(entity);
        }
        catch
        {
            return BusinessResponse.Error<ParsaludStyleSheet>("Ocurrió un error inesperado");
        }
    }

    public async Task<BusinessResponse<string>> GetBundleCssAsync(CancellationToken cancellationToken = default)
    {
        StringBuilder sb = new();

        var stylesheetsTask = GetByCriteriaAsync(new StyleSheetSearchCriteria(), cancellationToken);
        var response = await GetByFileNameAsync("app.css", cancellationToken);

        if (!response.IsSuccessWithData)
        {
            return BusinessResponse.Error<string>("Ocurrió un error inesperado");
        }

        sb.AppendLine(response.Data!.Content);

        var stylesheets = await stylesheetsTask;
        if (stylesheets.IsSuccessWithData)
        {
            foreach (var styleSheet in stylesheets.Data.Data)
            {
                if (styleSheet.FileName.Equals("app.css", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }
                sb.AppendLine(styleSheet.Content);
            }
        }

        var content = Uglify.Css(sb.ToString());
        return BusinessResponse.Success(content.Code);
    }
}