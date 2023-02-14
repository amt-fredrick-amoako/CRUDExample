using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePersonStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_UpdatePerson = @"
                CREATE PROCEDURE [dbo].[UpdatePerson]()
                AS BEGIN

                END
            ";

            migrationBuilder.Sql(sp_UpdatePerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
