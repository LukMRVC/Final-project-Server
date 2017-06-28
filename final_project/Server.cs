using System;
using Gtk;
using Npgsql;

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
		private string databaseConnectionString;
		private bool connectedToDB;
		private NpgsqlConnection connection;

		public Server() 
		{
			this.win = new MainWindow(this);
			win.Show();
		}

		public void connect(string connectionString) 
		{
            this.databaseConnectionString = connectionString;

			try
			{
                this.connection = new NpgsqlConnection(this.databaseConnectionString);
				this.connection.Open();

			}
			catch (Exception ex)
			{
				MessageDialog md = new MessageDialog(this.win, DialogFlags.NoSeparator, MessageType.Error, ButtonsType.Close, "Nastal problém při připojování k databázi.\nZkontrolujte data k připojení.");
				md.Run();
				md.Destroy();
			}
		}

		public void start() 
		{
		}

		private void restart() 
		{
			this.quit();
			this.start();
			this.connect(this.databaseConnectionString);
		}

		private void quit() 
		{
			
		}
	}
}
