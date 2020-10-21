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

using ASC.Xmpp.Server.Utils;
using System.Collections.Generic;

namespace ASC.Xmpp.Server.Gateway
{
    public class SignalRXmppListener : XmppListenerBase
    {
        public void AddXmppConnection(string connectionId, XmppServer xmppServer)
        {
            var connection = new SignalRXmppConnection(connectionId, xmppServer);
            base.AddNewXmppConnection(connection);
        }

        public new void CloseXmppConnection(string connectionId)
        {
            IdleWatcher.StopWatch(connectionId);
            base.CloseXmppConnection(connectionId);
        }

        public override void Configure(IDictionary<string, string> properties)
        {
        }

        protected override void DoStart()
        {
        }

        protected override void DoStop()
        {
        }
    }
}
