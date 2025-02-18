﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Parsalud.BusinessLayer.Abstractions;
using System;
using System.Globalization;
using System.Text;

namespace Parsalud.Client.Components;

public partial class PostsViewer : ComponentBase
{
    public Paginated<ParsaludPost[]>? Posts { get; set; }
    public ParsaludPost[]? LatestPosts { get; set; }
    public ParsaludPostCategory[]? Categories { get; set; }

    private int currentPageNumber = 1;
    private Dictionary<string, StringValues>? query;
    private const string formato = "MMMM d, yyyy";
    private static readonly CultureInfo cultura = new CultureInfo("es-ES");

    protected override async Task OnInitializedAsync()
    {
        PostSearchCriteria criteria = GetCurrentCriteria();
        await GetPostsByCriteria(criteria);
        await GetCategories();
        await GetLatestPosts();
    }

    private bool IsChecked(Guid id)
    {
        Uri uri = new(NM.Uri);
        query ??= QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("c", out var values))
        {
            var culture = StringComparison.InvariantCultureIgnoreCase;
            return values.Any(x => x?.Equals(id.ToString(), culture) ?? false);
        }
        return false;
    }

    private string? GetInputTextValue()
    {
        Uri uri = new(NM.Uri);
        query ??= QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("s", out var value))
        {
            return value;
        }
        return null;
    }

    private PostSearchCriteria GetCurrentCriteria()
    {
        PostSearchCriteria criteria = new()
        {
            Page = 0,
            Size = 4,
        };

        Uri uri = new(NM.Uri);
        query = QueryHelpers.ParseQuery(uri.Query);

        foreach (var param in query)
        {
            switch (param.Key)
            {
                case "s":
                    criteria.Title = param.Value[0];
                    criteria.Content = param.Value[0];
                    break;
                case "c":
                    List<Guid> ids = [];
                    foreach (var c in param.Value)
                    {
                        if (Guid.TryParse(c, out var id))
                        {
                            ids.Add(id);
                        }
                    }
                    criteria.CategoryIds = [.. ids];
                    break;
                case "p":
                    if (int.TryParse(param.Value[0], out var pageInt))
                    {
                        currentPageNumber = pageInt;
                        criteria.Page = pageInt - 1;
                    }
                    break;
                default:
                    break;
            }
        }

        return criteria;
    }

    private async Task GetPostsByCriteria(PostSearchCriteria criteria)
    {
        var response = await PostService.GetByCriteriaAsync(criteria);

        if (response.IsSuccessWithData)
        {
            Posts = response.Data;
        }
        else
        {
            Posts = new Paginated<ParsaludPost[]>()
            {
                Data = [],
                Page = null,
                PageSize = null,
                TotalItems = 0
            };
        }
    }

    private async Task GetCategories()
    {
        var response = await PostCategoryService.GetByCriteriaAsync(new PostCategorySearchCriteria());
        if (response.IsSuccessWithData)
        {
            Categories = response.Data;
        }
        else
        {
            Categories = [];
        }
    }

    private async Task GetLatestPosts()
    {
        var response = await PostService.GetLatestAsync();
        if (response.IsSuccessWithData)
        {
            LatestPosts = response.Data;
        }
        else
        {
            LatestPosts = [];
        }
    }

    private string GetPaginatorItems(int count)
    {
        StringBuilder sb = new();
        var pages = (int)Math.Ceiling(count / 4d);

        Uri uri = new(NM.Uri);
        string urlSinQueryString = $"{uri.Scheme}://{uri.Host}{(uri.IsDefaultPort ? "" : ":" + uri.Port)}{uri.AbsolutePath}";
        Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(uri.Query);
        bool prevPrinted = false;
        bool nextPrinted = false;

        for (int pageNumber = 1; pageNumber <= pages; pageNumber++)
        {
            if (!prevPrinted && pages > 1)
            {
                var pagePrev = currentPageNumber - 1;
                query.Remove("p");
                query.Add("p", string.Format("{0}", pagePrev));
                sb.AppendLine(
                $"""
                <li class="page-item {(pagePrev <= 0 ? "disabled" : "")}">
                    <a class="page-link" href="{QueryHelpers.AddQueryString(urlSinQueryString, query)}" aria-label="Previous" data-enhance-nav="false">
                        <span aria-hidden="true">&laquo;</span>
                    </a>
                </li>
                """);
                prevPrinted = true;
            }

            query.Remove("p");
            query.Add("p", string.Format("{0}", pageNumber));
            var page = QueryHelpers.AddQueryString(urlSinQueryString, query);
            sb.AppendLine(
            $"""
            <li class="page-item {(currentPageNumber == pageNumber ? "active" : "")}">
                <a class="page-link" href="{page}" data-enhance-nav="false">{pageNumber}</a>
            </li>
            """);
        
            if (!nextPrinted && pageNumber == pages && pages > 1)
            {
                var pageNext = currentPageNumber + 1;
                query.Remove("p");
                query.Add("p", string.Format("{0}", pageNext));
                sb.AppendLine(
                $"""
                <li class="page-item {(pageNext > pages ? "disabled" : "")}">
                    <a class="page-link" href="{QueryHelpers.AddQueryString(urlSinQueryString, query)}" aria-label="Next" data-enhance-nav="false">
                        <span aria-hidden="true">&raquo;</span>
                    </a>
                </li>
                """);
                nextPrinted = true;
            }
        }

        return sb.ToString();
    }
}
