using System;
using Gtk;
using System.Linq;
namespace final_project
{
	public partial class OrderHistory : Gtk.Window
	{
		private ListStore store;
		public OrderHistory(IQueryable<final_project.Model.Order> orders) :
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();
		}

		private BuildNodeView(IQueryable<final_project.Model.Order> orders)
		{
			this.nodeview.AppendColumn(@"Id ", new CellRendererText(), "text", 0);
			this.nodeview.AppendColumn(@"Uživatel ", new CellRendererText(), "text", 1);
			this.nodeview.AppendColumn(@"Objednáno ", new CellRendererText(), "text", 2);
			this.store = new ListStore(typeof(string), typeof(string), typeof(string));
			this.nodeview.Model = store;
			foreach(var order in order){
				string[] temp = { order.Id.toString(), order.User.Username, order.CreatedAt.toString() };
				store.appendValues(temp);
			}
	        this.nodeview.ShowAll();

		}
	}
}
