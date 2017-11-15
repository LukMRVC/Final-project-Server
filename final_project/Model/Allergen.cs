using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace final_project.Model
{
	[Table("Allergenes")]
	public class Allergen : Base
	{
		public Allergen() 
		{
			this.Food = new HashSet<Food>();
		}

		public string Name { get; set; }

		public virtual ICollection<Food> Food { get; set; }

	}
}
