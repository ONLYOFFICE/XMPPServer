/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2008 by AG-Software 											 *
 * All Rights Reserved.																 *
 * Contact information for AG-Software is available at http://www.ag-software.de	 *
 *																					 *
 * Licence:																			 *
 * The agsXMPP SDK is released under a dual licence									 *
 * agsXMPP can be used under either of two licences									 *
 * 																					 *
 * A commercial licence which is probably the most appropriate for commercial 		 *
 * corporate use and closed source projects. 										 *
 *																					 *
 * The GNU Public License (GPL) is probably most appropriate for inclusion in		 *
 * other open source projects.														 *
 *																					 *
 * See README.html for details.														 *
 *																					 *
 * For general enquiries visit our website at:										 *
 * http://www.ag-software.de														 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Runtime.InteropServices;

namespace agsXMPP.util
{
	/// <summary>
	/// Crypto API for Windows CE, Pocket PC and Smartphone
	/// will be used for Hashing and the RandomNumberGenerator
	/// </summary>
	internal class WinCeApi
	{
		public enum SecurityProviderType
		{
			RSA_FULL		    = 1,
			HP_HASHVAL		    = 2,
			CALG_MD5		    = 32771,
			CALG_SHA1		    = 32772            
		}        

		[DllImport("coredll.dll")]
		public static extern bool CryptAcquireContext(out IntPtr hProv, string pszContainer, string pszProvider, int dwProvType,int dwFlags);
		
		[DllImport("coredll.dll")]
		public static extern bool CryptCreateHash(IntPtr hProv, int Algid, IntPtr hKey, int dwFlags, out IntPtr phHash);
		
		[DllImport("coredll.dll")]
		public static extern bool CryptHashData(IntPtr hHash, byte [] pbData, int dwDataLen, int dwFlags);
		
		[DllImport("coredll.dll")]
		public static extern bool CryptGetHashParam(IntPtr hHash, int dwParam, byte[] pbData, ref int pdwDataLen, int dwFlags);
		
		[DllImport("coredll.dll")]
		public static extern bool CryptDestroyHash(IntPtr hHash);
		
		[DllImport("coredll.dll")]
		public static extern bool CryptReleaseContext(IntPtr hProv, int dwFlags);

		[DllImport("coredll.dll", EntryPoint="CryptGenRandom", SetLastError=true)]
		public static extern bool CryptGenRandomCe(IntPtr hProv, int dwLen, byte[] pbBuffer);
		
		[DllImport("advapi32.dll", EntryPoint="CryptGenRandom", SetLastError=true)]
		public static extern bool CryptGenRandomXp(IntPtr hProv, int dwLen, byte[] pbBuffer);
		
	}
}
