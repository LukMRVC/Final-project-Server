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
		buildNodeView();
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	private void buildNodeView() {
	/*	Gtk.TreeViewColumn IdColumn = new Gtk.TreeViewColumn();
		IdColumn.Title = @"Id objednávky ";

        Gtk.TreeViewColumn UserColumn = new Gtk.TreeViewColumn();
		UserColumn.Title = @"Uživatel ";

        Gtk.TreeViewColumn OrderColumn = new Gtk.TreeViewColumn();
		OrderColumn.Title = @"Objednávka";

		Gtk.CellRendererText IdCellText = new Gtk.CellRendererText();
		Gtk.CellRendererText UserCellText = new Gtk.CellRendererText();
		Gtk.CellRendererText OrderCellText = new Gtk.CellRendererText();

		IdColumn.PackStart(IdCellText, true);
		UserColumn.PackStart(UserCellText, true);
		OrderColumn.PackStart(OrderCellText, true);

		nodeview.AppendColumn(IdColumn);
		nodeview.AppendColumn(UserColumn);
		nodeview.AppendColumn(OrderColumn);

		IdColumn.AddAttribute(IdCellText, "text", 0);
		UserColumn.AddAttribute(UserCellText, "text", 1);
		OrderColumn.AddAttribute(OrderCellText, "text", 2);

		ListStore store = new ListStore(typeof(int), typeof(string), typeof(string));
				   */
       // this.nodeview.Model = store;		this.nodeview.AppendColumn(@"Id objednávky ", new CellRendererText(), "text", 0);
        this.nodeview.AppendColumn(@"Uživatel ", new CellRendererText(), "text", 1);
        this.nodeview.AppendColumn(@"Objednávka", new CellRendererText(), "text", 2);
		ListStore store = new ListStore(typeof(int), typeof(string), typeof(string));
		this.nodeview.Model = store;
        this.nodeview.ShowAll();


		
	}



	/*private void addCategory(object sender, EventArgs e)
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
	}*/

	protected void connectToDatabaseAction(object sender, EventArgs e)
	{
		this.statusbar.Push(0, "Navazování spojení...");
		Task.Factory.StartNew(() => server.connect());
		
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

	protected void BtnMenuDesignerClick(object sender, EventArgs e)
	{
		MenuDesigner designer;
		try
		{
			var data = System.IO.File.ReadLines(Constants.CSV_FILE_NAME);
			designer = new MenuDesigner(data, this.server);
		}
		catch (Exception)
		{
			try
			{
				designer = new MenuDesigner(this.server.getMenuData(), this.server);
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
					designer = new MenuDesigner(this.server);
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
						designer = new MenuDesigner(data, this.server);
					}
					filedlg.Destroy();
					filedlg.Dispose();

				}
				else if(response == 0){					this.databaseConnectionFormAction(this, EventArgs.Empty);
					this.connectToDatabaseAction(this, EventArgs.Empty);
					designer = new MenuDesigner(this.server.getMenuData(), this.server);
				}
				dlg.Destroy();
				dlg.Dispose();
			}
		}
	}



}
