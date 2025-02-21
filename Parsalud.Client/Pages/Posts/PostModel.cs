using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Posts;

public class PostModel : IModelBase<PostModel, ManagePostRequest, ParsaludPost>
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? ImgSrc { get; set; }
    public Guid? PostCategoryId { get; set; }
    public string? PostCategory { get; set; }

    public PostModel FromDto(ParsaludPost dto)
    {
        return new PostModel
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            Content = dto.Content,
            ImgSrc = dto.ImgSrc,
            PostCategoryId = dto.PostCategoryId,
        };
    }

    public ManagePostRequest ToRequest()
    {
        return new ManagePostRequest
        {
            Title = Title ?? "",
            Content = Content ?? "",
            Description = Description ?? "",
            Hidden = false,
            PostCategoryId = PostCategoryId.GetValueOrDefault(),
            ImgSrc = ImgSrc ?? "",
        };
    }
}
