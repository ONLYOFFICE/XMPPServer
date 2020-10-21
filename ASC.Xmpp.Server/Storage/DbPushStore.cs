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
using System.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Common.Logging;
using ASC.Xmpp.Server.Handler;

namespace ASC.Xmpp.Server.Storage
{
   
    public class UserPushInfo
    {
        public int id { get; set; }
        public string username { get; set; }
        public string endpoint { get; set; }
        public string browser { get; set; }


        public UserPushInfo(int id, string username, string endpoint, string browser)
        {
            this.id = id;
            this.username = username;
            this.endpoint = endpoint;
            this.browser = browser;
        }

    }

    public class DbPushStore : DbStoreBase
    {
        private static readonly ILog log = LogManager.GetLogger("ASC");

        protected override SqlCreate[] GetCreateSchemaScript()
        {
            var t1 = new SqlCreate.Table("jabber_push", true)
                .AddColumn(new SqlCreate.Column("id", DbType.Int32).NotNull(true).Autoincrement(true).PrimaryKey(true))
                .AddColumn("username", DbType.String, 255, true)
                .AddColumn("browser", DbType.String, 255)
                .AddColumn("endpoint", DbType.String, 255);

            return new[] { t1 };
        }

        public void SaveUserEndpoint(string username, string endpoint, string browser)
        {
            if (username == null) throw new ArgumentNullException("push");

            List<UserPushInfo> userPushList = new List<UserPushInfo>();
            userPushList = GetUserEndpoint(username,browser);

            if (userPushList.Count > 0)
            {
                //rewrite record
                foreach (UserPushInfo user in userPushList)
                {
                    ExecuteNonQuery(new SqlUpdate("jabber_push")
                        .Set("endpoint", endpoint)
                        .Where("id",user.id));
                }
            }
            else
            {
                //create a new record
                ExecuteNonQuery(new SqlInsert("jabber_push", true)
                    .InColumnValue("username", username)
                    .InColumnValue("browser", browser)
                    .InColumnValue("endpoint", endpoint));
            }
        }

        public List<UserPushInfo> GetUserEndpoint(string username)
        {
            return ExecuteList(new SqlQuery("jabber_push").Select("id", "username", "endpoint", "browser").Where(Exp.Like("username", username)))
                        .ConvertAll(r => new UserPushInfo((int)r[0], (string)r[1], (string)r[2], (string)r[3]));
        }

        public List<UserPushInfo> GetUserEndpoint(string username, string browser)
        {
            return ExecuteList(new SqlQuery("jabber_push").Select("id", "username", "endpoint", "browser")
                        .Where(Exp.Like("username", username))
                        .Where(Exp.Like("browser", browser)))
                        .ConvertAll(r => new UserPushInfo((int)r[0], (string)r[1], (string)r[2], (string)r[3]));
        }
    }
}