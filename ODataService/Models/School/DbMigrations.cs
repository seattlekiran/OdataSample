using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataService.Models.School
{
    internal sealed class ContosoSchoolMigrationConfiguration : DbMigrationsConfiguration<ContosoSchoolContext>
    {
        public ContosoSchoolMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ContosoSchoolContext context)
        {
            context.Database.Delete();
            context.Database.CreateIfNotExists();
        }
    }
}
