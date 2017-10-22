using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace final_project.Model
{
	[Table("Menu")]
	public class Food : Base
	{

		//Ensures the M:N relationship between tables Food nad Order
		public Food() {
			this.Order = new HashSet<Order>();
		}

		public string Name { get; set; }

		public string Path { get; set; }

		public decimal Price { get; set; }

		public int Gram { get; set; }

		public int EnergyKj { get; set; }

		public int EnergyKcal { get; set; }

		public decimal Protein { get; set; }

		public decimal TotalFat { get; set; }

		public decimal SaturatedFat { get; set; }

		public decimal Carbohydrates { get; set; }

		public decimal Sugar { get; set; }

		public decimal Salt { get; set; }

		public decimal Fiber { get; set; }

		public ICollection<Order> Order { get; set; }

	}
}
