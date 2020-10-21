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

namespace ASC.Xmpp.Core.protocol.extensions.multicast
{
    public class Address : Element
    {
        public Address()
        {
            TagName = "address";
            Namespace = protocol.Uri.ADDRESS;
        }

        public AddressType Type
        {
            get { return (AddressType) GetAttributeEnum("type", typeof (AddressType)); }

            set { SetAttribute("type", value.ToString()); }
        }

        public Jid Jid
        {
            get { return GetAttributeJid("jid"); }
            set { SetAttribute("jid", value); }
        }

        public bool Delivered
        {
            get { return GetAttributeBool("delivered"); }
            set { SetAttribute("delivered", value); }
        }

        public string Uri
        {
            get { return GetAttribute("uri"); }
            set { SetAttribute("uri", value); }
        }

        public string Desc
        {
            get { return GetAttribute("desc"); }
            set { SetAttribute("desc", value); }
        }

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }
    }
}