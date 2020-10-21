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

using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.extensions.ping;
using ASC.Xmpp.Core.protocol.iq.time;
using ASC.Xmpp.Core.protocol.iq.version;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Services.Jabber
{
    [XmppHandler(typeof(Version))]
    [XmppHandler(typeof(EntityTime))]
    [XmppHandler(typeof(Ping))]
    class VerTimePingHandler : XmppStanzaHandler
    {
        public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            var answer = new IQ(IqType.result)
            {
                Id = iq.Id,
                To = iq.From,
                From = iq.To,
            };

            //iq sended to server
            if (iq.Type == IqType.get && (!iq.HasTo || iq.To.IsServer || iq.To == iq.From))
            {
                if (iq.GetTag(typeof(Version)) != null)
                {
                    answer.Query = new Version()
                    {
                        Name = "OnlyOffice Jabber Server",
                        Os = System.Environment.OSVersion.ToString(),
                        Ver = "1.0",
                    };
                    return answer;
                }
                else if (iq.GetTag(typeof(Ping)) != null)
                {
                    return answer;
                }
                return XmppStanzaError.ToServiceUnavailable(iq);
            }

            if (iq.Type == IqType.get && iq.HasTo)
            {
                //resend iq
                var sessionTo = context.SessionManager.GetSession(iq.To);
                var sessionFrom = context.SessionManager.GetSession(iq.From);
                if (sessionTo != null && sessionFrom != null)
                {
                    if (string.IsNullOrEmpty(iq.Id))
                    {
                        iq.Id = System.Guid.NewGuid().ToString("N");
                    }

                    IdleWatcher.StartWatch(
                        iq.Id + iq.From,
                        System.TimeSpan.FromSeconds(3),
                        (s, e) => { context.Sender.SendTo(sessionFrom, XmppStanzaError.ToServiceUnavailable(iq)); });
                    context.Sender.SendTo(sessionTo, iq);
                }
                else
                {
                    return XmppStanzaError.ToRecipientUnavailable(iq);
                }
            }
            if (iq.Type == IqType.error || iq.Type == IqType.result)
            {
                if (!iq.HasTo)
                {
                    return XmppStanzaError.ToBadRequest(iq);
                }

                IdleWatcher.StopWatch(iq.Id + iq.To);
                var session = context.SessionManager.GetSession(iq.To);
                if (session != null)
                {
                    context.Sender.SendTo(session, iq);
                }
            }
            return null;
        }
    }
}