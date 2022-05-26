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
using ASC.Xmpp.Core.protocol.extensions.commands;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Jabber
{
    [XmppHandler(typeof(Command))]
    class CommandHandler : XmppStanzaHandler
    {
        public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            if (!iq.HasTo || !iq.To.HasUser) return XmppStanzaError.ToServiceUnavailable(iq);

            var session = context.SessionManager.GetSession(iq.To);
            if (session != null)
            {
                context.Sender.SendTo(session, iq);
                return null;
            }
            else
            {
                return XmppStanzaError.ToRecipientUnavailable(iq);
            }
        }
    }
}