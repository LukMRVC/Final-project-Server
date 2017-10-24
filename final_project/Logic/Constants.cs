using System.Collections.Generic;
using System.Data.Entity;
using final_project.Model;
namespace final_project
{
	public static class Constants
	{
		public const string CSV_FILE_NAME = "menu_data.csv";


		public static int IsIn(this Food instance, Food[] array) {
			for (int i = 0; i < array.Length; ++i) {
				if (instance.Name == array[i].Name)
					return i;
			}
			return -1;
		}

	}


}
