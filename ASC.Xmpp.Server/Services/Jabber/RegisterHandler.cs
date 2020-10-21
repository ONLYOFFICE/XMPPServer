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
using ASC.Xmpp.Core.protocol.iq.register;
using ASC.Xmpp.Core.utils.Idn;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Users;

namespace ASC.Xmpp.Server.Services.Jabber
{
	[XmppHandler(typeof(Register))]
	class RegisterHandler : XmppStanzaHandler
	{
		public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (iq.Type == IqType.get) return GetRegister(stream, iq, context);
			else if (iq.Type == IqType.set) return SetRegister(stream, iq, context);
			return null;
		}

		private IQ GetRegister(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			var register = (Register)iq.Query;
			register.Username = string.Empty;
			iq.Type = IqType.result;

			if (iq.From.HasUser && context.UserManager.IsUserExists(iq.From))
			{
				register.Username = iq.From.User;
				register.AddChild(new Element("registered"));
				iq.SwitchDirection();
				iq.From = null;
			}
			else
			{
				iq.From = iq.To = null;
			}
			return iq;
		}

		private IQ SetRegister(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			var register = (Register)iq.Query;
			iq.Type = IqType.result;

			if (register.RemoveAccount)
			{
				if (!stream.Authenticated || !iq.From.HasUser) context.Sender.SendToAndClose(stream, XmppStreamError.NotAuthorized);

				context.UserManager.RemoveUser(iq.From);
				foreach (var s in context.SessionManager.GetBareJidSessions(iq.From))
				{
					if (s.Stream.Id == stream.Id) continue;
					context.Sender.SendToAndClose(s.Stream, XmppStreamError.Conflict);
				}
				//TODO: remove roster subscriptions
				register.RemoveAllChildNodes();
				iq.SwitchDirection();
				return iq;
			}

			if (string.IsNullOrEmpty(register.Username) ||
				Stringprep.NamePrep(register.Username) != register.Username)
			{
				var error = XmppStanzaError.ToNotAcceptable(iq);
				if (string.IsNullOrEmpty(register.Username)) error.Error.Message = "Empty required field Username.";
				else if (Stringprep.NamePrep(register.Username) != register.Username) error.Error.Message = "Invalid character.";
				return error;
			}

			var userJid = new Jid(register.Username, stream.Domain, null);
			if (context.UserManager.IsUserExists(userJid))
			{
				return XmppStanzaError.ToConflict(iq);
			}

			var user = new User(userJid);
			context.UserManager.SaveUser(user);

			register.RemoveAllChildNodes();
			if (stream.Authenticated) iq.SwitchDirection();
			else iq.To = null;
			iq.From = null;
			return iq;
		}
	}
}