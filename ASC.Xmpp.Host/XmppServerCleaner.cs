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
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;

using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Logging;
using ASC.Common.Module;
using ASC.Core;
using ASC.Xmpp.Core.utils;

namespace ASC.Xmpp.Host
{
    public class XmppServerCleaner : IServiceController
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Xmpp.Host");
        private readonly ManualResetEvent stop = new ManualResetEvent(false);
        private Thread worker;


        public void Start()
        {
            worker = new Thread(Clear)
            {
                Name = "Xmpp Cleaner",
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
            };
            worker.Start();
        }

        public void Stop()
        {
            stop.Set();
            worker.Join(TimeSpan.FromSeconds(5));
        }


        private void Clear()
        {
            while (true)
            {
                try
                {
                    log.InfoFormat("Start cleaner interation.");

                    using (var db = new DbManager("default"))
                    {
                        var t = new SqlCreate.Table("jabber_clear", true)
                            .AddColumn("lastdate", DbType.DateTime);
                        db.ExecuteNonQuery(t);

                        var mindate = db.ExecuteScalar<DateTime>("select lastdate from jabber_clear limit 1");

                        var tenants = new List<Tuple<int, string>>();
                        var maxdate = DateTime.UtcNow.AddDays(-365);

                        var sql = @"select
t.id, t.alias
from tenants_tenants t, webstudio_uservisit v
where t.id = v.tenantid and t.creationdatetime < ?
group by 1,2
having max(v.visitdate) between ? and ?";

                        using (var cmd = CreateCommand(db, sql, maxdate, mindate, maxdate))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tenants.Add(Tuple.Create(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                        log.DebugFormat("Find {0} tenants for clear jabber messages", tenants.Count);

                        foreach (var tid in tenants)
                        {
                            // remove all service messages in inactive (no visits at last year) portals

                            var domain = CoreContext.Configuration.BaseDomain;
                            var replace = ConfigurationManager.AppSettings["jabber.replace-domain"];
                            if (!string.IsNullOrEmpty(replace))
                            {
                                var arr = replace.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
                                var from = arr[0];
                                var to = arr[1];
                                domain = domain.Replace(to, from); // revert replace
                            }
                            domain = (tid.Item2.EndsWith("_deleted") ? tid.Item2.Substring(0, tid.Item2.Length - 8) : tid.Item2) +
                                "." + domain;                            

                            if (stop.WaitOne(TimeSpan.Zero))
                            {
                                return;
                            }
                            RemoveFromArchive(db, domain, maxdate);

                            var users = new List<string>();
                            using (var cmd = CreateCommand(db, "select username from core_user where tenant = ? and username <= ?", tid.Item1, tid.Item2))
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    users.Add(reader.GetString(0));
                                }
                            }

                            foreach (var user in users)
                            {
                                var username = user.ToLowerInvariant().Trim();

                                if (stop.WaitOne(TimeSpan.Zero))
                                {
                                    return;
                                }
                                var jid = string.Format("{0}@{1}|{1}", username, domain);
                                RemoveFromArchive(db, jid, maxdate);
                            }
                        }
                        db.ExecuteNonQuery("delete from jabber_clear;");
                        db.ExecuteNonQuery("insert into jabber_clear values (?)", maxdate);

                        // remove offline messages
                        var id = 0;
                        using (var cmd = CreateCommand(db, "select id, message from jabber_offmessage order by 1"))
                        using (var reader = cmd.ExecuteReader())
                        {
                            var less = false;
                            while (reader.Read())
                            {
                                var message = reader.GetString(1);
                                var m = Regex.Match(message, "<x xmlns=\"jabber:x:delay\" stamp=\"(.+)\"");
                                if (m.Success)
                                {
                                    var date = Time.Date(m.Groups[1].Value);
                                    if (date != DateTime.MinValue && date <= maxdate)
                                    {
                                        less = true;
                                    }
                                    else
                                    {
                                        if (less)
                                        {
                                            id = reader.GetInt32(0);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (0 < id)
                        {
                            using (var cmd = CreateCommand(db, "delete from jabber_offmessage where id < ?", id))
                            {
                                var affected = cmd.ExecuteNonQuery();
                                log.DebugFormat("Remove {0} messages from jabber_offmessage", affected);
                            }
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    // ignore
                }
                catch (Exception err)
                {
                    log.Error(err);
                }
                if (stop.WaitOne(TimeSpan.FromDays(1)))
                {
                    break;
                }
            }
        }


        private IDbCommand CreateCommand(DbManager db, string sql, params object[] parameters)
        {
            var cmd = db.Connection.CreateCommand(sql, parameters);
            cmd.CommandTimeout = 60 * 60;
            return cmd;
        }

        private void RemoveFromArchive(DbManager db, string jid, DateTime lastdate)
        {
            jid = jid.Trim().TrimEnd('.').Replace("_", "\\_").Replace("%", "\\%");
            if (!jid.Contains("|"))
            {
                jid += "|%";
            }
            using (var del = CreateCommand(db, "delete from jabber_archive where jid like ? and stamp < ?", jid, lastdate))
            {
                var affected = del.ExecuteNonQuery();
                if (0 < affected)
                {
                    log.DebugFormat("Remove from jabber_archive {0} rows with jid {1}", affected, jid);
                }
            }
        }
    }
}
