using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Models;
using System;

public static class DbInitializer
{
    private static readonly List<Categories> RequiredCategories = new()
    {
        new Categories { id = 1, name = "Cute animals" },
        new Categories { id = 2, name = "Fairytales" },
        new Categories { id = 3, name = "Animal planet" },
        new Categories { id = 4, name = "Flowers" }
    };

    public static async Task EnsureDbIsInitializedAsync(AppDbContext db)
    {
        Console.WriteLine("🔍 Checking categories...");

        foreach (var categories in RequiredCategories)
        {
            var exists = await db.Species.AnyAsync(s => s.id == categories.id);
            if (!exists)
            {
                db.Species.Add(categories);
                Console.WriteLine($"➕ Added missing category: {categories.name} (id={categories.id})");
            }
        }

        await db.SaveChangesAsync();
        Console.WriteLine("✅ Category check complete.");
    }
}
