using System;
using System.Net;
using Gtk;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

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
		private MySqlConnection connection;
		private static HttpListener listener;

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
            
			this.win = new MainWindow(this);
			win.Show();
			if (!string.IsNullOrEmpty(Properties.Settings.Default.databaseConnectionString))
			{
				this.connect(Properties.Settings.Default.databaseConnectionString);

			}

		}

		public async Task<bool> connect(string databaseConnectionString)
        {
            try
            {
                this.connection = new MySqlConnection(databaseConnectionString);
				await this.connection.OpenAsync();
				this.win.statbar.Push(1, "Navázáno spojení s databází");
				return true;
            }
			catch (MySqlException  ex)
            {
				this.win.statbar.Push(1, "Spojení s databází nebylo úspěšně");
				return false;
            }

        }

		public void addCategory(string name) {
			try
			{
				MySqlCommand cmd = new MySqlCommand("INSERT INTO category (name) VALUES('" + name + "');", this.connection);
				MySqlDataReader reader = cmd.ExecuteReader();
				this.win.statbar.Push(1, "Vytvořena kategorie " + name);
				reader.Close();
			}
			catch (Exception ex)
			{
				//CREATE TABLE IF NOT EXISTS category(id int not null auto_increment primary key, name varchar(200) not null) DEFAULT CHARACTER SET=UTF8 DEFAULT COLLATE utf8_czech_ci
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = this.connection;
				cmd.CommandText = "CREATE TABLE IF NOT EXISTS category(id int not null auto_increment primary key, " +
					"name varchar(200) not null) DEFAULT CHARACTER SET=utf8 DEFAULT COLLATE utf8_czech_ci;";
				MySqlDataReader reader = cmd.ExecuteReader();
				reader.Close();
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

		public void startListening() {
			listener.Start();
			IAsyncResult result = listener.BeginGetContext(ContextCallback, null);
			showMessage(MessageType.Info, "Naslouchání na portu 8080");
		}

		public static void ContextCallback(IAsyncResult result) {
			var context = listener.EndGetContext(result);
			listener.BeginGetContext(ContextCallback, null);
			var request = context.Request;
			var response = context.Response;
			System.Threading.Thread.Sleep(5000);
			response.ContentType = "text/html; charset=utf-8";
			string responseString = "<HTML><BODY><h1>Hello, World!</h1></body></html>";
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
			response.ContentLength64 = buffer.Length;
			System.IO.Stream output = response.OutputStream;
			output.Write(buffer, 0, buffer.Length);
			output.Close();
		}

		public void showMessage(MessageType type, string message)
		{
			MessageDialog dlg = new MessageDialog(win, DialogFlags.Modal, type, ButtonsType.Ok, @message);
			dlg.Run();
			dlg.Destroy();
			dlg.Dispose();
		}

	}


}
