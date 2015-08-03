using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace MultiLingualPlayings
{
    public class Program
    {
        static void Main(string[] args)
        {
            var myService = new MyService(new MyDbContext());

            Log("Get English product translations");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            foreach (var product in myService.GetProductsInCurrentLanguage())
            {
                Console.WriteLine(product);
            }

            Log("Get Turkish product translation");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("tr");
            Console.WriteLine(myService.GetProductInCurrentLanguageWithFallbackToDefault(1));

            Log("Get French product translation");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("tr");
            Console.WriteLine(myService.GetProductInCurrentLanguageWithFallbackToDefault(1));

            Console.ReadLine();
        }

        private static void Log(string text)
        {
            Console.WriteLine(text);
        }
    }

    public class MyService
    {
        private readonly MyDbContext _context;

        public MyService(MyDbContext context)
        {
            _context = context;
        }

        public List<ProductTranslation> GetProductsInCurrentLanguage()
        {
            var language = GetCurrentLanguage();
            return _context.ProductTranslations.Where(p => p.Language == language).ToList();
        }

        public ProductTranslation GetProductInCurrentLanguageWithFallbackToDefault(int id)
        {
            return GetEntityTranslationWithFallbacks<Product, ProductTranslation>(_context.ProductTranslations, 1);
        }

        private TTranslation GetEntityTranslationWithFallbacks<TEntity, TTranslation>(IQueryable<TTranslation> dbSet, int id)
            where TTranslation : class, IEntityTranslation<TEntity>
        {
            //Get requested language
            var language = GetCurrentLanguage();
            var product = dbSet.FirstOrDefault(p => p.CoreId == id && p.Language == language);
            if (product != null)
            {
                return product;
            }

            //Get default language
            language = GetDefaultLanguage();
            product = dbSet.FirstOrDefault(p => p.CoreId == id && p.Language == language);
            if (product != null)
            {
                return product;
            }

            //Get first language
            product = dbSet.FirstOrDefault(p => p.CoreId == id);
            if (product != null)
            {
                return product;
            }

            throw new Exception("Could not found: " + id + " for entity type: " + typeof(TEntity).AssemblyQualifiedName);
        }

        public List<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        private static string GetCurrentLanguage()
        {
            return Thread.CurrentThread.CurrentUICulture.Name;
        }

        private string GetDefaultLanguage()
        {
            return "en";
        }
    }

    public interface IMultiLanguageEntity<TTranslation>
    {
        ICollection<TTranslation> Translations { get; set; }
    }

    public interface IEntityTranslation<TEntity>
    {
        TEntity Core { get; set; }

        int CoreId { get; set; }

        string Language { get; set; }
    }

    public class MyDbContext : DbContext
    {
        public IDbSet<Category> Categories { get; set; }

        public IDbSet<Product> Products { get; set; }

        public IDbSet<ProductTranslation> ProductTranslations { get; set; }

        public MyDbContext()
            : base("Default")
        {

        }
    }

    [Table("Products")]
    public class Product : IMultiLanguageEntity<ProductTranslation>
    {
        public virtual int Id { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public virtual int CategoryId { get; set; }

        public virtual int Price { get; set; }

        public virtual ICollection<ProductTranslation> Translations { get; set; }
    }

    [Table("ProductTranslations")]
    public class ProductTranslation : IEntityTranslation<Product>
    {
        public virtual int Id { get; set; }

        [ForeignKey("CoreId")]
        public virtual Product Core { get; set; }
        public virtual int CoreId { get; set; }

        public virtual string Language { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public override string ToString()
        {
            return string.Format(
                "[Product Id={0}, CategoryId={1}, Price={2}, Title={3}, Description={4}, Language={5}]",
                Id, Core.CategoryId, Core.Price, Title, Description, Language
                );
        }
    }

    public class Category
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
    }
}
