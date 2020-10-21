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
using System.Text;
using System.Web.Script.Serialization;

using ASC.Common.Data.Sql;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Security.Cryptography;

namespace ASC.Xmpp.Server.Storage
{
    class DbLdapSettingsStore : DbStoreBase
    {
        private readonly ILog log = LogManager.GetLogger("ASC");
        private readonly Dictionary<string, string> properties = new Dictionary<string, string>(1);
        private const string LDAP_SETTINGS_ID = "197149b3-fbc9-44c2-b42a-232f7e729c16";

        public bool EnableLdapAuthentication { get; private set; }

        public string Server { get; private set; }

        public int PortNumber { get; private set; }

        public string UserDN { get; private set; }

        public string LoginAttribute { get; private set; }

        public bool Authentication { get; private set; }

        public string Login { get; private set; }

        public string Password { get; private set; }

        public bool StartTls { get; private set; }

        public DbLdapSettingsStore()
        {
            properties["connectionStringName"] = "core";
            base.Configure(properties);
        }

        public void GetLdapSettings(string domain)
        {
            try
            {
                var tenant = CoreContext.TenantManager.GetTenant(domain);
                if (tenant != null)
                {
                    var q = new SqlQuery("webstudio_settings")
                        .Select("Data")
                        .Where("TenantID", tenant.TenantId)
                        .Where("ID", LDAP_SETTINGS_ID);

                    var settings = ExecuteList(q);
                    if (settings != null && settings.Count > 0 && settings[0] != null)
                    {
                        var stringSettings = (string)settings[0][0];
                        if (stringSettings != null)
                        {
                            var jsSerializer = new JavaScriptSerializer();
                            var settingsDictionary = (Dictionary<string, object>)jsSerializer.DeserializeObject(stringSettings);
                            EnableLdapAuthentication = Convert.ToBoolean(settingsDictionary["EnableLdapAuthentication"]);
                            Server = Convert.ToString(settingsDictionary["Server"]);
                            PortNumber = Convert.ToInt32(settingsDictionary["PortNumber"]);
                            UserDN = Convert.ToString(settingsDictionary["UserDN"]);
                            LoginAttribute = Convert.ToString(settingsDictionary["LoginAttribute"]);
                            Authentication = Convert.ToBoolean(settingsDictionary["Authentication"]);
                            Login = Convert.ToString(settingsDictionary["Login"]);
                            Password = GetPassword(settingsDictionary["PasswordBytes"]);
                            StartTls = Convert.ToBoolean(settingsDictionary["StartTls"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private string GetPassword(object passwordBytesObject)
        {
            string password = string.Empty;
            try
            {
                if (passwordBytesObject != null)
                {
                    object[] passwordBytesObjects = (object[])passwordBytesObject;
                    byte[] passwordBytes = new byte[passwordBytesObjects.Length];
                    for (int i = 0; i < passwordBytesObjects.Length; i++)
                    {
                        passwordBytes[i] = Convert.ToByte(passwordBytesObjects[i]);
                    }
                    if (passwordBytes.Length != 0)
                    {
                        password = new UnicodeEncoding().GetString(InstanceCrypto.Decrypt(passwordBytes));
                    }
                }
            }
            catch (Exception ex)
            {
                password = string.Empty;
                log.ErrorFormat("Can't decrypt password {0}, {1}", ex.ToString(), ex.StackTrace);
            }
            return password;
        }
    }
}
