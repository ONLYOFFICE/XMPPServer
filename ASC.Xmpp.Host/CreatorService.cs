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

using ASC.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.utils.Idn;
using ASC.Xmpp.Server;
using ASC.Xmpp.Server.Configuration;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Services;
using ASC.Xmpp.Server.Streams;
using System;
using System.Collections.Generic;
using System.Configuration;
using Stream = ASC.Xmpp.Core.protocol.Stream;
using Uri = ASC.Xmpp.Core.protocol.Uri;

namespace ASC.Xmpp.Host
{
    [XmppHandler(typeof(Stanza))]
    class CreatorStartStreamHandler : IXmppStreamStartHandler
    {
        private readonly Dictionary<string, Type> templates = new Dictionary<string, Type>();

        private XmppServiceManager serviceManager;

        private XmppHandlerManager handlerManager;

        public string Namespace
        {
            get { return Uri.CLIENT; }
        }

        public CreatorStartStreamHandler(Dictionary<string, Type> instanceTemplate)
        {
            this.templates = instanceTemplate;
        }

        public void StreamStartHandle(XmppStream xmppStream, Stream stream, XmppHandlerContext context)
        {
            //Check tennats here
            if (ValidateHost(stream.To))
            {
                lock (templates)
                {
                    //Create new services
                    foreach (var template in templates)
                    {
                        var service = (IXmppService)Activator.CreateInstance(template.Value);
                        service.Jid = new Jid(Stringprep.NamePrep(string.Format("{0}.{1}", template.Key, stream.To.Server).Trim('.')));

                        if (serviceManager.GetService(service.Jid) != null)
                        {
                            continue;
                        }

                        service.Name = service.Jid.ToString();
                        if (!string.IsNullOrEmpty(template.Key))
                        {
                            service.ParentService = serviceManager.GetService(new Jid(Stringprep.NamePrep(stream.To.Server)));
                        }
                        service.Configure(new Dictionary<string, string>());
                        serviceManager.RegisterService(service);
                    }
                }
                //Reroute
                handlerManager.ProcessStreamStart(stream, Uri.CLIENT, xmppStream);
            }
            else
            {
                context.Sender.SendToAndClose(xmppStream, XmppStreamError.HostUnknown);
            }
        }

        public void OnRegister(IServiceProvider serviceProvider)
        {
            serviceManager = (XmppServiceManager)serviceProvider.GetService(typeof(XmppServiceManager));
            handlerManager = (XmppHandlerManager)serviceProvider.GetService(typeof(XmppHandlerManager));
        }

        public void OnUnregister(IServiceProvider serviceProvider)
        {

        }

        private bool ValidateHost(Jid jid)
        {
            if (jid != null && jid.IsServer)
            {
                // for migration from teamlab.com to onlyoffice.com
                if (JabberConfiguration.ReplaceDomain)
                {
                    if (jid.Server.EndsWith(JabberConfiguration.ReplaceFromDomain) &&
                        CoreContext.TenantManager.GetTenant(jid.Server.Replace(JabberConfiguration.ReplaceFromDomain, JabberConfiguration.ReplaceToDomain)) != null)
                    {
                        return true;
                    }
                    if (!jid.Server.EndsWith(JabberConfiguration.ReplaceToDomain) && CoreContext.TenantManager.GetTenant(jid.Server) != null)
                    {
                        return true;
                    }
                }
                else
                {
                    if (CoreContext.TenantManager.GetTenant(jid.Server) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }


    public class CreatorService : XmppServiceBase
    {
        public override void Configure(IDictionary<string, string> properties)
        {
            var template = new Dictionary<string, Type>();
            foreach (var pair in properties)
            {
                template.Add(pair.Key, Type.GetType(pair.Value, true));
            }

            Handlers.Add(new CreatorStartStreamHandler(template));
        }
    }
}