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
using System.Linq;

using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Xmpp.Server.Configuration;

namespace ASC.Xmpp.Server.Storage
{
    public abstract class DbStoreBase : IConfigurable
    {
        private const int ATTEMPTS_COUNT = 2;
        protected static readonly int MESSAGE_COLUMN_LEN = (int)Math.Pow(2, 24) - 1;

        private string dbid;

        public virtual void Configure(IDictionary<string, string> properties)
        {
            if (!properties.ContainsKey("connectionStringName"))
            {
                throw new ConfigurationErrorsException("Cannot create database connection: no connectionString or connectionStringName properties.");
            }

            dbid = properties["connectionStringName"];
            using (var db = DbManager.FromHttpContext(dbid))
            {
                if (!properties.ContainsKey("generateSchema") || Convert.ToBoolean(properties["generateSchema"]))
                {
                    var creates = GetCreateSchemaScript();
                    if (creates != null && creates.Any())
                    {
                        foreach (var c in creates)
                        {
                            db.ExecuteNonQuery(c);
                        }
                    }
                }
            }
        }


        protected virtual SqlCreate[] GetCreateSchemaScript()
        {
            return new SqlCreate[0];
        }


        protected List<object[]> ExecuteList(ISqlInstruction sql)
        {
            return ExecWithAttempts(db => db.ExecuteList(sql), ATTEMPTS_COUNT);
        }

        protected T ExecuteScalar<T>(ISqlInstruction sql)
        {
            return ExecWithAttempts(db => db.ExecuteScalar<T>(sql), ATTEMPTS_COUNT);
        }

        protected int ExecuteNonQuery(ISqlInstruction sql)
        {
            return ExecWithAttempts(db => db.ExecuteNonQuery(sql), ATTEMPTS_COUNT);
        }

        protected int ExecuteBatch(IEnumerable<ISqlInstruction> batch)
        {
            return ExecWithAttempts(db => db.ExecuteBatch(batch), ATTEMPTS_COUNT);
        }

        private T ExecWithAttempts<T>(Func<IDbManager, T> action, int attempsCount)
        {
            var counter = 0;
            while (true)
            {
                try
                {
                    using (var db = DbManager.FromHttpContext(dbid))
                    {
                        return action(db);
                    }
                }
                catch (Exception err)
                {
                    if (attempsCount <= ++counter || err is TimeoutException || err.InnerException is TimeoutException)
                    {
                        throw;
                    }
                }
            }
        }
    }
}