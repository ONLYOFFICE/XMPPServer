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
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Utils;
using System.ComponentModel;

namespace ASC.Xmpp.Server.Session
{
	public class XmppSession
	{
		public string Id
		{
			get;
			private set;
		}

		public Jid Jid
		{
			get;
			private set;
		}

		public bool Active
		{
			get;
			set;
		}

		public XmppStream Stream
		{
			get;
			private set;
		}

		public bool RosterRequested
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			set;
		}

		public Presence Presence
		{
			get { return presence; }
			set
			{
				presence = value;
				Priority = presence != null ? presence.Priority : 0;
			}
		}

		private Presence presence;

		public bool Available
		{
			get { return Presence != null && (Presence.Type == PresenceType.available || Presence.Type == PresenceType.invisible); }
		}

		public ClientInfo ClientInfo
		{
			get;
			private set;
		}

        public DateTime GetRosterTime
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool IsSignalRFake
        {
            get;
            set;
        }

		public XmppSession(Jid jid, XmppStream stream)
		{
			if (jid == null) throw new ArgumentNullException("jid");
			if (stream == null) throw new ArgumentNullException("stream");

			Id = UniqueId.CreateNewId();
			Jid = jid;
			Stream = stream;
			Active = false;
			RosterRequested = false;
			ClientInfo = new ClientInfo();
		}

        public override string ToString()
        {
            return Jid.ToString();
        }
	}
}
