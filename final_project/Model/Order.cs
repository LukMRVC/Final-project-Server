using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace final_project.Model
{
	[Table("Orders")]
	public class Order : Base
	{
		public Order() {
			this.Food = new HashSet<Food>();
		}


		[ForeignKey("User")]
		public int UserId { get; set; }
		public User User { get; set; }

		public ICollection<Food> Food { get; set; }
	}
}
