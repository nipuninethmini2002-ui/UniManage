namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDateTimeIssue : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Submissions", "SubmittedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Submissions", "SubmittedDate", c => c.DateTime(nullable: false));
        }
    }
}
