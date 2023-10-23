using System.Security.Cryptography;
using System.Text;

namespace Energizet.Box.Vk;

public class HashProvider : IDisposable
{
	private readonly HashAlgorithm _hashAlgorithm;

	public HashProvider(HashAlgorithm hashAlgorithm)
	{
		_hashAlgorithm = hashAlgorithm;
	}

	public string GetHash(string input)
	{
		var data = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
		return string.Join("", data.Select(x => x.ToString("x2")));
	}

	public bool VerifyHash(string input, string hash)
	{
		var hashOfInput = GetHash(input);
		return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
	}

	public void Dispose()
	{
		_hashAlgorithm.Dispose();
	}
}