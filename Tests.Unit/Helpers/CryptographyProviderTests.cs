using System;
using System.Text;
using EmbyStat.Clients.EmbyClient.Cryptography;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Helpers
{
    public class CryptographyProviderTests
    {
	    private readonly CryptographyProvider _cryptographyProvider;
	    public CryptographyProviderTests()
	    {
		    _cryptographyProvider = new CryptographyProvider();
	    }

	    [Fact]
	    public void CreateSha1Hash()
	    {
		    var password = Encoding.UTF8.GetBytes("password");
			var hash = _cryptographyProvider.CreateSha1(password);

		    hash.Should().NotBeNull("because sha1 needs to be generated");
		    var stringHash = BitConverter.ToString(hash).Replace("-", string.Empty);

		    stringHash.Should().Be("5BAA61E4C9B93F3F0682250B6CF8331B7EE68FD8");
	    }

	    [Fact]
	    public void CreateMd5Hash()
	    {
		    var password = Encoding.UTF8.GetBytes("password");
		    var hash = _cryptographyProvider.CreateMD5(password);

		    hash.Should().NotBeNull("because MD5 needs to be generated");
		    var stringHash = BitConverter.ToString(hash).Replace("-", string.Empty);

		    stringHash.Should().Be("5F4DCC3B5AA765D61D8327DEB882CF99");
	    }
	}
}
