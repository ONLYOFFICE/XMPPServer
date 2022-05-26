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

using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.roster;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Services.Jabber
{
    [XmppHandler(typeof(Roster))]
    class RosterHandler : XmppStanzaHandler
    {
        public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            if (iq.HasTo && iq.To != iq.From) return XmppStanzaError.ToForbidden(iq);

            if (iq.Type == IqType.get) return GetRoster(stream, iq, context);
            else if (iq.Type == IqType.set) return SetRoster(stream, iq, context);
            else return null;
        }

        private IQ GetRoster(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            var answer = new IQ(IqType.result);
            answer.Id = iq.Id;
            answer.To = iq.From;
            var roster = new Roster();
            answer.Query = roster;

            foreach (var item in context.StorageManager.RosterStorage.GetRosterItems(iq.From))
            {
                roster.AddRosterItem(item.ToRosterItem());
            }
            var session = context.SessionManager.GetSession(iq.From);
            session.RosterRequested = true;
            session.GetRosterTime = DateTime.UtcNow;
            return answer;
        }

        private IQ SetRoster(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            var answer = new IQ(IqType.result);
            answer.Id = iq.Id;
            answer.To = iq.From;
            answer.From = iq.To;

            iq.Id = UniqueId.CreateNewId();
            var roster = (Roster)iq.Query;
            UserRosterItem item = null;
            try
            {
                var rosterItems = roster.GetRoster();
                if (rosterItems.Length != 1) throw new JabberException(ErrorCode.BadRequest);

                var rosterItem = rosterItems[0];
                item = UserRosterItem.FromRosterItem(rosterItem);

                if (rosterItem.Subscription == SubscriptionType.remove)
                {
                    context.StorageManager.RosterStorage.RemoveRosterItem(iq.From, item.Jid);

                    //Send presences
                    var unsubscribe = new Presence() { Type = PresenceType.unsubscribe, To = item.Jid, From = iq.From };
                    var unsubscribed = new Presence() { Type = PresenceType.unsubscribed, To = item.Jid, From = iq.From };
                    var unavailable = new Presence() { Type = PresenceType.unavailable, To = item.Jid, From = iq.From };

                    bool sended = false;
                    foreach (var session in context.SessionManager.GetBareJidSessions(item.Jid))
                    {
                        if (session.RosterRequested)
                        {
                            context.Sender.SendTo(session, unsubscribe);
                            context.Sender.SendTo(session, unsubscribed);
                            sended = true;
                        }
                        context.Sender.SendTo(session, unavailable);
                    }
                    if (!sended)
                    {
                        context.StorageManager.OfflineStorage.SaveOfflinePresence(unsubscribe);
                        context.StorageManager.OfflineStorage.SaveOfflinePresence(unsubscribed);
                    }
                }
                else
                {
                    item = context.StorageManager.RosterStorage.SaveRosterItem(iq.From, item);
                    roster.RemoveAllChildNodes();
                    roster.AddRosterItem(item.ToRosterItem());
                }
                //send all available user's resources
                context.Sender.Broadcast(context.SessionManager.GetBareJidSessions(iq.From), iq);
            }
            catch (System.Exception)
            {
                roster.RemoveAllChildNodes();
                item = context.StorageManager.RosterStorage.GetRosterItem(iq.From, item.Jid);
                if (item != null)
                {
                    roster.AddRosterItem(item.ToRosterItem());
                    context.Sender.Broadcast(context.SessionManager.GetBareJidSessions(iq.From), iq);
                }
                throw;
            }

            return answer;
        }
    }
}