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
using ASC.Xmpp.Server.Services.Muc2.Room.Member;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Xmpp.Server.Services.Muc2.Room
{
    internal class MucRoomMemberCollection : List<MucRoomMember>
    {
        private readonly XmppServiceManager manager;

        public MucRoomMemberCollection(XmppServiceManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            this.manager = manager;
        }

        internal event MemberActionDelegate MemberAdded = null;

        private void InvokeMemberAdded(MucRoomMember member)
        {
            MemberActionDelegate evt = MemberAdded;
            if (evt != null)
            {
                evt(member);
            }
        }

        internal event MemberActionDelegate MemberRemoved = null;

        private void InvokeMemberRemoved(MucRoomMember member)
        {
            MemberActionDelegate evt = MemberRemoved;
            if (evt != null)
            {
                evt(member);
            }
        }

        internal new void Add(MucRoomMember member)
        {
            if (!Contains(member))
            {
                base.Add(member);
                manager.RegisterService(member);
                InvokeMemberAdded(member);
            }
            else
            {
                throw new Exceptions.MucMemberExistsException();
            }
        }

        internal new void Remove(MucRoomMember member)
        {
            if (Contains(member))
            {
                base.Remove(member);
                InvokeMemberRemoved(member);
                manager.UnregisterService(member.Jid);
            }
        }

        internal new void Clear()
        {
            foreach (MucRoomMember member in this)
            {
                manager.UnregisterService(member.Jid);
            }
            base.Clear();
        }

        internal MucRoomMember FindByJid(Jid jid)
        {
            return Find((x) => x.Jid.Equals(jid));
        }

        internal MucRoomMember FindByRealJid(Jid jid)
        {
            return Find((x) => jid.Equals(x.RealJid));
        }

        internal MucRoomMember this[string jid]
        {
            get
            {
                return FindByJid(new Jid(jid));
            }
        }
        internal MucRoomMember this[Jid jid]
        {
            get
            {
                return FindByJid(jid);
            }
        }

        public void RebindAddress(MucRoomMember member, Jid address)
        {
            manager.UnregisterService(member.Jid);
            member.Jid = address;
            manager.RegisterService(member);
        }

        public MucRoomMember FindByNick(string nickname)
        {
            foreach (MucRoomMember member in this)
            {
                if (member != null && member.Nick != null && member.Nick.Equals(nickname, StringComparison.OrdinalIgnoreCase))
                {
                    return member;
                }
            }
            return null;
        }

        public MucRoomMember FindByBareJid(Jid jidBare)
        {
            return this.Where(member => member != null && member.RealJid != null && member.RealJid.Bare.Equals(jidBare.Bare, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
        }
    }
}