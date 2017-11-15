using System;
using Braintree;
using System.Linq;
using System.Net;
using Gtk;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace final_project
{
	public static class Http
	{
		private static BraintreeGateway gateway = new BraintreeGateway
		{
			Environment = Braintree.Environment.SANDBOX,
			MerchantId = Constants.Braintree.MERCHANT_ID,
			PublicKey = Constants.Braintree.PUBLIC_KEY,
			PrivateKey = Constants.Braintree.PRIVATE_KEY,
		};

		public class Details { 
			public int lastTwo { get; set; }
			public int lastFour { get; set; }
			public string cardType { get; set; }
		}

		public class BinData
		{
			public string prepaid { get; set; }
			public string healthcare { get; set; }
			public string debit { get; set; }
			public string derbinRegulated { get; set; }
			public string commercial { get; set; }
			public string payroll { get; set; }
			public string issuingBank { get; set; }
			public string countryOfIssuance { get; set; }
			public string productId {get; set; }
		}

		public class PaymentRequest
		{
			public string nonce { get; set; }
			public Details details{get; set; }
			public string type { get; set; }
			public string description { get; set; }
			public BinData binData { get; set; }
			public decimal amount { get; set; }
			public string currency { get; set; }
		}



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
				listener.Prefixes.Add("http://192.168.0.108:8088/pay/");
				listener.Prefixes.Add("http://192.168.0.108:8088/braintree_token/");
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
					if (request.RawUrl == "/get_food/")
						responseText = HandleGetFood(request.Headers);
					else if (request.RawUrl == "/braintree_token/")
						responseText = HandleGetBraintreeToken(request.Headers);
					break;
				case "POST":
					if (request.RawUrl == "/signup/")
						responseText = HandleSignUp(request.InputStream);
					else if (request.RawUrl == "/login/")
						responseText = HandleLogin(request.InputStream);
					else if (request.RawUrl == "/pay/")
						responseText = HandlePayment(request.InputStream, request.Headers);
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
				var user = server.AddUser(postText["password"], postText["email"]);
				CustomerRequest customer = new CustomerRequest
				{
					CustomerId = user.Id.ToString(),
					Id = user.Id.ToString(),
					Email = user.Email
				};
				gateway.Customer.Create(customer);
				
			}
			catch (Exception) {
				StatusCode = 422;
				return "Uživatel s tímto jménem nebo emailem již existuje!";
			}
			StatusCode = 201;
			return "Uživatel úspěšně registrován.";
		}

		public static string HandleLogin(Stream input) 
		{
			Dictionary<string, string> postText;
			using (var reader = new StreamReader(input, System.Text.Encoding.UTF8)) 
			{
				string val = reader.ReadToEnd();
				postText = JsonConvert.DeserializeObject<Dictionary<string, string>>(val);
			}
			try
			{				if (Token.IsValid(postText["token"]))
				{
					StatusCode = 202;
					return Token.GenerateNew(Token.GetUserId(postText["token"]));
				}
				else {					StatusCode = 422;
					return "Token expired";
				}
			}
			catch (Exception) 
			{
				string token = server.ValidateUser(postText["email"], postText["password"]);
				if (!string.IsNullOrWhiteSpace(token))
				{
					StatusCode = 202;
					return token;
				}
			}
			//method Validate User validates users and creates new Token

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
			if (Token.IsValid(postText["token"][0]))
			{
                System.Threading.Tasks.Task.Run( () => server.AddOrder(postText["food"], Token.GetUserId(postText["token"][0]), postText["totalprice"][0]));
				StatusCode = 201;
				return "Vaše objednávka byla zpracována.";
			}
			StatusCode = 401;
			return "Vaše přihlášení již není platné";

		}

			public static string HandlePayment(Stream input, System.Collections.Specialized.NameValueCollection headers) {
			if (!Token.IsValidFromHeader(headers.GetValues("Authorization")[0])) 
			{
				StatusCode = 403;
				return "Invalid user token";
			}
		
			using (var reader = new StreamReader(input, System.Text.Encoding.UTF8))
			{
				string val = reader.ReadToEnd();
				PaymentRequest payment = new PaymentRequest();
				payment = JsonConvert.DeserializeObject<PaymentRequest>(val);
				var request = new TransactionRequest
				{
					Amount = payment.amount,
					MerchantAccountId = "Sandbox_Project",
					PaymentMethodNonce = payment.nonce,
					CustomerId = Token.GetUserId(headers.GetValues("Authorization")[0]).ToString(),
					Options = new TransactionOptionsRequest
					{						SubmitForSettlement = true
					}
						//Token.GetUserId(headers.GetValues("Authorization")[0]).ToString(),
				};
				Result<Transaction> result = gateway.Transaction.Sale(request);
				if (result.IsSuccess())
				{
					StatusCode = 200;
					return "Successfully paid.";
				}
				else {					StatusCode = 400;
					return "Error while paying";
				}
			}

		}


		public static string HandleGetFood(System.Collections.Specialized.NameValueCollection headers)
		{
			var token = headers.GetValues("Authorization")[0];
			if (Token.IsValidFromHeader(token)) { 
				StatusCode = 200;
				var data = server.getMenuData();
				string json = dataToJson(data);
				return json;
			}
			else{
				StatusCode = 403;
				return "Invalid User Token";
				
			}
		}

		public static string HandleGetBraintreeToken(System.Collections.Specialized.NameValueCollection headers) 
		{
			var token = headers.GetValues("Authorization")[0];
			if (Token.IsValidFromHeader(token)) { 
				StatusCode = 200;
				return gateway.ClientToken.Generate();
			}
			else{
				StatusCode = 403;
				return "Invalid User Token";
				
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
			//44 token length + userId
			string token =  Convert.ToBase64String(time.Concat(key).ToArray()) + Constants.GenerateRandom(12, new System.Random())+UserId;
			return token;
		}

		public static bool IsValid(string token) 
		{
			string sub = token.Substring(0, 24);
			byte[] data = Convert.FromBase64String(sub);
			DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
			if (when < DateTime.UtcNow.AddHours(-24)) {
				return false;
			}
			return true;
		}

		public static bool IsValidFromHeader(string token) {
			//Take substring to validate
			string sub = token.Substring(6, 24);
			byte[] data = Convert.FromBase64String(sub);
			DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
			if (when<DateTime.UtcNow.AddHours(-24)) {
				return false;
			}
			return true;
		}

		public static int GetUserId(string token) {
			string sub = "";
			if (token.Contains("Basic"))
				sub = token.Substring(50);
			else
				sub = token.Substring(44);
			return Int32.Parse(sub);
		}

	}




}
