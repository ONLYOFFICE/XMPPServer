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
using ASC.Xmpp.Server.Authorization;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Services;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Users;
using System;

namespace ASC.Xmpp.Server
{
    public class XmppServer : IServiceProvider, IDisposable
	{
		private UserManager userManager;

		private XmppGateway gateway;

		private XmppSender sender;

		private XmppServiceManager serviceManager;

		public StorageManager StorageManager
		{
			get;
			private set;
		}

		public AuthManager AuthManager
		{
			get;
			private set;
		}

		public XmppSessionManager SessionManager
		{
			get;
			private set;
		}

        public XmppStreamManager StreamManager
        {
            get;
            private set;
        }

        public XmppHandlerManager HandlerManager
        {
            get;
            private set;
        }

		public XmppServer()
		{
			StorageManager = new StorageManager();
			userManager = new UserManager(StorageManager);
			AuthManager = new AuthManager();

			StreamManager = new XmppStreamManager();
			SessionManager = new XmppSessionManager();

			gateway = new XmppGateway();
			sender = new XmppSender(gateway);

			serviceManager = new XmppServiceManager(this);
            HandlerManager = new XmppHandlerManager(StreamManager, gateway, sender, this);
		}

		public void AddXmppListener(IXmppListener listener)
		{
			gateway.AddXmppListener(listener);
		}

		public void RemoveXmppListener(string name)
		{
			gateway.RemoveXmppListener(name);
		}

		public void StartListen()
		{
			gateway.Start();
		}

		public void StopListen()
		{
			gateway.Stop();
		}

		public void RegisterXmppService(IXmppService service)
		{
			serviceManager.RegisterService(service);
		}

		public void UnregisterXmppService(Jid jid)
		{
			serviceManager.UnregisterService(jid);
		}

		public IXmppService GetXmppService(Jid jid)
		{
			return serviceManager.GetService(jid);
		}

		public void Dispose()
        {
            StorageManager.Dispose();
        }

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IXmppReceiver))
			{
				return gateway;
			}
			if (serviceType == typeof(IXmppSender))
			{
				return sender;
			}
			if (serviceType == typeof(XmppSessionManager))
			{
				return SessionManager;
			}
			if (serviceType == typeof(XmppStreamManager))
			{
				return StreamManager;
			}
			if (serviceType == typeof(UserManager))
			{
				return userManager;
			}
			if (serviceType == typeof(StorageManager))
			{
				return StorageManager;
			}
			if (serviceType == typeof(XmppServiceManager))
			{
				return serviceManager;
			}
			if (serviceType == typeof(AuthManager))
			{
				return AuthManager;
			}
			if (serviceType == typeof(XmppHandlerManager))
			{
                return HandlerManager;
			}
			return null;
		}
	}
}