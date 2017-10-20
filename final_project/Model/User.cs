using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace final_project.Model
{
	[Table("Users")]
	public class User : Base
	{
		public string Username { get; set; }

		public string Email { get; set; }

		public ICollection<Order> Order { get; set; }

		public string Password { get; set; }

	}
}
