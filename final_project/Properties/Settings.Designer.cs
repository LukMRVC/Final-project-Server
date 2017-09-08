
namespace final_project.Properties
{
	[global::System.Runtime.CompilerServices.CompilerGenerated()]
	[global::System.CodeDom.Compiler.GeneratedCode("Xamarin Studio",  "6.1.0")]
	internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {

		private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

		public static Settings Default {
			get {
				return defaultInstance;
			}
		}

		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("")]
		public string databaseConnectionString
		{
			//get { return ((string)(this["databaseConnectionString"])); }
			get { return ((string)(this["databaseConnectionString"])); }
			set { this["databaseConnectionString"] = value;  }
		}

	}

}
