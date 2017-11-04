using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MySql.Data.Entity;
using System.Data.Entity;
namespace final_project.Model
{

	public abstract class Base { 
	
		[Key]
		public int Id { get; set; }
	
	}

	public class CzechDbInitializer : DropCreateDatabaseIfModelChanges<MenuDbContext> { 
		protected override void Seed(MenuDbContext context)
		{
			//find database name from connection string to set collation to czech utf8
			string con = System.Configuration.ConfigurationManager.ConnectionStrings["MenuDbContext"].ConnectionString;
			string[] sub = con.Split(';');
			string dbName = "";
			foreach (string word in sub) {
				if (word.Contains("database=")){
					dbName = word.Substring(word.IndexOf('=')+1).ToLower();
					break;
				}
			}
			string values = "";
			foreach (string allergen in Constants.Allergenes) 
			{
				values += "('" +allergen + "'),";
			}
			values = values.Remove(values.Length - 1);
			context.Database.ExecuteSqlCommand("ALTER DATABASE " + dbName + " COLLATE utf8_czech_ci");
			context.Database.ExecuteSqlCommand("ALTER TABLE order_has_food CONVERT TO CHARACTER SET utf8 COLLATE utf8_czech_ci");
			context.Database.ExecuteSqlCommand("ALTER TABLE menu CONVERT TO CHARACTER SET utf8 COLLATE utf8_czech_ci");
			context.Database.ExecuteSqlCommand("ALTER TABLE users CONVERT TO CHARACTER SET utf8 COLLATE utf8_czech_ci");
			context.Database.ExecuteSqlCommand("ALTER TABLE orders CONVERT TO CHARACTER SET utf8 COLLATE utf8_czech_ci");
			context.Database.ExecuteSqlCommand("ALTER TABLE allergenes CONVERT TO CHARACTER SET utf8 COLLATE utf8_czech_ci");
			context.Database.ExecuteSqlCommand("INSERT INTO allergenes (name) VALUES" +values) ;
			base.Seed(context);
		}
	}


	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class MenuDbContext : DbContext
	{
		//Tables
		public DbSet<Food> Menu { get; set; }

		public DbSet<Order> Orders { get; set; }

		public DbSet<User> Users { get; set; }

		public DbSet<Allergen> Allergenes { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			//Sets decimals range in DB
			modelBuilder.Entity<Food>().Property(x => x.Carbohydrates).HasPrecision(10, 2);
			modelBuilder.Entity<Food>().Property(x => x.Fiber).HasPrecision(10, 2);
			modelBuilder.Entity<Food>().Property(x => x.Protein).HasPrecision(10, 2);
			modelBuilder.Entity<Food>().Property(x => x.SaturatedFat).HasPrecision(10, 2);
			modelBuilder.Entity<Food>().Property(x => x.Salt).HasPrecision(10, 2);
			modelBuilder.Entity<Food>().Property(x => x.TotalFat).HasPrecision(10, 2);

			modelBuilder.Entity<Order>().HasMany<Food>(o => o.Food).WithMany(f => f.Order).Map(of => {
				of.ToTable("Order_has_Food");
			});

			/*modelBuilder.Entity<Food>().HasMany<Allergen>(o => o.Allergen).WithMany(f => f.Food).Map(of => {
				of.ToTable("Food_has_Allergen");			
			});*/


		}

		public MenuDbContext(string connectionString) : base (connectionString) {
			Database.SetInitializer(new CzechDbInitializer());
		}


		public MenuDbContext() : base("name=MenuDbContext")
		{
			Database.SetInitializer(new CzechDbInitializer());
		}
	}

}
