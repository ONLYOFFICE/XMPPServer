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

using ASC.Xmpp.Core.protocol.component;
using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Authorization
{
    [XmppHandler(typeof(Handshake))]
    class AuthHandshakeHandler : XmppStreamHandler
    {
        private readonly string password;

        public AuthHandshakeHandler(string password)
        {
            this.password = password;
        }

        public override void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
        {
            if (stream.Authenticated) return;

            var handshake = (Handshake)element;
            string digest = handshake.Digest;
            string hash = Hash.Sha1Hash(stream.Id + password);
            if (string.Compare(hash, digest, StringComparison.OrdinalIgnoreCase) == 0)
            {
                context.Sender.SendTo(stream, new Handshake()); //TODO: auth with sha1
                                                                //stream.Authenticated = true;
            }
            else
            {
                context.Sender.SendToAndClose(stream, XmppStreamError.NotAuthorized);
            }
        }
    }
}