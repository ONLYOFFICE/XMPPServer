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

using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.x
{

    #region usings

    #endregion

    // <presence to="gnauck@myjabber.net/myJabber v3.5" from="yahoo.myjabber.net/registered">
    // 		<status>Extended Away</status>
    // 		<show>xa</show><priority>5</priority>
    // 		<x stamp="20050206T13:09:50" from="gnauck@myjabber.net/myJabber v3.5" xmlns="jabber:x:delay"/>    
    // </presence> 

    /// <summary>
    ///   <para>Delay class for Timestamps</para> <para>Mainly used in offline and groupchat messages. This is the time when the message was received by the server</para>
    /// </summary>
    public class Delay : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Delay()
        {
            TagName = "x";
            Namespace = Uri.X_DELAY;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public Jid From
        {
            get
            {
                if (HasAttribute("from"))
                {
                    return new Jid(GetAttribute("from"));
                }
                else
                {
                    return null;
                }
            }

            set { SetAttribute("from", value.ToString()); }
        }

        /// <summary>
        /// </summary>
        public DateTime Stamp
        {
            get { return Time.Date(GetAttribute("stamp")); }

            set { SetAttribute("stamp", Time.Date(value)); }
        }

        #endregion
    }
}