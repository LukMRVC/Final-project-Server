using System;
using Gtk;

namespace final_project
{
	public partial class DatabaseConnectionDialog : Gtk.Dialog
	{
		private string host;
		private string username;
		private string password;
		private string database;
		private string port;
		private Gtk.Entry[] entryArr;

		public string connectionString
		{
			get;
			set;
		}


		public DatabaseConnectionDialog(Gtk.Window parent, bool modal)
		{
			this.Build();
            this.Parent = parent;
			this.Modal = modal;
			this.entryArr = new Gtk.Entry[3];
            this.entryArr[0] = this.hostEntry;
            this.entryArr[1] = this.usernameEntry;
            this.entryArr[2] = this.databaseEntry;
		}


		private void buildConnectionString()
		{
			string connString = "server=";
			connString += this.host;
			connString += ";user=";
			connString += this.username;
			connString += ";password=";
			connString += this.password;
			connString += ";port=";
			if (string.IsNullOrWhiteSpace(this.port) || this.port == "")
			{
				connString += "3306";
			}
			else
				connString += this.port;
			connString += ";database=";
			connString += this.database;
			connString += ";charset=utf8";
			this.connectionString = connString;
			Properties.Settings.Default.databaseConnectionString = connString;
			Properties.Settings.Default.Save();
		}

		private string formatError(string entryName)
		{
			switch (entryName.Replace("Entry", ""))
			{
				case "host": return "Host nemůže být prázdný!";
				case "username": return "Uživatelské jméno nemůže být prázdné!";
				case "password": return "Heslo nemůže být prázdné!";
				case "database": return "Databáze nemůže být prázdná!";
				default: return "Neznámá chyba";
			}
		}

		protected void buttonOkClicked(object sender, EventArgs e)
		{
			foreach (var widget in this.entryArr)
			{
				if (string.IsNullOrWhiteSpace(widget.Text))
				{
					MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, this.formatError(widget.Name));
					md.Run();
					md.Destroy();
					return;
				}
			}
			this.port = portEntry.Text;
			this.host = hostEntry.Text;
			this.username = usernameEntry.Text;
			this.password = passwordEntry.Text;
		    this.database = databaseEntry.Text;
            this.buildConnectionString();
		}

		protected void buttonOkPressed(object sender, EventArgs e)
		{
			foreach (Gtk.Entry widget in this.entryArr)
			{
				if (string.IsNullOrWhiteSpace(widget.Text))
				{
					MessageDialog md = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, this.formatError(widget.Name));
					md.Run();
					md.Destroy();
					return;
				}
			}
            this.port = portEntry.Text;
			this.host = hostEntry.Text;
			this.username = usernameEntry.Text;
			this.password = passwordEntry.Text;
			this.database = databaseEntry.Text;
            this.buildConnectionString();
		}
	}
}
