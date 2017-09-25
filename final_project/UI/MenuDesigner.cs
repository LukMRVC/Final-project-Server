using System;
using Gtk;
using System.Collections.Generic;
namespace final_project
{
    public partial class MenuDesigner : Gtk.Window
    {
		//List of tabs in notebook, because deleting tabs was kinda broken
		private List<string> tabCaptions;
		private Gtk.TreeStore foodTreeStore;
		private Dictionary<Gtk.TreePath, string> treeModelValues;
		string json = "";

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

            Gtk.TreeIter iter = foodTreeStore.AppendValues("Burgery");

            Gtk.TreeIter subIter = foodTreeStore.AppendValues(iter, "Bezmase");

            foodTreeStore.AppendValues(iter, "Chickenburger", "Houska, maso, sýr", "150g", "50 Kč");

            foodTreeStore.AppendValues(subIter, "Cheeseburger", "Houska, maso, sýr", "150g", "50 Kč");
			foodTreeStore.AppendValues(subIter, "Veganburger", "S/ója, sračky a tak", "150g", "150 Kč");

            iter = foodTreeStore.AppendValues("Snídaně");

            foodTreeStore.AppendValues(iter, "Vajíčka", "Vajíčka", "200g", "30Kč");

            this.treeview.Model = foodTreeStore;

			this.treeview.RowActivated += (sender , e) =>
            {
				Gtk.TreeIter iterator;
				foodTreeStore.GetIterFromString(out iterator, e.Path.ToString());
				if (foodTreeStore.GetValue(iterator, 1) != null)
				{
					bool exists = false;
					string label = foodTreeStore.GetValue(iterator, 0).ToString();
					foreach (string caption in tabCaptions) {
						if (caption == label) {
							exists = true;
							break;				
						}
					}
					if (!exists) {
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

		protected void OnDeleteEvent(object o, DeleteEventArgs args)
		{
			Gtk.TreeIter iter;
			foodTreeStore.GetIterFirst(out iter);
            buildTreeJson(iter);
			serializeToJson();
			using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"C:\Users\Lukas\Documents\Projects\Server\test.txt", false)) 
			{
				writer.Write(json);
				writer.Close();
			}
			this.Destroy();
			args.RetVal = true;
		}

		private void buildTreeJson(Gtk.TreeIter iter) {
			Gtk.TreeIter childIter;
			do
			{
				treeModelValues.Add(foodTreeStore.GetPath(iter), foodTreeStore.GetValue(iter, 0).ToString());

				if (foodTreeStore.IterHasChild(iter) )
				{
					foodTreeStore.IterChildren(out childIter, iter);
                    buildTreeJson(childIter);
				}
			} while (foodTreeStore.IterNext(ref iter));
		}

		private string serializeToJson() {
			string json = "";
			foreach (var path in treeModelValues.Keys)
			{
				path.ToString().Equals("0:0:0"); 
			}
			return json;
		}
	}
}
