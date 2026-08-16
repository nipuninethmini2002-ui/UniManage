namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsLateToSubmission : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Submissions", "SubmissionDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Submissions", "IsLate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Submissions", "IsLate");
            DropColumn("dbo.Submissions", "SubmissionDate");
        }
    }
}
