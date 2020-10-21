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
using System.Configuration;
using System.Xml;

namespace ASC.Xmpp.Server.Configuration
{
    public class JabberConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty(Schema.NAME, IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)this[Schema.NAME]; }
			set { this[Schema.NAME] = value; }
		}

		[ConfigurationProperty(Schema.TYPE, IsRequired = true)]
		public string TypeName
		{
			get { return (string)this[Schema.TYPE]; }
			set { this[Schema.TYPE] = value; }
		}

		[ConfigurationProperty(Schema.PROPERTIES, IsDefaultCollection = false)]
		public NameValueConfigurationCollection JabberProperties
		{
			get { return (NameValueConfigurationCollection)this[Schema.PROPERTIES]; }
			set { this[Schema.PROPERTIES] = value; }
		}


		public JabberConfigurationElement()
		{

		}

		public JabberConfigurationElement(string name, Type type)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			if (type == null) throw new ArgumentNullException("type");

			Name = name;
			TypeName = type.FullName;
		}

		public IDictionary<string, string> GetProperties()
		{
			var properties = new Dictionary<string, string>();
			foreach (NameValueConfigurationElement nameValuePair in JabberProperties)
			{
				properties.Add(nameValuePair.Name, nameValuePair.Value);
			}
			return properties;
		}

		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (elementName == Schema.PROPERTY)
			{
                var name = string.Empty;
                var value = string.Empty;
                while (reader.MoveToNextAttribute())
                {
                    if (reader.LocalName == "name")
                    {
                        name = reader.Value;
                    }
                    if (reader.LocalName == "value")
                    {
                        value = reader.Value;
                    }
                }
                JabberProperties.Add(new NameValueConfigurationElement(name, value));
                return true;
			}
			return base.OnDeserializeUnrecognizedElement(elementName, reader);
		}
	}
}