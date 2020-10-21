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
using System.ServiceModel;
using ASC.Common.Logging;
using ASC.Common.Module;
using ASC.Xmpp.Server;
using ASC.Xmpp.Server.Configuration;

namespace ASC.Xmpp.Host
{
    public class XmppServerLauncher : IServiceController
    {
        private ServiceHost host;
        private XmppServer xmppServer;
        private XmppServerCleaner cleaner;
        private static readonly ILog log = LogManager.GetLogger("ASC");

        public void Start()
        {
            xmppServer = new XmppServer();
            JabberConfiguration.Configure(xmppServer);
            xmppServer.StartListen();

            host = new ServiceHost(new JabberService(xmppServer));
            host.Open();

            cleaner = new XmppServerCleaner();
            cleaner.Start();
        }

        public void Stop()
        {
            try
            {
                if (xmppServer != null)
                {
                    xmppServer.StopListen();
                    xmppServer.Dispose();
                    xmppServer = null;
                }
                if (host != null)
                {
                    host.Close();
                    host = null;
                }
                if (cleaner != null)
                {
                    cleaner.Stop();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while stopping the service", ex);
            }
        }
    }
}
