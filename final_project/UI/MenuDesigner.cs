using System;
namespace final_project
{
    public partial class MenuDesigner : Gtk.Window
    {
        public MenuDesigner() :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
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

            Gtk.TreeStore foodTreeStore = new Gtk.TreeStore(typeof(string), typeof(string), typeof(string), typeof(string));

            Gtk.TreeIter iter = foodTreeStore.AppendValues("Burgery");

            Gtk.TreeIter subIter = foodTreeStore.AppendValues(iter, "Bezmase");

            foodTreeStore.AppendValues(iter, "Chickenburger", "Houska, maso, sýr", "150g", "50 Kč");

            foodTreeStore.AppendValues(subIter, "Cheeseburger", "Houska, maso, sýr", "150g", "50 Kč");

            iter = foodTreeStore.AppendValues("Snídaně");

            foodTreeStore.AppendValues(iter, "Vajíčka", "Vajíčka", "200g", "30Kč");

            this.treeview.Model = foodTreeStore;

			this.treeview.RowActivated += (sender , e) =>
            {
				Gtk.TreeIter iterator;
				foodTreeStore.GetIterFromString(out iterator, e.Path.ToString());
				Console.WriteLine(foodTreeStore.GetValue(iterator, 0));
            };


        }

        

    }
}
