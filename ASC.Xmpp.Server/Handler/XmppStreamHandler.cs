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

using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Streams;


namespace ASC.Xmpp.Server.Handler
{
    public class XmppStreamHandler : IXmppStreamHandler
    {
        public virtual void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
        {

        }

        public virtual void StreamEndHandle(XmppStream stream, ICollection<Node> notSendedBuffer, XmppHandlerContext context)
        {

        }

        public virtual void OnRegister(IServiceProvider serviceProvider)
        {

        }

        public virtual void OnUnregister(IServiceProvider serviceProvider)
        {

        }
    }
}
