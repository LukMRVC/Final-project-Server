using System.Data.Common;
using MySql.Data.Entity;
using System.Data.Entity;
namespace final_project.Model
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class MenuDbContext : DbContext	
	{
		public DbSet<Food> Menu
		{
			get;
			set;
		}

		public MenuDbContext() : base("name=MenuDbContext")
		{
		}
	}
}
