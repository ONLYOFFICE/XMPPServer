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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.x.muc.iq.owner
{

    #region usings

    #endregion

    /*
        <iq id="jcl_110" to="xxxxxx@conference.jabber.org" type="set">
            <query xmlns="http://jabber.org/protocol/muc#owner">
                <x type="submit" xmlns="jabber:x:data"/>
            </query>
        </iq>
    */

    /// <summary>
    /// </summary>
    public class Owner : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Owner()
        {
            TagName = "query";
            Namespace = Uri.MUC_OWNER;
        }

        #endregion
    }
}