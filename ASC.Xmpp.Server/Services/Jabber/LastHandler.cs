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
using ASC.Xmpp.Core.protocol.iq.last;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using System;

namespace ASC.Xmpp.Server.Services.Jabber
{
	[XmppHandler(typeof(Last))]
	class LastHandler : XmppStanzaHandler
	{
		private DateTime startedTime = DateTime.UtcNow;


		public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (iq.Type != IqType.get || !iq.HasTo) return XmppStanzaError.ToNotAcceptable(iq);

			var currSession = context.SessionManager.GetSession(iq.From);
			if (currSession == null || !currSession.Available) return XmppStanzaError.ToForbidden(iq);

            double seconds = 0;//available
			
            if (iq.To.IsServer)
			{
				seconds = (DateTime.UtcNow - startedTime).TotalSeconds;
			}
			else
			{
				var session = context.SessionManager.GetSession(iq.To);
				if (session == null || !session.Available)
				{
					var lastActivity = context.StorageManager.OfflineStorage.GetLastActivity(iq.To);
					if (lastActivity != null)
					{
						seconds = (DateTime.UtcNow - lastActivity.LogoutDateTime).TotalSeconds;
						iq.Query.Value = lastActivity.Status;
					}
					else
					{
						return XmppStanzaError.ToRecipientUnavailable(iq);
					}
				}
			}

			((Last)(iq.Query)).Seconds = (int)seconds;
			iq.Type = IqType.result;
			iq.SwitchDirection();
			return iq;
		}
	}
}