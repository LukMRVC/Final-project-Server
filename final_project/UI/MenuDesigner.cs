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
		private Dictionary<Gtk.TreePath, string> treeModelValues;
		//values from CSV will be stored here
		private Dictionary<string, string[]> rebuildTreeValues;
		private Server server;
		private List<Food> food;
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
			foreach (var f in menuData) 
			{
				f.SetAllergenes(alg.GetAllergenes(this.server.GetAllergenes(f.Id)));
				food.Add(f);
			}
			rebuildTreeValues = menuData.Select(o => o.toCsvString()).Select(s => s.Split(';')).ToDictionary(s => s[0], s => s.SubArray(3, s.Length));
            this.rebuildTree();
			appendEventHandlers();		
		}

		//CSV constructor
		public MenuDesigner(IEnumerable<string> menuData, Server serv) :
		                base(Gtk.WindowType.Toplevel)
		        {
			this.Build();
			this.initiliaze(serv);
			this.buildTreeView();
			this.server = serv;
			var alg = this.server.database.Allergenes.AsEnumerable();
			foreach (string[] arr in menuData.Select(line => line.Split(';')).ToArray()) {
				Food f = new Food(arr);
				try
				{					var indices = arr[arr.Length - 2].Split(',').Select(Int32.Parse).ToArray();
					f.SetAllergenes(alg.GetAllergenes(indices));
				}
				catch (Exception)
				{
				}
				finally {					food.Add(f);
				}
			}
			rebuildTreeValues = menuData.Select(line => line.Split(';')).ToDictionary(line => line[0], line => line.SubArray(1, line.Length));
			rebuildTree();
			appendEventHandlers();   		}

		//Plain Constructor
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
            this.hpaned1.Position = 475;

			this.treeview.AppendColumn(@"Jídlo", new CellRendererText(), "text", 0);
            this.treeview.AppendColumn(@"Gramáž", new CellRendererText(), "text", 1);
            this.treeview.AppendColumn(@"Cena", new CellRendererText(), "text", 2);
            this.treeview.AppendColumn(@"Složení", new CellRendererText(), "text", 3);
			foodTreeStore = new Gtk.TreeStore(typeof(string), typeof(string), typeof(string), typeof(string));

			this.treeview.Model = foodTreeStore;

		}


		private void appendEventHandlers()
		{
		// appends new page to notebook with the same label as row that was clicked	
		this.treeview.RowActivated += (sender, e) =>
			{
				Gtk.TreeIter iterator;
				foodTreeStore.GetIterFromString(out iterator, e.Path.ToString());
				//checks if valid row was clicked
				if (foodTreeStore.GetValue(iterator, 2) != null)
				{
					
					string label = foodTreeStore.GetValue(iterator, 0).ToString();
					int result = food.FindFoodIndex(label);
					var dlg = new AddFoodDialog(food[result].toStringArray());
					dlg.SetAllergenes(food[result].GetAllergenIds());
					if (dlg.Run() == (int)ResponseType.Ok) 
					{
						foodTreeStore.SetValues(iterator, dlg.Values.ToArray().SubArray(1, 5));
						food[result].SetValues(dlg.Values.ToArray());
						//aId = allergen Id
						foreach (int aId in dlg.Allergenes) 
						{
							//dlg.Allergenes is full of 0
							if (aId == 0)
								continue;
							var allergen = (from a in this.server.database.Allergenes where a.Id == aId select a).First();
							food[result].SetAllergen(allergen);
						}
					}
				}
			};
		}

		//Upon closing this window, values and all information that was needed will be stored 
		//in a CSV file
		protected void OnDeleteEvent(object o, DeleteEventArgs args)
		{
			Gtk.TreeIter iter;
			foodTreeStore.GetIterFirst(out iter);
            getTreeValues(iter);
			food = food.Intersect(distinct).ToList();
			try
			{
				String csv = String.Join(
					Environment.NewLine,
					food.Select(d =>  d.toCsvString() + d.GetAllergenIdsString() + ";")
				);
				System.IO.File.WriteAllText(Constants.CSV_FILE_NAME, csv);
				
			}
			catch (Exception ex) { Console.WriteLine(ex.ToString()); }
			this.Destroy();
			this.server.CompareAndSave(food.ToArray());
			args.RetVal = true;
		}

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

		private void rebuildTree() {
			int pathIndex;
			int firstPos = 0;
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
					if (i == 0)
					{
						if ((int)Char.GetNumericValue(pathString[i]) == pathIndex)
						{
							if (rebuildTreeValues[pathString][4] == "True")
							{
								foodTreeStore.AppendValues(rebuildTreeValues[pathString][0]);
							}
							else {
								foodTreeStore.AppendValues(rebuildTreeValues[pathString]);
							}
							++pathIndex;
						}
					}
					else
					{
						//Exception would be thrown otherwise
						if (pathString.Length > i) {
							// this is because path 0:1 and 1:0 are different.
							if ( firstPos != (int)Char.GetNumericValue(pathString[i - 2]) ) {
								pathIndex = 0;

							}
							//Gets parent TreeIter and appends new Node to it
							if ((int)Char.GetNumericValue(pathString[i]) == pathIndex)
							{
								TreeIter iter;
								foodTreeStore.GetIterFromString(out iter, pathString.Substring(0, i-1) );
								if (rebuildTreeValues[pathString][4] == "True")
								{
										foodTreeStore.AppendValues(iter, rebuildTreeValues[pathString][0]);
								}
								else {
										foodTreeStore.AppendValues(iter, rebuildTreeValues[pathString].SubArray(0, 4));
								}
								//foodTreeStore.AppendValues(iter, rebuildTreeValues[pathString]);
								++pathIndex;
								firstPos = (int)Char.GetNumericValue(pathString[i-2]);

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
				if (iter.HasValue)
				{
					node = iter.Value;
					this.foodTreeStore.Remove(ref node);
				}
			}
			dialog.Destroy();
			dialog.Dispose();
		}

		private TreeIter? GetSelectedRow() 
		{
			Gtk.TreePath path;
			Gtk.TreeIter node;
			Gtk.TreeViewColumn column;
			this.treeview.GetCursor(out path, out column);
			if (path == null)
			{
				var message = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Musíte vybrat řádek!");
				message.Run();
				message.Destroy();
				message.Dispose();
				return null;
			}
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
				node = iter.Value;
				if (foodTreeStore.GetValue(node, 2) != null)
					return;
				var dlg = new AddFoodDialog();

				if (dlg.Run() == (int)ResponseType.Ok)
				{
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
