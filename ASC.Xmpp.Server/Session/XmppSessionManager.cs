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
using ASC.Xmpp.Core.protocol.client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Xmpp.Server.Session
{
    public class XmppSessionManager
    {
        private readonly ConcurrentDictionary<Jid, XmppSession> sessions = new ConcurrentDictionary<Jid, XmppSession>();


        public XmppSession GetSession(Jid jid)
        {
            if (jid == null)
            {
                throw new ArgumentNullException("jid");
            }

            if (jid.HasResource)
            {
                XmppSession s;
                sessions.TryGetValue(jid, out s);
                return s;
            }
            return sessions.Values
                .Where(s => s.Jid.Bare == jid.Bare)
                .OrderBy(s => s.Priority)
                .LastOrDefault();
        }

        public XmppSession GetAvailableSession(Jid jid)
        {
            if (jid == null)
            {
                throw new ArgumentNullException("jid");
            }

            if (jid.HasResource)
            {
                XmppSession s;
                sessions.TryGetValue(jid, out s);
                return s;
            }
            return sessions.Values
                .Where(s => s.Jid.Bare == jid.Bare && s.Presence != null && s.Presence.Type != PresenceType.unavailable)
                .OrderBy(s => s.Priority)
                .LastOrDefault();
        }

        public ICollection<XmppSession> GetSessions()
        {
            return sessions.Values;
        }

        public IEnumerable<XmppSession> GetStreamSessions(string streamId)
        {
            if (string.IsNullOrEmpty(streamId))
            {
                return new XmppSession[0];
            }

            return sessions.Values
                .Where(s => s.Stream.Id == streamId)
                .ToList();
        }

        public ICollection<XmppSession> GetBareJidSessions(Jid jid)
        {
            return GetBareJidSessions(jid, GetSessionsType.All);
        }

        public ICollection<XmppSession> GetBareJidSessions(Jid jid, GetSessionsType getType)
        {
            if (jid == null)
            {
                return new XmppSession[0];
            }

            var bares = sessions.Values.Where(s => s.Jid.Bare == jid.Bare);
            if (getType == GetSessionsType.Available)
            {
                bares = bares.Where(s => s.Available);
            }
            else if (getType == GetSessionsType.RosterRequested)
            {
                bares = bares.Where(s => s.RosterRequested);
            }
            return bares.ToList();
        }

        public void AddSession(XmppSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            sessions.TryAdd(session.Jid, session);
        }

        public void CloseSession(Jid jid)
        {
            var session = GetSession(jid);
            if (session != null && !session.IsSignalRFake && session.Available)
            {
                SoftInvokeEvent(SessionUnavailable, session);
            }

            sessions.TryRemove(jid, out session);
        }

        public void SetSessionPresence(XmppSession session, Presence presence)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            if (presence == null)
            {
                throw new ArgumentNullException("presence");
            }

            var oldPresence = session.Presence;
            session.Presence = presence;
            if (!IsAvailablePresence(oldPresence) && IsAvailablePresence(presence))
            {
                SoftInvokeEvent(SessionAvailable, session);
            }
            if (IsAvailablePresence(oldPresence) && !IsAvailablePresence(presence))
            {
                SoftInvokeEvent(SessionUnavailable, session);
            }
        }

        public event EventHandler<XmppSessionArgs> SessionAvailable;

        public event EventHandler<XmppSessionArgs> SessionUnavailable;

        private void SoftInvokeEvent(EventHandler<XmppSessionArgs> eventHandler, XmppSession session)
        {
            try
            {
                var handler = eventHandler;
                if (handler != null) handler(this, new XmppSessionArgs(session));
            }
            catch { }
        }

        private bool IsAvailablePresence(Presence presence)
        {
            return presence != null && (presence.Type == PresenceType.available || presence.Type == PresenceType.invisible);
        }
    }

    [Flags]
    public enum GetSessionsType
    {
        RosterRequested = 1,
        Available = 2,
        All = 3,
    }
}
