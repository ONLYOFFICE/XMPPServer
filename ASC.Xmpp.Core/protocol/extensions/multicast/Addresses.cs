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
    public class Addresses : Element
    {
        public Addresses()
        {
            TagName = "addresses";
            Namespace = Uri.ADDRESS;
        }

        public Address AddAddress(Address address)
        {
            AddChild(address);
            return address;
        }

        public Jid[] GetAddressList()
        {
            ElementList nl = SelectElements("address");
            var addresses = new Jid[nl.Count];

            int i = 0;
            foreach (Element e in nl)
            {
                addresses[i] = ((Address)e).Jid;
                i++;
            }
            return addresses;
        }

        public void RemoveAllBcc()
        {
            foreach (Address address in GetAddresses())
            {
                if (address.Type == AddressType.bcc)
                {
                    address.Remove();
                }
            }
        }

        public Address[] GetAddresses()
        {
            ElementList nl = SelectElements("address");
            var addresses = new Address[nl.Count];

            int i = 0;
            foreach (Element e in nl)
            {
                addresses[i] = (Address)e;
                i++;
            }
            return addresses;
        }
    }
}