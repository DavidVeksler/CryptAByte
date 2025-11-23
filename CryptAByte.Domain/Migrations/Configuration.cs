namespace CryptAByte.Domain.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class MyMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messags",
                c => new
                         {
                             Created = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                         });
            //.PrimaryKey(t => t.);
        }
    }

    internal sealed class Configuration : DbMigrationsConfiguration<CryptAByte.Domain.DataContext.CryptAByteContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
            
        }


        protected override void Seed(CryptAByte.Domain.DataContext.CryptAByteContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
