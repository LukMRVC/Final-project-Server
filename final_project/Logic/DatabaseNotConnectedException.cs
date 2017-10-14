using System;
namespace final_project
{
	public class DatabaseNotConnectedException : System.Exception
	{
		public DatabaseNotConnectedException()
		{
		}

		public DatabaseNotConnectedException(string message) : base(message) { 
			
		}


	}
}
