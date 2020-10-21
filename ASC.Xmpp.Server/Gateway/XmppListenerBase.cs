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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Xmpp.Server.Gateway
{
    public abstract class XmppListenerBase : IXmppListener
    {
        private readonly object locker = new object();
        private readonly ConcurrentDictionary<string, IXmppConnection> connections = new ConcurrentDictionary<string, IXmppConnection>();

        protected bool Started
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public void Start()
        {
            lock (locker)
            {
                if (!Started)
                {
                    Started = true;
                    DoStart();
                }
            }
        }

        public void Stop()
        {
            lock (locker)
            {
                connections.Values
                    .ToList()
                    .ForEach(c => c.Close());
                connections.Clear();

                if (Started)
                {
                    Started = false;
                    DoStop();
                }
            }
        }

        public IXmppConnection GetXmppConnection(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                return null;
            }
            IXmppConnection conn;
            connections.TryGetValue(connectionId, out conn);
            return conn;
        }

        public event EventHandler<XmppConnectionOpenEventArgs> OpenXmppConnection = delegate { };

        protected void AddNewXmppConnection(IXmppConnection xmppConnection)
        {
            if (xmppConnection == null)
            {
                throw new ArgumentNullException("xmppConnection");
            }

            connections.TryAdd(xmppConnection.Id, xmppConnection);
            xmppConnection.Closed += XmppConnectionClosed;

            OpenXmppConnection(this, new XmppConnectionOpenEventArgs(xmppConnection));
            xmppConnection.BeginReceive();
        }

        protected void CloseXmppConnection(string connectionId)
        {
            IXmppConnection conn;
            if (connections.TryRemove(connectionId, out conn))
            {
                conn.Closed -= XmppConnectionClosed;
            }
        }

        private void XmppConnectionClosed(object sender, XmppConnectionCloseEventArgs e)
        {
            var connection = (IXmppConnection)sender;
            if (connection != null)
            {
                connection.Closed -= XmppConnectionClosed;
                connections.TryRemove(connection.Id, out connection);
            }
        }

        public abstract void Configure(IDictionary<string, string> properties);

        protected abstract void DoStart();

        protected abstract void DoStop();
    }
}