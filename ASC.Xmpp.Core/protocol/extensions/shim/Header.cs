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

namespace ASC.Xmpp.Core.protocol.extensions.shim
{
    /// <summary>
    ///   JEP-0131: Stanza Headers and Internet Metadata (SHIM)
    /// </summary>
    public class Header : Element
    {
        #region << Constructors >>

        public Header()
        {
            TagName = "header";
            Namespace = Uri.SHIM;
        }

        public Header(string name, string val) : this()
        {
            Name = name;
            Value = val;
        }

        #endregion

        // <headers xmlns='http://jabber.org/protocol/shim'>
        //	 <header name='In-Reply-To'>123456789@capulet.com</header>
        // <header name='Keywords'>shakespeare,&lt;xmpp/&gt;</header>
        // </headers>

        public string Name
        {
            get { return GetAttribute("name"); }
            set { SetAttribute("name", value); }
        }
    }
}