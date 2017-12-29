using System;
using System.Reflection;
using System.Linq;
using Gtk;
using System.Collections.Generic;
using final_project.Model;
namespace final_project
{
    public partial class MenuDesigner : Gtk.Window
    {
		//treeview store/model
		private Gtk.TreeStore foodTreeStore;
		//Hodnoty
		private Dictionary<Gtk.TreePath, string> treeModelValues;
		//Pro CSV hodnoty z importu
		private Dictionary<string, string[]> rebuildTreeValues;
		private Server server;
		private List<Food> food;
		//
		private List<Food> distinct;

		//DB data constructor
		public MenuDesigner(IEnumerable<Food> menuData, Server serv) :
								base(Gtk.WindowType.Toplevel)
		{
			this.Build();
			this.initiliaze(serv);
			this.buildTreeView();
			this.server = serv;
			var alg = this.server.database.Allergenes.ToList();
			//Nastavení alergenů k jídlům, protože lokální list jídel nemůže kvůli konfiluktům brát přímo vazby z Entity Frameworku
			foreach (var f in menuData) 
			{
				f.SetAllergenes(alg.GetAllergenes(this.server.GetAllergenes(f.Id)));
				food.Add(f);
			}
			//Převede každé jídlo do CSV stringu, rozdělí do pole a všechno je potom převedeno na slovník, kde klíčem cesta komponenty Treeview
			rebuildTreeValues = menuData.Select(o => o.toCsvString()).Select(s => s.Split(';')).ToDictionary(s => s[0], s => s.SubArray(1, s.Length));
			//Vytvoří treeview z aktuálních dat
            this.rebuildTree();
			appendEventHandlers();		
		}


		public MenuDesigner(Server serv) : base(Gtk.WindowType.Toplevel) {
			this.Build();
			this.initiliaze(serv);
			this.buildTreeView();
			appendEventHandlers();

		}

		private void initiliaze(Server serv) {
			this.server = serv;
			this.distinct = new List<Food>();
			this.food = new List<Food>();
			treeModelValues = new Dictionary<Gtk.TreePath, string>();

		}

		private void buildTreeView() { 
			//Vytvoří komponenty treeview
            this.hpaned1.Position = 475;

			this.treeview.AppendColumn(@"Jídlo", new CellRendererText(), "text", 0);
            this.treeview.AppendColumn(@"Gramáž", new CellRendererText(), "text", 1);
            this.treeview.AppendColumn(@"Cena", new CellRendererText(), "text", 2);
            this.treeview.AppendColumn(@"Složení", new CellRendererText(), "text", 3);
			foodTreeStore = new Gtk.TreeStore(typeof(string), typeof(string), typeof(string), typeof(string));

			this.treeview.Model = foodTreeStore;

		}

		//Přídání eventu na kliknutí řádku v treeview
		private void appendEventHandlers()
		{

		this.treeview.RowActivated += (sender, e) =>
			{
				//Třída TreeIter je důležitá pro upravení hodnot řádku
				Gtk.TreeIter iterator;
				//Získa TreeIter z cesty řádku, na který bylo kliknuto
				foodTreeStore.GetIterFromString(out iterator, e.Path.ToString());
				//Kontola, jestli řádek na který bylo kliknuto je řádek, který se může upravovat, což jsou jen řádky s jídlem
				if (foodTreeStore.GetValue(iterator, 2) != null)
				{
					//Název jídla
					string label = foodTreeStore.GetValue(iterator, 0).ToString();
					//Najde index v listu jídla a podle toho je třída předána do dialogu, který přídává nebo upraví jídlo
					int result = food.FindFoodIndex(label);
					var dlg = new AddFoodDialog(food[result]);
					//dlg.SetAllergenes(food[result].GetAllergenIds());
					if (dlg.Run() == (int)ResponseType.Ok) 
					{
						//Upraví data
						foodTreeStore.SetValues(iterator, dlg.Values.ToArray().SubArray(1, 5));
						food[result].SetValues(dlg.Values.ToArray());
						//Clear all relations with allergens
						food[result].Allergen.Clear();
						//aId = allergen Id
						foreach (int aId in dlg.Allergenes) 
						{
							//Pole alergenů jsou ve výchozí hodnotě 0, opět aby se dali kontrolovat
							if (aId == 0)
								continue;
							//Vezme se alergen přímo z databáze, aby se nevytvářel v DB nový záznam, ale aby vznikla vazba
							var allergen = (from a in this.server.database.Allergenes.ToList() where a.Id == aId select a).First();
							food[result].SetAllergen(allergen);
						}
					}
				}
			};
		}

		//Při zavírání tohohle okna se všechna data uloží do CSV souboru a do DB
		protected void OnDeleteEvent(object o, DeleteEventArgs args)
		{
			Gtk.TreeIter iter;
			foodTreeStore.GetIterFirst(out iter);
            //Získání dat z Treeview
			getTreeValues(iter);
			//Následující řádek udělá průnik mezi dvěma listy, to z toho důvodu, aby list food obsahoval
			//jen aktuální jídla a nezůstávalo něco, co už bylo smazané
			food = food.Intersect(distinct).ToList();
			try
			{
				//Převede pomocí Linq list do csv stringu
				String csv = String.Join(
					Environment.NewLine,
					food.Select(d =>  d.toCsvString() + d.GetAllergenIdsString() + ";")
				);
				//A zapíše se do souboru
				System.IO.File.WriteAllText(Constants.CSV_FILE_NAME, csv);
				
			}
			catch (Exception ex) { Console.WriteLine(ex.ToString()); }
			this.Destroy();
			//Porovnání a uložení do databáze, více popsáno u konktrétní metody
			this.server.CompareAndSave(food.ToArray());
			args.RetVal = true;
		}

		//Rekurzivní fuknce na procházení treeview, který má strukturu stromu
		private void getTreeValues(TreeIter iter) {
			TreeIter childIter;
			do
			{
				try { 
					string name = foodTreeStore.GetValue(iter, 0).ToString();
	                treeModelValues.Add(foodTreeStore.GetPath(iter), name);
					int result = food.FindFoodIndex(name);
					distinct.Add(food.FindFood(name));
					if (result != -1)
					{
						food[result].Path = foodTreeStore.GetPath(iter).ToString();
					}
					if (foodTreeStore.IterHasChild(iter) )
					{
						foodTreeStore.IterChildren(out childIter, iter);
						getTreeValues(childIter);
					}
				} catch(Exception){ return; };
			} while (foodTreeStore.IterNext(ref iter));

		}

		//Doplnění aktuálních dat do treeview při vytváření okna návrháře
		private void rebuildTree() {
			//Druhá část cesty treeview
			int pathIndex;
			//Hloubka hodnot, abych věděl, kolikrát má cyklus iterovat
			int depth = 1;
			try
			{				depth = rebuildTreeValues.Keys.OrderByDescending(s => s.Length).First().Length;
			}
			catch (Exception e) {
				Destroy(); 
				throw e;
			}
			for (int i = 0; i < depth; i += 2) {
				pathIndex = 0;
				foreach (string pathString in rebuildTreeValues.Keys)
				{
					//Celkem zbytečné, dalo by se to udělat i jinak, ale lenost tentokrát vyhrála
					//Jedná se o kontrolu vnoření, pokud i = 0, tak ještě nebylo vnořeno a nepotřebuji TreeIter
					if (i == 0)
					{
						if ((int)Char.GetNumericValue(pathString[i]) == pathIndex && pathString.Length == i+1)
						{
							//Index 4 je hodnota, zda se jedná o kategorii nebo konkrétní jídlo
							if (rebuildTreeValues[pathString][4] == "True")
							{
								//Přidá se pouze název kategorie
								foodTreeStore.AppendValues(rebuildTreeValues[pathString][0]);
							}
							else {
								//Přidají se hodnoty
								foodTreeStore.AppendValues(rebuildTreeValues[pathString]);
							}
							++pathIndex;
						}
					}
					// Zde již TreeIter budu potřebovat
					else
					{
						
						if (pathString.Length == i + 1) 
						{
							TreeIter iter;
							foodTreeStore.GetIterFromString(out iter, pathString.Substring(0, i-1) );
							//Opět kontrola kategorie
							if (rebuildTreeValues[pathString][4] == "True")
							{
								foodTreeStore.AppendValues(iter, rebuildTreeValues[pathString][0]);
							}
							else {

								foodTreeStore.AppendValues(iter, rebuildTreeValues[pathString].SubArray(0, 5));
							}
						}
					}
				}
			}
		}



		protected void OnBtnSubcategoryClicked(object sender, EventArgs e)
		{
			
			Gtk.TreeIter? node = GetSelectedRow();
			if (node == null)
				return;
			//Pokud je vybráno jídlo, nemůže se přidat podkategorie, ta se může připojit jen ke kategorii
			if (foodTreeStore.GetValue(node.Value, 2) != null)
				return;
			CategoryDialog dlg = new CategoryDialog(this, true);
			string name = null;
			if (dlg.Run() == (int)ResponseType.Ok)
			{
				name = dlg.name;
				food.Add(new Food
				{
					Category = true,
					Name = name,
				});
				dlg.Destroy();
				dlg.Dispose();
				this.foodTreeStore.AppendValues(node.Value, name);
                this.treeview.ShowAll();
			}
		}

		protected void OnBtnDeleteRowClicked(object sender, EventArgs e)
		{
			
			var dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, "Jste si jistí že chcete smazat vybraný řádek?");
			if (dialog.Run() == (int)ResponseType.Yes) {				TreeIter? iter = GetSelectedRow();
				TreeIter node;
				//Pokud nebyl vrácen null, tak se přiřadí hodnota a smažou se řádky
				if (iter.HasValue)
				{
					node = iter.Value;
					this.foodTreeStore.Remove(ref node);
				}
			}
			dialog.Destroy();
			dialog.Dispose();
		}

		//Vybere kliknutý řádek a vrátí TreeIter nebo null
		private TreeIter? GetSelectedRow() 
		{
			Gtk.TreePath path;
			Gtk.TreeIter node;
			Gtk.TreeViewColumn column;
			//Get kurzor do dvou parametrů zadá TreePath a sloupec na kterém se nachází kurzor v kliku
			this.treeview.GetCursor(out path, out column);
			if (path == null)
			{
				var message = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Musíte vybrat řádek!");
				message.Run();
				message.Destroy();
				message.Dispose();
				return null;
			}
			//A z TreePath získám TreeIter
            this.foodTreeStore.GetIter(out node, path);
			return node;
		}

		protected void OnBtnAddRowClicked(object sender, EventArgs e)
		{
			TreeIter? iter = GetSelectedRow();
			TreeIter node;
			if (!iter.HasValue)
				return;
			else {
				//Pokud byl vracen TreeIter, přiřadí se jeho hodnota
				node = iter.Value;
				if (foodTreeStore.GetValue(node, 2) != null)
					return;
				//Zobrazí se dialog na nové jídlo
				var dlg = new AddFoodDialog();

				if (dlg.Run() == (int)ResponseType.Ok)
				{
					//Jídlo se vytvoří i jako třída Food a přidá se do listu a zobrazí se na treeview
					Food fd = new Food();
					fd.SetValues(dlg.Values);
					var alg =  this.server.database.Allergenes.AsEnumerable();
					fd.SetAllergenes(alg.GetAllergenes(dlg.Allergenes));
					food.Add(fd);
					foodTreeStore.AppendValues(node, dlg.Values.ToArray().SubArray(1, 5));
					treeview.ShowAll();
				}
				dlg.Destroy();
				dlg.Dispose();
			}
		}

		protected void OnBtnCategoryClicked(object sender, EventArgs e)
		{
			CategoryDialog dlg = new CategoryDialog(this, true);
			string name = null;
			if (dlg.Run() == (int)ResponseType.Ok)
			{
				name = dlg.name;
				food.Add(new Food
				{
					Category = true,					Name = name,
				});
				
				this.foodTreeStore.AppendValues(name);
				this.treeview.ShowAll();

			}
			dlg.Destroy();
			dlg.Dispose();
		}
	}
}
