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

        public Food(string name)
        {
            this.Name = name;
            this.Order = new HashSet<Order>();
        }

		public void SetValuesFromInstance(Food instance) {
			this.Name = instance.Name;
			this.Category = instance.Category;
			this.Path = instance.Path;
			this.Price = instance.Price;
			this.Gram = instance.Gram;
			this.EnergyKj = instance.EnergyKj;
			this.EnergyKcal = instance.EnergyKcal;
			this.Protein = instance.Protein;
			this.TotalFat = instance.TotalFat;
			this.SaturatedFat = instance.SaturatedFat;
			this.Carbohydrates = instance.Carbohydrates;
			this.Sugar = instance.Sugar;
			this.Salt = instance.Salt;
			this.Fiber = instance.Fiber;
			
		}

		public string Name { get; set; }

		public bool Category { get; set; }

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
