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
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using System;

namespace ASC.Xmpp.Server.Services
{
	[XmppHandler(typeof(DiscoInfo))]
	[XmppHandler(typeof(DiscoItems))]
	class ServiceDiscoHandler : XmppStanzaHandler
	{
		protected Jid Jid
		{
			get;
			private set;
		}

		protected XmppServiceManager ServiceManager
		{
			get;
			private set;
		}

		public ServiceDiscoHandler(Jid jid)
		{
			Jid = jid;
		}

		public override void OnRegister(IServiceProvider serviceProvider)
		{
			ServiceManager = (XmppServiceManager)serviceProvider.GetService(typeof(XmppServiceManager));
		}

		public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (iq.HasTo && iq.To != Jid) return XmppStanzaError.ToServiceUnavailable(iq);
			if (iq.Query is DiscoInfo && iq.Type == IqType.get) return GetDiscoInfo(stream, iq, context);
			if (iq.Query is DiscoItems && iq.Type == IqType.get) return GetDiscoItems(stream, iq, context);
			return XmppStanzaError.ToServiceUnavailable(iq);
		}

		protected virtual IQ GetDiscoInfo(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (((DiscoInfo)iq.Query).Node != null) return XmppStanzaError.ToServiceUnavailable(iq);

			var service = ServiceManager.GetService(Jid);
			if (service == null) return XmppStanzaError.ToItemNotFound(iq);

			var answer = new IQ(IqType.result);
			answer.Id = iq.Id;
			answer.From = Jid;
			answer.To = iq.From;
			answer.Query = service.DiscoInfo;
			return answer;
		}

		protected virtual IQ GetDiscoItems(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (((DiscoItems)iq.Query).Node != null) return XmppStanzaError.ToServiceUnavailable(iq);

			var answer = new IQ(IqType.result);
			answer.Id = iq.Id;
			answer.From = Jid;
			answer.To = iq.From;
			var items = new DiscoItems();
			answer.Query = items;
			foreach (var service in ServiceManager.GetChildServices(Jid))
			{
				if (service.DiscoItem != null) items.AddDiscoItem(service.DiscoItem);
			}
			return answer;
		}
	}
}
