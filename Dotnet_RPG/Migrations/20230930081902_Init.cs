using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dotnet_RPG.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Damage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HitPoints = table.Column<int>(type: "int", nullable: false),
                    Strength = table.Column<int>(type: "int", nullable: false),
                    Defense = table.Column<int>(type: "int", nullable: false),
                    Intelligence = table.Column<int>(type: "int", nullable: false),
                    Class = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSkill",
                columns: table => new
                {
                    CharactersId = table.Column<int>(type: "int", nullable: false),
                    SkillsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSkill", x => new { x.CharactersId, x.SkillsId });
                    table.ForeignKey(
                        name: "FK_CharacterSkill_Characters_CharactersId",
                        column: x => x.CharactersId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterSkill_Skills_SkillsId",
                        column: x => x.SkillsId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weapons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Damage = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Weapons_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Damage", "Name" },
                values: new object[,]
                {
                    { 1, 30, "Fireball" },
                    { 2, 20, "Frenzy" },
                    { 3, 50, "Blizzard" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "PasswordSalt", "Username" },
                values: new object[,]
                {
                    { 1, new byte[] { 194, 208, 130, 191, 72, 82, 148, 215, 5, 133, 12, 37, 8, 72, 182, 219, 254, 249, 210, 164, 249, 0, 246, 119, 68, 39, 162, 212, 236, 114, 165, 5, 8, 23, 165, 185, 34, 189, 37, 175, 255, 235, 107, 61, 124, 69, 125, 22, 62, 119, 27, 5, 207, 162, 93, 48, 153, 109, 140, 80, 38, 189, 166, 118 }, new byte[] { 101, 4, 122, 50, 48, 58, 78, 229, 118, 206, 81, 54, 227, 155, 105, 127, 24, 63, 248, 68, 53, 176, 122, 33, 104, 35, 71, 208, 105, 108, 6, 59, 52, 242, 14, 175, 199, 195, 139, 127, 165, 65, 50, 8, 73, 23, 55, 213, 212, 96, 166, 179, 54, 10, 143, 218, 82, 193, 66, 54, 84, 39, 61, 247, 219, 3, 207, 58, 146, 40, 80, 60, 62, 127, 38, 120, 0, 95, 248, 63, 77, 247, 191, 229, 138, 207, 205, 165, 146, 125, 16, 243, 134, 126, 234, 196, 37, 144, 116, 206, 80, 59, 183, 50, 200, 129, 122, 252, 110, 78, 115, 41, 208, 229, 246, 82, 124, 141, 122, 205, 126, 44, 17, 37, 84, 204, 143, 241 }, "Nico" },
                    { 2, new byte[] { 194, 208, 130, 191, 72, 82, 148, 215, 5, 133, 12, 37, 8, 72, 182, 219, 254, 249, 210, 164, 249, 0, 246, 119, 68, 39, 162, 212, 236, 114, 165, 5, 8, 23, 165, 185, 34, 189, 37, 175, 255, 235, 107, 61, 124, 69, 125, 22, 62, 119, 27, 5, 207, 162, 93, 48, 153, 109, 140, 80, 38, 189, 166, 118 }, new byte[] { 101, 4, 122, 50, 48, 58, 78, 229, 118, 206, 81, 54, 227, 155, 105, 127, 24, 63, 248, 68, 53, 176, 122, 33, 104, 35, 71, 208, 105, 108, 6, 59, 52, 242, 14, 175, 199, 195, 139, 127, 165, 65, 50, 8, 73, 23, 55, 213, 212, 96, 166, 179, 54, 10, 143, 218, 82, 193, 66, 54, 84, 39, 61, 247, 219, 3, 207, 58, 146, 40, 80, 60, 62, 127, 38, 120, 0, 95, 248, 63, 77, 247, 191, 229, 138, 207, 205, 165, 146, 125, 16, 243, 134, 126, 234, 196, 37, 144, 116, 206, 80, 59, 183, 50, 200, 129, 122, 252, 110, 78, 115, 41, 208, 229, 246, 82, 124, 141, 122, 205, 126, 44, 17, 37, 84, 204, 143, 241 }, "Giu" }
                });

            migrationBuilder.InsertData(
                table: "Characters",
                columns: new[] { "Id", "Class", "Defense", "HitPoints", "Intelligence", "Name", "Strength", "UserId" },
                values: new object[,]
                {
                    { 1, 1, 20, 50, 10, "Frodo", 60, 1 },
                    { 2, 1, 40, 80, 60, "Sam", 10, 1 },
                    { 3, 1, 100, 200, 100, "Sauron", 100, 2 }
                });

            migrationBuilder.InsertData(
                table: "Weapons",
                columns: new[] { "Id", "CharacterId", "Damage", "Name" },
                values: new object[,]
                {
                    { 1, 1, 200, "Excalibur" },
                    { 2, 2, 150, "Master Sword" },
                    { 3, 3, 100, "Ashbringer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSkill_SkillsId",
                table: "CharacterSkill",
                column: "SkillsId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_CharacterId",
                table: "Weapons",
                column: "CharacterId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterSkill");

            migrationBuilder.DropTable(
                name: "Weapons");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
