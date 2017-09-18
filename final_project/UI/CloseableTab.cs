using System;

namespace final_project
{
	public class CloseableTab : Gtk.Box
	{
		public Gtk.Label caption;
		public Gtk.Image img = new Gtk.Image("../Images/close.png");
		public Gtk.ToolButton close;
		public Gtk.Notebook parent;

		public CloseableTab(string name, Gtk.Notebook parent)
		{
            this.parent = parent;
			this.caption = new Gtk.Label(name);
			close = new Gtk.ToolButton(img, "");
			PackStart(caption);
			PackStart(close);
			ShowAll();
			//close.Hide();
			this.addHandler();
		}

		void addHandler() {
			close.Clicked += (sender, e) => {
				if(parent.CurrentPage != 0)
					parent.RemovePage(parent.CurrentPage);
			};
		}
	}
}
