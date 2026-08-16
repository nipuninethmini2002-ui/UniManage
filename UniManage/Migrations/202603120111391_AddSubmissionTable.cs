namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubmissionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Submissions",
                c => new
                    {
                        SubmissionId = c.Int(nullable: false, identity: true),
                        AssignmentId = c.Int(nullable: false),
                        StudentId = c.String(maxLength: 128),
                        FileName = c.String(),
                        SubmittedDate = c.DateTime(nullable: false),
                        Grade = c.String(),
                        Feedback = c.String(),
                    })
                .PrimaryKey(t => t.SubmissionId)
                .ForeignKey("dbo.Assignments", t => t.AssignmentId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.StudentId)
                .Index(t => t.AssignmentId)
                .Index(t => t.StudentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Submissions", "StudentId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Submissions", "AssignmentId", "dbo.Assignments");
            DropIndex("dbo.Submissions", new[] { "StudentId" });
            DropIndex("dbo.Submissions", new[] { "AssignmentId" });
            DropTable("dbo.Submissions");
        }
    }
}
