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

using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Server.Services.Muc2.Room.Member
{
    using Handler;

    [XmppHandler(typeof(Stanza))]
    internal class MucRoomMemberStanzaHandler : XmppStanzaHandler
    {
        public MucRoomMember Member { get; set; }

        internal MucRoomMemberStanzaHandler(MucRoomMember member)
        {
            Member = member;
        }

        public override IQ HandleIQ(ASC.Xmpp.Server.Streams.XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            if (iq.Vcard!=null && iq.Type==IqType.get)
            {
                //Handle vcard
                iq.Vcard = Member.GetVcard();
                iq.Type = IqType.result;
                iq.SwitchDirection();
                return iq;
            }
            return base.HandleIQ(stream, iq, context);
        }

        public override void HandlePresence(Streams.XmppStream stream, Presence presence, XmppHandlerContext context)
        {
            if (presence.Type == PresenceType.available || presence.Type == PresenceType.unavailable)
            {
                if (!ReferenceEquals(Member.Stream, stream))
                {
                    //Set stream
                    Member.Stream = stream;
                    if (presence.Type == PresenceType.available)
                    {
                        //If stream changed then we should broadcast presences
                        Member.ReEnterRoom();
                    }
                }
                Member.ChangePesence(presence);
            }
            else
            {
                //Bad request                
                presence.SwitchDirection();
                presence.From = Member.RoomFrom;
                presence.Type = PresenceType.error;
                presence.Error = new Error(ErrorCondition.BadRequest);
                context.Sender.SendTo(stream, presence);
            }
        }

        public override void HandleMessage(Streams.XmppStream stream, Message msg, XmppHandlerContext context)
        {
            //Private msg
            if (msg.Type==MessageType.chat)
            {
                if (Member.ResolveRoomJid(msg.From)==null)
                {
                    //Error
                    msg.SwitchDirection();
                    msg.From = Member.RoomFrom;
                    msg.Type = MessageType.error;
                    msg.Error = new Error(ErrorCondition.ItemNotFound);
                    context.Sender.SendTo(stream, msg);
                }
                else
                {
                    //Send
                    msg.To = Member.RealJid;
                    msg.From = Member.ResolveRoomJid(msg.From);
                    Member.Send(msg);
                }

            }
            else
            {
                msg.SwitchDirection();
                msg.From = Member.RoomFrom;
                msg.Type = MessageType.error;
                msg.Error = new Error(ErrorCondition.BadRequest);
                context.Sender.SendTo(stream, msg);
            }
        }
    }
}