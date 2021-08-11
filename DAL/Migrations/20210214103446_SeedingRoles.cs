using Allprimetech.Interfaces.Roles;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Allprimetech.DAL.Migrations
{
    public partial class SeedingRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            int[] rolesPositions = (int[])Enum.GetValues(typeof(RolesEnum));

            foreach (RolesEnum item in rolesPositions)
            {
                var result = Definition.GetRoleValue(item);
                string queryString = String.Format("INSERT INTO ApplicationRoles (_RoleName) VALUES ({0})", result._Value);
                migrationBuilder.Sql(queryString);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            int[] rolesPositions = (int[])Enum.GetValues(typeof(RolesEnum));
            foreach (RolesEnum item in rolesPositions)
            {
                var result = Definition.GetRoleValue(item);
                string queryString = String.Format("Delete From ApplicationRoles WHERE _RoleName = {0}", result._Value);
                migrationBuilder.Sql(queryString);
            }
        }
    }
}
