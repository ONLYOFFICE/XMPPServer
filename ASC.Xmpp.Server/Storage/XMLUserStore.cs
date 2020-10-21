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

namespace ASC.Xmpp.Server.storage
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Interface;

    public class XMLUserStore:IUserStore
    {
        private readonly string path;
        private Dictionary<string, DataSet> userDatas = new Dictionary<string, DataSet>();

        public XMLUserStore(string path)
        {
            this.path = path;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private DataSet GetUserSet(string username)
        {
            if (!userDatas.ContainsKey(username))
            {
                DataSet userSet = new DataSet("userdata");
                //try load
                if (File.Exists(Path.Combine(path,username+".xml")))
                {
                    //Load
                    try
                    {
                        userSet.ReadXml(Path.Combine(path, username + ".xml"), XmlReadMode.ReadSchema);
                    }
                    catch
                    {
                    }
                }
                userDatas.Add(username, userSet);
            }
            return userDatas[username];
        }

        private void SaveUserSet(string username)
        {
            DataSet userSet = GetUserSet(username);
            try
            {
                userSet.WriteXml(Path.Combine(path, username + ".xml"), XmlWriteMode.WriteSchema);
            }
            catch {}
        }

        private DataTable GetUserTable(string userName, UserStorageSections section)
        {
            DataSet userSet = GetUserSet(userName);
            if (!userSet.Tables.Contains(section.ToString()))
            {
                userSet.Tables.Add(section.ToString());
            }
            return userSet.Tables[section.ToString()];
        }

        public void SetUserItem(string userName, UserStorageSections section, object data)
        {
            try
            {
                DataTable userTable = GetUserTable(userName, section);
                if (!userTable.Columns.Contains("userdata"))
                {
                    userTable.Columns.Add("userdata");
                    userTable.Columns.Add("datatype");
                }
                DataRow row;
                if (userTable.Rows.Count == 0)
                {
                    row = userTable.NewRow();
                    userTable.Rows.Add(row);
                }
                else
                {
                    row = userTable.Rows[0];
                }
                Serialize(row, data);
                SaveUserSet(userName);
            }
            catch (Exception)
            {
            }
        }

        private void Serialize(DataRow row, object data) 
        {
            row["datatype"] = data.GetType();
            XmlSerializer serializer = new XmlSerializer(data.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, data);
                row["userdata"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(writer.ToString()));
            }
            
        }

        private object Deserialize(DataRow row)
        {
            Type type = Type.GetType(row["datatype"].ToString());
            XmlSerializer serializer = new XmlSerializer(type);
            using (StringReader reader = new StringReader(Encoding.UTF8.GetString(Convert.FromBase64String(row["userdata"].ToString()))))
            {
                return serializer.Deserialize(reader);
            }
        }


        public object GetUserItem(string userName, UserStorageSections section)
        {
            try
            {
                DataTable userTable = GetUserTable(userName, section);
                if (!userTable.Columns.Contains("userdata"))
                {
                    userTable.Columns.Add("userdata");
                    userTable.Columns.Add("datatype");
                }
                DataRow row;
                if (userTable.Rows.Count == 0)
                {
                    return null;
                }
                row = userTable.Rows[0];
                return Deserialize(row);
            }
            catch
            {
                return null;
            }
        }

    }
}