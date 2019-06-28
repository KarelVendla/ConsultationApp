namespace ConsultationApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consultations", "TUsername", c => c.String());
            AddColumn("dbo.Consultations", "SUsername", c => c.String());
            DropColumn("dbo.Consultations", "ValidThrough");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Consultations", "ValidThrough", c => c.DateTime());
            DropColumn("dbo.Consultations", "SUsername");
            DropColumn("dbo.Consultations", "TUsername");
        }
    }
}
