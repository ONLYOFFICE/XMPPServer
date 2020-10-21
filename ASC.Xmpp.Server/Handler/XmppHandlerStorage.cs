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

using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.Base;
using System;
using System.Collections.Generic;

namespace ASC.Xmpp.Server.Handler
{
    public class XmppHandlerStorage
	{
        private readonly IDictionary<Jid, List<IXmppStreamStartHandler>> streamStartHandlers = new Dictionary<Jid, List<IXmppStreamStartHandler>>();

        private readonly IDictionary<string, List<IXmppStreamHandler>> streamHandlers = new Dictionary<string, List<IXmppStreamHandler>>();

        private readonly IDictionary<string, List<IXmppStanzaHandler>> stanzaHandlers = new Dictionary<string, List<IXmppStanzaHandler>>();

		private readonly object locker = new object();

		private readonly IServiceProvider serviceProvider;


		public XmppHandlerStorage(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}


		public void AddXmppHandler(Jid address, IXmppHandler handler)
		{
			if (handler == null) throw new ArgumentNullException("handler");

            lock (locker)
			{
				if (handler is IXmppStreamStartHandler)
				{
					if (!streamStartHandlers.ContainsKey(address)) streamStartHandlers[address] = new List<IXmppStreamStartHandler>();
					streamStartHandlers[address].Add((IXmppStreamStartHandler)handler);
				}

				if (handler is IXmppStreamHandler)
				{
					foreach (var type in GetHandledTypes(handler))
					{
						var key = GetHandlerKey(address, type);
						if (!streamHandlers.ContainsKey(key)) streamHandlers[key] = new List<IXmppStreamHandler>();
						streamHandlers[key].Add((IXmppStreamHandler)handler);
					}
				}

				if (handler is IXmppStanzaHandler)
				{
					foreach (var type in GetHandledTypes(handler))
					{
						var key = GetHandlerKey(address, type);
						if (!stanzaHandlers.ContainsKey(key)) stanzaHandlers[key] = new List<IXmppStanzaHandler>();
						stanzaHandlers[key].Add((IXmppStanzaHandler)handler);
					}
				}
			}

			handler.OnRegister(serviceProvider);
		}

		public void RemoveXmppHandler(IXmppHandler handler)
		{
			if (handler == null) throw new ArgumentNullException("handler");

            lock (locker)
            {
				if (handler is IXmppStreamStartHandler)
				{
					foreach (var keyValuePair in new Dictionary<Jid, List<IXmppStreamStartHandler>>(streamStartHandlers))
					{
						foreach (var h in new List<IXmppStreamStartHandler>(keyValuePair.Value))
						{
							if (handler == h) streamStartHandlers[keyValuePair.Key].Remove(h);
						}
					}
				}

				if (handler is IXmppStreamHandler)
				{
					foreach (var keyValuePair in new Dictionary<string, List<IXmppStreamHandler>>(streamHandlers))
					{
						foreach (var h in new List<IXmppStreamHandler>(keyValuePair.Value))
						{
							if (handler == h) streamHandlers[keyValuePair.Key].Remove(h);
							if (streamHandlers[keyValuePair.Key].Count == 0)
							{
								streamHandlers.Remove(keyValuePair.Key);
							}

						}
					}
				}

				if (handler is IXmppStanzaHandler)
				{
					foreach (var keyValuePair in new Dictionary<string, List<IXmppStanzaHandler>>(stanzaHandlers))
					{
						foreach (var h in new List<IXmppStanzaHandler>(keyValuePair.Value))
						{
							if (handler == h) stanzaHandlers[keyValuePair.Key].Remove(h);
							if (stanzaHandlers[keyValuePair.Key].Count == 0)
							{
								stanzaHandlers.Remove(keyValuePair.Key);
							}

						}
					}
				}
			}

			handler.OnUnregister(serviceProvider);
		}

		public List<IXmppStreamStartHandler> GetStreamStartHandlers(Jid address)
		{
            lock (locker)
            {
				return streamStartHandlers.ContainsKey(address) ? streamStartHandlers[address] :
					streamStartHandlers.ContainsKey(Jid.Empty) ? streamStartHandlers[Jid.Empty] : new List<IXmppStreamStartHandler>();
			}
		}

		public List<IXmppStreamHandler> GetStreamHandlers(string domain)
		{
            lock (locker)
            {
				var handlers = new List<IXmppStreamHandler>();
				foreach (var pair in streamHandlers)
				{
                    var jid = new Jid(pair.Key.Substring(0, pair.Key.IndexOf('|')));
                    if (jid.Server == domain)
                    {
                        foreach (var handler in pair.Value)
                        {
                            if (!handlers.Contains(handler)) handlers.Add(handler);
                        }
                    }
				}
				return handlers;
			}
		}

		public List<IXmppStreamHandler> GetStreamHandlers(Jid address, Type streamElementType)
		{
            lock (locker)
            {
				var key = GetHandlerKey(address, streamElementType);
				return streamHandlers.ContainsKey(key) ? new List<IXmppStreamHandler>(streamHandlers[key]) : new List<IXmppStreamHandler>();
			}
		}

		public List<IXmppStanzaHandler> GetStanzaHandlers(Jid to, Type stanzaType)
		{
            lock (locker)
            {
				var key = GetHandlerKey(to, stanzaType);
				if (stanzaHandlers.ContainsKey(key)) return new List<IXmppStanzaHandler>(stanzaHandlers[key]);

				if (to.Resource != null && to.Resource.Contains("/"))
				{
					var newTo = new Jid(to.ToString());
					newTo.Resource = newTo.Resource.Substring(0, newTo.Resource.IndexOf('/'));
					key = GetHandlerKey(newTo, stanzaType);
					if (stanzaHandlers.ContainsKey(key)) return new List<IXmppStanzaHandler>(stanzaHandlers[key]);
				}

				key = GetHandlerKey(to.Bare, stanzaType);
				if (stanzaHandlers.ContainsKey(key)) return new List<IXmppStanzaHandler>(stanzaHandlers[key]);

				key = GetHandlerKey(to.Server, stanzaType);
				if (stanzaHandlers.ContainsKey(key)) return new List<IXmppStanzaHandler>(stanzaHandlers[key]);

				if (stanzaType != typeof(Stanza)) return GetStanzaHandlers(to, typeof(Stanza));

				return new List<IXmppStanzaHandler>();
			}
		}

		private Type[] GetHandledTypes(IXmppHandler handler)
		{
			var types = new List<object>(handler.GetType().GetCustomAttributes(typeof(XmppHandlerAttribute), true))
				.ConvertAll(o => ((XmppHandlerAttribute)o).XmppElementType);
			if (types.Count == 0) types.Add(null);
			return types.ToArray();
		}

		private string GetHandlerKey(object address, Type type)
		{
			return string.Format("{0}|{1}", address, type);
		}
	}
}
