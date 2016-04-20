namespace PixelPrinterService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IndexUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UserUnique", c => c.String(maxLength: 450));
            AlterColumn("dbo.Users", "UserName", c => c.String(maxLength: 450));
            CreateIndex("dbo.Users", "UserName", unique: true, name: "Index_UserName");
            CreateIndex("dbo.Users", "UserUnique", unique: true, name: "Index_UserUnique");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "Index_UserUnique");
            DropIndex("dbo.Users", "Index_UserName");
            AlterColumn("dbo.Users", "UserName", c => c.String());
            DropColumn("dbo.Users", "UserUnique");
        }
    }
}
