using System.Text.Json.Serialization;

namespace Models;

public class paintings
{
    public Guid id { get; set; }
    public int? height_id { get; set; } 
    public int? width_id { get; set; } 
    public int? category_id { get; set; } 
    public string? name { get; set; }
}

public class height
{
    public int id { get; set; }
    public int? cm { get; set; }
}

public class width
{
    public int id { get; set; } // UUID
    public int? cm { get; set; } // UUID of the user
}

public class category
{
    public int id { get; set; }
    public string? cat { get; set; } // species name
}