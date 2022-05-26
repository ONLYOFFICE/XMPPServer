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

namespace ASC.Xmpp.Core.protocol.iq.jingle
{
    public class Stun : Element
    {
        public Stun()
        {
            TagName = "stun";
            Namespace = Uri.IQ_GOOGLE_JINGLE;
        }

        public Server[] GetServers()
        {
            ElementList nl = SelectElements(typeof(Server));
            int i = 0;
            var result = new Server[nl.Count];
            foreach (Server ri in nl)
            {
                result[i] = ri;
                i++;
            }
            return result;
        }

        public void AddServer(Server r)
        {
            ChildNodes.Add(r);
        }
    }

    public class Server : Element
    {
        public Server()
        {
            TagName = "server";
            Namespace = Uri.IQ_GOOGLE_JINGLE;
        }

        public Server(string host, int udp) : this()
        {
            Host = host;
            Udp = udp;
        }

        public string Host
        {
            get { return GetAttribute("host"); }

            set { SetAttribute("host", value); }
        }

        public int Udp
        {
            get { return GetAttributeInt("udp"); }

            set { SetAttribute("udp", value); }
        }
    }


    public class GoogleJingle : Element
    {
        public GoogleJingle()
        {
            TagName = "query";
            Namespace = Uri.IQ_GOOGLE_JINGLE;
        }


        public virtual Stun Stun
        {
            get { return SelectSingleElement(typeof(Stun)) as Stun; }

            set
            {
                RemoveTag(typeof(Stun));
                if (value != null)
                {
                    AddChild(value);
                }
            }
        }
    }
}