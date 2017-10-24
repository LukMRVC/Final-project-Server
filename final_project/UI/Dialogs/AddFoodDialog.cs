using System;
namespace final_project
{
	public partial class AddFoodDialog : Gtk.Dialog
	{
		public AddFoodDialog()
		{
			this.Build();
			Values = new string[8];
		}

		public string[] Values { get; private set; }

		protected void OnButtonOkClicked(object sender, EventArgs e)
		{
			Values[0] = EntryName.Text;
			Values[1] = EntryGram.Text;
			Values[2] = EntryPrice.Text;
			Values[3] = EntryComposition.Text;
		}
	}
}
