namespace BinarApp.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Equipments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Address = c.String(),
                        GeoJson = c.String(),
                        DayImage = c.String(),
                        NightImage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Fixations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PUNKT = c.String(),
                        EntityId = c.String(),
                        FixationDate = c.DateTime(nullable: false),
                        PenaltySum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SY1 = c.String(),
                        R05 = c.String(),
                        GRNZ = c.String(nullable: false),
                        VU = c.String(),
                        PDD = c.String(),
                        Description = c.String(),
                        Speed = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        MiddleName = c.String(),
                        BirthDate = c.DateTime(nullable: false),
                        Image = c.String(),
                        NickName = c.String(maxLength: 50),
                        EquipmentId = c.Int(),
                        Intruder_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Equipments", t => t.EquipmentId)
                .ForeignKey("dbo.Intruders", t => t.Intruder_Id)
                .Index(t => t.EquipmentId)
                .Index(t => t.Intruder_Id);
            
            CreateTable(
                "dbo.FixationDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FixationId = c.Int(nullable: false),
                        Image = c.String(),
                        ImagePlate = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Fixations", t => t.FixationId, cascadeDelete: true)
                .Index(t => t.FixationId);
            
            CreateTable(
                "dbo.Intruders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        MiddleName = c.String(),
                        IIN = c.String(),
                        BirthDate = c.DateTime(nullable: false),
                        ImageUrl = c.String(),
                        ImageBlobName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Fixations", "Intruder_Id", "dbo.Intruders");
            DropForeignKey("dbo.FixationDetails", "FixationId", "dbo.Fixations");
            DropForeignKey("dbo.Fixations", "EquipmentId", "dbo.Equipments");
            DropIndex("dbo.FixationDetails", new[] { "FixationId" });
            DropIndex("dbo.Fixations", new[] { "Intruder_Id" });
            DropIndex("dbo.Fixations", new[] { "EquipmentId" });
            DropTable("dbo.Intruders");
            DropTable("dbo.FixationDetails");
            DropTable("dbo.Fixations");
            DropTable("dbo.Equipments");
        }
    }
}
