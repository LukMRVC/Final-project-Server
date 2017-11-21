using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using System.Threading.Tasks;
using final_project;

public partial class MainWindow : Gtk.Window
{

	private Server server;
	private ListStore store;
	public Statusbar statbar;
	private bool isListening = false;

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

	private void buildNodeView()
	{
		this.nodeview.AppendColumn(@"Id objednávky ", new CellRendererText(), "text", 0);
		this.nodeview.AppendColumn(@"Objednávka ", new CellRendererText(), "text", 1);
		this.nodeview.AppendColumn(@"Cena (v Kč)", new CellRendererText(), "text", 2);
		this.nodeview.AppendColumn(@"Čas objednání ", new CellRendererText(), "text", 3);

		this.store = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string));
		this.nodeview.Model = store;
		this.nodeview.ShowAll();
	}

	public void PushToNodeView(int id, IEnumerable<final_project.Model.OrderFood> content, string price, string time)
	{
		string val = "";
		foreach (var x in content)
		{
			val += x.foodCount + "x " + x.Food.Name + ",";
		}
		val = val.Remove(val.Length - 1);
		string[] values = { id.ToString(), val, price, time };
		this.store.InsertWithValues(0, values);
		//this.store.AppendValues(values);
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
		if (!this.isListening)
		{
			Http.startListening();
			this.isListening = true;
			this.BtnStartListening.Label = @"Zastavit naslouchání";
			this.statusbar1.Push(0, @"Naslouchání na portu 8088");
		}
		else{
			Http.stopListening();
			this.isListening = false;
            this.BtnStartListening.Label = @"Začít naslouchat";
            this.statusbar1.Push(0, @"Zastaveno naslouchání");
		}

	}

	protected void BtnMenuDesignerClick(object sender, EventArgs e)
	{
		MenuDesigner designer;
		try {
			if (this.server.isConnected)
			{
				designer = new MenuDesigner(this.server.getMenuData(), this.server);
			}
			else {
				var dail = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Připojte se k databázi.");
				if (dail.Run() == (int)ResponseType.Ok)
				{
					dail.Destroy();
					dail.Dispose();
				}
			}
		}catch(Exception){
			
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
			{				ImportData();
			}
			dlg.Destroy();
			dlg.Dispose();
		}
	}

	protected void OnDisplayUsersActivated(object sender, EventArgs e)
	{
		var display = new Users(this.server.GetUsers());
	}

	protected void OnOrderHistoryActionActivated(object sender, EventArgs e)
	{
		try
		{
			var display = new Orders(this.server.GetOrders());
		}
		catch (Exception ex) {Console.WriteLine(ex.ToString()); }
	}

	protected void OnExportActionActivated(object sender, EventArgs e)
	{
		var filedlg = new FileChooserDialog("Uložit soubor", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
		var filter = new FileFilter();
		filter.Name = "*.csv";
		filter.AddPattern("*.csv");
		filedlg.AddFilter(filter); 
		var result = filedlg.Run();
		if (result == (int)ResponseType.Accept) 
		{
			var data = System.IO.File.ReadLines(Constants.CSV_FILE_NAME);
			string csv = string.Join(Environment.NewLine, data.Select(s => s));
			string filename = filedlg.Filename;
			//If file has extension
			if (!System.Text.RegularExpressions.Regex.IsMatch(filename, @".+\..+")) {
				filename += @".csv";
			}
			System.IO.File.WriteAllText(filename, csv);
		}
		filedlg.Destroy();
		filedlg.Dispose();
	}

	protected void OnImportActionActivated(object sender, EventArgs e) {
		ImportData();
	}

	protected void ImportData()
	{
		if (!this.server.isConnected) {
			var dial = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Připojte se k databázi.");
			if (dial.Run() == (int)ResponseType.Ok)
			{
				dial.Destroy();
				dial.Dispose();
			}
		}

		var dail = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.OkCancel, @"Import přemaže všechna vaše dosavadní data, jste si jistí?");
		if (dail.Run() == (int)ResponseType.Ok)
		{
			dail.Destroy();
			dail.Dispose();
		}
		else {
			return;
		}
		
		var filedlg = new FileChooserDialog("Choose file", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
		FileFilter filter = new FileFilter();
		filter.Name = "CSV files";
		filter.AddPattern("*.csv");
		filedlg.AddFilter(filter);
		if (filedlg.Run() == (int)ResponseType.Accept)
		{
			var data = System.IO.File.ReadLines(filedlg.Filename);
			server.ImportData(data);
		}
		filedlg.Destroy();
		filedlg.Dispose();
	}
}
