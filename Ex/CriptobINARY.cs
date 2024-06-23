
using System.Security.Cryptography;

namespace EterPharma.Ex
{
	public static class CriptoBinary
	{
		public static byte[] Criptografar(byte[] dados)
		{
			return ProtectedData.Protect(dados, null, DataProtectionScope.CurrentUser);
		}

		public static byte[] Descriptografar(byte[] dadosCriptografados)
		{
			return ProtectedData.Unprotect(dadosCriptografados, null, DataProtectionScope.CurrentUser);
		}

	}
}
