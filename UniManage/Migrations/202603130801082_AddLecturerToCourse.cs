namespace UniManage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLecturerToCourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Courses", "UserId");
            AddForeignKey("dbo.Courses", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Courses", new[] { "UserId" });
            DropColumn("dbo.Courses", "UserId");
        }
    }
}
