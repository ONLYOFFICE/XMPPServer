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
using ASC.Xmpp.Core.protocol.iq.roster;
using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Storage.Interface;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Utils;
using System;
using System.Collections.Generic;
using Uri = ASC.Xmpp.Core.protocol.Uri;

namespace ASC.Xmpp.Server.Services.Jabber
{
    class OfflineProvider : IXmppStreamHandler
    {
        private IOfflineStore offlineStore;

        private IRosterStore rosterStore;

        private IXmppSender sender;

        private XmppSessionManager sessionManager;

        private XmppHandlerManager handlerManager;

        private readonly Jid jid;


        public OfflineProvider(Jid jid)
        {
            if (jid == null) throw new ArgumentNullException("jid");

            this.jid = jid;
        }

        public void OnRegister(IServiceProvider serviceProvider)
        {
            offlineStore = ((StorageManager)serviceProvider.GetService(typeof(StorageManager))).OfflineStorage;
            rosterStore = ((StorageManager)serviceProvider.GetService(typeof(StorageManager))).RosterStorage;
            sender = (IXmppSender)serviceProvider.GetService(typeof(IXmppSender));
            sessionManager = (XmppSessionManager)serviceProvider.GetService(typeof(XmppSessionManager));
            handlerManager = (XmppHandlerManager)serviceProvider.GetService(typeof(XmppHandlerManager));

            sessionManager.SessionAvailable += SessionAvailable;
            sessionManager.SessionUnavailable += SessionUnavailable;
        }

        public void OnUnregister(IServiceProvider serviceProvider)
        {
            sessionManager.SessionAvailable -= SessionAvailable;
            sessionManager.SessionUnavailable -= SessionUnavailable;
        }


        private void SessionAvailable(object sender, XmppSessionArgs e)
        {
            if (e.Session.Jid.Server == jid.Server)
            {
                SendRosterPresences(e.Session);
                SendOfflinePresences(e.Session);
                SendOfflineMessages(e.Session);
            }
        }

        private void SessionUnavailable(object sender, XmppSessionArgs e)
        {
            if (e.Session.Jid.Server == jid.Server)
            {
                try
                {
                    offlineStore.SaveLastActivity(e.Session.Jid, new LastActivity(e.Session.Presence != null ? e.Session.Presence.Status : null));
                }
                catch { }

                if (e.Session.Available)
                {
                    var presence = new Presence() { Type = PresenceType.unavailable, From = e.Session.Jid, };
                    handlerManager.ProcessStreamElement(presence, e.Session.Stream);
                }
            }
        }


        private void SendOfflineMessages(XmppSession session)
        {
            if (!session.IsSignalRFake)
            {
                var domain = session.Stream.Domain;
                foreach (var m in offlineStore.GetOfflineMessages(session.Jid))
                {
                    var delay = new Element("delay");
                    delay.SetNamespace("urn:xmpp:delay");
                    delay.SetAttribute("From", domain);
                    delay.SetAttribute("stamp", Time.Date(m.XDelay.Stamp));
                    delay.Value = "Offline Storage";
                    m.AddChild(delay);

                    m.XDelay.From = new Jid(domain);
                    m.XDelay.Value = "Offline Storage";

                    sender.SendTo(session, m);
                }
                if (session.Jid.Resource != "TMTalk")
                {
                    offlineStore.RemoveAllOfflineMessages(session.Jid);
                }
            }
        }

        private void SendOfflinePresences(XmppSession session)
        {
            foreach (var presence in offlineStore.GetOfflinePresences(session.Jid))
            {
                sender.SendTo(session, presence);
            }
            offlineStore.RemoveAllOfflinePresences(session.Jid);
        }

        private void SendRosterPresences(XmppSession session)
        {
            //It's correct!
            //Get other statuses
            foreach (var ri in rosterStore.GetRosterItems(session.Jid))
            {
                //"none" -- the user does not have a subscription to the contact's presence information, and the contact does not have a subscription to the user's presence information
                //"to" -- the user has a subscription to the contact's presence information, but the contact does not have a subscription to the user's presence information
                //"from" -- the contact has a subscription to the user's presence information, but the user does not have a subscription to the contact's presence information
                //"both" -- both the user and the contact have subscriptions to each other's presence information
                if (ri.Subscribtion == SubscriptionType.to || ri.Subscribtion == SubscriptionType.both)
                {
                    foreach (var contactSession in sessionManager.GetBareJidSessions(ri.Jid))
                    {
                        if (contactSession.Presence != null)
                        {
                            //Send roster contact presence to newly availible session
                            contactSession.Presence.To = null;//To no one
                            sender.SendTo(session, contactSession.Presence);

                            if (contactSession.GetRosterTime.AddMinutes(1) < DateTime.UtcNow)
                            {
                                //Send roster push
                                var roster = new Roster();
                                roster.AddRosterItem(rosterStore.GetRosterItem(contactSession.Jid, session.Jid).ToRosterItem());
                                var iq = new IQ
                                {
                                    Type = IqType.set,
                                    Namespace = Uri.SERVER,
                                    Id = UniqueId.CreateNewId(),
                                    To = contactSession.Jid,
                                    Query = roster
                                };
                                sender.SendTo(contactSession, iq);
                            }
                        }
                    }
                }
            }
            var server = new Presence() { Type = PresenceType.available, From = new Jid(session.Jid.Server) };
            sender.SendTo(session, server);
        }

        public void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
        {

        }

        public void StreamEndHandle(XmppStream stream, ICollection<Node> notSendedBuffer, XmppHandlerContext context)
        {
            if (notSendedBuffer.Count == 0 || string.IsNullOrEmpty(stream.User)) return;

            var messages = new List<Message>();
            foreach (var node in notSendedBuffer)
            {
                if (node is Presence)
                {
                    var p = (Presence)node;
                    if (p.Type == PresenceType.subscribe || p.Type == PresenceType.subscribed || p.Type == PresenceType.unsubscribe || p.Type == PresenceType.unsubscribed)
                    {
                        offlineStore.SaveOfflinePresence(p);
                    }
                }
                if (node is Message) messages.Add((Message)node);
            }
            offlineStore.SaveOfflineMessages(messages.ToArray());
        }
    }
}