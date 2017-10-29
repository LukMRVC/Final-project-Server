using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
namespace final_project.Model
{
	[Table("Orders")]
	public class Order : Base
	{
		public Order()
		{
			this.Food = new HashSet<Food>();
			this.OrderedAt = DateTime.Now;
		}

		public Order(User user, Food[] food)
		{
			this.Food = new HashSet<Food>();
			this.OrderedAt = DateTime.Now;
			this.UserId = user.Id;
			this.User = user;
            this.OrderedAt = DateTime.Now;
			foreach (Food f in food) 
			{
				this.Food.Add(f);
			}
		}


		[ForeignKey("User")]
		public int UserId { get; set; }
		public virtual User User { get; set; }

		public DateTime OrderedAt { get; set; }

		public ICollection<Food> Food { get; set; }
	}
}
