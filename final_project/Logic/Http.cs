using System;
using Braintree;
using System.Linq;
using System.Net;
using Gtk;
using System.Security.Cryptography;
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
		private static bool stop;
		private static HttpListener listener;
		private static int StatusCode;
		//starts HTTPListener on port 8080, responses are handled asynchronously in a static method)
		public static void startListening() {
			try
			{
				listener = new HttpListener();
				listener.Prefixes.Add("http://localhost:8088/");
				listener.Prefixes.Add("http://localhost:8088/login/");
				listener.Prefixes.Add("http://localhost:8088/signup/");
				listener.Prefixes.Add("http://localhost:8088/get_food/");
				listener.Prefixes.Add("http://localhost:8088/get_user_history/");
				listener.Prefixes.Add("http://localhost:8088/order/");
				listener.Prefixes.Add("http://localhost:8088/pay/");
				listener.Prefixes.Add("http://localhost:8088/braintree_token/");
				listener.Start();
			}
			catch (PlatformNotSupportedException ex)
			{
				Server.showMessage(MessageType.Error, @"Tato platforma není podporována, prosím upgradujte systém" + ex.ToString());

			}
			catch (Exception e) { Console.WriteLine(e.ToString()); }
			stop = false;
			IAsyncResult result = listener.BeginGetContext(ContextCallback, listener);

		}

		public static void stopListening() 
		{
			stop = true;
			listener.Close();
			listener = null;
		}


		//static method for handling requests
		public static void ContextCallback(IAsyncResult result)
		{
			if (stop && listener == null) 
			{
				return;
			}
			var context = listener.EndGetContext(result);
			listener.BeginGetContext(ContextCallback, listener);
			HttpListenerRequest request = context.Request;
			HttpListenerResponse response = context.Response;
			response.ContentType = "application/json; charset=utf-8";
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
					else if (request.RawUrl == "/get_user_history/") 
						responseText = HandleGetHistory(request.Headers);
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
				Console.WriteLine(val);

				var decircularized = JsonConvert.DeserializeObject(val, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
				Console.WriteLine(decircularized.ToString());
				//Now decrypt from string using AES
				var key = System.Text.Encoding.UTF8.GetBytes("1x3v9r8tp:f?.485");
				var iv = System.Text.Encoding.UTF8.GetBytes("8808880080808568");

				var enc = System.Text.Encoding.UTF8.GetBytes(decircularized.ToString());
				var decrypted = DecryptStringFromBytes(enc, key, iv);
				Console.WriteLine("Decrypted:D " + decrypted)	;
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

			//Try to login with existing token
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
			//login with credentials
			catch (Exception) 
			{
				//string hash = server.GetUserHash(postText["email"]);
				try
				{
					string token = server.ValidateUser(postText["email"], postText["password"]);
					if (!string.IsNullOrWhiteSpace(token))
					{
						StatusCode = 202;
						return token;
					}
				}
				catch (Exception) {
					StatusCode = 500;
					return "Internal server error, please try again.";
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

		public static string HandleGetHistory(System.Collections.Specialized.NameValueCollection headers) {
			var token = headers.GetValues("Authorization")[0];
			string response;
			if (Token.IsValidFromHeader(token)) {
				StatusCode = 200;
				int userId = Token.GetUserId(token);
				response = server.GetHistory(userId);
				Console.WriteLine(response);

				return response;
			}

			StatusCode = 403;
			return "Invalid User Token";
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












private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
{
	// Check arguments.  
	if (cipherText == null || cipherText.Length <= 0)
	{
		throw new ArgumentNullException("cipherText");
	}
	if (key == null || key.Length <= 0)
	{
		throw new ArgumentNullException("key");
	}
	if (iv == null || iv.Length <= 0)
	{
		throw new ArgumentNullException("key");
	}

	// Declare the string used to hold  
	// the decrypted text.  
	string plaintext = null;

	// Create an RijndaelManaged object  
	// with the specified key and IV.  
	using (var rijAlg = new RijndaelManaged())
	{
		//Settings  
		rijAlg.Mode = CipherMode.CBC;
		rijAlg.Padding = PaddingMode.PKCS7;
		rijAlg.FeedbackSize = 128;
		rijAlg.Key = key;
		rijAlg.IV = iv;
		

		// Create a decrytor to perform the stream transform.  
		var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

		try
		{
			// Create the streams used for decryption.  
			using (var msDecrypt = new MemoryStream(cipherText))
			{
				using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
				{

					using (var srDecrypt = new StreamReader(csDecrypt))
					{
						// Read the decrypted bytes from the decrypting stream  
						// and place them in a string.  
						plaintext = srDecrypt.ReadToEnd();

					}

				}
			}
		}
		catch
		{
			plaintext = "keyError";
		}
	}

	return plaintext;
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