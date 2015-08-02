namespace MultiLingualPlayings.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ProductTranslations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CoreId = c.Int(nullable: false),
                        Language = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.CoreId)
                .Index(t => t.CoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductTranslations", "CoreId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropIndex("dbo.ProductTranslations", new[] { "CoreId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropTable("dbo.ProductTranslations");
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
        }
    }
}
