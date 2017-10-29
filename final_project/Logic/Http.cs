using System;
using System.Net;
using Gtk;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace final_project
{
	public static class Http
	{

		public static Server server { get; set; }

		private static HttpListener listener;
		private static int StatusCode;
		//starts HTTPListener on port 8080, responses are handled asynchronously in a static method)
		public static void startListening() {
			try
			{
                listener = new HttpListener();
				listener.Prefixes.Add("http://localhost:8080/");
				listener.Prefixes.Add("http://localhost:8080/get/");
				
			}
			catch (PlatformNotSupportedException ex)
			{
				Server.showMessage(MessageType.Error, @"Tato platforma není podporována, prosím upgradujte systém" + ex.ToString());
				
			}
			listener.Start();
			IAsyncResult result = listener.BeginGetContext(ContextCallback, listener);
			Server.showMessage(MessageType.Info, @"Naslouchání na portu 8080");

		}


		//static method for handling requests
		public static void ContextCallback(IAsyncResult result)
		{
			var context = listener.EndGetContext(result);
			//starts new listening
			listener.BeginGetContext(ContextCallback, listener);
			HttpListenerRequest request = context.Request;
			HttpListenerResponse response = context.Response;
			
			response.ContentType = "text/plain; charset=utf-8";
			string responseString = HandleMethod(request);
			response.StatusCode = StatusCode;
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
			response.ContentLength64 = buffer.Length;
			System.IO.Stream output = response.OutputStream;
			output.Write(buffer, 0, buffer.Length);
			output.Close();
		}

		public static string HandleMethod(HttpListenerRequest request) {
			string responseText = "";
			switch (request.HttpMethod) {
				case "GET": Console.WriteLine("GET");
					break; 
				case "POST":
					//0 for signup, 1 for order
					if (request.RawUrl == "/signup/")
						responseText = HandleSignUp(request.InputStream);
					else
						responseText = HandleOrder(request.InputStream);
					break;
				default: StatusCode = 400;
					return "{ \"Error\": [ {\"Code\": \"400\"}, {\"Text\" : \"Chyba! špatný požadavek\" } ] }";
					
			}

			return responseText;
		}

		public static string HandleSignUp(Stream input) {
			Dictionary<string, string> postText;
			using (var reader = new StreamReader(input, System.Text.Encoding.UTF8)) 
			{
				string val = reader.ReadToEnd();
				postText = JsonConvert.DeserializeObject<Dictionary<string, string>>(val);
			}
			server.database.Users.Add(new Model.User(postText["username"], postText["password"], postText["email"]));
			server.database.SaveChangesAsync();
			StatusCode = 200;
			return "User " + postText["username"] + " registered succesfully";
		}

		public static string HandleOrder(Stream input) {
			Dictionary<string, string[]> postText;
			using (var reader = new StreamReader(input, System.Text.Encoding.UTF8))
			{
				string val = reader.ReadToEnd();
				postText = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(val);
			}
			if (Token.IsValid(postText["Token"][0]))
			{
				System.Threading.Tasks.Task.Run(() => server.AddOrder(postText));
				StatusCode = 200;
				return "Vaše objednávka byla zpracována.";
			}
			StatusCode = 401;
			return "Vaše přihlášení již není platné";

		}
	}

	public static class Token {

		public static string GenerateNew() {
			return "token";
		}

		public static bool IsValid(string token) 
		{
			if (token == "token")
				return true;
			return false;
			
		}

	}
}
