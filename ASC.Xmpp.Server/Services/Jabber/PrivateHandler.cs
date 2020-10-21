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

using System.Collections.Generic;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.@private;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Jabber
{
	[XmppHandler(typeof(Private))]
	class PrivateHandler : XmppStanzaHandler
	{
		public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (iq.To != null && iq.From != iq.To) return XmppStanzaError.ToForbidden(iq);

			if (iq.Type == IqType.get) return GetPrivate(stream, iq, context);
			else if (iq.Type == IqType.set) return SetPrivate(stream, iq, context);
			else return XmppStanzaError.ToBadRequest(iq);
		}

		private IQ SetPrivate(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			var @private = (Private)iq.Query;
			
            if (!@private.HasChildElements) return XmppStanzaError.ToNotAcceptable(iq);

			foreach (var childNode in @private.ChildNodes)
			{
				if (childNode is Element)
				{
                    context.StorageManager.PrivateStorage.SetPrivate(iq.From, (Element)childNode);
				}
			}
            iq.Query = null;
			iq.SwitchDirection();
			iq.Type = IqType.result;
			return iq;
		}

		private IQ GetPrivate(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			var privateStore = (Private)iq.Query;
			
            if (!privateStore.HasChildElements) return XmppStanzaError.ToNotAcceptable(iq);

			var retrived = new List<Element>();
			foreach (var childNode in privateStore.ChildNodes)
			{
				if (childNode is Element)
				{
					var elementToRetrive = (Element)childNode;
                    var elementRestored = context.StorageManager.PrivateStorage.GetPrivate(iq.From, elementToRetrive);
					retrived.Add(elementRestored ?? elementToRetrive);
				}
			}

            privateStore.RemoveAllChildNodes();
			foreach (var element in retrived)
			{
				privateStore.AddChild(element);
			}
			
            iq.SwitchDirection();
			iq.Type = IqType.result;
			return iq;
		}
	}
}