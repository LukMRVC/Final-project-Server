using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using MySql;
using Gtk;
using System.Linq;
using final_project.Model;

namespace final_project
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Application.Init();
			Server server = new Server();
			Application.Run();
		}
	}

	public class Server
	{
		private MainWindow win;
		public string databaseConnectionString;
		public MenuDbContext database;
		public RsaCryption cryption;
		public bool isConnected { get; private set; }
		public Server()		{
			//Vytvoří se nová instance třídy pro RSA šifrování
			this.cryption = new RsaCryption();
			//Přiradí se reference na třídy
			Http.server = this;
			Http.cryptor = this.cryption;
			Token.cryption = this.cryption;
			this.win = new MainWindow(this);
			try
			{
				//Připojení k databázi pomocí dat z app.config
				database = new MenuDbContext();
				database.Database.Initialize(true);
				this.win.statbar.Push(0, @"Připojeno k databázi");
				this.isConnected = true;
			}
			catch (Exception)
			{				connect();
			}
			win.Show();
		}

		/*public void CollectionChanged(object o, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
		{
			Console.WriteLine("Order changed");
		}*/

		//Připojení pomocí setting, které občas nespolupracují... 
		//Naštěstí by tato aplikace měla stejně být programátorsky upravena pro každého, kdo ji chce použít
		public void connect()
		{
			try
			{
				database.Database.Connection.ConnectionString = Properties.Settings.Default.databaseConnectionString;
				database.Database.Initialize(true);
				this.win.statbar.Push(0, @"Připojeno k databázi");
				this.isConnected = true;
			}
			catch (Exception)
			{
				this.isConnected = false;
				this.win.statbar.Push(0, @"Nenalezeno připojení k databázi");
			}

		}

		//To co není v array se smaže
		//to co v array je navíc, se přidá

		public void CompareAndSave(Food[] comparator)
		{

			//Kontrola, aby se z DB smazali staré záznamy a aktualizace upravených 
			for (int i = 0; i < database.Menu.Count(); ++i)
			{
				//Opravdu zvlášní způsob, jak procházet entity... Seřadí se, a vždy se přeskočí o i entit a vybere jedna.
				var instance = database.Menu.OrderBy(s => s.Id).Skip(i).First();
				int result = instance.IsIn(comparator);
				if (result != -1)
				{
					//Změní se stav entity, aby se naznačilo, že se entita upravila
					database.Entry(instance).State = System.Data.Entity.EntityState.Modified;
					database.Menu.OrderBy(s => s.Id).Skip(i).First().SetValues(comparator[result]);
					continue;
				}
				//Smazání entity
				else if (result == -1)
				{
					database.Menu.Remove(instance);
					continue;
				}
			}

			//Přidání nových záznamů
			for (int i = 0; i < comparator.Length; ++i)
			{
				//Pokud entita z porovnávacího pole není v již v DB, přidá se 
				if (comparator[i].IsIn(database.Menu.ToArray()) == -1)
				{

					database.Menu.Add(comparator[i]);
				}
			}
			try
			{
				database.SaveChangesAsync();
			}
			catch (Exception ex) { Console.WriteLine(ex.ToString()); }
		}

		//Získá z DB záznamy jídel a vrátí je jako jednu z instancí z rozhraní IEnumerable
		public IEnumerable<Food> getMenuData()
		{
			var data = (from Food in database.Menu.ToList() select Food);
			return data;
		}


		//Získá vazby alergenu k jídlu pomoci id jídla
		public int[] GetAllergenes(int foodId)
		{
			var query = this.database.Database.SqlQuery<string>("SELECT Allergen_Id FROM foodallergens WHERE Food_Id=" + foodId);
			var list = query.ToList();
			var w = list.Select(Int32.Parse).ToArray();
			return w;
		}

		//Přidá objednávku
		public void AddOrder(string[] orderArr, int uid, string totalPrice)
		{
			try
			{				var food = new List<Food>();
				//Najde uživatel, který zadal objednávnku
				User user = this.database.Users.Find(uid);
				foreach (string foodId in orderArr)				{
					//Pro každé id jídla z pole objednávku se najde entita a přidá do listu
					food.Add(database.Menu.Find(Int32.Parse(foodId)));
				}
				//Vytvoří se nová objednávka, který se spojí s uživatelem a objednaným jídlem
				var order = new Order(user, food.ToArray());
				//Také cena se samozřejmě uloží
				order.TotalPrice = Decimal.Parse(totalPrice);
				//A přídá do DB
				database.Orders.Add(order);
				//Duplikáty jídel
				var duplicates = new List<string>();
				//Ty se najdou a uloží jen pro elegantnější zobraní počtu jídla v objednávce např. 5x Tofu
				foreach (string foodId in orderArr)
				{
					
					if (duplicates.Contains(foodId))
						continue;
					database.OrderFood.Add(new OrderFood { Order = order, Food = database.Menu.Find(Int32.Parse(foodId)), foodCount = orderArr.Duplicates(foodId) });
					duplicates.Add(foodId);
				}
				database.SaveChanges();
				//A objednávka se zobrazí v hlavním okně
				this.win.PushToNodeView(order.Id, database.OrderFood.Where(s => s.OrderId == order.Id).AsEnumerable(), order.TotalPrice.ToString(), order.OrderedAt.ToString("HH:mm:ss"));
			}
			catch (Exception ex) { Console.WriteLine(ex.ToString()); }
		}

		//Přidání uživatele s heslem a mailem, heslo je uložené v hashi
		public User AddUser(string password, string email)
		{
			//Kontrola, jestli už uživatel se stejným mailem není zapsán
			if (CheckUserUniqueConstraint(email))			{
				var user = new User(password, email);
				this.database.Users.Add(user);
				this.database.SaveChanges();
				return user;
			}
			else
			{
				throw new Exception("Unique constraint violation exception");
			}

		}

		public void ImportData(IEnumerable<string> menuData)
		{
			try
			{
				//Smazání všech dat z DB
				this.database.Database.ExecuteSqlCommand("TRUNCATE TABLE foodallergens");				this.database.Database.ExecuteSqlCommand("DELETE FROM menu");
			}
			catch (Exception e) { Console.WriteLine(e.ToString()); }
			//A jejich přidání ze souboru
			foreach (string[] arr in menuData.Select(line => line.Split(';')).ToArray())
			{
				Food f = new Food(arr);
				this.database.Menu.Add(f);
			}
			this.database.SaveChangesAsync();
		}

		private bool CheckUserUniqueConstraint(string email)
		{
			//Check by expcetion, if exception is thrown, user doesnt exist and can be added
			try
			{				var mail = (from u in this.database.Users.ToList() where u.Email == email select u).First();
				return false;

			}
			catch (Exception)
			{
				return true;			}

		}



		//Zobrazí dialog se zprávou
		public static void showMessage(MessageType type, string message)
		{
			MessageDialog dlg = new MessageDialog(null, DialogFlags.Modal, type, ButtonsType.Ok, @message);
			dlg.Run();
			dlg.Destroy();
			dlg.Dispose();
		}

		//Autentizace uživatele
		public string ValidateUser(string email, string password)		{
			try
			{				var user = (from u in this.database.Users where u.Email == email select u).First();
				if (user.ValidatePassword(password))
					//Pokud je uživatel autentikován, je mu vracen zašifrovaný token					return Token.GenerateNew(user.Id);
			}
			catch (Exception)
			{
				return "Invalid password";
			}
			return "User doesnt exist!";
		}

		public IEnumerable<User> GetUsers()		{
			return (from u in database.Users.ToList() select u);
		}

		public IEnumerable<Order> GetOrders()
		{
			return (from o in database.Orders.ToList() select o);
		}

		//uid  = user id
		//Napsat tohle bylo těžší, než jsem si myslel
		public string GetHistory(int uid)
		{
			//Seženou se z DB objednávky od uživatele
			var orders = (from o in database.Orders.ToList() where o.UserId == uid select o);
			Dictionary<int, List<string>> history = new Dictionary<int, List<string>>();
			foreach (Order order in orders)
			{
				//Počet jídel v objednávce
				var orderFood = (from of in database.OrderFood.ToList() where of.OrderId == order.Id select of);
				if (orderFood.Count() == 0)
					continue;
				foreach (var orderF in orderFood)
				{
					//Přidání do slovníku
					history[order.Id] = new List<string>();
					history[order.Id].Add(order.OrderedAt.ToString("dd.MM.yyyy"));
					//Opět, akorát elegantněji zapsaný tvar
					history[order.Id].Add(string.Join(", ", orderFood.Select(food => food.foodCount.ToString() + "x " + food.Food.Name)));
					history[order.Id].Add(order.TotalPrice.ToString());
				}
				//Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(orderFoodList));
			}
			//Vrátí slovník převedený to stringu v JSON tvaru
			return Newtonsoft.Json.JsonConvert.SerializeObject(history, Newtonsoft.Json.Formatting.Indented);
		}


	}

}
