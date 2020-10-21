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

using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.x.muc;

namespace ASC.Xmpp.Server.Services.Muc2.Helpers
{
    #region usings

    

    #endregion

    /// <summary>
    /// </summary>
    public class MucHelpers
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="presence">
        /// </param>
        /// <returns>
        /// </returns>
        public static Muc GetMuc(Presence presence)
        {
            return (Muc) presence.SelectSingleElement(typeof (Muc));
        }

        /// <summary>
        /// </summary>
        /// <param name="presence">
        /// </param>
        /// <returns>
        /// </returns>
        public static string GetPassword(Presence presence)
        {
            Muc muc = GetMuc(presence);
            return muc != null ? muc.Password : null;
        }

        public static History GetHistroy(Presence presence)
        {
            Muc muc = GetMuc(presence);
            return muc != null ? muc.History : null;
        }

        /// <summary>
        /// </summary>
        /// <param name="presence">
        /// </param>
        /// <returns>
        /// </returns>
        public static bool IsJoinRequest(Presence presence)
        {
            return presence.Type == PresenceType.available;//Group chat 1.0 and MUC
        }

        #endregion
    }
}