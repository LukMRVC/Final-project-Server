using System;
using System.Linq;
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
			//Naství všechny hodnoty preventivně na prázdný string
			for (int i = 0; i < Values.Length; i++)
			{
				Values[i] = "";
			}
		}

		//Přednastaví alergeny, aby se zobrazily již přidáné alegeny k jídlu,
		//kdyby se jídlo potřebovalo upravit
		public void SetAllergenes(int[] indices) {
			foreach (Widget w in this.CheckBoxesTable) {
				if (indices.Has(Int32.Parse(Regex.Match(w.Name, @"\d+").Value))){
					(w as CheckButton).Active = true;
				}
			}
		}

		//Z okna vezme alergeny a a přiradí do pole
		private void GetAllergenes() {
			int i = 0;
			//Projde checkboxy, jejich jména obsahují id alergenů, vezme id a převede ze stringu na int
			foreach (Widget w in this.CheckBoxesTable) {
				if ((w as CheckButton).Active) {
					Allergenes[i] = Int32.Parse(Regex.Match(w.Name, @"\d+").Value);
					++i;
				}
			}
		}

		//Přetížený konstruktor
		public AddFoodDialog(Model.Food food)
		{
			this.Initialize();
			setDefaultValues(food);
			this.SetAllergenes(food.GetAllergenIds());
		}

		//Nastaví výchozí hodnoty z hodnot jídla
		public void setDefaultValues(Model.Food food) {
			//path is necessary			this.Values[0] = food.Path;
			this.EntryName.Text = food.Name;
			this.EntryGram.Text = food.Gram.ToString();
			this.EntryPrice.Text = food.Price.ToString();
			this.EntryComposition.Text = food.Composition;
			//přeskočím 5 protože to je kategorie
			this.EntryEnergyKj.Text = food.EnergyKj.ToString();
			this.EntryEnergyKcal.Text = food.EnergyKcal.ToString();
			this.EntryProtein.Text = food.Protein.ToString();
			this.EntryCarbs.Text = food.Carbohydrates.ToString();
			this.EntrySugar.Text = food.Sugar.ToString();
			this.EntryTotalFat.Text = food.TotalFat.ToString();
			this.EntrySaturatedFat.Text = food.SaturatedFat.ToString();
			this.EntryFiber.Text = food.Fiber.ToString();
			this.EntrySalt.Text = food.Salt.ToString();
		}


		//Projede všechny textboxy a nastaví jejich hodnoty do pole Values
		//Opět by to šlo udělat lípe, ale už není čas
		protected void OnButtonOkClicked(object sender, EventArgs e)
		{
			Values[1] = (this.EntryName.Text.First().ToString().ToUpper() + this.EntryName.Text.Substring(1));
			Values[2] = this.EntryGram.Text;
			Values[3] = this.EntryPrice.Text;
			Values[4] = this.EntryComposition.Text;
			Values[5] = "0";
			Values[6] = this.EntryEnergyKj.Text;
			Values[7] = this.EntryEnergyKcal.Text;
			Values[8] = this.EntryProtein.Text;
			Values[9] = this.EntryCarbs.Text;
			Values[10] = this.EntrySugar.Text;
			Values[11] = this.EntryTotalFat.Text;
			Values[12] = this.EntrySaturatedFat.Text;
			Values[13] = this.EntryFiber.Text;
			Values[14] = this.EntrySalt.Text;
			this.GetAllergenes();
			this.Destroy();
			this.Dispose();
		}

		protected void OnButtonCancelClicked(object sender, EventArgs e)
		{
			this.Destroy();
			this.Dispose();
		}

		//Zobrazení zprávy
		private void ShowMessage(string message) {
			var dlg = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, message);
			dlg.Run();
			dlg.Destroy();
			dlg.Dispose();
		}


		private void Check(object sender, EventArgs e) {
			//Kontrola povinných polí
            foreach (Widget w in this.MainTable)
            {
				if (w is Gtk.Entry) { 
					if (string.IsNullOrWhiteSpace((w as Gtk.Entry).Text))
                    {
                        ShowMessage("Položka \'" + (w as Gtk.Entry).TooltipText+ "\' nesmí být prázdná.");
                    }
				}
            }
			//A kontrola platnosti hodnot
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
