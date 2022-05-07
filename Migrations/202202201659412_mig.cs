namespace ProjetGED.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Admins", "userID", "dbo.Utilisateurs");
            DropIndex("dbo.Admins", new[] { "userID" });
            DropTable("dbo.Admins");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        userID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.userID);
            
            CreateIndex("dbo.Admins", "userID");
            AddForeignKey("dbo.Admins", "userID", "dbo.Utilisateurs", "userID");
        }
    }
}
