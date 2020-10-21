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
using ASC.Xmpp.Core.protocol.iq.bind;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Streams;
using System;

namespace ASC.Xmpp.Server.Services.Jabber
{
	[XmppHandler(typeof(Core.protocol.iq.session.Session))]
	[XmppHandler(typeof(Bind))]
	class BindSessionHandler : XmppStanzaHandler
	{
		public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (iq.Session != null) return ProcessSession(stream, iq, context);
			else if (iq.Bind != null) return ProcessBind(stream, iq, context);
			else return XmppStanzaError.ToServiceUnavailable(iq);
		}

		private IQ ProcessBind(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (iq.Type != IqType.set) return XmppStanzaError.ToBadRequest(iq);

			var answer = new IQ(IqType.result);
			answer.Id = iq.Id;

			var bind = (Bind)iq.Bind;
			var resource = !string.IsNullOrEmpty(bind.Resource) ? bind.Resource : stream.User;

			if (bind.TagName.Equals("bind", StringComparison.OrdinalIgnoreCase))
			{
				var jid = new Jid(stream.User, stream.Domain, resource);

				var session = context.SessionManager.GetSession(jid);
				if (session != null)
				{
					if (session.Stream.Id != stream.Id)
					{
						context.Sender.SendToAndClose(session.Stream, XmppStreamError.Conflict);
						answer.AddTag("newTalkWindow", "true");
					}
					else
					{
						return XmppStanzaError.ToConflict(iq);
					}
				}

				stream.BindResource(resource);
				context.SessionManager.AddSession(new XmppSession(jid, stream));
				answer.Bind = new Bind(jid);
			}
			else if (bind.TagName.Equals("unbind", StringComparison.OrdinalIgnoreCase))
			{
				if (!stream.Resources.Contains(resource)) return XmppStanzaError.ToNotFound(iq);

				context.SessionManager.CloseSession(iq.From);
				stream.UnbindResource(resource);
				if (stream.Resources.Count == 0)
				{
					context.Sender.CloseStream(stream);
				}
			}
			else
			{
				return XmppStanzaError.ToBadRequest(iq);
			}
			if (stream.MultipleResources) answer.To = iq.From;
			return answer;
		}

		private IQ ProcessSession(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			var session = context.SessionManager.GetSession(iq.From);
			if (session == null) return XmppStanzaError.ToItemNotFound(iq);

			var answer = new IQ(IqType.result);
			answer.Id = iq.Id;
			answer.Session = new Core.protocol.iq.session.Session();
			session.Active = true;
			return answer;
		}
	}
}