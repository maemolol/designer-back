using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Models;
using System;

public static class DbInitializer
{
    private static readonly List<category> RequiredCategories = new()
    {
        new category { id = 1, cat = "Cute animals" },
        new category { id = 2, cat = "Fairytales" },
        new category { id = 3, cat = "Animal planet" },
        new category { id = 4, cat = "Flowers" }
    };

    public static async Task EnsureDbIsInitializedAsync(AppDbContext db)
    {
        Console.WriteLine("🔍 Checking categories...");

        foreach (var categories in RequiredCategories)
        {
            var exists = await db.category.AnyAsync(s => s.id == categories.id);
            if (!exists)
            {
                db.category.Add(categories);
                Console.WriteLine($"➕ Added missing category: {categories.cat} (id={categories.id})");
            }
        }

        await db.SaveChangesAsync();
        Console.WriteLine("✅ Category check complete.");
    }
}
