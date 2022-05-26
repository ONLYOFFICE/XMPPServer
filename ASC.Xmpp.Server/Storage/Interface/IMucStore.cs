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

using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Server.Storage.Interface
{
    using Services.Muc2.Room.Settings;

    public interface IMucStore
    {
        List<MucRoomInfo> GetMucs(string server);

        MucRoomInfo GetMuc(Jid mucName);

        void SaveMuc(MucRoomInfo muc);

        void RemoveMuc(Jid mucName);

        List<Message> GetMucMessages(Jid mucName, int count, int startindex);

        void AddMucMessages(Jid mucName, params Message[] message);

        void RemoveMucMessages(Jid mucName);

        MucRoomSettings GetMucRoomSettings(Jid roomName);

        void SetMucRoomSettings(Jid roomName, MucRoomSettings settings);
    }
}
