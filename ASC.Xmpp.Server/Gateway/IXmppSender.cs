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

using System.Collections.Generic;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Server.Gateway
{
	public interface IXmppSender
	{
		void SendTo(XmppStream to, Node node);

		void SendTo(XmppStream to, string text);

		void SendTo(XmppSession to, Node node);

		void SendToAndClose(XmppStream to, Node node);

		bool Broadcast(ICollection<XmppSession> sessions, Node node);

		void CloseStream(XmppStream stream);

		void ResetStream(XmppStream stream);

		IXmppConnection GetXmppConnection(string connectionId);

        void SendPresenceToSignalR(Presence presence, XmppSessionManager sessionManager);
    }
}
