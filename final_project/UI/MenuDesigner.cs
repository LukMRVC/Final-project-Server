using System;
using System.Linq;
using Gtk;
using System.Collections.Generic;
using final_project.Model;
namespace final_project
{
    public partial class MenuDesigner : Gtk.Window
    {
		//List of tabs in notebook, because deleting tabs was kinda broken
		private List<string> tabCaptions;
		//treeview store/model
		private Gtk.TreeStore foodTreeStore;
		private Dictionary<Gtk.TreePath, string> treeModelValues;
		//values from CSV will be stored here
		private Dictionary<string, string> rebuildTreeValues;
        public List<Food> food { get; private set; }

		public MenuDesigner( IEnumerable<string> csvDdata) :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
			this.initiliaze();
			this.buildTreeView();
            food = new List<Food>();
			//tries to read and serialize csv file with treeview info to rebuild treeview
			try
			{
				rebuildTreeValues = csvDdata.Select(line => line.Split(';')).ToDictionary(line => line[0], line => line[1]);
            	rebuildTree();
			}
			catch (Exception)
			{
				
				Console.WriteLine("Fuck up");
			}

			appendEventHandlers();
        }

		public MenuDesigner() : base(Gtk.WindowType.Toplevel) {
			this.Build();
			this.initiliaze();
			this.buildTreeView();
			appendEventHandlers();

		}

		private void initiliaze() { 
			tabCaptions = new List<string>();
			treeModelValues = new Dictionary<Gtk.TreePath, string>();

		}

		private void buildTreeView() { 
            this.hpaned1.Position = 475;
			Gtk.TreeViewColumn categoryColumn = new Gtk.TreeViewColumn();
			categoryColumn.Title = "Jídlo";

            Gtk.TreeViewColumn compositionColumn = new Gtk.TreeViewColumn();
			compositionColumn.Title = "Složení";

            Gtk.TreeViewColumn weightColumn = new Gtk.TreeViewColumn();
			weightColumn.Title = "Váha";

            Gtk.TreeViewColumn priceColumn = new Gtk.TreeViewColumn();
			priceColumn.Title = "Cena";

            Gtk.CellRendererText categoryCellText = new Gtk.CellRendererText();
			Gtk.CellRendererText compositionCellText = new Gtk.CellRendererText();
			Gtk.CellRendererText weightCellText = new Gtk.CellRendererText();
			Gtk.CellRendererText priceCellText = new Gtk.CellRendererText();

			categoryColumn.PackStart(categoryCellText, true);
            compositionColumn.PackStart(compositionCellText, true);
            weightColumn.PackStart(weightCellText, true);
            priceColumn.PackStart(priceCellText, true);


			this.treeview.AppendColumn(categoryColumn);
            this.treeview.AppendColumn(compositionColumn);
            this.treeview.AppendColumn(weightColumn);
            this.treeview.AppendColumn(priceColumn);

			categoryColumn.AddAttribute(categoryCellText, "text", 0);
			compositionColumn.AddAttribute(compositionCellText, "text", 1);
			weightColumn.AddAttribute(weightCellText, "text", 2);
			priceColumn.AddAttribute(priceCellText, "text", 3);

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
				if (foodTreeStore.GetValue(iterator, 1) != null)
				{
					bool exists = false;
					string label = foodTreeStore.GetValue(iterator, 0).ToString();
					//checks if tab with same name is already opened
					foreach (string caption in tabCaptions)
					{
						if (caption == label)
						{
							exists = true;
							break;
						}
					}
					if (!exists)
					{
						this.notebook1.AppendPage(new Gtk.TextView(), new CloseableTab(label, this.notebook1));
						tabCaptions.Add(label);
						this.notebook1.ShowAll();
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
			String csv = String.Join(
				Environment.NewLine,
				treeModelValues.Select(d => d.Key + ";" + d.Value + ";")
			);
			System.IO.File.WriteAllText(Constants.CSV_FILE_NAME, csv);
			this.Destroy();
			args.RetVal = true;
		}

		private void getTreeValues(Gtk.TreeIter iter) {
			Gtk.TreeIter childIter;
			do
			{
                string name = foodTreeStore.GetValue(iter, 0).ToString();
                //finds equal object to a treeview row and sets it path correctly
                foreach(Food f in food) { 
                    if (name == f.Name)
                    {
                        f.Path = foodTreeStore.GetPath(iter).ToString();
                        break;
                    }
                }
                treeModelValues.Add(foodTreeStore.GetPath(iter), name);

				if (foodTreeStore.IterHasChild(iter) )
				{
					foodTreeStore.IterChildren(out childIter, iter);
					getTreeValues(childIter);
				}
			} while (foodTreeStore.IterNext(ref iter));
		}

		private void rebuildTree() {
			int pathIndex;
			int firstPos = 0;
			//gets biggest string length, so that i know tree depth
			int depth = rebuildTreeValues.Keys.OrderByDescending(s => s.Length).First().Length;
			for (int i = 0; i < depth; i += 2) {
				//last TreePath index number
				pathIndex = 0;
				foreach (string pathString in rebuildTreeValues.Keys)
				{
					if (i == 0)
					{
						if ((int)Char.GetNumericValue(pathString[i]) == pathIndex)
						{
							foodTreeStore.AppendValues(rebuildTreeValues[pathString]);
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
								foodTreeStore.AppendValues(iter, rebuildTreeValues[pathString]);
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
				return;
			}
			this.foodTreeStore.GetIter(out node, path);
			CategoryDialog dlg = new CategoryDialog(this, true);
			string name = null;
			if (dlg.Run() == (int)ResponseType.Ok)
			{
				name = dlg.name;
				dlg.Destroy();
				dlg.Dispose();
                this.foodTreeStore.AppendValues(node, name);
                food.Add(new Food(name));
                this.treeview.ShowAll();
			}
		}


		protected void BtnCategoryClicked(object sender, EventArgs e)
		{
			CategoryDialog dlg = new CategoryDialog(this, true);
			string name = null;
			if (dlg.Run() == (int)ResponseType.Ok)
			{
				name = dlg.name;
			}
			this.foodTreeStore.AppendValues(name);
			this.treeview.ShowAll();
			dlg.Destroy();
			dlg.Dispose();
		}

		protected void OnBtnDeleteRowClicked(object sender, EventArgs e)
		{
			var dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, "Jste si jistí že chcete smazat vybraný řádek?");
			if (dialog.Run() == (int)ResponseType.Yes) {				TreePath path;
				TreeViewColumn column;
				TreeIter iter;
				this.treeview.GetCursor(out path, out column);
				if (path == null)
				{
					var message = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Musíte vybrat řádek!");
					message.Run();
					message.Destroy();
					message.Dispose();
				}
				else
				{
    	            this.foodTreeStore.GetIter(out iter, path);
	                this.foodTreeStore.Remove(ref iter);
				}
			}
			dialog.Destroy();
			dialog.Dispose();
		}
	}

}
