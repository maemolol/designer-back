using System.Text.Json.Serialization;

namespace Models;

public class Paintings
{
    public Guid id { get; set; }
    public int? height_id { get; set; } 
    public int? width_id { get; set; } 
    public int? category_id { get; set; } 
    public string? name { get; set; }
    public string? image_link { get; set; }
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
    public string? cat { get; set; } // species name
}