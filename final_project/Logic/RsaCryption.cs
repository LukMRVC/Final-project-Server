using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
namespace final_project
{
	public class RsaCryption
	{

		private RSAParameters publicKey;
		private RSAParameters privateKey;
		private RSACryptoServiceProvider csp;
		private const int KEY_SIZE = 2048;

		public RsaCryption()
		{
			this.csp = new RSACryptoServiceProvider(KEY_SIZE);
			//Klíče budou zachovány v instanci RSA
			this.csp.PersistKeyInCsp = true;
			this.publicKey = this.csp.ExportParameters(false);
			this.privateKey = this.csp.ExportParameters(true);
		}

		public string GetRemKey()
		{
			string rem = "";
			using (StringWriter sw = new StringWriter())
			{
				ExportPublicKey(this.csp, sw, this.publicKey);
				rem = sw.ToString();
			}

			return rem;
		}

		private byte[] ConvertHexToByte(string hex)
		{
			return Enumerable.Range(0, hex.Length)
					 .Where(x => x % 2 == 0)
					 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
					 .ToArray();
		}


		private string ConvertByteToHex(byte[] b)
		{
			string hex = BitConverter.ToString(b);
			return hex.Replace("-", "");
		}

		public string Encrypt(string toEncrypt)
		{
			var encryptedBytes = this.csp.Encrypt(System.Text.Encoding.UTF8.GetBytes(toEncrypt), true);

			return ConvertByteToHex(encryptedBytes);
		}

		public string Decrypt(string toDecrypt)
		{

			var bytes = this.ConvertHexToByte(toDecrypt);

			byte[] decryptedBytes = this.csp.Decrypt(bytes, true);

			return System.Text.Encoding.UTF8.GetString(decryptedBytes);


		}

		//Kod převodu C# veřejného klíče do formát PEM
		//Následující kód byl zkopírován z následujícího zdroje:
		//https://stackoverflow.com/questions/28406888/c-sharp-rsa-public-key-output-not-correct/28407693#28407693
		//Aneb proč vymýšlet věci, které už někdo vymyslel
		private static void ExportPublicKey(RSACryptoServiceProvider csp, TextWriter outputStream, RSAParameters publicKey)
		{
			var parameters = publicKey;
			using (var stream = new MemoryStream())
			{
				var writer = new BinaryWriter(stream);
				writer.Write((byte)0x30); // SEQUENCE
				using (var innerStream = new MemoryStream())
				{
					var innerWriter = new BinaryWriter(innerStream);
					innerWriter.Write((byte)0x30); // SEQUENCE
					EncodeLength(innerWriter, 13);
					innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
					var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
					EncodeLength(innerWriter, rsaEncryptionOid.Length);
					innerWriter.Write(rsaEncryptionOid);
					innerWriter.Write((byte)0x05); // NULL
					EncodeLength(innerWriter, 0);
					innerWriter.Write((byte)0x03); // BIT STRING
					using (var bitStringStream = new MemoryStream())
					{
						var bitStringWriter = new BinaryWriter(bitStringStream);
						bitStringWriter.Write((byte)0x00); // # of unused bits
						bitStringWriter.Write((byte)0x30); // SEQUENCE
						using (var paramsStream = new MemoryStream())
						{
							var paramsWriter = new BinaryWriter(paramsStream);
							EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
							EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
							var paramsLength = (int)paramsStream.Length;
							EncodeLength(bitStringWriter, paramsLength);
							bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
						}
						var bitStringLength = (int)bitStringStream.Length;
						EncodeLength(innerWriter, bitStringLength);
						innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
					}
					var length = (int)innerStream.Length;
					EncodeLength(writer, length);
					writer.Write(innerStream.GetBuffer(), 0, length);
				}

				var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
				outputStream.WriteLine("-----BEGIN PUBLIC KEY-----");
				for (var i = 0; i < base64.Length; i += 64)
				{
					outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
				}
				outputStream.WriteLine("-----END PUBLIC KEY-----");
			}
		}

		private static void EncodeLength(BinaryWriter stream, int length)
		{
			if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
			if (length < 0x80)
			{
				// Short form
				stream.Write((byte)length);
			}
			else
			{
				// Long form
				var temp = length;
				var bytesRequired = 0;
				while (temp > 0)
				{
					temp >>= 8;
					bytesRequired++;
				}
				stream.Write((byte)(bytesRequired | 0x80));
				for (var i = bytesRequired - 1; i >= 0; i--)
				{
					stream.Write((byte)(length >> (8 * i) & 0xff));
				}
			}
		}

		private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
		{
			stream.Write((byte)0x02); // INTEGER
			var prefixZeros = 0;
			for (var i = 0; i < value.Length; i++)
			{
				if (value[i] != 0) break;
				prefixZeros++;
			}
			if (value.Length - prefixZeros == 0)
			{
				EncodeLength(stream, 1);
				stream.Write((byte)0);
			}
			else
			{
				if (forceUnsigned && value[prefixZeros] > 0x7f)
				{
					// Add a prefix zero to force unsigned if the MSB is 1
					EncodeLength(stream, value.Length - prefixZeros + 1);
					stream.Write((byte)0);
				}
				else
				{
					EncodeLength(stream, value.Length - prefixZeros);
				}
				for (var i = prefixZeros; i < value.Length; i++)
				{
					stream.Write(value[i]);
				}
			}
		}




	}
}
