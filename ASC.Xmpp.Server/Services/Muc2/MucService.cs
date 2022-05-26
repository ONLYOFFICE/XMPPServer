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

using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Services.Jabber;
using ASC.Xmpp.Server.Services.Muc2.Room;
using ASC.Xmpp.Server.Services.Muc2.Room.Settings;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Storage.Interface;

namespace ASC.Xmpp.Server.Services.Muc2
{
    internal class MucService : XmppServiceBase
    {
        private XmppHandlerManager handlerManager;

        #region Properties

        public IMucStore MucStorage
        {
            get
            {
                return ((StorageManager)context.GetService(typeof(StorageManager))).MucStorage;
            }
        }

        public IVCardStore VcardStorage
        {
            get
            {
                return ((StorageManager)context.GetService(typeof(StorageManager))).VCardStorage;
            }
        }

        public XmppServiceManager ServiceManager
        {
            get { return ((XmppServiceManager)context.GetService(typeof(XmppServiceManager))); }
        }

        public XmppHandlerManager HandlerManager
        {
            get { return handlerManager; }
        }

        #endregion

        public override void Configure(IDictionary<string, string> properties)
        {
            base.Configure(properties);

            DiscoInfo.AddIdentity(new DiscoIdentity("text", Name, "conference"));
            DiscoInfo.AddFeature(new DiscoFeature(ASC.Xmpp.Core.protocol.Uri.MUC));
            DiscoInfo.AddFeature(new DiscoFeature(Features.FEAT_MUC_ROOMS));
            lock (Handlers)
            {
                Handlers.Add(new MucStanzaHandler(this));
                Handlers.Add(new VCardHandler());
                Handlers.Add(new ServiceDiscoHandler(Jid));
            }
        }

        protected override void OnRegisterCore(XmppHandlerManager handlerManager, XmppServiceManager serviceManager, IServiceProvider provider)
        {
            context = provider;
            this.handlerManager = handlerManager;
            LoadRooms();
        }

        private void LoadRooms()
        {
            List<MucRoomInfo> rooms = MucStorage.GetMucs(Jid.Server);
            foreach (MucRoomInfo room in rooms)
            {
                CreateRoom(room.Jid, room.Description);
            }
        }


        private IServiceProvider context;

        /// <summary>
        /// </summary>
        /// <param name="name">
        /// </param>
        /// <param name="description">
        /// </param>
        /// <returns>
        /// </returns>
        internal MucRoom CreateRoom(Jid roomJid, string description)
        {
            MucRoom room = new MucRoom(roomJid, roomJid.User, this, context);
            room.ParentService = this;
            ServiceManager.RegisterService(room);
            return room;
        }

        public void RemoveRoom(MucRoom room)
        {
            ServiceManager.UnregisterService(room.Jid);
            MucStorage.RemoveMuc(room.Jid);
        }
    }
}