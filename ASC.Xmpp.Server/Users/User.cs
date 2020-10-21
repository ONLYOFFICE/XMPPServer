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

using ASC.Xmpp.Core.protocol;
using System;

namespace ASC.Xmpp.Server.Users
{
	public class User
	{
		public Jid Jid
		{
			get;
			private set;
		}

		public bool IsAdmin
		{
			get;
			set;
		}

        public string Sid
        {
            get;
            set;
        }


		public User(Jid jid)
			: this(jid, true)
		{

		}

		public User(Jid jid, bool admin)
		{
			if (jid == null) throw new ArgumentNullException("jid");

			Jid = jid;
			IsAdmin = admin;
		}

        public User(Jid jid, bool admin, string sid)
        {
            if (jid == null) throw new ArgumentNullException("jid");

            Jid = jid;
            IsAdmin = admin;
            Sid = sid;
        }

		public override string ToString()
		{
			return Jid.ToString();
		}

		public override int GetHashCode()
		{
			return Jid.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var u = obj as User;
			return u != null && Jid.Equals(u.Jid);
		}
	}
}