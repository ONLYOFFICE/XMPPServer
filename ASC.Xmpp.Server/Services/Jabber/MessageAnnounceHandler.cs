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
using ASC.Xmpp.Core.protocol.extensions.nickname;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Xmpp.Server.Services.Jabber
{
    [XmppHandler(typeof(Message))]
    class MessageAnnounceHandler : XmppStanzaHandler
    {
        public const string ANNOUNCE = "announce";

        public const string ONLINE = "online";

        public const string ONLINEBROADCAST = "onlinebroadcast";

        public const string SERVICE = "service";

        public const string MOTD = "motd";

        public override void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context)
        {
            if (!message.HasTo || !message.To.HasResource) return;

            string[] commands = message.To.Resource.Split('/');
            if (commands.Length == 1 && commands[0] == ANNOUNCE)
            {
                Announce(stream, message, context);
            }
            else if (commands.Length == 2 && commands[1] == ONLINE)
            {
                AnnounceOnline(stream, message, context);
            }
            else if (commands.Length == 2 && commands[1] == ONLINEBROADCAST)
            {
                AnnounceOnlineBroadcast(stream, message, context);
            }
            else if (commands.Length == 2 && commands[1] == SERVICE)
            {
                AnnounceService(stream, message, context);
            }
            else
            {
                context.Sender.SendTo(stream, XmppStanzaError.ToServiceUnavailable(message));
            }
        }

        private void Announce(XmppStream stream, Message message, XmppHandlerContext context)
        {
            var userName = GetUser(message);
            message.Body = string.Format("{0} announces {1}", userName, message.Body);
            var offlineMessages = new List<Message>();

            foreach (var user in context.UserManager.GetUsers(stream.Domain))
            {
                message.To = user.Jid;
                var session = context.SessionManager.GetSession(message.To);
                if (session != null)
                {
                    context.Sender.SendTo(session, message);
                }
                else
                {
                    offlineMessages.Add(message);
                }
            }
            context.StorageManager.OfflineStorage.SaveOfflineMessages(offlineMessages.ToArray());
        }

        private void AnnounceOnline(XmppStream stream, Message message, XmppHandlerContext context)
        {
            foreach (var session in context.SessionManager.GetSessions().Where(x => x.Available))
            {
                message.To = session.Jid;
                context.Sender.SendTo(session, message);
            }
        }

        private void AnnounceOnlineBroadcast(XmppStream stream, Message message, XmppHandlerContext context)
        {
            string user = GetUser(message);
            message.Body = string.Format("{0} says:\r\n{1}", user, message.Body);
            AnnounceService(stream, message, context);
        }

        private void AnnounceService(XmppStream stream, Message message, XmppHandlerContext context)
        {
            message.From = new Jid(stream.Domain);
            message.Nickname = null;
            AnnounceOnline(stream, message, context);
        }

        private string GetUser(Message message)
        {
            var nick = message.SelectSingleElement<Nickname>();
            return nick != null ? nick.Value : message.From.User;
        }
    }
}