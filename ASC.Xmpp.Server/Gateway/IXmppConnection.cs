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

using ASC.Xmpp.Core.utils.Xml.Dom;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Xmpp.Server.Gateway
{
	public interface IXmppConnection
	{
		string Id
		{
			get;
		}

		void Reset();

		void Close();

		void Send(Node node, Encoding encoding);

		void Send(string text, Encoding encoding);

		void BeginReceive();

		event EventHandler<XmppStreamStartEventArgs> XmppStreamStart;

		event EventHandler<XmppStreamEventArgs> XmppStreamElement;

		event EventHandler<XmppStreamEndEventArgs> XmppStreamEnd;

		event EventHandler<XmppConnectionCloseEventArgs> Closed;
	}

	public class XmppStreamEventArgs : EventArgs
	{
		public string ConnectionId
		{
			get;
			private set;
		}

		public Node Node
		{
			get;
			private set;
		}

		public XmppStreamEventArgs(string connectionId, Node node)
		{
			if (string.IsNullOrEmpty(connectionId)) throw new ArgumentNullException("connectionId");
			if (node == null) throw new ArgumentNullException("node");

			ConnectionId = connectionId;
			Node = node;
		}
	}

	public class XmppStreamStartEventArgs : XmppStreamEventArgs
	{
		public string Namespace
		{
			get;
			private set;
		}

		public XmppStreamStartEventArgs(string connectionId, Node node, string ns)
			: base(connectionId, node)
		{
			Namespace = ns;
		}
	}

	public class XmppStreamEndEventArgs : EventArgs
	{
		public string ConnectionId
		{
			get;
			private set;
		}

		public ICollection<Node> NotSendedBuffer
		{
			get;
			private set;
		}

		public XmppStreamEndEventArgs(string connectionId, IEnumerable<Node> notSendedBuffer)
		{
            
			if (string.IsNullOrEmpty(connectionId)) throw new ArgumentNullException("connectionId");
		    
			ConnectionId = connectionId;
			if (notSendedBuffer == null)
			{
				NotSendedBuffer = new List<Node>();
			}
			else
			{
				NotSendedBuffer = new List<Node>(notSendedBuffer);
			}
		}
	}

	public class XmppConnectionCloseEventArgs : EventArgs
	{

	}
}
