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
using ASC.Xmpp.Core.protocol.extensions.bytestreams;
using ASC.Xmpp.Core.protocol.extensions.filetransfer;
using ASC.Xmpp.Core.protocol.extensions.jivesoftware.phone;
using ASC.Xmpp.Core.protocol.extensions.si;
using ASC.Xmpp.Core.protocol.iq.jingle;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

using XmppIbb = ASC.Xmpp.Core.protocol.extensions.ibb;

namespace ASC.Xmpp.Server.Services.Jabber
{
    //si
    [XmppHandler(typeof(SI))]

    //bytestreams
    [XmppHandler(typeof(Activate))]
    [XmppHandler(typeof(ByteStream))]
    [XmppHandler(typeof(StreamHost))]
    [XmppHandler(typeof(StreamHostUsed))]
    [XmppHandler(typeof(UdpSuccess))]

    //filetransfer
    [XmppHandler(typeof(File))]
    [XmppHandler(typeof(Range))]

    //ibb
    [XmppHandler(typeof(XmppIbb.Base))]
    [XmppHandler(typeof(XmppIbb.Close))]
    [XmppHandler(typeof(XmppIbb.Data))]
    [XmppHandler(typeof(XmppIbb.Open))]

    //livesoftware.phone
    [XmppHandler(typeof(PhoneAction))]
    [XmppHandler(typeof(PhoneEvent))]
    [XmppHandler(typeof(PhoneStatus))]

    //jingle
    [XmppHandler(typeof(GoogleJingle))]
    [XmppHandler(typeof(Jingle))]
    [XmppHandler(typeof(Core.protocol.iq.jingle.Server))]
    [XmppHandler(typeof(Stun))]
    class TransferHandler : XmppStanzaHandler
    {
        public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            if (!iq.HasTo || !iq.To.HasUser) return XmppStanzaError.ToServiceUnavailable(iq);

            var session = context.SessionManager.GetSession(iq.To);
            if (session != null) context.Sender.SendTo(session, iq);
            return null;
        }
    }
}