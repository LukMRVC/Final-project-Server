﻿using System;
using Gtk;
using System.Linq;
using System.Text.RegularExpressions;

namespace final_project
{
	public partial class CategoryDialog : Gtk.Dialog
	{

		public string name
		{
			get;
			private set;
		}

		public CategoryDialog(Gtk.Window parent, bool modal)
		{
			this.Build();
			this.Parent = parent;
			this.Modal = modal;
		}

		protected void Cancelled(object sender, EventArgs e)
		{
			this.name = null;
			this.Destroy();
			this.Dispose();
			return;
		}

		protected void buttonOkClicked(object sender, EventArgs e)
		{
			check(sender, e);
			this.Destroy();
		}

		protected void check(object sender, EventArgs e)
		{
			string name = this.nameEntry.Text;
			//Kontrola, jestli jméno není prázdné
			if (string.IsNullOrWhiteSpace(name))
			{
				MessageDialog dlg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, "Název nesmí být prázdný!");
				dlg.Run();
				dlg.Destroy();
				dlg.Dispose();
				return;
			}
			//Kontrola platnosti
			else if (!(new Regex(@"^(\p{L}+\s?)+$").IsMatch(name)))
			{
				MessageDialog dlg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, "Název nesmí obsahovat čísla a speciální znaky!");
				dlg.Run();
				dlg.Destroy();
				dlg.Dispose();
				return;
			}
            this.name = name.First().ToString().ToUpper() + name.Substring(1);;
		}
	}
}
