using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Models;
using System;

public static class DbInitializer
{
    private static readonly List<Category> RequiredCategories = new()
    {
        new Category { id = 1, cat = "Cute animals" },
        new Category { id = 2, cat = "Fairytales" },
        new Category { id = 3, cat = "Animal planet" },
        new Category { id = 4, cat = "Flowers" }
    };

    public static async Task EnsureDbIsInitializedAsync(AppDbContext db)
    {
        Console.WriteLine("🔍 Checking categories...");

        foreach (var categories in RequiredCategories)
        {
            var exists = await db.Category.AnyAsync(s => s.id == categories.id);
            if (!exists)
            {
                db.Category.Add(categories);
                Console.WriteLine($"➕ Added missing category: {categories.cat} (id={categories.id})");
            }
        }

        await db.SaveChangesAsync();
        Console.WriteLine("✅ Category check complete.");
    }
}
