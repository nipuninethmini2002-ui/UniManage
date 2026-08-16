namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCourseMaterialsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseMaterials",
                c => new
                    {
                        MaterialId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        FilePath = c.String(),
                        UploadDate = c.DateTime(nullable: false),
                        CourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaterialId)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseMaterials", "CourseId", "dbo.Courses");
            DropIndex("dbo.CourseMaterials", new[] { "CourseId" });
            DropTable("dbo.CourseMaterials");
        }
    }
}
