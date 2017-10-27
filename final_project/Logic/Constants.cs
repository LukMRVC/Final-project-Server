using System.Collections.Generic;
using System.Linq;
using final_project.Model;

namespace final_project
{
	public static class Constants
	{
		public const string CSV_FILE_NAME = "menu_data.csv";

		public static string[] Allergenes = { "Obiloviny obsahující lepek", "Korýši", "Vejce", "Ryby", "Jádra podzemnice olejné", "Sójové boby (sója)",
			"Mléko", "Skořápkové plody", "Celer", "Hořčice", "Sezamové semena", "Oxid siřičtý a siřičitany", "Vlčí bob", "Měkkýši"};

		public static int IsIn(this Food instance, Food[] array) {
			for (int i = 0; i < array.Length; ++i) {
				if (instance.Name == array[i].Name)
					return i;
			}
			return -1;
		}

		public static int FindFoodIndex(this List<Food> list, string name) 
		{
			Food[] arr = list.ToArray();
			for (int i = 0; i < arr.Length; ++i) 
			{
				if (arr[i].Name == name)
					return i;
			}
			return -1;
		}

		public static Food FindFood(this List<Food> list, string name)
		{
			Food[] arr = list.ToArray();
			for (int i = 0; i < arr.Length; ++i)
			{
				if (arr[i].Name == name)
					return arr[i];
			}
			throw new System.Exception("Food was not found");
		}

		public static string[] SubArray(this string[] array, int index, int length)
		{
			string[] result = new string[length-1];
			for (int i = 0; i < length-1; ++i) { 
				result[i] = array[index + i]; 
			}
			return result;
		}
	}
}
