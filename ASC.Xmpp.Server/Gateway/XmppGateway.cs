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
using System.Collections.Generic;
using ASC.Common.Logging;

namespace ASC.Xmpp.Server.Gateway
{
    public class XmppGateway : IXmppReceiver
    {
        private readonly object syncRoot = new object();

        private bool started = false;

        private readonly IDictionary<string, IXmppListener> listeners = new Dictionary<string, IXmppListener>();

        private readonly IDictionary<string, string> connectionListenerMap = new Dictionary<string, string>();

        private readonly static ILog log = LogManager.GetLogger("ASC");


        public void AddXmppListener(IXmppListener listener)
        {
            lock (syncRoot)
            {
                try
                {
                    if (started)
                        throw new InvalidOperationException();
                    if (listener == null)
                        throw new ArgumentNullException("listener");

                    listeners.Add(listener.Name, listener);
                    listener.OpenXmppConnection += OpenXmppConnection;

                    log.DebugFormat("Add listener '{0}'", listener.Name);
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Error add listener '{0}': {1}", listener.Name, e);
                    throw;
                }
            }
        }

        public void RemoveXmppListener(string name)
        {
            lock (syncRoot)
            {
                try
                {
                    if (started)
                        throw new InvalidOperationException();
                    if (string.IsNullOrEmpty(name))
                        throw new ArgumentNullException("name");

                    if (listeners.ContainsKey(name))
                    {
                        var listener = listeners[name];
                        listener.OpenXmppConnection -= OpenXmppConnection;
                        listeners.Remove(name);

                        log.DebugFormat("Remove listener '{0}'", listener.Name);
                    }
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Error remove listener '{0}': {1}", name, e);
                    throw;
                }
            }
        }

        public void Start()
        {
            lock (syncRoot)
            {
                foreach (var listener in listeners.Values)
                {
                    try
                    {
                        listener.Start();
                        log.DebugFormat("Started listener '{0}'", listener.Name);
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("Error start listener '{0}': {1}", listener.Name, e);
                    }
                }
                started = true;
            }
        }

        public void Stop()
        {
            lock (syncRoot)
            {
                foreach (var listener in listeners.Values)
                {
                    try
                    {
                        listener.Stop();
                        log.DebugFormat("Stopped listener '{0}'", listener.Name);
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("Error stop listener '{0}': {1}", listener.Name, e);
                    }
                }
                started = false;
            }
        }

        public IXmppListener GetXmppListener(string listenerName)
        {
            lock (syncRoot)
            {
                return listeners[listenerName];
            }
        }

        public IXmppConnection GetXmppConnection(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
                return null;

            string listenerName = null;
            IXmppListener listener = null;
            lock (syncRoot)
            {
                if (!connectionListenerMap.TryGetValue(connectionId, out listenerName) || listenerName == null)
                    return null;
                if (!listeners.TryGetValue(listenerName, out listener) || listener == null)
                    return null;
            }
            return listener.GetXmppConnection(connectionId);
        }


        public event EventHandler<XmppStreamStartEventArgs> XmppStreamStart;

        public event EventHandler<XmppStreamEndEventArgs> XmppStreamEnd;

        public event EventHandler<XmppStreamEventArgs> XmppStreamElement;


        private void OpenXmppConnection(object sender, XmppConnectionOpenEventArgs e)
        {
            lock (syncRoot)
            {
                connectionListenerMap[e.XmppConnection.Id] = ((IXmppListener)sender).Name;
            }
            e.XmppConnection.Closed += XmppConnectionClose;
            e.XmppConnection.XmppStreamEnd += XmppConnectionXmppStreamEnd;
            e.XmppConnection.XmppStreamElement += XmppConnectionXmppStreamElement;
            e.XmppConnection.XmppStreamStart += XmppConnectionXmppStreamStart;
        }

        private void XmppConnectionClose(object sender, XmppConnectionCloseEventArgs e)
        {
            var connection = (IXmppConnection)sender;

            connection.XmppStreamStart -= XmppConnectionXmppStreamStart;
            connection.XmppStreamElement -= XmppConnectionXmppStreamElement;
            connection.XmppStreamEnd -= XmppConnectionXmppStreamEnd;
            connection.Closed -= XmppConnectionClose;
            lock (syncRoot)
            {
                connectionListenerMap.Remove(connection.Id);
            }
        }


        private void XmppConnectionXmppStreamStart(object sender, XmppStreamStartEventArgs e)
        {
            var handler = XmppStreamStart;
            if (handler != null)
                handler(this, e);
        }

        private void XmppConnectionXmppStreamElement(object sender, XmppStreamEventArgs e)
        {
            var handler = XmppStreamElement;
            if (handler != null)
                handler(this, e);
        }

        private void XmppConnectionXmppStreamEnd(object sender, XmppStreamEndEventArgs e)
        {
            var handler = XmppStreamEnd;
            if (handler != null)
                handler(this, e);
        }
    }
}
