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
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Server.Handler
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class XmppHandlerAttribute : Attribute
	{
		public Type XmppElementType
		{
			get;
			private set;
		}

		public XmppHandlerAttribute(Type xmppElementType) {
			if (xmppElementType == null) throw new ArgumentNullException("xmppElementType");

			if (!typeof(Element).IsAssignableFrom(xmppElementType)) throw new ArgumentException("xmppElementType not assigned from Element.");
			XmppElementType = xmppElementType;
		}
	}
}
