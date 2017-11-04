using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
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
			this.Food = new HashSet<Food>(food);
			this.OrderedAt = DateTime.Now;
			this.UserId = user.Id;
			this.User = user;
			this.OrderedAt = DateTime.Now;
		}


		[ForeignKey("User")]
		public int UserId { get; set; }
		public virtual User User { get; set; }

		public DateTime OrderedAt { get; set; }

		public ICollection<Food> Food { get; set; }

		public virtual ICollection<OrderFood> OrderFood { get; set; }
	}

	[Table("OrdersHasFood")]
	public class OrderFood
	{
		[Key, Column(Order = 0)]
		public int OrderId { get; set; }

		[Key, Column(Order = 1)]
		public int FoodId { get; set; }

		public virtual Food Food { get; set; }
		public virtual Order Order { get; set; }

		public int foodCount { get; set; }
	}
}
