using System;
using System.Collections.Generic;
using Gtk;
using System.Threading.Tasks;
using final_project;

public partial class MainWindow : Gtk.Window
{

	private Server server;

	public Statusbar statbar;

	public MainWindow(Server s) : base(Gtk.WindowType.Toplevel)
	{
		this.server = s;
		Build();
		this.statbar = this.statusbar;
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	private void addCategory(object sender, EventArgs e)
	{
		CategoryDialog cd = new CategoryDialog(this, true);
		string name = null;
		if (cd.Run() == (int)ResponseType.Ok)
		{
			name = cd.name;
			cd.Destroy();
			cd.Dispose();
		}
		Task.Factory.StartNew(() => server.addCategory(name));
	}

	protected void connectToDatabaseAction(object sender, EventArgs e)
	{
		this.statusbar.Push(0, "Navazování spojení...");
		Task.Factory.StartNew(() => server.connect(server.databaseConnectionString));
		//		this.server.connect();
	}

	protected void databaseConnectionFormAction(object sender, EventArgs e)
	{
		DatabaseConnectionDialog dcd = new DatabaseConnectionDialog(this, true);
		if (dcd.Run() == (int)ResponseType.Ok)
		{
			this.server.databaseConnectionString = dcd.connectionString;
		}
		dcd.Destroy();
		dcd.Dispose();

	}

	protected void BtnStartListeningClicked(object sender, EventArgs e)
	{
		this.server.startListening();
	}


	//Obsolete 
	/*
	public void updateMenu(string[] categories) {
		for (int i = 0; i < categories.Length; ++i) {
			MenuItem item = new MenuItem(categories[0]);
		}
      /*  this.databaseInfoMenuAction = new global::Gtk.Action("databaseInfoMenuAction", global::Mono.Unix.Catalog.GetString("Informace k připojení"), null, null);
		this.databaseInfoMenuAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Informace k připojení");
		w1.Add(this.databaseInfoMenuAction, null);*/


	//}

	protected void BtnMenuDesignerClick(object sender, EventArgs e)
	{
		MenuDesigner designer;
		try
		{
			var data = System.IO.File.ReadLines(Constants.CSV_FILE_NAME);
			designer = new MenuDesigner(data);		}
		catch (Exception)
		{
			try
			{
				designer = new MenuDesigner(this.server.getMenuData());
			}
			catch (DatabaseNotConnectedException)
			{
				//new dialog comes here
				var dlg = new MenuDesignerWarningDialog();

				//response id 0 = database connection
				//response id 1 = file choose
				//response id 2 = continue
				int response = dlg.Run();

				if (response == 2)
				{
					designer = new MenuDesigner();
				}
				else if (response == 1)
				{					var filedlg = new FileChooserDialog("Choose file", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
					FileFilter filter = new FileFilter();
					filter.Name = "CSV files";
					filter.AddPattern("*.csv");
					filedlg.AddFilter(filter);
					if (filedlg.Run() == (int)ResponseType.Accept)
					{
						var data = System.IO.File.ReadLines(filedlg.Filename);
						designer = new MenuDesigner(data);
					}
					filedlg.Destroy();
					filedlg.Dispose();

				}
				else {					this.databaseConnectionFormAction(this, EventArgs.Empty);
					this.connectToDatabaseAction(this, EventArgs.Empty);
				}

				dlg.Destroy();
				dlg.Dispose();
			}

		}

		//designer.Show();
		/*designer.DeleteEvent += (object menuDesigner, DeleteEventArgs args) => {			this.server.saveMenuData();
		};*/
	}

	public static string[] getMenuDataFromDatabase(){
		
		return new string[2];
	}
}
