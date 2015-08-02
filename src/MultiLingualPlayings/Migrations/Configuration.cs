using System.Collections.Generic;

namespace MultiLingualPlayings.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MultiLingualPlayings.MyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MultiLingualPlayings.MyDbContext context)
        {
            var monitorsCategory = context.Categories.FirstOrDefault(c => c.Name == "Monitors");
            if (monitorsCategory == null)
            {
                monitorsCategory = context.Categories.Add(new Category {Name = "Monitors"});
                context.SaveChanges();
            }

            var keyboardsCategory = context.Categories.FirstOrDefault(c => c.Name == "Keyboards");
            if (keyboardsCategory == null)
            {
                keyboardsCategory = context.Categories.Add(new Category { Name = "Keyboards" });
                context.SaveChanges();
            }

            var asusMonitor = context.Products.FirstOrDefault(p => p.CategoryId == monitorsCategory.Id && p.Price == 699);
            if (asusMonitor == null)
            {
                asusMonitor = context.Products.Add(
                    new Product
                    {
                        CategoryId = monitorsCategory.Id,
                        Price = 699,
                        Translations = new List<ProductTranslation>
                        {
                            new ProductTranslation
                            {
                                Language = "en",
                                Title = "A monitor English title",
                                Description = "A monitor English description"
                            },
                            new ProductTranslation
                            {
                                Language = "tr",
                                Title = "A monitor Turkish title",
                                Description = "A monitor Turkish description"
                            }
                        }
                    });
                context.SaveChanges();
            }
        }
    }
}
