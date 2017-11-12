using System.Collections.Generic;
using System.Linq;
using System;
using final_project.Model;

namespace final_project
{
	public static class Constants
	{
		public const string SECRET = "8Hwt9evygMOU";

		public static class Braintree
		{
			public const string MERCHANT_ID = "ghv94zkc2x36bxwc";
			public const string PUBLIC_KEY = "833vs2h75r24h67m";
			public const string PRIVATE_KEY = "704e435eb9199ea6f9210b786125c445";
		}


		public static string GenerateRandom(int length, Random rng)
		{
			const string pool = "0123456789";
			var chars = Enumerable.Range(0, length).Select(x => pool[rng.Next(0, pool.Length)]);
			return new string(chars.ToArray());
		}

		public const string CSV_FILE_NAME = "menu_data.csv";

		public static string[] Allergenes = { "Obiloviny obsahující lepek", "Korýši", "Vejce", "Ryby", "Jádra podzemnice olejné", "Sójové boby (sója)",
			"Mléko", "Skořápkové plody", "Celer", "Hořčice", "Sezamové semena", "Oxid siřičtý a siřičitany", "Vlčí bob", "Měkkýši"};

		public static int IsIn(this Food instance, Food[] array)
		{
			for (int i = 0; i < array.Length; ++i)
			{
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
			string[] result = new string[length - 1];
			for (int i = 0; i < length - 1; ++i)
			{
				result[i] = array[index + i];
			}
			return result;
		}

		public static int UsefulLength(this int[] arr)
		{
			int length = 0;
			for (int i = 0; i < arr.Length; ++i)
			{
				if (arr[i] != 0)
					++length;
			}
			return length;
		}

		public static Allergen[] GetAllergenes(this IEnumerable<Allergen> enumerable, int[] indices)
		{
			Allergen[] arr = new Allergen[indices.UsefulLength()];
			for (int i = 0; i < arr.Length; ++i)
			{
				arr[i] = enumerable.ElementAt((indices[i] - 1));
			}
			return arr;
		}

		public static bool Has(this int[] arr, int number)
		{
			foreach (int num in arr)
			{
				if (num == number)
					return true;
			}
			return false;
		}

		public static string toJsonArray(this int[] arr)
		{
			string json = "[";
			foreach (int i in arr)
			{
				json += i + ",";
			}
			if (arr.Length != 0)
				json = json.Remove(json.Length - 1);
			json += "]";
			return json;
		}

		public static int Duplicates(this string[] arr, string id) 
		{
			int duplicates = 0;
			foreach (string d in arr) 
			{
				if (d == id)
					++duplicates;
			}
			return duplicates;
		
		}
	}
}
