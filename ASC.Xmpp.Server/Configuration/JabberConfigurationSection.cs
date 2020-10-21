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

using System.Configuration;

namespace ASC.Xmpp.Server.Configuration
{
    public class JabberConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty(Schema.LISTENERS, IsDefaultCollection = false)]
		public JabberConfigurationCollection Listeners
		{
			get { return (JabberConfigurationCollection)base[Schema.LISTENERS]; }
		}

		[ConfigurationProperty(Schema.STORAGES, IsDefaultCollection = false)]
		public JabberConfigurationCollection Storages
		{
			get { return (JabberConfigurationCollection)base[Schema.STORAGES]; }
		}

		[ConfigurationProperty(Schema.SERVICES, IsDefaultCollection = false)]
		public ServiceConfigurationCollection Services
		{
			get { return (ServiceConfigurationCollection)base[Schema.SERVICES]; }
		}
	}
}