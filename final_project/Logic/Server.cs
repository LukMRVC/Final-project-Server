using System;
using System.Collections.Generic;
using System.Net;
using Gtk;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
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
			using (var context = new MenuDbContext()) {
				var food = new Food
				{
					Name = "Burger",
					Price = 50,
					Path = "0",
				};
				context.Menu.Add(food);
				context.SaveChanges();
			
			}
			Application.Run();
		}
	}

	public class Server 
	{
		private MainWindow win;
		public string databaseConnectionString;
		private MySqlConnection connection;
		private static HttpListener listener;
		public bool isConnected {			
			get;
			private set;

		}


		public Server() 
		{
			try
			{
                listener = new HttpListener();
				listener.Prefixes.Add("http://localhost:8080/");
			}
			catch (PlatformNotSupportedException ex)
			{
				this.showMessage(MessageType.Error, "Tato platforma není podporována, prosím upgradujte systém\n" + ex.ToString());
				this.quit();
			}

			this.isConnected = false;
			this.win = new MainWindow(this);
			win.Show();
			//tries to reload connection from property
			if (!string.IsNullOrEmpty(Properties.Settings.Default.databaseConnectionString))
			{
				this.databaseConnectionString = Properties.Settings.Default.databaseConnectionString;
				this.connect(this.databaseConnectionString);

			}

		}

		//connects to mySql database, with connection string
		public async Task<bool> connect(string databaseConnectionString)
        {
            try
            {
                this.connection = new MySqlConnection(databaseConnectionString);
				await this.connection.OpenAsync();
				this.win.statbar.Push(1, "Navázáno spojení s databází");
				this.isConnected = true;
				return true;
            }
			catch (MySqlException)
            {
				this.win.statbar.Push(1, "Spojení s databází nebylo úspěšně");
				this.isConnected = false;
				return false;
            }

        }

		//adds category to mySql database, of course, this one will be replaced as well
		public void addCategory(string name) {
			try
			{
				MySqlCommand cmd = new MySqlCommand("INSERT INTO category (name) VALUES('" + name + "');", this.connection);
				cmd.ExecuteNonQuery();
				this.win.statbar.Push(1, "Vytvořena kategorie " + name);
			}
			catch (Exception)
			{
				//CREATE TABLE IF NOT EXISTS category(id int not null auto_increment primary key, name varchar(200) not null) DEFAULT CHARACTER SET=UTF8 DEFAULT COLLATE utf8_czech_ci
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = this.connection;
				cmd.CommandText = "CREATE TABLE IF NOT EXISTS category(id int not null auto_increment primary key, " +
					"name varchar(200) not null) DEFAULT CHARACTER SET=utf8 DEFAULT COLLATE utf8_czech_ci;";
				cmd.ExecuteNonQuery();
				this.addCategory(name);
			}
		}

        public  void start() 
		{

		}

		private async void restart() 
		{
			this.quit();
			this.start();
			await this.connect(this.databaseConnectionString);
		}

		private void quit() 
		{
			
		}

		public void saveMenuData() {
			var data = System.IO.File.ReadLines(Constants.CSV_FILE_NAME).Select(line => line.Split(';')).ToArray();
			try
			{
				MySqlCommand cmd = new MySqlCommand();
				cmd.CommandText = "INSERT INTO menu (path, name) VALUES ";
				foreach (string[] line in data) {					cmd.CommandText += "('" + line[0] + "', '" + line[1] + "'),";
				}
				cmd.CommandText = cmd.CommandText.Remove( cmd.CommandText.Length - 1 );
				cmd.CommandText += ";";
				cmd.Connection = this.connection;
				cmd.ExecuteNonQuery();
			}
			catch (MySqlException)
			{
				//Create table menu if not exists
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = this.connection;
				cmd.CommandText = "CREATE TABLE IF NOT EXISTS menu(id int not null auto_increment primary key, " +
				"path varchar(200) not null, name varchar(200) not null) DEFAULT CHARACTER SET=utf8 DEFAULT COLLATE utf8_czech_ci;";
				cmd.ExecuteNonQuery();
                saveMenuData();
			}
			Console.WriteLine("Zapis do databaze probehl");

		}

		public IEnumerable<string> getMenuData(){			List<string> data = new List<string>();
			try
			{
				var command = new MySqlCommand("SELECT path, name FROM menu", this.connection);
				MySqlDataReader reader = command.ExecuteReader();
				while (reader.Read()) {
					data.Add(reader[0].ToString() + ";" + reader[1].ToString());	
				}				}
			catch (Exception)
			{
				throw new DatabaseNotConnectedException("No database connection found!");			}
			return data;
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

		private string[] getCategories() {
			var command = new MySqlCommand("SELECT name FROM category", this.connection);
			var reader = command.ExecuteReader();
			List<string> categories = new List<string>();
			while (reader.Read()) {
				categories.Add(reader[0].ToString());
			}
			reader.Close();
			return categories.ToArray();
		}

	}




}
