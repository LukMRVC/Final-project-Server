using System;
namespace final_project
{
	public partial class AddFoodDialog : Gtk.Dialog
	{
		public AddFoodDialog()
		{
			this.Build();
			Values = new string[4];
		}

		public string[] Values { get; private set; }

		protected void OnButtonOkClicked(object sender, EventArgs e)
		{
			Values[0] = this.EntryName.Text;
			Values[1] = this.EntryGram.Text;
			Values[2] = this.EntryPrice.Text;
			Values[3] = this.EntryComposition.Text;
		}
	}
}
