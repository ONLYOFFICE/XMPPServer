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
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.x.muc.iq;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Services.Muc2.Helpers;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Utils;

using Error = ASC.Xmpp.Core.protocol.client.Error;

namespace ASC.Xmpp.Server.Services.Muc2
{
    [XmppHandler(typeof(Stanza))]
    internal class MucStanzaHandler : XmppStanzaHandler
    {
        public MucService Service { get; set; }

        internal MucStanzaHandler(MucService service)
        {
            Service = service;
        }

        public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            Unique unique = (Unique)iq.SelectSingleElement(typeof(Unique));
            if (unique != null)
            {
                // Gen unique id
                unique.Value = UniqueId.CreateNewId(16);
                iq.Type = IqType.result;
                iq.SwitchDirection();
                return iq;
            }
            iq.SwitchDirection();
            iq.Type = IqType.error;
            iq.Error = new Error(ErrorType.cancel, ErrorCondition.ItemNotFound);
            return iq;
        }

        public override void HandlePresence(XmppStream stream, Presence presence, XmppHandlerContext context)
        {
            //Presence to open new room
            if (MucHelpers.IsJoinRequest(presence))
            {
                //Register
                Service.CreateRoom(new Jid(presence.To.Bare), null);
                Service.HandlerManager.ProcessStreamElement(presence, stream);//Forward to room
            }
            else
            {
                //Return error
                presence.Type = PresenceType.error;
                presence.Error = new Error(ErrorType.cancel, ErrorCondition.NotAllowed);
                presence.SwitchDirection();
                context.Sender.SendTo(stream, presence);
            }
        }

        public override void HandleMessage(XmppStream stream, Message msg, XmppHandlerContext context)
        {
            msg.SwitchDirection();
            msg.Type = MessageType.error;
            msg.Error = new Error(ErrorType.cancel, ErrorCondition.ItemNotFound);
            context.Sender.SendTo(stream, msg);
        }
    }
}