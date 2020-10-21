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
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Notify.Jabber;
using ASC.Core.Notify.Jabber;
using ASC.Core.Notify.Signalr;
using ASC.Core.Users;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.extensions.commands;
using ASC.Xmpp.Server;
using ASC.Xmpp.Server.Configuration;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Services.Jabber;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Streams;

using Stream = ASC.Xmpp.Core.protocol.Stream;

namespace ASC.Xmpp.Host
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true,
        InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public class JabberService : IJabberService
    {
        private static readonly ILog _log = LogManager.GetLogger("ASC");
        private readonly XmppServer _xmppServer;
        private readonly SignalrServiceClient signalrServiceClient;

        public JabberService(XmppServer xmppServer)
        {
            _xmppServer = xmppServer;
            signalrServiceClient = new SignalrServiceClient("chat");
        }


        public string GetVersion()
        {
            return ConfigurationManager.AppSettings["version.number"] ?? "1.0.0";
        }
        public int GetNewMessagesCount(int tenantId, string userName)
        {
            var count = 0;
            try
            {
                count = _xmppServer.StorageManager.OfflineStorage.GetOfflineMessagesCount(GetJid(userName, tenantId));
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            return count;
        }

        public string GetUserToken(int tenantId, string userName)
        {
            string token = null;
            try
            {
                token = _xmppServer.AuthManager.GetUserToken(userName);
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            return token;
        }

        public void SendMessage(int tenantId, string from, string to, string text, string subject)
        {

            try
            {
                _log.DebugFormat("Send Message: tenantId={0}, from={1}, to={2}, text={3}", tenantId, from, to, text);
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                if (from == null)
                {
                    signalrServiceClient.SendMessage(string.Empty, to.ToLowerInvariant(), text, tenantId, string.Empty);
                }
                var jidFrom = GetJid(from, tenantId);
                var jidTo = to != string.Empty ? GetJid(to, tenantId) : new Jid(jidFrom.Server);
                var message = new Message(jidTo, jidFrom, MessageType.chat, text);

                var sessions = _xmppServer.SessionManager.GetBareJidSessions(jidTo, GetSessionsType.All);
                if (sessions.Count != 0)
                {
                    foreach (var session in sessions)
                    {
                        if (session != null && !session.IsSignalRFake)
                        {
                            ((IXmppSender)_xmppServer.GetService(typeof(IXmppSender))).SendTo(session, message);
                           
                        }
                    }
                }
                else
                {
                    _xmppServer.StorageManager.OfflineStorage.SaveOfflineMessages(message);
                }

                var handlers = _xmppServer.HandlerManager.HandlerStorage.GetStanzaHandlers(jidFrom, typeof(Message));
                if (handlers.Count > 1)
                {
                    var messageArchiveHandler = handlers[1] as MessageArchiveHandler;
                    if (messageArchiveHandler != null)
                    {
                        messageArchiveHandler.HandleMessage(null, message, null);
                    }
                }
                else
                {
                    var messageArchiveHandler = new MessageArchiveHandler();
                    messageArchiveHandler.HandleMessage(null, message, null);
                    messageArchiveHandler.FlushMessageBuffer();
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        public void SendCommand(int tenantId, string from, string to, string command, bool fromTenant)
        {
            try
            {
                _log.DebugFormat("Send Command: tenantId={0}, from={1}, to={2}, text={3}", tenantId, from, to, command);

                if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || string.IsNullOrEmpty(command)) return;
                var toJid = GetJid(to, tenantId, "TMTalk");
                var iq = new IQ(IqType.set, fromTenant ? new Jid(from) : GetJid(from, tenantId/*, "TMTalk"*/), toJid)
                {
                    Query = new Command(command)
                };
                var session = _xmppServer.SessionManager.GetSession(toJid);
                if (session != null)
                {
                    var sender = (IXmppSender)_xmppServer.GetService(typeof(IXmppSender));
                    sender.SendTo(session, iq);
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        public byte AddXmppConnection(string connectionId, string userName, byte state, int tenantId)
        {
            try
            {
                _log.DebugFormat("Add Xmpp Connection: connectionId={0}, userName={1}, state={2}, tenantId={3}", connectionId, userName, state, tenantId);
                var jid = GetJid(userName, tenantId, SignalRHelper.SIGNALR_RESOURCE);
                var listener = (SignalRXmppListener)((XmppGateway)_xmppServer.GetService(typeof(IXmppReceiver))).GetXmppListener("SignalR Listener");
                if (listener.GetXmppConnection(connectionId) != null)
                {
                    RemoveXmppConnection(connectionId, userName, tenantId);
                }
                listener.AddXmppConnection(connectionId, _xmppServer);
                var xmppStream = ((XmppStreamManager)_xmppServer.GetService(typeof(XmppStreamManager))).GetOrCreateNewStream(connectionId);
                xmppStream.Authenticate(userName);
                string domain = CoreContext.TenantManager.GetTenant(tenantId).TenantDomain;
                if (JabberConfiguration.ReplaceDomain && domain.EndsWith(JabberConfiguration.ReplaceFromDomain))
                {
                    int place = domain.LastIndexOf(JabberConfiguration.ReplaceFromDomain);
                    if (place >= 0)
                    {
                        domain = domain.Remove(place, JabberConfiguration.ReplaceFromDomain.Length).Insert(place, JabberConfiguration.ReplaceToDomain);
                    }
                }
                xmppStream.Domain = domain;
                xmppStream.Connected = true;
                xmppStream.BindResource(SignalRHelper.SIGNALR_RESOURCE);

                var handler = _xmppServer.HandlerManager.HandlerStorage.GetStreamStartHandlers(jid)[0];
                var stream = new Stream
                {
                    To = new Jid(jid.Server),
                    Namespace = "http://etherx.jabber.org/streams",
                    Version = "1.6",
                    Language = string.Empty
                };

                handler.StreamStartHandle(xmppStream, stream, null);

                var session = new XmppSession(jid, xmppStream)
                {
                    RosterRequested = false,
                    Active = true,
                    IsSignalRFake = true
                };

                ((XmppSessionManager)_xmppServer.GetService(typeof(XmppSessionManager))).AddSession(session);

                var presence = new Presence(SignalRHelper.GetShowType(state), String.Empty, SignalRHelper.PRIORITY)
                {
                    From = jid,
                    Type = SignalRHelper.GetPresenceType(state)
                };

                _xmppServer.SessionManager.SetSessionPresence(session, presence);

                var sender = (IXmppSender)_xmppServer.GetService(typeof(IXmppSender));
                var sessions = _xmppServer.SessionManager.GetSessions().Where(s => s.Id != session.Id).ToArray();
                sender.Broadcast(sessions, session.Presence);

                var offlineMessages = _xmppServer.StorageManager.OfflineStorage.GetOfflineMessages(jid);
                if (offlineMessages.Count > 0)
                {
                    var users = new List<string>();
                    for (int i = 0; i < offlineMessages.Count; i++)
                    {
                        var from = offlineMessages[i].From;
                        var name = from.User != null ? from.User.ToLowerInvariant() : string.Empty;
                        if (!users.Contains(name))
                        {
                            users.Add(name);
                        }
                    }
                    signalrServiceClient.SendOfflineMessages(userName, users, tenantId);
                    //_xmppServer.StorageManager.OfflineStorage.RemoveAllOfflineMessages(jid);
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            return GetState(tenantId, userName);
        }

        public byte RemoveXmppConnection(string connectionId, string userName, int tenantId)
        {
            try
            {
                _log.DebugFormat("Remove Xmpp Connection: connectionId={0}, userName={1}, tenantId={2}", connectionId, userName, tenantId);

                var jid = GetJid(userName, tenantId, SignalRHelper.SIGNALR_RESOURCE);
                var listener = (SignalRXmppListener)((XmppGateway)_xmppServer.GetService(typeof(IXmppReceiver))).GetXmppListener("SignalR Listener");
                _xmppServer.SessionManager.CloseSession(jid);
                _xmppServer.StreamManager.RemoveStream(connectionId);
                listener.CloseXmppConnection(connectionId);
                var sender = (IXmppSender)_xmppServer.GetService(typeof(IXmppSender));
                sender.Broadcast(_xmppServer.SessionManager.GetSessions(),
                    new Presence { Priority = SignalRHelper.PRIORITY, From = jid, Type = PresenceType.unavailable });
                var userSession = _xmppServer.SessionManager.GetAvailableSession(jid.BareJid);
                if (userSession != null && userSession.Presence != null && userSession.Presence.Type != PresenceType.unavailable)
                {
                    return SignalRHelper.GetState(userSession.Presence.Show, userSession.Presence.Type);
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            return SignalRHelper.USER_OFFLINE;
        }

        public byte SendState(int tenantId, string userName, byte state)
        {
            try
            {
                _log.DebugFormat("Send State: tenantId={0}, userName={1}, state={2}", tenantId, userName, state);
                var jid = GetJid(userName, tenantId, SignalRHelper.SIGNALR_RESOURCE);
                var userSession = _xmppServer.SessionManager.GetSession(jid);
                if (userSession != null)
                {
                    var sessions = _xmppServer.SessionManager.GetSessions().Where(s => s.Id != userSession.Id).ToArray();
                    var sender = (IXmppSender)_xmppServer.GetService(typeof(IXmppSender));
                    var presence = GetNewPresence(state, null, jid);

                    _xmppServer.SessionManager.SetSessionPresence(userSession, presence);
                    sender.Broadcast(sessions, presence);
                }
                return GetState(tenantId, userName);
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            return SignalRHelper.USER_OFFLINE;
        }

        public MessageClass[] GetRecentMessages(int tenantId, string from, string to, int id)
        {
            MessageClass[] messageClasses = null;
            try
            {
                _log.DebugFormat("Get Recent Messages: tenantId={0}, from={1}, to={2}, id={3}", tenantId, from, to, id);
                var jidFrom = GetJid(from, tenantId);
                var jidTo = GetJid(to, tenantId);
                var archiveStore = ((StorageManager)_xmppServer.GetService(typeof(StorageManager))).GetStorage<DbMessageArchive>("archive");
                var handlers = _xmppServer.HandlerManager.HandlerStorage.GetStanzaHandlers(jidFrom, typeof(Message));
                if (handlers.Count > 1)
                {
                    var messageArchiveHandler = handlers[1] as MessageArchiveHandler;
                    if (messageArchiveHandler != null)
                    {
                        messageArchiveHandler.FlushMessageBuffer();
                    }
                }

                var messages = archiveStore.GetMessages(jidFrom, jidTo, id, SignalRHelper.NUMBER_OF_RECENT_MSGS);

                messageClasses = new MessageClass[messages.Length];
                for (int i = 0; i < messages.Length; i++)
                {
                    messageClasses[i] = new MessageClass();
                    messageClasses[i].DateTime = messages[i].XDelay != null ? messages[i].XDelay.Stamp : messages[i].DbStamp;
                    messageClasses[i].Id = messages[i].InternalId;
                    messageClasses[i].Text = messages[i].Body;
                    messageClasses[i].UserName = messages[i].From.User;
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            return messageClasses;
        }

        public Dictionary<string, byte> GetAllStates(int tenantId, string from)
        {
            var states = new Dictionary<string, byte>();
            try
            {
                _log.Debug("Get All States");
                var userJid = GetJid(from, tenantId);
                ASCContext.SetCurrentTenant(userJid.Server);

                foreach (var user in ASCContext.UserManager.GetUsers().Where(u => !u.IsMe()))
                {
                    userJid = GetJid(user.UserName, tenantId);
                    var session = _xmppServer.SessionManager.GetAvailableSession(userJid.BareJid);
                    if (session != null && session.Presence != null && session.Presence.Type != PresenceType.unavailable)
                    {
                        states.Add(userJid.User, SignalRHelper.GetState(session.Presence.Show, session.Presence.Type));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            return states;
        }

        public byte GetState(int tenantId, string from)
        {
            try
            {
                var session = _xmppServer.SessionManager.GetAvailableSession(GetJid(from, tenantId).BareJid);
                if (session != null && session.Presence != null && session.Presence.Type != PresenceType.unavailable)
                {
                    return SignalRHelper.GetState(session.Presence.Show, session.Presence.Type);
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            return SignalRHelper.USER_OFFLINE;
        }

        public void Ping(string connectionId, int tenantId, string userName, byte state)
        {
            try
            {
                _log.DebugFormat("Ping, connectionId={0}, tenantId={1}, userName={2}, state={3}", connectionId, tenantId, userName, state);
                var listener = (SignalRXmppListener)((XmppGateway)_xmppServer.GetService(typeof(IXmppReceiver))).GetXmppListener("SignalR Listener");
                var connection = listener.GetXmppConnection(connectionId) as SignalRXmppConnection;
                if (connection != null)
                {
                    connection.UpdateTimeout();
                }
                else
                {
                    AddXmppConnection(connectionId, userName, state, tenantId);
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        public string HealthCheck(string userName, int tenantId)
        {
            return string.Empty;
        }

        private Presence GetNewPresence(byte state, Presence presence, Jid jid)
        {
            if (presence == null)
            {
                presence = new Presence(SignalRHelper.GetShowType(state), String.Empty) { From = jid, Priority = SignalRHelper.PRIORITY };
            }

            presence.Show = SignalRHelper.GetShowType(state);
            presence.Type = SignalRHelper.GetPresenceType(state);

            return presence;
        }

        private Jid GetJid(string userName, int tenant, string resource = null)
        {
            var t = CoreContext.TenantManager.GetTenant(tenant);
            if (t == null)
            {
                throw new Exception(string.Format("Tenant with id = {0} not found.", tenant));
            }
            string domain = t.TenantDomain;
            if (JabberConfiguration.ReplaceDomain && domain.EndsWith(JabberConfiguration.ReplaceToDomain))
            {
                int place = domain.LastIndexOf(JabberConfiguration.ReplaceToDomain);
                if (place >= 0)
                {
                    domain = domain.Remove(place, JabberConfiguration.ReplaceToDomain.Length).Insert(place, JabberConfiguration.ReplaceFromDomain);
                }
            }
            return new Jid(userName, domain, resource);
        }
    }
}