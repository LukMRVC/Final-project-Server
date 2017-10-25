using System.Data.Entity;
using MySql.Data.Entity;
using System.ComponentModel.DataAnnotations;
namespace final_project.Model
{
	public class Category : Base
	{
		public Category()
		{
		}

		public string Name { get; set; }

		public string Path { get; set; }

		public bool IsSubCategory { get; set; }
	}
}
