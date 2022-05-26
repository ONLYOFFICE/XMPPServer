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
using System.Linq;

using ASC.Common.Data.Sql;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Server.Storage.Interface;
using ASC.Xmpp.Server.Users;

namespace ASC.Xmpp.Server.Storage
{
    public class DbUserStore : DbStoreBase, IUserStore
    {
        private readonly object syncRoot = new object();

        private IDictionary<string, User> users;

        private IDictionary<string, User> Users
        {
            get
            {
                if (users == null)
                {
                    lock (syncRoot)
                    {
                        if (users == null) users = LoadFromDb();
                    }
                }
                return users;
            }
        }


        protected override SqlCreate[] GetCreateSchemaScript()
        {
            var t1 = new SqlCreate.Table("jabber_user", true)
                .AddColumn("jid", DbType.String, 255, true)
                .AddColumn("pwd", DbType.String, 255)
                .AddColumn(new SqlCreate.Column("admin", DbType.Int32).NotNull(true).Default(0))
                .PrimaryKey("jid");
            return new[] { t1 };
        }


        #region IUserStore Members

        public ICollection<User> GetUsers(string domain)
        {
            lock (syncRoot)
            {
                return Users.Values.Where(u => string.Compare(u.Jid.Server, domain, true) == 0).ToList();
            }
        }

        public User GetUser(Jid jid)
        {
            var bareJid = GetBareJid(jid);
            lock (syncRoot)
            {
                return Users.ContainsKey(bareJid) ? Users[bareJid] : null;
            }
        }

        public void SaveUser(User user)
        {
            if (user == null) throw new ArgumentNullException("user");

            var bareJid = GetBareJid(user.Jid);
            ExecuteNonQuery(new SqlInsert("jabber_user", true)
                .InColumnValue("jid", bareJid)
                .InColumnValue("admin", user.IsAdmin));
            lock (syncRoot)
            {
                Users[bareJid] = user;
            }
        }

        public void RemoveUser(Jid jid)
        {
            var bareJid = GetBareJid(jid);
            ExecuteNonQuery(new SqlDelete("jabber_user").Where("jid", bareJid));
            lock (syncRoot)
            {
                Users.Remove(bareJid);
            }
        }

        #endregion

        private IDictionary<string, User> LoadFromDb()
        {
            return ExecuteList(new SqlQuery("jabber_user").Select("jid", "admin"))
                .ConvertAll(r => new User(new Jid((string)r[0]), Convert.ToBoolean(r[2])))
                .ToDictionary(u => u.Jid.ToString());
        }

        private string GetBareJid(Jid jid)
        {
            if (jid == null) throw new ArgumentNullException("jid");
            return jid.Bare.ToLowerInvariant();
        }
    }
}