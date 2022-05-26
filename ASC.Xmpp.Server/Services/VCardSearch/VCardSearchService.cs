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

using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Server.Services.Jabber;

namespace ASC.Xmpp.Server.Services.VCardSearch
{
    class VCardSearchService : XmppServiceBase
    {
        public override void Configure(IDictionary<string, string> properties)
        {
            DiscoInfo.AddIdentity(new DiscoIdentity("service", Name, "jud"));
            lock (Handlers)
            {
                Handlers.Add(new VCardSearchHandler());
                Handlers.Add(new VCardHandler());
                Handlers.Add(new ServiceDiscoHandler(Jid));
            }
        }
    }
}