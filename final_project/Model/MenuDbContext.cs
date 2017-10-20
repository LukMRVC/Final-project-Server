using System.ComponentModel.DataAnnotations;
using MySql.Data.Entity;
using System.Data.Entity;
namespace final_project.Model
{

	public abstract class Base { 
	
		[Key]
		public int Id { get; set; }
	
	}


	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class MenuDbContext : DbContext
	{
		//jednotlivé tabulky
		public DbSet<Food> Menu { get; set; }

		public DbSet<Order> Orders { get; set; }

		public DbSet<User> Users { get; set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			//Nastaví hodnoty decimal;
			modelBuilder.Entity<Food>().Property(x => x.Carbohydrates).HasPrecision(10, 2);
			modelBuilder.Entity<Food>().Property(x => x.Fiber).HasPrecision(10, 2);
			modelBuilder.Entity<Food>().Property(x => x.Protein).HasPrecision(10, 2);
			modelBuilder.Entity<Food>().Property(x => x.SaturatedFat).HasPrecision(10, 2);
			modelBuilder.Entity<Food>().Property(x => x.Salt).HasPrecision(10, 2);
			modelBuilder.Entity<Food>().Property(x => x.TotalFat).HasPrecision(10, 2);

			modelBuilder.Entity<Order>().HasMany<Food>(o => o.Food).WithMany(f => f.Order).Map(of => {
				of.ToTable("Order_has_Food");
			});
		}

		//Nastaví DatabaseInitializer tak, že se vytvoří nová databáze pokaždé, když se změní model
		public MenuDbContext() : base("name=MenuDbContext")
		{
			Database.SetInitializer<MenuDbContext>(new DropCreateDatabaseIfModelChanges<MenuDbContext>());
		}
	}
}
