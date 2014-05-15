using System.Data.Entity.Migrations;

namespace Scrummage.Migrations {
	public partial class RoleMemberAvatarMigration : DbMigration {
		public override void Up() {
			CreateTable(
					"dbo.Avatars",
					c => new {
						MemberId = c.Int(nullable: false),
						Image = c.Binary(nullable: false),
					})
					.PrimaryKey(t => t.MemberId)
					.ForeignKey("dbo.Members", t => t.MemberId)
					.Index(t => t.MemberId);

			CreateTable(
					"dbo.Members",
					c => new {
						MemberId = c.Int(nullable: false, identity: true),
						Name = c.String(nullable: false, maxLength: 30),
						ShortName = c.String(nullable: false, maxLength: 3),
						Username = c.String(nullable: false, maxLength: 30),
						Password = c.String(nullable: false),
						Email = c.String(nullable: false),
					})
					.PrimaryKey(t => t.MemberId);

			CreateTable(
					"dbo.Roles",
					c => new {
						RoleId = c.Int(nullable: false, identity: true),
						Title = c.String(nullable: false, maxLength: 30),
						Description = c.String(maxLength: 180),
					})
					.PrimaryKey(t => t.RoleId);

			CreateTable(
					"dbo.MemberRoles",
					c => new {
						Member_MemberId = c.Int(nullable: false),
						Role_RoleId = c.Int(nullable: false),
					})
					.PrimaryKey(t => new { t.Member_MemberId, t.Role_RoleId })
					.ForeignKey("dbo.Members", t => t.Member_MemberId, cascadeDelete: true)
					.ForeignKey("dbo.Roles", t => t.Role_RoleId, cascadeDelete: true)
					.Index(t => t.Member_MemberId)
					.Index(t => t.Role_RoleId);
		}

		public override void Down() {
			DropForeignKey("dbo.Avatars", "MemberId", "dbo.Members");
			DropForeignKey("dbo.MemberRoles", "Role_RoleId", "dbo.Roles");
			DropForeignKey("dbo.MemberRoles", "Member_MemberId", "dbo.Members");
			DropIndex("dbo.MemberRoles", new[] { "Role_RoleId" });
			DropIndex("dbo.MemberRoles", new[] { "Member_MemberId" });
			DropIndex("dbo.Avatars", new[] { "MemberId" });
			DropTable("dbo.MemberRoles");
			DropTable("dbo.Roles");
			DropTable("dbo.Members");
			DropTable("dbo.Avatars");
		}
	}
}
