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
            this.Allergen = new HashSet<Allergen>();
		}

		public void setValues(string[] values) 
		{
			this.Name = values[0];
			bool.TryParse(values[1], out this.Category); 
			this.Path = values[2];
			decimal.TryParse(values[3], out this.Price);
			int.TryParse(values[4], out this.Gram);
			int.TryParse(values[5], out this.EnergyKj);
			int.TryParse(values[6], out this.EnergyKcal);
			decimal.TryParse(values[7], out this.Protein);
			decimal.TryParse(values[8], out this.TotalFat);
			decimal.TryParse(values[9], out this.SaturatedFat);
			decimal.TryParse(values[10], out this.Carbohydrates);
			decimal.TryParse(values[11], out this.Sugar);
			decimal.TryParse(values[12], out this.Salt);
			decimal.TryParse(values[13], out this.Fiber);
		}

		public void SetValues(Food instance) {
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

		public ICollection<Allergen> Allergen { get; set; }

	}

	
}
