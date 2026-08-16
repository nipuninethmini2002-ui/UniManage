namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUploadedByProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseMaterials", "UploadedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseMaterials", "UploadedBy");
        }
    }
}
