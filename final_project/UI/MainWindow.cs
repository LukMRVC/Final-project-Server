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
		//Nastavení referencí
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
		//Vytvoří tabulku s informacemi objednávek		this.nodeview.AppendColumn(@"Id objednávky ", new CellRendererText(), "text", 0);
		this.nodeview.AppendColumn(@"Objednávka ", new CellRendererText(), "text", 1);
		this.nodeview.AppendColumn(@"Cena (v Kč)", new CellRendererText(), "text", 2);
		this.nodeview.AppendColumn(@"Čas objednání ", new CellRendererText(), "text", 3);

		this.store = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string));
		this.nodeview.Model = store;
		this.nodeview.ShowAll();
	}

	//Přidá informace objednávky do tabulky
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

	}

	//Připojí server k datábázi na novém vlákně
	protected void connectToDatabaseAction(object sender, EventArgs e)
	{
		this.statusbar.Push(0, "Navazování spojení...");
		Task.Factory.StartNew(() => server.connect());

	}

	//Zavolá formulář o připojení k databázi
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

	//Zapínání a zastavení naslouchání
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

	//Vytvoření okna návháře
	protected void BtnMenuDesignerClick(object sender, EventArgs e)
	{
		MenuDesigner designer;
		try {
			//Otestování jestli je server připojen k databázi
			if (this.server.isConnected)
			{
				//Pokud je, tak se vytáhnou existující data z databáze a předají se návrháři, aby je mohl zobrazit, taky se mu předá reference
				//na třídu serveru, aby návrhář mohl jednodušeji přidávat data do databáze
				designer = new MenuDesigner(this.server.getMenuData(), this.server);
			}
			//Pokud server není připojen, vytvoření se dialogové okno, aby se uživatel připojil k databázi
			else {
				var dail = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Připojte se k databázi.");
				if (dail.Run() == (int)ResponseType.Ok)
				{
					dail.Destroy();
					dail.Dispose();
				}
			}
		}catch(Exception){
			
			//Dialog o varování
			var dlg = new MenuDesignerWarningDialog();

			//response id 1 = Import data
			//response id 2 = Pokračování na prázdný návrhář, mělo by se stát pouze pokud je návrhář spuštěn poprvé
			//nebo se měnila databáze
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

	//Zobrazení uživatelů
	protected void OnDisplayUsersActivated(object sender, EventArgs e)
	{
		var display = new Users(this.server.GetUsers());
	}

	//Zobrazení historie objednávek
	protected void OnOrderHistoryActionActivated(object sender, EventArgs e)
	{
		try
		{
			var display = new Orders(this.server.GetOrders());
		}
		catch (Exception ex) {Console.WriteLine(ex.ToString()); }
	}

	//Exportování dat návrháře do nového CSV souboru
	protected void OnExportActionActivated(object sender, EventArgs e)
	{
		//Vytvoření dialogu otevření souboru
		var filedlg = new FileChooserDialog("Uložit soubor", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
		var filter = new FileFilter();
		filter.Name = "*.csv";
		filter.AddPattern("*.csv");
		filedlg.AddFilter(filter); 
		var result = filedlg.Run();
		if (result == (int)ResponseType.Accept) 
		{
			//Při exportu se data prakticky jen přepíšou, protože návrhář je vždy ukládá do souboru
			var data = System.IO.File.ReadLines(Constants.CSV_FILE_NAME);
			string csv = string.Join(Environment.NewLine, data.Select(s => s));
			string filename = filedlg.Filename;
			//Pokud název souboru ještě nemá příponu, tak ji připojíme
			if (!System.Text.RegularExpressions.Regex.IsMatch(filename, @".+\..+")) {
				filename += @".csv";
			}
			//Vypsání dat do souboru
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
		//Testování, jestli se server připojený k DB
		if (!this.server.isConnected) {
			var dial = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Připojte se k databázi.");
			if (dial.Run() == (int)ResponseType.Ok)
			{
				dial.Destroy();
				dial.Dispose();
			}
		}

		//Import maže všechna dosavadní data, aby nedocházelo ke konfliktům v DB
		var dail = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.OkCancel, @"Import přemaže všechna vaše dosavadní data, jste si jistí?");
		if (dail.Run() == (int)ResponseType.Ok)
		{
			dail.Destroy();
			dail.Dispose();
		}
		else {
			return;
		}

		//Dialog na zvolení souboru
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
