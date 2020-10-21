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

using ASC.Xmpp.Core.protocol.tls;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using System;
using System.Collections.Generic;

namespace ASC.Xmpp.Server.Services.Jabber
{
    [XmppHandler(typeof(StartTls))]
    class StartTlsHandler : IXmppStreamHandler
    {
        public void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
        {
            var tcpXmppListener = (TcpXmppListener)(context.XmppGateway.GetXmppListener("Jabber Listener"));
            if (!stream.Authenticated && tcpXmppListener.StartTls != XmppStartTlsOption.None && tcpXmppListener.Certificate != null)
            {
                var connection = tcpXmppListener.GetXmppConnection(stream.ConnectionId) as TcpXmppConnection;
                if (connection != null)
                {
                    var proceed = new Proceed();
                    context.Sender.SendTo(stream, proceed);
                    connection.StartTls(tcpXmppListener.Certificate);
                    return;
                }
            }
            var failure = new Failure();
            context.Sender.SendToAndClose(stream, failure);
        }

        public void StreamEndHandle(XmppStream stream, ICollection<Node> notSendedBuffer, XmppHandlerContext context)
        {
        }

        public void OnRegister(IServiceProvider serviceProvider)
        {
        }

        public void OnUnregister(IServiceProvider serviceProvider)
        {
        }
    }
}
