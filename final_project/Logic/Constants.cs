using System.Collections.Generic;
using System.Linq;
using System;
using final_project.Model;

namespace final_project
{
	public static class Constants
	{
		public const string SECRET = "8Hwt9evygMOU";

		public static string[] Prefixes = { "/login/", "/signup/", "/get_key/", "/get_food/", "/get_user_history/", "/order/", "/pay/", "/braintree_token/" };

		//Data z Braintree, které jsou dostal k sandboxovému účtu
		public static class Braintree
		{
			public const string MERCHANT_ID = "ghv94zkc2x36bxwc";
			public const string PUBLIC_KEY = "833vs2h75r24h67m";
			public const string PRIVATE_KEY = "704e435eb9199ea6f9210b786125c445";
		}

		//Vytvoří náhodný řetězec čísel
		public static string GenerateRandom(int length, Random rng)
		{
			const string pool = "0123456789";
			var chars = Enumerable.Range(0, length).Select(x => pool[rng.Next(0, pool.Length)]);
			return new string(chars.ToArray());
		}

		public const string CSV_FILE_NAME = "menu_data.csv";

		public static string[] Allergenes = { "Obiloviny obsahující lepek", "Korýši", "Vejce", "Ryby", "Jádra podzemnice olejné", "Sójové boby (sója)",
			"Mléko", "Skořápkové plody", "Celer", "Hořčice", "Sezamové semena", "Oxid siřičtý a siřičitany", "Vlčí bob", "Měkkýši"};

		//Najde, jestli konkrénní instance třídy Food se nachází v poli
		public static int IsIn(this Food instance, Food[] array)
		{
			for (int i = 0; i < array.Length; ++i)
			{
				if (instance.Name == array[i].Name)
					return i;
			}
			return -1;
		}

		//Najde index instance podle názvu z list
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

		//Najde konkrétní instanci podle názvu z listu
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

		//Vezme část pole
		public static string[] SubArray(this string[] array, int index, int length)
		{
			string[] result = new string[length - 1];
			for (int i = 0; i < length - 1; ++i)
			{
				result[i] = array[index + i];
			}
			return result;
		}

		//Vrátí počet užitečné délky pole, protože v poli mohou být duplikáty, nebo záznamy bez hodnot
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

		//Vrátí jen vybrané alergeny z pole
		public static Allergen[] GetAllergenes(this IEnumerable<Allergen> enumerable, int[] indices)
		{
			Allergen[] arr = new Allergen[indices.UsefulLength()];
			for (int i = 0; i < arr.Length; ++i)
			{
				arr[i] = enumerable.ElementAt((indices[i] - 1));
			}
			return arr;
		}

		//Tato funkce je tady proto, aby se dialog pro přídání jídla dobře obnovil
		//Pokud jídlo má alergen a checkbox má z názvu id toho alegenu, vrátí se true, aby se checkbox zaškrtnul
		public static bool Has(this int[] arr, int number)
		{
			foreach (int num in arr)
			{
				if (num == number)
					return true;
			}
			return false;
		}

		//Převede pole na JSON array
		//Toto bylo napsáno, než jsem začal používat JSON serializer
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

		//Najde duplikáty
		public static int Duplicates(this string[] arr, string id)		{
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
