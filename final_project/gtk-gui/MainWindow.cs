
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;

	private global::Gtk.Action NovAction;

	private global::Gtk.Action TabulkaAction;

	private global::Gtk.Action NovAction1;

	private global::Gtk.Action TabulkaAction1;

	private global::Gtk.Action UpravitAction;

	private global::Gtk.Action NovAction2;

	private global::Gtk.Action TabulkaAction2;

	private global::Gtk.Action UpravitAction1;

	private global::Gtk.Action NovAction3;

	private global::Gtk.Action KategorieAction;

	private global::Gtk.Action PridatAction;

	private global::Gtk.Action DatabzeAction;

	private global::Gtk.Action PipojenAction;

	private global::Gtk.Action PipojitAction;

	private global::Gtk.Action ZobrazitAction;

	private global::Gtk.Action DisplayUsersAction;

	private global::Gtk.Action OrderHistoryAction;

	private global::Gtk.Action JdelnLstekAction;

	private global::Gtk.Action ExportAction;

	private global::Gtk.Action ImportAction;

	private global::Gtk.VBox vbox3;

	private global::Gtk.HBox hbox2;

	private global::Gtk.MenuBar menubar7;

	private global::Gtk.Statusbar statusbar;

	private global::Gtk.HBox hbox4;

	private global::Gtk.Button BtnMenuDesigner;

	private global::Gtk.Statusbar statusbar1;

	private global::Gtk.Button BtnStartListening;

	private global::Gtk.ScrolledWindow GtkScrolledWindow;

	private global::Gtk.TreeView nodeview;

	protected virtual void Build()
	{
		global::Stetic.Gui.Initialize(this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup("Default");
		this.NovAction = new global::Gtk.Action("NovAction", global::Mono.Unix.Catalog.GetString("Nový..."), null, null);
		this.NovAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Nový...");
		w1.Add(this.NovAction, null);
		this.TabulkaAction = new global::Gtk.Action("TabulkaAction", global::Mono.Unix.Catalog.GetString("Tabulka"), null, null);
		this.TabulkaAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Tabulka");
		w1.Add(this.TabulkaAction, null);
		this.NovAction1 = new global::Gtk.Action("NovAction1", global::Mono.Unix.Catalog.GetString("Nový..."), null, null);
		this.NovAction1.ShortLabel = global::Mono.Unix.Catalog.GetString("Nový...");
		w1.Add(this.NovAction1, null);
		this.TabulkaAction1 = new global::Gtk.Action("TabulkaAction1", global::Mono.Unix.Catalog.GetString("Tabulka"), null, null);
		this.TabulkaAction1.ShortLabel = global::Mono.Unix.Catalog.GetString("Tabulka");
		w1.Add(this.TabulkaAction1, null);
		this.UpravitAction = new global::Gtk.Action("UpravitAction", global::Mono.Unix.Catalog.GetString("Upravit..."), null, null);
		this.UpravitAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Upravit...");
		w1.Add(this.UpravitAction, null);
		this.NovAction2 = new global::Gtk.Action("NovAction2", global::Mono.Unix.Catalog.GetString("Nová..."), null, null);
		this.NovAction2.ShortLabel = global::Mono.Unix.Catalog.GetString("Nová...");
		w1.Add(this.NovAction2, null);
		this.TabulkaAction2 = new global::Gtk.Action("TabulkaAction2", global::Mono.Unix.Catalog.GetString("Tabulka"), null, null);
		this.TabulkaAction2.ShortLabel = global::Mono.Unix.Catalog.GetString("Tabulka");
		w1.Add(this.TabulkaAction2, null);
		this.UpravitAction1 = new global::Gtk.Action("UpravitAction1", global::Mono.Unix.Catalog.GetString("Upravit..."), null, null);
		this.UpravitAction1.ShortLabel = global::Mono.Unix.Catalog.GetString("Upravit...");
		w1.Add(this.UpravitAction1, null);
		this.NovAction3 = new global::Gtk.Action("NovAction3", global::Mono.Unix.Catalog.GetString("Nová..."), null, null);
		this.NovAction3.ShortLabel = global::Mono.Unix.Catalog.GetString("Nový...");
		w1.Add(this.NovAction3, null);
		this.KategorieAction = new global::Gtk.Action("KategorieAction", global::Mono.Unix.Catalog.GetString("Kategorie"), null, null);
		this.KategorieAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Tabulka");
		w1.Add(this.KategorieAction, null);
		this.PridatAction = new global::Gtk.Action("PridatAction", global::Mono.Unix.Catalog.GetString("Přidat do..."), null, null);
		this.PridatAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Upravit");
		w1.Add(this.PridatAction, null);
		this.DatabzeAction = new global::Gtk.Action("DatabzeAction", global::Mono.Unix.Catalog.GetString("Databáze"), null, null);
		this.DatabzeAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Připojení k databázi");
		w1.Add(this.DatabzeAction, null);
		this.PipojenAction = new global::Gtk.Action("PipojenAction", global::Mono.Unix.Catalog.GetString("Připojení"), null, null);
		this.PipojenAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Informace k připojení");
		w1.Add(this.PipojenAction, null);
		this.PipojitAction = new global::Gtk.Action("PipojitAction", global::Mono.Unix.Catalog.GetString("Připojit"), null, null);
		this.PipojitAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Připojit");
		w1.Add(this.PipojitAction, null);
		this.ZobrazitAction = new global::Gtk.Action("ZobrazitAction", global::Mono.Unix.Catalog.GetString("Zobrazit"), null, null);
		this.ZobrazitAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Zobrazit");
		w1.Add(this.ZobrazitAction, null);
		this.DisplayUsersAction = new global::Gtk.Action("DisplayUsersAction", global::Mono.Unix.Catalog.GetString("Uživatele"), null, null);
		this.DisplayUsersAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Uživatele");
		w1.Add(this.DisplayUsersAction, null);
		this.OrderHistoryAction = new global::Gtk.Action("OrderHistoryAction", global::Mono.Unix.Catalog.GetString("Historie objednávek"), null, null);
		this.OrderHistoryAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Historie objednávek");
		w1.Add(this.OrderHistoryAction, null);
		this.JdelnLstekAction = new global::Gtk.Action("JdelnLstekAction", global::Mono.Unix.Catalog.GetString("Jídelní lístek"), null, null);
		this.JdelnLstekAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Jídelní lístek");
		w1.Add(this.JdelnLstekAction, null);
		this.ExportAction = new global::Gtk.Action("ExportAction", global::Mono.Unix.Catalog.GetString("Exportovat"), null, null);
		this.ExportAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Exportovat");
		w1.Add(this.ExportAction, null);
		this.ImportAction = new global::Gtk.Action("ImportAction", global::Mono.Unix.Catalog.GetString("Importovat"), null, null);
		this.ImportAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Importovat");
		w1.Add(this.ImportAction, "<Primary>i");
		this.UIManager.InsertActionGroup(w1, 0);
		this.AddAccelGroup(this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString("Hlavní okno");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox3 = new global::Gtk.VBox();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox2 = new global::Gtk.HBox();
		this.hbox2.Name = "hbox2";
		this.hbox2.Spacing = 6;
		// Container child hbox2.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString(@"<ui><menubar name='menubar7'><menu name='DatabzeAction' action='DatabzeAction'><menuitem name='PipojenAction' action='PipojenAction'/><menuitem name='PipojitAction' action='PipojitAction'/></menu><menu name='ZobrazitAction' action='ZobrazitAction'><menuitem name='DisplayUsersAction' action='DisplayUsersAction'/><menuitem name='OrderHistoryAction' action='OrderHistoryAction'/></menu><menu name='JdelnLstekAction' action='JdelnLstekAction'><menuitem name='ExportAction' action='ExportAction'/><menuitem name='ImportAction' action='ImportAction'/></menu></menubar></ui>");
		this.menubar7 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget("/menubar7")));
		this.menubar7.Name = "menubar7";
		this.hbox2.Add(this.menubar7);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.menubar7]));
		w2.Position = 0;
		// Container child hbox2.Gtk.Box+BoxChild
		this.statusbar = new global::Gtk.Statusbar();
		this.statusbar.Name = "statusbar";
		this.statusbar.Spacing = 6;
		this.hbox2.Add(this.statusbar);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.statusbar]));
		w3.Position = 1;
		this.vbox3.Add(this.hbox2);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.hbox2]));
		w4.Position = 0;
		w4.Expand = false;
		w4.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox4 = new global::Gtk.HBox();
		this.hbox4.Name = "hbox4";
		this.hbox4.Spacing = 6;
		// Container child hbox4.Gtk.Box+BoxChild
		this.BtnMenuDesigner = new global::Gtk.Button();
		this.BtnMenuDesigner.CanFocus = true;
		this.BtnMenuDesigner.Name = "BtnMenuDesigner";
		this.BtnMenuDesigner.UseUnderline = true;
		this.BtnMenuDesigner.Relief = ((global::Gtk.ReliefStyle)(1));
		this.BtnMenuDesigner.Label = global::Mono.Unix.Catalog.GetString("Návrhář jídelního lístku");
		this.hbox4.Add(this.BtnMenuDesigner);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox4[this.BtnMenuDesigner]));
		w5.Position = 0;
		// Container child hbox4.Gtk.Box+BoxChild
		this.statusbar1 = new global::Gtk.Statusbar();
		this.statusbar1.Name = "statusbar1";
		this.statusbar1.Spacing = 6;
		this.statusbar1.HasResizeGrip = false;
		this.hbox4.Add(this.statusbar1);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox4[this.statusbar1]));
		w6.Position = 1;
		this.vbox3.Add(this.hbox4);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.hbox4]));
		w7.Position = 1;
		w7.Expand = false;
		w7.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.BtnStartListening = new global::Gtk.Button();
		this.BtnStartListening.CanFocus = true;
		this.BtnStartListening.Name = "BtnStartListening";
		this.BtnStartListening.UseUnderline = true;
		this.BtnStartListening.Label = global::Mono.Unix.Catalog.GetString("Začít naslouchat");
		this.vbox3.Add(this.BtnStartListening);
		global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.BtnStartListening]));
		w8.Position = 2;
		w8.Expand = false;
		w8.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.nodeview = new global::Gtk.TreeView();
		this.nodeview.WidthRequest = 500;
		this.nodeview.HeightRequest = 200;
		this.nodeview.CanFocus = true;
		this.nodeview.Name = "nodeview";
		this.GtkScrolledWindow.Add(this.nodeview);
		this.vbox3.Add(this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.GtkScrolledWindow]));
		w10.Position = 3;
		this.Add(this.vbox3);
		if ((this.Child != null))
		{
			this.Child.ShowAll();
		}
		this.DefaultWidth = 777;
		this.DefaultHeight = 499;
		this.Show();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
		this.PipojenAction.Activated += new global::System.EventHandler(this.databaseConnectionFormAction);
		this.PipojitAction.Activated += new global::System.EventHandler(this.connectToDatabaseAction);
		this.DisplayUsersAction.Activated += new global::System.EventHandler(this.OnDisplayUsersActivated);
		this.OrderHistoryAction.Activated += new global::System.EventHandler(this.OnOrderHistoryActionActivated);
		this.ExportAction.Activated += new global::System.EventHandler(this.OnExportActionActivated);
		this.ImportAction.Activated += new global::System.EventHandler(this.OnImportActionActivated);
		this.BtnMenuDesigner.Clicked += new global::System.EventHandler(this.BtnMenuDesignerClick);
		this.BtnStartListening.Clicked += new global::System.EventHandler(this.BtnStartListeningClicked);
	}
}
