using System;
using System.Data;
using System.Collections.Generic;
using System.Net;

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
		private static HttpListener listener;
		public MenuDbContext database;
		public bool isConnected { get; private set; }
		private IEnumerable<Food> f;
		public Server() 
		{
			try
			{
                listener = new HttpListener();
				listener.Prefixes.Add("http://localhost:8080/");
			}
			catch (PlatformNotSupportedException ex)
			{
				this.showMessage(MessageType.Error, @"Tato platforma není podporována, prosím upgradujte systém" + ex.ToString());
				this.quit();
			}
            this.win = new MainWindow(this);
			try
			{
				database = new MenuDbContext();
				database.Database.Initialize(true);
				//ChangeCollation();
				this.win.statbar.Push(0, @"Připojeno k databázi");
				this.isConnected = true;
			}
			catch (Exception) {				connect();
			}

			win.Show();

		}

		/*public void ChangeCollation() { 
			string con = System.Configuration.ConfigurationManager.ConnectionStrings["MenuDbContext"].ConnectionString;
			string []sub = con.Split(';');
			string dbName = "";
			foreach (string word in sub) {				if (word.Contains("database=")){					dbName = word.Substring(word.IndexOf('=')+1).ToLower();
					break;
				}
			}

			database.Database.ExecuteSqlCommand("ALTER DATABASE "+ dbName +" COLLATE utf8_czech_ci");
		}*/

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

        public  void start() 
		{

		}

		private async void restart() 
		{
			this.quit();
			this.start();
			this.connect();
		}

		private void quit() 
		{
			
		}


		/* Rework this so you return only array of <Food>, then you search for changes and update that */
		//To co není v array se smaže
		//to co v array je se updatne
		//to co v array je navíc, se přidá

		public void CompareAndSave(Food[] comparator) 
		{
			try
			{
				for (int i = 0; i < database.Menu.Count(); ++i)
				{
					var instance = database.Menu.OrderBy(s => s.Id).Skip(i).First();
					int result = instance.IsIn(comparator);
					if (result != -1)
					{
						database.Entry(instance).State = System.Data.Entity.EntityState.Modified;
						database.Menu.OrderBy(s => s.Id).Skip(i).First().SetValues(comparator[result]);
						continue;
					}
					else if (result == -1)
					{
						database.Menu.Remove(instance);
						continue;
					}
				}

				for (int i = 0; i < comparator.Length; ++i)
				{
					if (comparator[i].IsIn(database.Menu.ToArray()) == -1)
					{
						database.Menu.Add(comparator[i]);
					}
				}
			}
			catch (Exception e) { Console.WriteLine(e.ToString()); }

			database.SaveChangesAsync();
		}

		public void saveMenuData() {

			/* var data = System.IO.File.ReadLines(Constants.CSV_FILE_NAME).Select(line => line.Split(';')).ToArray();
			 foreach (string[] line in data) {				database.Menu.Add(new Food { Path = line[0], Name = line[1] });
			 }*/
			database.SaveChangesAsync();
		}

		public IEnumerable<Food> getMenuData() 
		{
			var data = (from Food in database.Menu.ToList() select Food);
			return data;
		}

		public int[] GetAllergenes(int foodId) 
		{
			var query = this.database.Database.SqlQuery<string>("SELECT Allergen_Id FROM food_has_allergen WHERE Food_Id=" + foodId);
			var list = query.ToList();
			var w = list.Select(Int32.Parse).ToArray();
			return w;
		}

		//starts HTTPListener on port 8080, responses are handled asynchronously in a static method
		public void startListening() {
			listener.Start();
			IAsyncResult result = listener.BeginGetContext(ContextCallback, null);
			showMessage(MessageType.Info, "Naslouchání na portu 8080");
		}

		//static method for handling requests
		public static void ContextCallback(IAsyncResult result) {
			var context = listener.EndGetContext(result);
			//starts new listening
			listener.BeginGetContext(ContextCallback, null);
			var request = context.Request;
			var response = context.Response;
			response.ContentType = "text/html; charset=utf-8";
			string responseString = "<HTML><BODY><h1>Hello, World!</h1></body></html>";
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
			response.ContentLength64 = buffer.Length;
			System.IO.Stream output = response.OutputStream;
			output.Write(buffer, 0, buffer.Length);
			output.Close();
		}

		//Show message dialog with given message type and message
		public void showMessage(MessageType type, string message)
		{
			MessageDialog dlg = new MessageDialog(win, DialogFlags.Modal, type, ButtonsType.Ok, @message);
			dlg.Run();
			dlg.Destroy();
			dlg.Dispose();
		}

	}

}
