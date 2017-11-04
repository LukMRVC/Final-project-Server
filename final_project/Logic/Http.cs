using System;
using System.Text;
using System.Linq;
using System.Net;
using Gtk;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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
				listener.Prefixes.Add("http://192.168.0.108:8088/");
				listener.Prefixes.Add("http://192.168.0.108:8088/login/");
				listener.Prefixes.Add("http://192.168.0.108:8088/signup/");
				listener.Prefixes.Add("http://192.168.0.108:8088/get_food/");
				listener.Prefixes.Add("http://192.168.0.108:8088/order/");
				listener.Start();
			}
			catch (PlatformNotSupportedException ex)
			{
				Server.showMessage(MessageType.Error, @"Tato platforma není podporována, prosím upgradujte systém" + ex.ToString());

			}
			catch (Exception e) { Console.WriteLine(e.ToString()); }
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
				case "GET":
					responseText = HandleGetFood(request.Headers);
					break;
				case "POST":
					if (request.RawUrl == "/signup/")
						responseText = HandleSignUp(request.InputStream);
					else if (request.RawUrl == "/login/")
						responseText = HandleLogin(request.InputStream);
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
			try
			{
				server.AddUser(postText["username"], postText["password"], postText["email"]);
			}
			catch (Exception) {
				StatusCode = 422;
				return "Uživatel s tímto jménem nebo emailem již existuje!";
			}
			StatusCode = 201;
			return server.ValidateUser(postText["username"], postText["password"]);
		}

		public static string HandleLogin(Stream input) 
		{
			Dictionary<string, string> postText;
			using (var reader = new StreamReader(input, System.Text.Encoding.UTF8)) 
			{
				string val = reader.ReadToEnd();
			postText = JsonConvert.DeserializeObject<Dictionary<string, string>>(val);
			}
			//method Validate User validates users and creates new Token
			string token = server.ValidateUser(postText["username"], postText["password"]);
			if (!string.IsNullOrWhiteSpace(token))
			{
				StatusCode = 202;
				return token;
			}
			StatusCode = 422;
			return "špatné přihlašovací údaje!";
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
				System.Threading.Tasks.Task.Run(() => server.AddOrder(postText["Order"], Token.GetUserId(postText["Token"][0])));
				StatusCode = 201;
				return "Vaše objednávka byla zpracována.";
			}
			StatusCode = 401;
			return "Vaše přihlášení již není platné";

		}


		public static string HandleGetFood(System.Collections.Specialized.NameValueCollection headers)
		{
			var token = headers.GetValues("Authorization")[0];
			if (Token.IsValid(token)) { 
				StatusCode = 200;
				var data = server.getMenuData();
				string json = dataToJson(data);
				return json;
			}
			else{
				StatusCode = 403;
				return "Invalid Token";
				
			}
		}

		private static string dataToJson(IEnumerable<Model.Food> data)
		{
			string json = "{";
			foreach (Model.Food f in data)
			{
				json += "\""+f.Name+"\" : [" + f.toClientData() + ", "+server.GetAllergenes(f.Id).toJsonArray() +"],";
			}
			json = json.Remove(json.Length - 1);
			json += "}";
			return json;
		}

	}




	public static class Token {

		public static string GenerateNew(int UserId) {
			byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
			byte[] key = Guid.NewGuid().ToByteArray();
			string token =  Convert.ToBase64String(time.Concat(key).ToArray()) + Constants.GenerateRandom(12, new System.Random())+UserId;
			return token;
		}

		public static bool IsValid(string token) 
		{
			//Take substring to validate			string sub = token.Substring(6, 24);
			byte[] data = Convert.FromBase64String(sub);
			DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
			if (when < DateTime.UtcNow.AddHours(-24)) {
				return false;
			}
			return true;
		}

		public static int GetUserId(string token) {
			string sub = token.Substring(token.Length - 48);
			return Int32.Parse(sub);
		}

	}




}
