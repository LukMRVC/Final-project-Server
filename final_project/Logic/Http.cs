using System;
using Braintree;
using System.Linq;
using System.Net;
using Gtk;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace final_project
{
	//Třída Http se stará o zpracování požadavků
	public static class Http
	{

		//Nastavení Braintree
		private static BraintreeGateway gateway = new BraintreeGateway
		{
			Environment = Braintree.Environment.SANDBOX,
			MerchantId = Constants.Braintree.MERCHANT_ID,
			PublicKey = Constants.Braintree.PUBLIC_KEY,
			PrivateKey = Constants.Braintree.PRIVATE_KEY,
		};

		public class Details
		{
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
			public string productId { get; set; }
		}

		public class PaymentRequest
		{
			public string nonce { get; set; }
			public Details details { get; set; }
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
		public static RsaCryption cryptor { get; set; }

		//Spustí naslouchání
		public static void startListening()
		{
			try
			{
				listener = new HttpListener();
				foreach (string prefix in Constants.Prefixes) {
					listener.Prefixes.Add("http://localhost:8088" + prefix);
				}
			
				listener.Start();
			}
			catch (PlatformNotSupportedException ex)
			{
				Server.showMessage(MessageType.Error, @"Tato platforma není podporována, prosím upgradujte systém" + ex.ToString());

			}
			catch (Exception e) { Console.WriteLine(e.ToString()); }
			stop = false;
			//Nastaví callback na pro asynchonní naslouchání
			IAsyncResult result = listener.BeginGetContext(ContextCallback, listener);
		}

		//Zastaví naslouchání
		public static void stopListening()
		{
			stop = true;
			listener.Close();
			listener = null;
		}


		//Callback na zpracování požadavků
		public static void ContextCallback(IAsyncResult result)
		{
			var context = listener.EndGetContext(result);
			if (stop && listener == null)
			{
				return;
			}
			//Znovu zpustí naslouchání
			listener.BeginGetContext(ContextCallback, listener);
			//Instance požadavku
			HttpListenerRequest request = context.Request;
			//Odpověď
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

		public static string HandleMethod(HttpListenerRequest request)
		{
			string responseText = "";
			switch (request.HttpMethod)
			{
				case "GET":
					switch (request.RawUrl)
					{
						case "/get_food/":
							responseText = HandleGetFood(request.Headers);
							break;
						case "/braintree_token/":
							responseText = HandleGetBraintreeToken(request.Headers);
							break;
						case "/get_user_history/":
							responseText = HandleGetHistory(request.Headers);
							break;
						case "/get_key/":
							responseText = HandleGetKey();
							break;
					}
					break;
				case "POST":
					switch (request.RawUrl)
					{
						case "/signup/":
							responseText = HandleSignUp(request.InputStream);
							break;
						case "/login/":
							responseText = HandleLogin(request.InputStream);
							break;
						case "/pay/":
							responseText = HandlePayment(request.InputStream, request.Headers);
							break;
						case "/order/":
							responseText = HandleOrder(request.InputStream);
							break;
					}
					break;
				default:
					StatusCode = 400;
					return "{ \"Error\": [ {\"Code\": \"400\"}, {\"Text\" : \"Chyba! špatný požadavek\" } ] }";

			}
			return responseText;
		}

		public static string HandleSignUp(Stream input)
		{
			Dictionary<string, string> postText;
			//Požadavek se přečte ze streamu
			using (var reader = new StreamReader(input, System.Text.Encoding.UTF8))
			{
				string val = reader.ReadToEnd();
				//A JSON se rozparsuje na slovník
				postText = JsonConvert.DeserializeObject<Dictionary<string, string>>(val);
			}
			try
			{
				//Poté se přidá nový uživatel, jehož data musíme rozšifrovat, a do braintree se přidá nový zákazník
				var user = server.AddUser(Decrypt(postText["password"]), Decrypt(postText["email"]));
				CustomerRequest customer = new CustomerRequest
				{
					CustomerId = user.Id.ToString(),
					Id = user.Id.ToString(),
					Email = user.Email
				};
				gateway.Customer.Create(customer);

			}
			catch (Exception)
			{
				StatusCode = 422;
				return "Uživatel s tímto jménem nebo emailem již existuje!";
			}
			StatusCode = 201;
			return "Uživatel úspěšně registrován.";
		}


		public static string HandleLogin(Stream input)		{
			Dictionary<string, string> postText;
			//Opět přečtení JSONu a rozparsování na slonvík
			using (var reader = new StreamReader(input, System.Text.Encoding.UTF8))			{
				string val = reader.ReadToEnd();
				postText = JsonConvert.DeserializeObject<Dictionary<string, string>>(val);
			}
			//Zkoušení přihlášení pomocí tokenu
			try
			{
				//Pokud ještě nevypršela platnost tokenu, přihlášení pomocí něj				if (Token.IsValid(postText["token"]))
				{
					StatusCode = 202;
					return Token.GenerateNew(Token.GetUserId(postText["token"]));
				}
				else
				{					StatusCode = 422;
					return "Token expired";
				}
			}
			//Přihlášení s údajy
			catch (Exception)			{
				try
				{
					//Server uživatele autentizuje a rovnou vygeneruje nový, zašifrovaný token
					string token = server.ValidateUser(Decrypt(postText["email"]), Decrypt(postText["password"]));
					if (!string.IsNullOrWhiteSpace(token))
					{
						StatusCode = 202;
						return token;
					}
				}
				catch (Exception)
				{
					StatusCode = 500;
					return "Internal server error, please try again.";
				}
			}
			StatusCode = 422;
			return "špatné přihlašovací údaje!";
		}




		public static string HandleOrder(Stream input)
		{
			Dictionary<string, string[]> postText;
			using (var reader = new StreamReader(input, System.Text.Encoding.UTF8))
			{
				//Opět rozparsování JSON
				string val = reader.ReadToEnd();
				postText = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(val);
			}
			if (Token.IsValid(postText["token"][0]))
			{
				//Na novém vlákně se přidá objednávka pomocí pole id jídel
				System.Threading.Tasks.Task.Run(() => server.AddOrder(postText["food"], Token.GetUserId(postText["token"][0]), postText["totalprice"][0]));
				StatusCode = 201;
				return "Vaše objednávka byla zpracována.";
			}
			StatusCode = 401;
			return "Vaše přihlášení již není platné";

		}

		public static string HandlePayment(Stream input, System.Collections.Specialized.NameValueCollection headers)
		{
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
				};
				Result<Transaction> result = gateway.Transaction.Sale(request);
				if (result.IsSuccess())
				{
					StatusCode = 200;
					return "Successfully paid.";
				}
				else
				{					StatusCode = 400;
					return "Error while paying";
				}
			}

		}

		//Vrátí veřejný RSA klíč ve formátu PEM
		public static string HandleGetKey()
		{
			StatusCode = 200;
			return cryptor.GetRemKey();
			//return cryption.GetPublicKeyString();
		}

		//Vrátí seznam jídel
		public static string HandleGetFood(System.Collections.Specialized.NameValueCollection headers)
		{
			var token = headers.GetValues("Authorization")[0];
			if (Token.IsValidFromHeader(token))
			{
				StatusCode = 200;
				var data = server.getMenuData();
				string json = dataToJson(data);
				return json;
			}
			else
			{
				StatusCode = 403;
				return "Invalid User Token";

			}
		}

		//Vrátí braintree token, který se vygenerován k jednorázově platbě a inicialici uživatelského rozhraní u klienta
		public static string HandleGetBraintreeToken(System.Collections.Specialized.NameValueCollection headers)
		{
			var token = headers.GetValues("Authorization")[0];
			if (Token.IsValidFromHeader(token))
			{
				StatusCode = 200;
				return gateway.ClientToken.Generate();
			}
			else
			{
				StatusCode = 403;
				return "Invalid User Token";

			}
		}

		
		public static string HandleGetHistory(System.Collections.Specialized.NameValueCollection headers)
		{
			var token = headers.GetValues("Authorization")[0];
			string response;
			if (Token.IsValidFromHeader(token))
			{
				StatusCode = 200;
				int userId = Token.GetUserId(token);
				response = server.GetHistory(userId);
				Console.WriteLine(response);

				return response;
			}

			StatusCode = 403;
			return "Invalid User Token";
		}

		//Převede opět data třídy Food to JSONu
		private static string dataToJson(IEnumerable<Model.Food> data)
		{
			string json = "{";
			foreach (Model.Food f in data)
			{
				json += "\"" + f.Name + "\" : [" + f.toClientData() + ", " + server.GetAllergenes(f.Id).toJsonArray() + "],";
			}
			json = json.Remove(json.Length - 1);
			json += "}";
			return json;
		}

		private static string Decrypt(string toDecrypt)
		{
			return cryptor.Decrypt(toDecrypt);
		}

		private static string Encrypt(string toEncrypt)
		{
			return cryptor.Encrypt(toEncrypt);
		}

	}

	public static class Token
	{

		public static RsaCryption cryption { get; set; }

		//Vytvoří nový token
		//Není můj kód
		//https://stackoverflow.com/questions/14643735/how-to-generate-a-unique-token-which-expires-after-24-hours
		public static string GenerateNew(int UserId)
		{
			//Vezme aktuální čas a převede na pole bytů
			byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
			//Vygeneruje globálně jedinečný identifikátor a převede na byty
			byte[] key = Guid.NewGuid().ToByteArray();
			//44 token length + userId
			//Token vznikne kombinací času, globálně unikatního id, náhodných čísel a id uživatele
			string token = Convert.ToBase64String(time.Concat(key).ToArray()) + Constants.GenerateRandom(12, new System.Random()) + UserId;
			//Nakonec je zašifrován
			return cryption.Encrypt(token);
		}

		//Ověření platnosti
		public static bool IsValid(string token)
		{
			string decrypted = cryption.Decrypt(token);
			string sub = decrypted.Substring(0, 24);
			byte[] data = Convert.FromBase64String(sub);
			DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
			if (when < DateTime.UtcNow.AddHours(-24))
			{
				return false;
			}
			return true;
		}

		//Ověření platnosti z hlavičky
		//To kvůli rozdílům v POST request, kde je token posílín v těle a GET request, kde je posílán jako hlavička
		//A hlavička obsahuje text "Basic " a poté následuje token
		public static bool IsValidFromHeader(string token)
		{
			string decrypted = cryption.Decrypt(token.Substring(6));
			string sub = decrypted.Substring(0, 24);
			byte[] data = Convert.FromBase64String(sub);
			DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
			if (when < DateTime.UtcNow.AddHours(-24))
			{
				return false;
			}
			return true;
		}

		//Vezme id uživatele z tokenu
		public static int GetUserId(string token)
		{
			string sub = "";
			if (token.Contains("Basic")){
				var decrypted = cryption.Decrypt(token.Substring(6));
				sub = decrypted.Substring(44);
			}else{
				var decrypted = cryption.Decrypt(token);
				sub = decrypted.Substring(44);
			}
			return Int32.Parse(sub);
		}





	}

}