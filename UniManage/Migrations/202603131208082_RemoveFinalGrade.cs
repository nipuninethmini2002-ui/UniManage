namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFinalGrade : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Enrollments", "FinalGrade");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Enrollments", "FinalGrade", c => c.String());
        }
    }
}
