namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEnrollmentAndStudent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enrollments", "EnrollmentDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enrollments", "EnrollmentDate");
        }
    }
}
