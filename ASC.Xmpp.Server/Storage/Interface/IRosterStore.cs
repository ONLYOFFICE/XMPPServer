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
using ASC.Xmpp.Core.protocol.iq.roster;
using System.Collections.Generic;

namespace ASC.Xmpp.Server.Storage.Interface
{
	public interface IRosterStore
	{

		List<UserRosterItem> GetRosterItems(Jid rosterJid);

		List<UserRosterItem> GetRosterItems(Jid rosterJid, SubscriptionType subscriptionType);

		UserRosterItem GetRosterItem(Jid rosterJid, Jid itemJid);

		UserRosterItem SaveRosterItem(Jid rosterJid, UserRosterItem item);

		void RemoveRosterItem(Jid rosterJid, Jid itemJid);
	}
}
