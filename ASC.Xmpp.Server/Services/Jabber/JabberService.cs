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
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server.Authorization;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using System;
using System.Collections.Generic;

namespace ASC.Xmpp.Server.Services.Jabber
{
    class JabberService : XmppServiceBase
    {
        private MessageAnnounceHandler messageAnnounceHandler;

        public override Vcard Vcard
        {
            get { return new Vcard() { Fullname = Name, Description = "© 2008-2015 Assensio System SIA", Url = "http://onlyoffice.com" }; }
        }

        public override void Configure(IDictionary<string, string> properties)
        {
            DiscoInfo.AddIdentity(new DiscoIdentity("server", Name, "im"));
            lock (Handlers)
            {
                Handlers.Add(new ClientNamespaceHandler());
                Handlers.Add(new StartTlsHandler());
                Handlers.Add(new AuthHandler());
                Handlers.Add(new AuthTMTokenHandler());
                Handlers.Add(new BindSessionHandler());
                Handlers.Add(new RosterHandler());
                Handlers.Add(new VCardHandler());
                Handlers.Add(new VerTimePingHandler());
                Handlers.Add(new PrivateHandler());
                Handlers.Add(new PresenceHandler());
                Handlers.Add(new MessageHandler());
                Handlers.Add(new MessageArchiveHandler());
                Handlers.Add(new LastHandler());
                Handlers.Add(new RegisterHandler());
                Handlers.Add(new TransferHandler());
                Handlers.Add(new CommandHandler());
                Handlers.Add(new OfflineProvider(Jid));
                Handlers.Add(new DiscoHandler(Jid));
            }
            messageAnnounceHandler = new MessageAnnounceHandler();
        }

        protected override void OnRegisterCore(XmppHandlerManager handlerManager, XmppServiceManager serviceManager, IServiceProvider serviceProvider)
        {
            var jid = new Jid(Jid.ToString());
            jid.Resource = MessageAnnounceHandler.ANNOUNCE;
            handlerManager.AddXmppHandler(jid, messageAnnounceHandler);
        }

        protected override void OnUnregisterCore(XmppHandlerManager handlerManager, XmppServiceManager serviceManager, IServiceProvider serviceProvider)
        {
            handlerManager.RemoveXmppHandler(messageAnnounceHandler);
        }
    }
}