using System.Linq;
using System.Collections.Generic;
using Gtk;
namespace final_project
{
	public partial class Users : Gtk.Window
	{
		private ListStore store;
		public Users(IEnumerable<Model.User> users) :
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();
			this.BuildNodeView(users);
		}

		private void BuildNodeView(IEnumerable<Model.User> users)
		{
            this.nodeview.AppendColumn(@"Uživatel ", new CellRendererText(), "text", 0);
			this.nodeview.AppendColumn(@"Vytvořen ", new CellRendererText(), "text", 1);
			this.store = new ListStore(typeof(string), typeof(string), typeof(string));
			this.nodeview.Model = store;
			foreach(var user in users){
				string[] temp = { user.Email, user.CreatedDatetime.ToString() };
				store.AppendValues(temp);
			}
	        this.nodeview.ShowAll();
		}
	}
}
