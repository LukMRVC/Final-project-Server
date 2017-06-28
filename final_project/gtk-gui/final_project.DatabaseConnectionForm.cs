
// This file has been generated by the GUI designer. Do not modify.
namespace final_project
{
	public partial class DatabaseConnectionForm
	{
		private global::Gtk.HBox hbox5;

		private global::Gtk.VBox vbox9;

		private global::Gtk.Label hostLabel;

		private global::Gtk.Label usernameLabel;

		private global::Gtk.Label passwordLabel;

		private global::Gtk.Label databaseLabel;

		private global::Gtk.VBox vbox10;

		private global::Gtk.Entry hostEntry;

		private global::Gtk.Entry usernameEntry;

		private global::Gtk.Entry passwordEntry;

		private global::Gtk.Entry databaseEntry;

		private global::Gtk.VButtonBox vbuttonbox2;

		private global::Gtk.Button saveDatabaseInfoButton;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget final_project.DatabaseConnectionForm
			this.Name = "final_project.DatabaseConnectionForm";
			this.Title = global::Mono.Unix.Catalog.GetString("DatabaseConnectionForm");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child final_project.DatabaseConnectionForm.Gtk.Container+ContainerChild
			this.hbox5 = new global::Gtk.HBox();
			this.hbox5.Name = "hbox5";
			this.hbox5.Spacing = 6;
			// Container child hbox5.Gtk.Box+BoxChild
			this.vbox9 = new global::Gtk.VBox();
			this.vbox9.Name = "vbox9";
			this.vbox9.Spacing = 6;
			// Container child vbox9.Gtk.Box+BoxChild
			this.hostLabel = new global::Gtk.Label();
			this.hostLabel.HeightRequest = 25;
			this.hostLabel.Name = "hostLabel";
			this.hostLabel.LabelProp = global::Mono.Unix.Catalog.GetString("Host:");
			this.vbox9.Add(this.hostLabel);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox9[this.hostLabel]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Padding = ((uint)(5));
			// Container child vbox9.Gtk.Box+BoxChild
			this.usernameLabel = new global::Gtk.Label();
			this.usernameLabel.HeightRequest = 25;
			this.usernameLabel.Name = "usernameLabel";
			this.usernameLabel.LabelProp = global::Mono.Unix.Catalog.GetString("Uživatelské jméno:");
			this.vbox9.Add(this.usernameLabel);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox9[this.usernameLabel]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			w2.Padding = ((uint)(5));
			// Container child vbox9.Gtk.Box+BoxChild
			this.passwordLabel = new global::Gtk.Label();
			this.passwordLabel.HeightRequest = 25;
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.LabelProp = global::Mono.Unix.Catalog.GetString("Heslo:");
			this.vbox9.Add(this.passwordLabel);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox9[this.passwordLabel]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Padding = ((uint)(5));
			// Container child vbox9.Gtk.Box+BoxChild
			this.databaseLabel = new global::Gtk.Label();
			this.databaseLabel.HeightRequest = 25;
			this.databaseLabel.Name = "databaseLabel";
			this.databaseLabel.LabelProp = global::Mono.Unix.Catalog.GetString("Databáze:");
			this.vbox9.Add(this.databaseLabel);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox9[this.databaseLabel]));
			w4.Position = 3;
			w4.Expand = false;
			w4.Fill = false;
			w4.Padding = ((uint)(5));
			this.hbox5.Add(this.vbox9);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox5[this.vbox9]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox5.Gtk.Box+BoxChild
			this.vbox10 = new global::Gtk.VBox();
			this.vbox10.Name = "vbox10";
			this.vbox10.Spacing = 6;
			// Container child vbox10.Gtk.Box+BoxChild
			this.hostEntry = new global::Gtk.Entry();
			this.hostEntry.HeightRequest = 25;
			this.hostEntry.CanFocus = true;
			this.hostEntry.Name = "hostEntry";
			this.hostEntry.IsEditable = true;
			this.hostEntry.InvisibleChar = '●';
			this.vbox10.Add(this.hostEntry);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox10[this.hostEntry]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			w6.Padding = ((uint)(5));
			// Container child vbox10.Gtk.Box+BoxChild
			this.usernameEntry = new global::Gtk.Entry();
			this.usernameEntry.HeightRequest = 25;
			this.usernameEntry.CanFocus = true;
			this.usernameEntry.Name = "usernameEntry";
			this.usernameEntry.IsEditable = true;
			this.usernameEntry.InvisibleChar = '●';
			this.vbox10.Add(this.usernameEntry);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox10[this.usernameEntry]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			w7.Padding = ((uint)(5));
			// Container child vbox10.Gtk.Box+BoxChild
			this.passwordEntry = new global::Gtk.Entry();
			this.passwordEntry.HeightRequest = 25;
			this.passwordEntry.CanFocus = true;
			this.passwordEntry.Name = "passwordEntry";
			this.passwordEntry.IsEditable = true;
			this.passwordEntry.Visibility = false;
			this.passwordEntry.InvisibleChar = '●';
			this.vbox10.Add(this.passwordEntry);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox10[this.passwordEntry]));
			w8.Position = 2;
			w8.Expand = false;
			w8.Fill = false;
			w8.Padding = ((uint)(5));
			// Container child vbox10.Gtk.Box+BoxChild
			this.databaseEntry = new global::Gtk.Entry();
			this.databaseEntry.HeightRequest = 25;
			this.databaseEntry.CanFocus = true;
			this.databaseEntry.Name = "databaseEntry";
			this.databaseEntry.IsEditable = true;
			this.databaseEntry.InvisibleChar = '●';
			this.vbox10.Add(this.databaseEntry);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox10[this.databaseEntry]));
			w9.Position = 3;
			w9.Expand = false;
			w9.Fill = false;
			w9.Padding = ((uint)(5));
			// Container child vbox10.Gtk.Box+BoxChild
			this.vbuttonbox2 = new global::Gtk.VButtonBox();
			this.vbuttonbox2.Name = "vbuttonbox2";
			this.vbuttonbox2.Homogeneous = true;
			this.vbuttonbox2.Spacing = 3;
			this.vbuttonbox2.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(1));
			// Container child vbuttonbox2.Gtk.ButtonBox+ButtonBoxChild
			this.saveDatabaseInfoButton = new global::Gtk.Button();
			this.saveDatabaseInfoButton.CanFocus = true;
			this.saveDatabaseInfoButton.Name = "saveDatabaseInfoButton";
			this.saveDatabaseInfoButton.UseUnderline = true;
			this.saveDatabaseInfoButton.Label = global::Mono.Unix.Catalog.GetString("Uložit");
			this.vbuttonbox2.Add(this.saveDatabaseInfoButton);
			global::Gtk.ButtonBox.ButtonBoxChild w10 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.vbuttonbox2[this.saveDatabaseInfoButton]));
			w10.Expand = false;
			w10.Fill = false;
			this.vbox10.Add(this.vbuttonbox2);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox10[this.vbuttonbox2]));
			w11.Position = 4;
			w11.Expand = false;
			w11.Fill = false;
			this.hbox5.Add(this.vbox10);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox5[this.vbox10]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			this.Add(this.hbox5);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 338;
			this.DefaultHeight = 225;
			this.Show();
			this.saveDatabaseInfoButton.Clicked += new global::System.EventHandler(this.saveDatabaseInfoAction);
		}
	}
}
