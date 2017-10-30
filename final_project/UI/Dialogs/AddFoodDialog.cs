using System;
using System.Text.RegularExpressions;
using Gtk;
namespace final_project
{
	public partial class AddFoodDialog : Gtk.Dialog
	{
		public string[] Values { get; set; }

		public int[] Allergenes { get; set; }

		public AddFoodDialog()
		{
			this.Initialize();
		}

		private void Initialize()
		{
			this.Build();
			Values = new string[15];
			Allergenes = new int[14];
			for (int i = 0; i < Values.Length; i++)
			{
				Values[i] = "";
			}
		}

		public void SetAllergenes(int[] indices) {
			foreach (Widget w in this.CheckBoxesTable) {
				if (indices.Has(Int32.Parse(Regex.Match(w.Name, @"\d+").Value))){
					(w as CheckButton).Active = true;
				}
			}
		}

		private void GetAllergenes() {
			int i = 0;
			foreach (Widget w in this.CheckBoxesTable) {
				if ((w as CheckButton).Active) { 
					int.TryParse(Regex.Match(w.Name, @"\d+").Value, out Allergenes[i]);
					++i;
				}
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


		protected void OnButtonOkClicked(object sender, EventArgs e)
		{
			Values[1] = this.EntryName.Text;
			Values[2] = this.EntryGram.Text;
			Values[3] = this.EntryPrice.Text;
			Values[4] = this.EntryComposition.Text;
			Values[5] = "0";
			this.GetAllergenes();
			this.Destroy();
			this.Dispose();
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

		private void Check(object sender, EventArgs e) {
            foreach (Widget w in this.MainTable)
            {
				if (w is Gtk.Entry) { 
					if (string.IsNullOrWhiteSpace((w as Gtk.Entry).Text))
                    {
                        ShowMessage("Položka \'" + (w as Gtk.Entry).TooltipText+ "\' nesmí být prázdná.");
                    }
				}
            }

			foreach (Widget w in this.ExpandedTable) 
			{
				if (w is Gtk.Entry) 
				{
					if ( !string.IsNullOrWhiteSpace((w as Gtk.Entry).Text) && !Regex.IsMatch( (w as Gtk.Entry).Text, @"^\d+([,]\d+)?$") ) 
					{
						ShowMessage("Položka \'" + (w as Gtk.Entry).TooltipText + "\' smí obsahovat pouze číslice oddělená čárkou.");
					}
				}	
			}
            
		}

	}
}
