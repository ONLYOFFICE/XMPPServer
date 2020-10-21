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
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.auth;
using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Users;

namespace ASC.Xmpp.Server.Authorization
{
	[XmppHandler(typeof(AuthIq))]
	class AuthIQHandler : XmppStreamHandler
	{
		public override void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
		{
			var iq = (AuthIq)element;

			if (stream.Authenticated)
			{
				context.Sender.SendTo(stream, XmppStanzaError.ToConflict(iq));
				return;
			}

			if (iq.Type == IqType.get) ProcessAuthIQGet(stream, iq, context);
			else if (iq.Type == IqType.set) ProcessAuthIQSet(stream, iq, context);
			else context.Sender.SendTo(stream, XmppStanzaError.ToNotAcceptable(iq));
		}

		private void ProcessAuthIQSet(XmppStream stream, AuthIq iq, XmppHandlerContext context)
		{
			if (string.IsNullOrEmpty(iq.Query.Username) || string.IsNullOrEmpty(iq.Query.Resource))
			{
				context.Sender.SendTo(stream, XmppStanzaError.ToNotAcceptable(iq));
				return;
			}

			context.Sender.SendTo(stream, XmppStanzaError.ToNotAuthorized(iq));

		}

		private void ProcessAuthIQGet(XmppStream stream, AuthIq iq, XmppHandlerContext context)
		{
			iq.SwitchDirection();
			iq.Type = IqType.result;
			iq.Query.AddChild(new Element("password"));
			iq.Query.AddChild(new Element("digest"));
			iq.Query.AddChild(new Element("resource"));
			context.Sender.SendTo(stream, iq);
		}

	}
}