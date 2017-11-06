using System;
using System.Linq;
using Gtk;
namespace final_project
{
	public partial class UserDisplay : Gtk.Window
	{
		private ListStore store;

		public UserDisplay(IQueryable<final_project.Model.User> users) :
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();
			this.BuildNodeView(users);
		}

		private BuildNodeView(IQueryable<final_project.Model.User> users){
			this.nodeview.AppendColumn(@"Uživatel ", new CellRendererText(), "text", 0);
	        this.nodeview.AppendColumn(@"Email ", new CellRendererText(), "text", 1);
			this.nodeview.AppendColumn(@"Vytvořen ", new CellRendererText(), "text", 2);
			this.store = new ListStore(typeof(string), typeof(string), typeof(string));
			this.nodeview.Model = store;
			foreach(var user in users){
				string[] temp = { user.Username, user.Email, user.CreatedDateTime.ToString() };
				store.appendValues(temp);
			}
	        this.nodeview.ShowAll();
		}
	}
}
