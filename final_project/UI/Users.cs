using System.Linq;
using Gtk;
namespace final_project
{
	public partial class Users : Gtk.Window
	{
		private ListStore store;
		public Users(IQueryable<Model.User> users) :
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();
			this.BuildNodeView(users);
		}

		private void BuildNodeView(IQueryable<Model.User> users)
		{
            this.nodeview.AppendColumn(@"Uživatel ", new CellRendererText(), "text", 0);
			this.nodeview.AppendColumn(@"Email ", new CellRendererText(), "text", 1);
			this.nodeview.AppendColumn(@"Vytvořen ", new CellRendererText(), "text", 2);
			this.store = new ListStore(typeof(string), typeof(string), typeof(string));
			this.nodeview.Model = store;
			foreach(var user in users){
				string[] temp = { user.Username, user.Email, user.CreatedDatetime.ToString() };
				store.AppendValues(temp);
			}
	        this.nodeview.ShowAll();
		}
	}
}
