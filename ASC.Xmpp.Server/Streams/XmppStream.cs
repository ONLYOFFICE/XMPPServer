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
using System.Collections.Generic;

using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.utils.Idn;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Streams
{
    public class XmppStream
    {

        private string ns;

        private string language;

        private string version;

        private string domain;

        private readonly List<string> resources;


        public string Id
        {
            get;
            private set;
        }

        public string ConnectionId
        {
            get;
            private set;
        }

        public bool Authenticated
        {
            get;
            set;
        }

        public bool Connected
        {
            get;
            set;
        }

        public string Namespace
        {
            get { return ns; }
            set { ns = Nameprep(value); }
        }

        public string Language
        {
            get { return language; }
            set { language = Nameprep(value); }
        }

        public string Version
        {
            get { return version; }
            set { version = Nameprep(value); }
        }

        public string User
        {
            get;
            set;
        }


        public string Domain
        {
            get { return domain; }
            set { domain = Nameprep(value); }
        }

        public IList<string> Resources
        {
            get
            {
                lock (resources)
                {
                    return resources.AsReadOnly();
                }
            }
        }

        public bool MultipleResources
        {
            get { return 1 < Resources.Count; }
        }

        public XmppStream(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId)) throw new ArgumentNullException("connectionId");

            Id = UniqueId.CreateNewId();
            ConnectionId = connectionId;
            Version = "1.0";
            resources = new List<string>();
        }

        public void Authenticate(string userName)
        {
            User = !string.IsNullOrEmpty(userName) ? Stringprep.NodePrep(userName) : null;
            Authenticated = true;
        }

        public bool JidBinded(Jid jid)
        {
            if (jid == null) throw new ArgumentNullException("jid");
            if (jid.User != User) return false;
            if (jid.Server != Domain) return false;

            lock (resources)
            {
                if (resources.Count == 0 && string.IsNullOrEmpty(jid.Resource)) return true;
                return resources.Exists(r => { return string.Compare(r, jid.Resource, StringComparison.OrdinalIgnoreCase) == 0; });
            }
        }

        public void BindResource(string resource)
        {
            lock (resources)
            {
                resources.Add(Stringprep.ResourcePrep(resource));
            }
            Connected = true;
        }

        public void UnbindResource(string resource)
        {
            lock (resources)
            {
                resources.Remove(Stringprep.ResourcePrep(resource));
            }
            Connected = resources.Count != 0;
        }

        private string Nameprep(string value)
        {
            return string.IsNullOrEmpty(value) ? value : Stringprep.NamePrep(value);
        }
    }
}