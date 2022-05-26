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
using System.Text;

using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.bind;
using ASC.Xmpp.Core.protocol.sasl;
using ASC.Xmpp.Core.protocol.stream;
using ASC.Xmpp.Core.protocol.stream.feature;
using ASC.Xmpp.Core.protocol.tls;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Storage;

using Uri = ASC.Xmpp.Core.protocol.Uri;

namespace ASC.Xmpp.Server.Streams
{
    class ClientNamespaceHandler : IXmppStreamStartHandler
    {
        public string Namespace
        {
            get { return Uri.CLIENT; }
        }

        public void StreamStartHandle(XmppStream xmppStream, Stream stream, XmppHandlerContext context)
        {
            var streamHeader = new StringBuilder();
            streamHeader.AppendLine("<?xml version='1.0' encoding='UTF-8'?>");
            streamHeader.AppendFormat("<stream:{0} xmlns:{0}='{1}' xmlns='{2}' from='{3}' id='{4}' version='1.0'>",
                Uri.PREFIX, Uri.STREAM, Uri.CLIENT, stream.To, xmppStream.Id);
            context.Sender.SendTo(xmppStream, streamHeader.ToString());

            var features = new Features();
            features.Prefix = Uri.PREFIX;
            if (xmppStream.Authenticated)
            {
                features.AddChild(new Bind());
                features.AddChild(new Core.protocol.iq.session.Session());
            }
            else
            {
                features.Mechanisms = new Mechanisms();
                var connection = context.Sender.GetXmppConnection(xmppStream.ConnectionId);
                var storage = new DbLdapSettingsStore();
                storage.GetLdapSettings(xmppStream.Domain);
                if (!storage.EnableLdapAuthentication || connection is BoshXmppConnection)
                {
                    features.Mechanisms.AddChild(new Mechanism(MechanismType.DIGEST_MD5));
                    features.Mechanisms.AddChild(new Mechanism(MechanismType.PLAIN));
                }
                else
                {
                    features.Mechanisms.AddChild(new Mechanism(MechanismType.PLAIN));
                }
                features.Mechanisms.AddChild(new Element("required"));
                features.Register = new Register();
                var auth = new Auth();
                auth.Namespace = Uri.FEATURE_IQ_AUTH;
                features.ChildNodes.Add(auth);
                if (connection is TcpXmppConnection)
                {
                    var tcpXmppListener = (TcpXmppListener)(context.XmppGateway.GetXmppListener("Jabber Listener"));
                    if (tcpXmppListener.StartTls != XmppStartTlsOption.None && !((TcpXmppConnection)connection).TlsStarted)
                    {
                        features.StartTls = new StartTls();
                        if (tcpXmppListener.StartTls == XmppStartTlsOption.Required)
                        {
                            features.StartTls.Required = true;
                        }
                    }
                }
            }
            context.Sender.SendTo(xmppStream, features);
        }

        public void OnRegister(IServiceProvider serviceProvider)
        {

        }

        public void OnUnregister(IServiceProvider serviceProvider)
        {

        }
    }
}