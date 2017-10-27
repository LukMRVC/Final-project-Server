using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Reflection;
namespace final_project.Model
{
	[Table("Menu")]
	public class Food : Base
	{

		//Ensures the M:N relationship between tables Food nad Order
		public Food() {
			this.Order = new HashSet<Order>();
            this.Allergen = new HashSet<Allergen>();
		}

        public Food(string name)
        {
            this.Name = name;
            this.Order = new HashSet<Order>();
            this.Allergen = new HashSet<Allergen>();
		}

		public Food(string[] values) {
            this.Order = new HashSet<Order>();
            this.Allergen = new HashSet<Allergen>();
			SetValues(values);

		}

		public void SetValues(string[] values) 
		{
			int integer;
			decimal dec;
			bool cat;
			this.Path = values[0];
            this.Name = values[1];
            this.Composition = values[4];
			decimal.TryParse(values[3], out dec);
			this.Price = dec;
			bool.TryParse(values[5], out cat);
			this.Category = cat;
			int.TryParse(values[2], out integer);
			this.Gram = integer;
			int.TryParse(values[6], out integer);
			this.EnergyKj = integer;
			int.TryParse(values[7], out integer);
			this.EnergyKcal = integer;
			decimal.TryParse(values[8], out dec);
			this.Protein = dec;
			decimal.TryParse(values[9], out dec);
			this.TotalFat = dec;
			decimal.TryParse(values[10], out dec);
			this.SaturatedFat = dec;
			decimal.TryParse(values[11], out dec);
			this.Carbohydrates = dec;
			decimal.TryParse(values[12], out dec);
			this.Sugar = dec;
			decimal.TryParse(values[13], out dec);
			this.Salt = dec;
			decimal.TryParse(values[14], out dec);
			this.Fiber = dec;
		}

		public void SetAllergenes(Allergen[] allergenes) {
			foreach (Allergen a in allergenes) {
				this.Allergen.Add(a);
			}
		}

		public string[] toStringArray() 
		{
			string[] arr = new string[this.GetType().GetProperties().Length];
			int i = 0;
			foreach (PropertyInfo prop in this.GetType().GetProperties()) 
			{
				arr[i] = prop.GetValue(this).ToString();
				++i;
			}
			return arr;
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

		public string Path { get; set; }

		public string Name { get; set; }

		public int Gram { get; set; }

		public decimal Price { get; set; }

		public string Composition { get; set; } 

		public bool Category { get; set; }

		public int EnergyKj { get; set; }

		public int EnergyKcal { get; set; }

		public decimal Protein { get; set; }

		public decimal Carbohydrates { get; set; }

		public decimal Sugar { get; set; }

		public decimal TotalFat { get; set; }

		public decimal SaturatedFat { get; set; }

		public decimal Fiber { get; set; }

		public decimal Salt { get; set; }

		public ICollection<Order> Order { get; set; }

		public ICollection<Allergen> Allergen { get; set; }

		public override string ToString()
		{
			return string.Format("[Food: Name={0}, Category={1}, Path={2}, Price={3}, Gram={4}, Composition={5}, EnergyKj={6}, EnergyKcal={7}, Protein={8}, TotalFat={9}, SaturatedFat={10}, Carbohydrates={11}, Sugar={12}, Salt={13}, Fiber={14}, Order={15}, Allergen={16}]", Name, Category, Path, Price, Gram, Composition, EnergyKj, EnergyKcal, Protein, TotalFat, SaturatedFat, Carbohydrates, Sugar, Salt, Fiber, Order, Allergen);
		}

		public string toCsvString() 
		{
			return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};", Path, Name, Gram, Price, Composition, Category, EnergyKj, EnergyKcal, Protein, Carbohydrates, Sugar, TotalFat, SaturatedFat, Fiber, Salt);
		}

		public int[] GetAllergenIds() 
		{
			List<int> list = new List<int>();
			foreach (Allergen a in Allergen) 
			{
				list.Add(a.Id);
			}
			return list.ToArray();

		}

		public string GetAllergenIdsString()
		{
			string str = "";
			bool rmv = false;
			foreach (Allergen a in Allergen)
			{
				rmv = true;
				str += a.Id + ",";
			}
			if(rmv)
				return str.Remove(str.Length - 1);
			return str;			

		}

	}

	
}
