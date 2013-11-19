namespace Master.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tt : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientCases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Age = c.Int(nullable: false),
                        Sex = c.Int(nullable: false),
                        Race = c.Int(nullable: false),
                        TobaccoUse = c.Int(nullable: false),
                        Bmi = c.Double(nullable: false),
                        Symptoms = c.String(),
                        Diagnose = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PatientCases");
        }
    }
}
