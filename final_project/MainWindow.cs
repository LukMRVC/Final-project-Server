using System;
using Gtk;
using final_project;

public partial class MainWindow : Gtk.Window
{

	//private Server serv;

	private Server server;

	public MainWindow(Server s) : base(Gtk.WindowType.Toplevel)
	{
		this.server = s;
		Build();
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void addCategory(object sender, EventArgs e)
	{
		string name = new CategoryDialog().name;
	}

	protected void connectToDatabaseAction(object sender, EventArgs e)
	{
	}

	protected void databaseConnectionFormAction(object sender, EventArgs e)
	{
		string databaseConnectionString = new DatabaseConnectionForm().connectionString;
	}
}
