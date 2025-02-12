﻿using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Posts;

public class PostModel
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public Guid? PostCategoryId { get; set; }

    public ManagePostRequest ToRequest()
    {
        return new ManagePostRequest
        {
            Title = Title ?? "",
            Content = Content ?? "",
            Hidden = false,
            PostCategoryId = PostCategoryId.GetValueOrDefault(),
        };
    }
}
