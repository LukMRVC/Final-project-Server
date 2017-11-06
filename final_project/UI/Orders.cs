using System.Linq;
using Gtk;
namespace final_project
{
	public partial class Orders : Gtk.Window
	{
		private ListStore store;
		public Orders(IQueryable<Model.Order> orders) :
				base(Gtk.WindowType.Toplevel)
		{
			this.Build();
			this.BuildNodeView(orders);
		}

		private void BuildNodeView(IQueryable<Model.Order> orders)
		{
			this.nodeview.AppendColumn(@"Id ", new CellRendererText(), "text", 0);
			this.nodeview.AppendColumn(@"Uživatel ", new CellRendererText(), "text", 1);
			this.nodeview.AppendColumn(@"Objednáno ", new CellRendererText(), "text", 2);
			this.store = new ListStore(typeof(string), typeof(string), typeof(string));
			this.nodeview.Model = store;
			foreach (var order in orders)
			{
				string[] temp = { order.Id.ToString(), order.User.Username, order.OrderedAt.ToString() };
				store.AppendValues(temp);
			}
			this.nodeview.ShowAll();
		}
	}
}
