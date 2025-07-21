using System.Text.Json.Serialization;

namespace Models;

public class paintings
{
    public Guid id { get; set; }
    public Guid? height_id { get; set; } 
    public Guid? width_id { get; set; } 
    public Guid? category_id { get; set; } 
}

public class height
{
    public Guid id { get; set; }
    public int? height { get; set; }
}

public class width
{
    public Guid id { get; set; } // UUID
    public int? width { get; set; } // UUID of the user
}

public class category
{
    public Guid id { get; set; }
    public string? category { get; set; } // species name
}