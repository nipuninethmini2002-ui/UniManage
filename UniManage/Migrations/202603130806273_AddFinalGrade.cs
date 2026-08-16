namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinalGrade : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enrollments", "FinalGrade", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enrollments", "FinalGrade");
        }
    }
}
