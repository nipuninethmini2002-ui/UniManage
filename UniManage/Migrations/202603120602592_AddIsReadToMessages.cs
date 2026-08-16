namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsReadToMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMessages", "IsRead", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMessages", "IsRead");
        }
    }
}
