using System;
using System.Linq;
using Gtk;
using System.Collections.Generic;
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
		//string json = "";

        public MenuDesigner() :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
			tabCaptions = new List<string>();
			treeModelValues = new Dictionary<Gtk.TreePath, string>();

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

          /*  Gtk.TreeIter iter = foodTreeStore.AppendValues("Burgery");

            Gtk.TreeIter subIter = foodTreeStore.AppendValues(iter, "Bezmase");

            foodTreeStore.AppendValues(iter, "Chickenburger", "Houska, maso, sýr", "150g", "50 Kč");

            foodTreeStore.AppendValues(subIter, "Cheeseburger", "Houska, maso, sýr", "150g", "50 Kč");
			foodTreeStore.AppendValues(subIter, "Veganburger", "S/ója, sračky a tak", "150g", "150 Kč");

            iter = foodTreeStore.AppendValues("Snídaně");

            foodTreeStore.AppendValues(iter, "Vajíčka", "Vajíčka", "200g", "30Kč");*/

            this.treeview.Model = foodTreeStore;

			//tries to read and serialize csv file with treeview info to rebuild treeview
			try
			{
				rebuildTreeValues = System.IO.File.ReadLines("test.csv").Select(line => line.Split(';')).ToDictionary(line => line[0], line => line[1]);
				rebuildTree();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Fuck up");
			}

			appendEventHandlers();
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


		protected void BtnCategoryClicked(object sender, EventArgs e)
		{
			CategoryDialog dlg = new CategoryDialog(this, true);
			string name = null;
			if (dlg.Run() == (int)ResponseType.Ok) {
				name = dlg.name;
				dlg.Destroy();
				dlg.Dispose();
			}
			Gtk.TreeIter iter = this.foodTreeStore.AppendValues(name);
			this.treeview.ShowAll();

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
			System.IO.File.WriteAllText("test.csv", csv);
			this.Destroy();
			args.RetVal = true;
		}

		private void getTreeValues(Gtk.TreeIter iter) {
			Gtk.TreeIter childIter;
			do
			{
				treeModelValues.Add(foodTreeStore.GetPath(iter), foodTreeStore.GetValue(iter, 0).ToString());

				if (foodTreeStore.IterHasChild(iter) )
				{
					foodTreeStore.IterChildren(out childIter, iter);
					getTreeValues(childIter);
				}
			} while (foodTreeStore.IterNext(ref iter));
		}

		private void rebuildTree() {
			int pathIndex;
			//path prefix for appending
			string prefix = "";
			int firstPos = 0;
			//counter is used for appending correct prefix to tree depth
			int counter = 1;
			//gets biggest string length, so that i know tree depth
			int depth = rebuildTreeValues.Keys.OrderByDescending(s => s.Length).First().Length;
			try
			{
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
							//exception would be thrown otherwise
							if (pathString.Length > i) {
								// this is because path 0:1 and 1:0 are different.
								if ( firstPos != (int)Char.GetNumericValue(pathString[i - 2]) ) {
									pathIndex = 0;
								}
								//Get parent TreeIter and appends new Node to it
								if ((int)Char.GetNumericValue(pathString[i]) == pathIndex)
								{
									TreeIter iter;
									foodTreeStore.GetIterFromString(out iter, (string.IsNullOrEmpty(prefix)) ? pathString[i - 2].ToString() : prefix + pathString[i - 2].ToString() );
									foodTreeStore.AppendValues(iter, rebuildTreeValues[pathString]);
									++pathIndex;
									firstPos = (int)Char.GetNumericValue(pathString[i-2]);
								}
							}
						}
					}
					if (i == 2)
						prefix += "0:";
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
