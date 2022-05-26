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
using System.Configuration;
using System.Linq;

using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.utils.Idn;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Services;

namespace ASC.Xmpp.Server.Configuration
{
    public static class JabberConfiguration
    {
        public static bool ReplaceDomain
        {
            get;
            private set;
        }

        public static string ReplaceToDomain
        {
            get;
            private set;
        }

        public static string ReplaceFromDomain
        {
            get;
            private set;
        }

        public static void Configure(XmppServer server)
        {
            Configure(server, null);
        }

        public static void Configure(XmppServer server, string configFile)
        {
            var jabberSection = GetSection(configFile);

            ConfigureListeners(jabberSection, server);
            ConfigureStorages(jabberSection, server);
            ConfigureServices(jabberSection, server);



            var replaceSetting = ConfigurationManager.AppSettings["jabber.replace-domain"];
            if (!string.IsNullOrEmpty(replaceSetting))
            {
                ReplaceDomain = true;
                var q = replaceSetting.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLowerInvariant());
                ReplaceFromDomain = q.ElementAt(0);
                ReplaceToDomain = q.ElementAt(1);
            }
        }

        private static void ConfigureServices(JabberConfigurationSection jabberSection, XmppServer server)
        {
            foreach (ServiceConfigurationElement se in jabberSection.Services)
            {
                var service = (IXmppService)Activator.CreateInstance(Type.GetType(se.TypeName, true));
                service.Jid = new Jid(Stringprep.NamePrep(se.Jid));
                service.Name = se.Name;
                if (!string.IsNullOrEmpty(se.Parent))
                {
                    service.ParentService = server.GetXmppService(new Jid(Stringprep.NamePrep(se.Parent)));
                }
                service.Configure(se.GetProperties());
                server.RegisterXmppService(service);
            }
        }

        private static void ConfigureStorages(JabberConfigurationSection jabberSection, XmppServer server)
        {
            foreach (JabberConfigurationElement se in jabberSection.Storages)
            {
                var storage = Activator.CreateInstance(Type.GetType(se.TypeName, true));
                if (storage is IConfigurable) ((IConfigurable)storage).Configure(se.GetProperties());
                server.StorageManager.SetStorage(se.Name, storage);
            }
        }

        private static void ConfigureListeners(JabberConfigurationSection jabberSection, XmppServer server)
        {
            foreach (JabberConfigurationElement le in jabberSection.Listeners)
            {
                var listener = (IXmppListener)Activator.CreateInstance(Type.GetType(le.TypeName, true));
                listener.Name = le.Name;
                listener.Configure(le.GetProperties());
                server.AddXmppListener(listener);
            }
        }


        private static JabberConfigurationSection GetSection(string configFile)
        {
            if (string.IsNullOrEmpty(configFile))
            {
                return (JabberConfigurationSection)ConfigurationManager.GetSection(Schema.SECTION_NAME);
            }

            var cfg = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap() { ExeConfigFilename = configFile },
                ConfigurationUserLevel.None
            );
            return (JabberConfigurationSection)cfg.GetSection(Schema.SECTION_NAME);
        }
    }
}