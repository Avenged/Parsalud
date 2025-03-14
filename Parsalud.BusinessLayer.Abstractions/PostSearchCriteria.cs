﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Parsalud.BusinessLayer.Abstractions;

public class PostSearchCriteria
{
    public Guid[]? ExcludedIds { get; set; }
    public Guid[]? Ids { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool IncludeHidden { get; set; }
    public Guid[]? CategoryIds { get; set; }
    public int? Page { get; set; }
    public int? Size { get; set; }

    [NotMapped]
    [JsonIgnore]
    public IEnumerable<Guid>? EnumCategoryIds 
    { 
        get => CategoryIds; 
        set
        {
            CategoryIds = value?.ToArray();
        }
    }
}