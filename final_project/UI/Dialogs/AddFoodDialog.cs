using System;
using Gtk;
namespace final_project
{
	public partial class AddFoodDialog : Gtk.Dialog
	{
		public AddFoodDialog()
		{
			this.Initialize();
		}

		private void Initialize()
		{
			this.Build();
			Values = new string[15];
			for (int i = 0; i < Values.Length; i++)
			{
				Values[i] = "";
			}
		}

		public AddFoodDialog(string[] defaultValues) 
		{
            this.Initialize();
			setDefaultValues(defaultValues);
		}

		public void setDefaultValues(string[] Values) {
			//path is necessary
			this.Values[0] = Values[0];
			this.EntryName.Text = Values[1];
            this.EntryGram.Text = Values[2];
            this.EntryPrice.Text = Values[3];
            this.EntryComposition.Text = Values[4];
			//přeskočím 5 protože to je kategorie
			this.EntryEnergyKj.Text = Values[6];
            this.EntryEnergyKcal.Text = Values[7];
            this.EntryProtein.Text = Values[8];
            this.EntryCarbs.Text = Values[9];
            this.EntrySugar.Text = Values[10];
			this.EntryTotalFat.Text = Values[11];
			this.EntrySaturatedFat.Text = Values[12];
            this.EntryFiber.Text = Values[13];
            this.EntrySalt.Text = Values[14];
		}

		public string[] Values { get; set; }

		protected void OnButtonOkClicked(object sender, EventArgs e)
		{
			Values[1] = this.EntryName.Text;
			Values[2] = this.EntryGram.Text;
			Values[3] = this.EntryPrice.Text;
			Values[4] = this.EntryComposition.Text;
			Values[5] = "0";
			this.Destroy();
			this.Dispose();
		}

		public string[] getValues() {
			return this.Values;
		}

		protected void OnButtonCancelClicked(object sender, EventArgs e)
		{
			this.Destroy();
			this.Dispose();
		}

		private void ShowMessage(string message) {
			var dlg = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, message);
			dlg.Run();
			dlg.Destroy();
			dlg.Dispose();
		}

		private void ShowMessage(string title, string message){
			
		}

		private void Check(object sender, EventArgs e) {
			if (string.IsNullOrWhiteSpace(EntryName.Text)) {
				ShowMessage("Název nesmí být prázdný!");
			}
			else if (string.IsNullOrWhiteSpace(EntryPrice.Text)) {
				ShowMessage("Cena nesmí být prázdná!");
			}
			else if (string.IsNullOrWhiteSpace(EntryGram.Text)) {
				ShowMessage("Hmotnost nesmí být prázdná!");
			}
			else if (string.IsNullOrWhiteSpace(EntryComposition.Text)) {
				ShowMessage("Složení nesmí být prázdné!");
			}
		}

	}
}
