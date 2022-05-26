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

using ASC.Common.Logging;
using ASC.Xmpp.Core.protocol;

namespace ASC.Xmpp.Server.Services
{
    public class XmppServiceManager
    {
        private readonly IServiceProvider serviceProvider;

        private readonly ConcurrentDictionary<Jid, IXmppService> services = new ConcurrentDictionary<Jid, IXmppService>();

        private readonly static ILog log = LogManager.GetLogger("ASC");


        public XmppServiceManager(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            this.serviceProvider = serviceProvider;
        }

        public void RegisterService(IXmppService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            services.TryAdd(service.Jid, service);

            log.DebugFormat("Register XMPP service '{0}' on '{1}'", service.Name, service.Jid);

            try
            {
                service.OnRegister(serviceProvider);
            }
            catch (Exception error)
            {
                log.ErrorFormat("Error on register service '{0}' and it will be unloaded. {1}", service.Name, error);
                UnregisterService(service.Jid);
                throw;
            }
        }

        public void UnregisterService(Jid address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            IXmppService service;
            if (services.TryRemove(address, out service))
            {
                log.DebugFormat("Unregister XMPP service '{0}' on '{1}'", service.Name, service.Jid);
                service.OnUnregister(serviceProvider);
            }
        }

        public ICollection<IXmppService> GetChildServices(Jid parentAddress)
        {
            var list = new List<IXmppService>();
            foreach (var s in services.Values)
            {
                var parentJid = s.ParentService != null ? s.ParentService.Jid : null;
                if (parentAddress == parentJid)
                    list.Add(s);
            }
            return list;
        }

        public IXmppService GetService(Jid address)
        {
            if (address == null)
                return null;

            IXmppService service;
            services.TryGetValue(address, out service);
            return service;
        }
    }
}