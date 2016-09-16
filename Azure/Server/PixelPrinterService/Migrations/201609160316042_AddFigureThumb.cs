namespace PixelPrinterService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFigureThumb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Figures", "ThumbId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Figures", "ThumbId");
        }
    }
}
