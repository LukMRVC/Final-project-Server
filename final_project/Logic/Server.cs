using System;
using Gtk;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Threading;

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

		public Server() 
		{
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
	}
}
