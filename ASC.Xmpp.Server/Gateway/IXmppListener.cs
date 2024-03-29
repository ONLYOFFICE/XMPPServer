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

using ASC.Xmpp.Server.Configuration;

namespace ASC.Xmpp.Server.Gateway
{
    public interface IXmppListener : IConfigurable
    {
        string Name
        {
            get;
            set;
        }

        void Start();

        void Stop();

        IXmppConnection GetXmppConnection(string connectionId);

        event EventHandler<XmppConnectionOpenEventArgs> OpenXmppConnection;
    }

    public class XmppConnectionOpenEventArgs : EventArgs
    {
        public IXmppConnection XmppConnection
        {
            get;
            private set;
        }

        public XmppConnectionOpenEventArgs(IXmppConnection xmppConnection)
        {
            if (xmppConnection == null) throw new ArgumentNullException("xmppConnection");

            XmppConnection = xmppConnection;
        }
    }
}
