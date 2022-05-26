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
using System.Threading;

using ASC.Common.Logging;

namespace ASC.Xmpp.Server.Utils
{
    static class IdleWatcher
    {
        private static readonly ILog log = LogManager.GetLogger("ASC");

        private static readonly object locker = new object();
        private static readonly TimeSpan timerPeriod = TimeSpan.FromSeconds(1.984363);
        private static readonly Timer timer;
        private static int timerInWork;

        private static readonly IDictionary<string, IdleItem> items = new Dictionary<string, IdleItem>();


        static IdleWatcher()
        {
            timer = new Timer(OnTimer, null, timerPeriod, timerPeriod);
        }

        public static void StartWatch(string id, TimeSpan timeout, EventHandler<TimeoutEventArgs> handler)
        {
            StartWatch(id, timeout, handler, null);
        }

        public static void StartWatch(string id, TimeSpan timeout, EventHandler<TimeoutEventArgs> handler, object data)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (timeout == TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }

            lock (locker)
            {
                if (items.ContainsKey(id))
                {
                    log.WarnFormat("An item with the same key ({0}) has already been added.", id);
                }
                items[id] = new IdleItem(id, timeout, handler, data);
            }
            log.DebugFormat("Start watch idle object: {0}, timeout: {1}", id, timeout);
        }

        public static void UpdateTimeout(string id, TimeSpan timeout)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            lock (locker)
            {
                IdleItem item;
                if (items.TryGetValue(id, out item))
                {
                    item.UpdateItemTimeout(timeout);
                }
            }
            log.DebugFormat("Update timeout idle object: {0}, timeout: {1}", id, timeout);
        }

        public static bool StopWatch(string id)
        {
            var result = false;

            if (id == null)
            {
                return result;
            }

            lock (locker)
            {
                result = items.Remove(id);
            }
            log.DebugFormat("Stop watch idle object: {0}" + (result ? "" : " - idle object not found."), id);

            return result;
        }

        private static void OnTimer(object _)
        {
            if (Interlocked.CompareExchange(ref timerInWork, 1, 0) == 0)
            {
                try
                {
                    var expiredItems = new List<IdleItem>();
                    lock (locker)
                    {
                        foreach (var item in new Dictionary<string, IdleItem>(items))
                        {
                            if (item.Value.IsExpired())
                            {
                                items.Remove(item.Key);
                                expiredItems.Add(item.Value);
                            }
                        }
                    }

                    foreach (var item in expiredItems)
                    {
                        try
                        {
                            log.DebugFormat("Find idle object: {0}, invoke handler.", item.Id);
                            item.InvokeHandler();
                        }
                        catch (Exception err)
                        {
                            log.ErrorFormat("Error in handler idle object: {0}", err);
                        }
                    }
                }
                catch (Exception err)
                {
                    log.Error(err);
                }
                finally
                {
                    timerInWork = 0;
                }
            }
            else
            {
                log.WarnFormat("Idle timer works more than {0}s.", Math.Round(timerPeriod.TotalSeconds, 2));
            }
        }


        private class IdleItem
        {
            private readonly EventHandler<TimeoutEventArgs> handler;
            private readonly object data;
            private DateTime created;
            private TimeSpan timeout;

            public string Id { get; private set; }


            public IdleItem(string id, TimeSpan timeout, EventHandler<TimeoutEventArgs> handler, object data)
            {
                Id = id;
                this.data = data;
                this.handler = handler;
                UpdateItemTimeout(timeout);
            }

            public void UpdateItemTimeout(TimeSpan timeout)
            {
                created = DateTime.UtcNow;
                if (timeout != TimeSpan.Zero)
                {
                    this.timeout = timeout.Duration();
                }
            }

            public bool IsExpired()
            {
                return timeout < (DateTime.UtcNow - created);
            }

            public void InvokeHandler()
            {
                handler(this, new TimeoutEventArgs(Id, data));
            }
        }
    }

    class TimeoutEventArgs : EventArgs
    {
        public string Id
        {
            get;
            private set;
        }

        public object Data
        {
            get;
            private set;
        }

        public TimeoutEventArgs(string id, object data)
        {
            Id = id;
            Data = data;
        }
    }
}