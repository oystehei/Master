namespace Master.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedClassModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClassModels",
                c => new
                    {
                        Class = c.Int(nullable: false, identity: true),
                        ValueFrom = c.Int(nullable: false),
                        ValueTo = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Class);
            
            AddColumn("dbo.PatientCases", "ClassModel_Class", c => c.Int());
            CreateIndex("dbo.PatientCases", "ClassModel_Class");
            AddForeignKey("dbo.PatientCases", "ClassModel_Class", "dbo.ClassModels", "Class");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientCases", "ClassModel_Class", "dbo.ClassModels");
            DropIndex("dbo.PatientCases", new[] { "ClassModel_Class" });
            DropColumn("dbo.PatientCases", "ClassModel_Class");
            DropTable("dbo.ClassModels");
        }
    }
}
