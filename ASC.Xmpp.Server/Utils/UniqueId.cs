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

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UniqueId.cs" company="">
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography;

using ASC.Xmpp.Core.utils;

namespace ASC.Xmpp.Server.Utils
{
    /// <summary>
    /// Summary description for UniqueId.
    /// </summary>
    public class UniqueId
    {
        // Lenght of the Session ID on bytes,
        // 4 bytes equaly 8 chars
        // 16^8 possibilites for the session IDs (4.294.967.296)
        // This should be unique enough
        #region Members

        /// <summary>
        /// </summary>
        private static readonly int m_lenght = 4;

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public static string CreateNewId()
        {
            return CreateNewId(m_lenght);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public static string CreateNewId(int length)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] buf = new byte[length];
            rng.GetBytes(buf);

            return Hash.HexToString(buf);
        }

        #endregion
    }
}