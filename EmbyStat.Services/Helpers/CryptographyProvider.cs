using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Emby.ApiClient.Cryptography;

namespace EmbyStat.Services.Helpers
{
	public class CryptographyProvider : ICryptographyProvider
	{
		public byte[] CreateSha1(byte[] value)
		{
			using (var provider = SHA1.Create())
			{
				return provider.ComputeHash(value);
			}
		}

		public byte[] CreateMD5(byte[] value)
		{
			using (var provider = MD5.Create())
			{
				return provider.ComputeHash(value);
			}
		}
	}
}
