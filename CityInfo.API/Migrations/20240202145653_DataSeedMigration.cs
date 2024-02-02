using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityInfo.API.Migrations
{
    public partial class DataSeedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Descripion", "Name" },
                values: new object[] { 1, "The one with big park.", "New York City" });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Descripion", "Name" },
                values: new object[] { 2, "The one with the cathedral that was never really finished.", "Antwerp" });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Descripion", "Name" },
                values: new object[] { 3, "The one with that big tower.", "Paris" });

            migrationBuilder.InsertData(
                table: "PointsOfInterests",
                columns: new[] { "Id", "CityId", "Description", "Name" },
                values: new object[] { 1, 1, "The most visited urban park in the USA.", "Central Park" });

            migrationBuilder.InsertData(
                table: "PointsOfInterests",
                columns: new[] { "Id", "CityId", "Description", "Name" },
                values: new object[] { 2, 1, "A 102-story skyscraper located in Midtown Manhattan.", "Empire State Building" });

            migrationBuilder.InsertData(
                table: "PointsOfInterests",
                columns: new[] { "Id", "CityId", "Description", "Name" },
                values: new object[] { 3, 2, "A Gothic style cathedral.", "Cathedral" });

            migrationBuilder.InsertData(
                table: "PointsOfInterests",
                columns: new[] { "Id", "CityId", "Description", "Name" },
                values: new object[] { 4, 2, "The finest example of railway architecture in Belgium.", "Antwerp Central Station" });

            migrationBuilder.InsertData(
                table: "PointsOfInterests",
                columns: new[] { "Id", "CityId", "Description", "Name" },
                values: new object[] { 5, 3, "A wrought iron tower on the Champ de Mars.", "Eifel Tower" });

            migrationBuilder.InsertData(
                table: "PointsOfInterests",
                columns: new[] { "Id", "CityId", "Description", "Name" },
                values: new object[] { 6, 3, "The world's largest museum.", "The Louver" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PointsOfInterests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PointsOfInterests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PointsOfInterests",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PointsOfInterests",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PointsOfInterests",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PointsOfInterests",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
