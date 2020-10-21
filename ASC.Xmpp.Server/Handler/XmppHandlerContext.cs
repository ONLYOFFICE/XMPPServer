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
using ASC.Xmpp.Server.Authorization;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Users;

namespace ASC.Xmpp.Server.Handler
{
    public class XmppHandlerContext
    {
        public IServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public IXmppSender Sender
        {
            get { return (IXmppSender)ServiceProvider.GetService(typeof(IXmppSender)); }
        }

        public UserManager UserManager
        {
            get { return (UserManager)ServiceProvider.GetService(typeof(UserManager)); }
        }

        public XmppSessionManager SessionManager
        {
            get { return (XmppSessionManager)ServiceProvider.GetService(typeof(XmppSessionManager)); }
        }

        public StorageManager StorageManager
        {
            get { return (StorageManager)ServiceProvider.GetService(typeof(StorageManager)); }
        }

        public XmppGateway XmppGateway
        {
            get { return (XmppGateway)ServiceProvider.GetService(typeof(IXmppReceiver)); }
        }

        public AuthManager AuthManager
        {
            get { return (AuthManager)ServiceProvider.GetService(typeof(AuthManager)); }
        }

        public XmppHandlerContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");

            ServiceProvider = serviceProvider;
        }
    }
}
