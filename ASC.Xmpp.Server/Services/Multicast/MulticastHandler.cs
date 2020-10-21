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
using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.extensions.multicast;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Multicast
{
	[XmppHandler(typeof(Stanza))]
	class MulticastHandler : XmppStanzaHandler
	{
		public override void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context)
		{
			HandleMulticastStanza(stream, message, context);
		}

		public override void HandlePresence(XmppStream stream, Presence presence, XmppHandlerContext context)
		{
			HandleMulticastStanza(stream, presence, context);
		}


		private void HandleMulticastStanza(XmppStream stream, Stanza stanza, XmppHandlerContext context)
		{
			var addresses = stanza.SelectSingleElement<Addresses>();
            if (addresses != null)
            {
                var jids = addresses.GetAddressList();
                
                addresses.RemoveAllBcc();
                Array.ForEach(addresses.GetAddresses(), a => a.Delivered = true);

                var handlerManager = (XmppHandlerManager)context.ServiceProvider.GetService(typeof(XmppHandlerManager));
                foreach (var to in jids)
                {
                    stanza.To = to;
                    handlerManager.ProcessStreamElement(stanza, stream);
                }
            }
		}
	}
}