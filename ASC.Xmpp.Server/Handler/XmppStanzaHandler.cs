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
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Handler
{
	public class XmppStanzaHandler : IXmppStanzaHandler
	{
		public virtual IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			return null;
		}

		public virtual void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context)
		{

		}

		public virtual void HandlePresence(XmppStream stream, Presence presence, XmppHandlerContext context)
		{

		}

		public virtual void OnRegister(IServiceProvider serviceProvider)
		{

		}

		public virtual void OnUnregister(IServiceProvider serviceProvider)
		{

		}
	}
}
