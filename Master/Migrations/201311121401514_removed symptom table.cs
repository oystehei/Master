namespace Master.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedsymptomtable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Symptoms", "PatientCase_Id", "dbo.PatientCases");
            DropIndex("dbo.Symptoms", new[] { "PatientCase_Id" });
            AddColumn("dbo.PatientCases", "Symptoms", c => c.String());
            DropTable("dbo.Symptoms");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Symptoms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        PatientCase_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.PatientCases", "Symptoms");
            CreateIndex("dbo.Symptoms", "PatientCase_Id");
            AddForeignKey("dbo.Symptoms", "PatientCase_Id", "dbo.PatientCases", "Id");
        }
    }
}
