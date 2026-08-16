namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixPrerequisiteCascade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prerequisites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        PrerequisiteCourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .ForeignKey("dbo.Courses", t => t.PrerequisiteCourseId)
                .Index(t => t.CourseId)
                .Index(t => t.PrerequisiteCourseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prerequisites", "PrerequisiteCourseId", "dbo.Courses");
            DropForeignKey("dbo.Prerequisites", "CourseId", "dbo.Courses");
            DropIndex("dbo.Prerequisites", new[] { "PrerequisiteCourseId" });
            DropIndex("dbo.Prerequisites", new[] { "CourseId" });
            DropTable("dbo.Prerequisites");
        }
    }
}
