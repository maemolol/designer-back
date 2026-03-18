using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models;

public class Paintings
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("height_id")]
    public int? Heightid { get; set; } 
    public Height? Height { get; set; }

    [Column("width_id")]
    public int? Widthid { get; set; } 
    public Width? Width { get; set; }

    [Column("category_id")]
    public int? Categoryid { get; set; }
    public Category? Category { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("image_link")]
    public string? Imagelink { get; set; }

    [Column("price")]
    public float? Price {get; set;}

    [Column("sold")]
    public bool Sold { get; set; }
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