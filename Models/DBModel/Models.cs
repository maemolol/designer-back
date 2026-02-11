using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models;

public class Paintings
{
    public Guid id { get; set; }

    [Column("height_id")]
    public int? Heightid { get; set; } 
    public Height? Height { get; set; }

    [Column("width_id")]
    public int? Widthid { get; set; } 
    public Width? Width { get; set; }

    [Column("category_id")]
    public int? Categoryid { get; set; }
    public Category? Category { get; set; }

    public string? name { get; set; }

    [Column("image_link")]
    public string? Imagelink { get; set; }
}

public class Height
{
    public int id { get; set; }
    public int? cm { get; set; }
}

public class Width
{
    public int id { get; set; } // UUID
    public int? cm { get; set; } // UUID of the user
}

public class Category
{
    public int id { get; set; }
    public string? cat { get; set; } // category name
}