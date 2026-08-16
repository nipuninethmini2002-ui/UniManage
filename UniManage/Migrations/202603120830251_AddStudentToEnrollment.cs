namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStudentToEnrollment : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Enrollments", "StudentId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Enrollments", "StudentId");
            AddForeignKey("dbo.Enrollments", "StudentId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enrollments", "StudentId", "dbo.AspNetUsers");
            DropIndex("dbo.Enrollments", new[] { "StudentId" });
            AlterColumn("dbo.Enrollments", "StudentId", c => c.String());
        }
    }
}
