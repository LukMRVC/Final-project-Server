using System;
namespace final_project
{
	public partial class DatabaseConnectionForm : Gtk.Window
	{

		private string host;
		private string username;
		private string password;
		private string database;

		public DatabaseConnectionForm() :
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();
		}

		public string connectionString
		{
			get;
			private set;
		}

		private void buildConnectionString() 
		{
			string connString = "Host=";
			connString += this.host;
			connString += ";Username=";
			connString += this.username;
			connString += ";Password=";
			connString += this.password;
			connString += ";Database=";
			connString += this.database;
			this.connectionString = connString;
		}

		protected void saveDatabaseInfoAction(object sender, EventArgs e)
		{
			this.host = hostEntry.Text;
			this.username = usernameEntry.Text;
			this.password = passwordEntry.Text;
			this.database = databaseEntry.Text;
			buildConnectionString();
			this.Destroy();
		}
	}
}
