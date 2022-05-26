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
using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.x.muc;
using ASC.Xmpp.Core.protocol.x.muc.iq.admin;
using ASC.Xmpp.Core.protocol.x.muc.iq.owner;
using ASC.Xmpp.Core.protocol.x.tm.history;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Services.Muc2.Room.Member;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Streams;

using Error = ASC.Xmpp.Core.protocol.client.Error;

namespace ASC.Xmpp.Server.Services.Muc2.Room
{
    [XmppHandler(typeof(Stanza))]
    internal class MucRoomStanzaHandler : XmppStanzaHandler
    {
        public MucRoom Room { get; set; }


        internal MucRoomStanzaHandler(MucRoom room)
        {
            Room = room;
        }

        public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            //Admins iq

            //New member
            MucRoomMember member = Room.GetRealMember(iq.From);
            if (member != null)
            {
                if (iq.Query != null)
                {
                    if (iq.Query is Admin && (member.Affiliation == Affiliation.admin || member.Affiliation == Affiliation.owner))
                    {
                        Room.AdminCommand(iq, member);
                    }
                    else if (iq.Query is Owner && (member.Affiliation == Affiliation.owner))
                    {
                        Room.OwnerCommand(iq, member);
                    }
                    else if (iq.Query is Core.protocol.x.tm.history.History && iq.Type == IqType.get)
                    {
                        Jid jid = iq.To;
                        var mucStore = new DbMucStore();
                        var properties = new Dictionary<string, string>(1) { { "connectionStringName", "core" } };
                        mucStore.Configure(properties);

                        var history = (Core.protocol.x.tm.history.History)iq.Query;

                        foreach (var msg in mucStore.GetMucMessages(jid, history.Count, history.StartIndex))
                        {
                            if (msg == null) continue;

                            history.AddChild(HistoryItem.FromMessage(msg));
                        }
                        iq.Type = IqType.result;
                        iq.SwitchDirection();
                        return iq;
                    }
                    else
                    {
                        XmppStanzaError.ToForbidden(iq);
                    }
                }
                else
                {
                    XmppStanzaError.ToBadRequest(iq);
                }
            }
            else
            {
                XmppStanzaError.ToForbidden(iq);
            }
            if (!iq.Switched)
            {
                iq.SwitchDirection();
            }
            iq.From = Room.Jid;
            return iq;
        }

        public override void HandlePresence(Streams.XmppStream stream, Presence presence, XmppHandlerContext context)
        {
            string userName = presence.To.Resource;
            if (!string.IsNullOrEmpty(userName) && presence.Type == PresenceType.available)
            {
                //New member
                MucRoomMember member = Room.GetRealMember(presence.From);
                if (member != null)
                {
                    if (ReferenceEquals(stream, member.Stream))
                    {
                        if (!Room.TryNickChange(presence))
                        {
                            ErrorPresence(presence, ErrorCondition.NotAcceptable);
                            context.Sender.SendTo(stream, presence);
                        }
                    }
                    else
                    {
                        //Conflict. user with this jid already in room
                        ErrorPresence(presence, ErrorCondition.Conflict);
                        context.Sender.SendTo(stream, presence);
                    }
                }
                else
                {
                    //Doesn't exists
                    MucRoomMember newMember = new MucRoomMember(Room, presence.To, presence.From, stream, context);
                    Room.TryEnterRoom(newMember, presence);
                }
            }
            else
            {
                ErrorPresence(presence, ErrorCondition.BadRequest);
                context.Sender.SendTo(stream, presence);
            }
        }

        private static void ErrorPresence(Presence presence, ErrorCondition condition)
        {
            presence.SwitchDirection();
            presence.RemoveAllChildNodes();
            presence.AddChild(new Muc());
            presence.Type = PresenceType.error;
            presence.Error = new Error(condition);
        }

        public override void HandleMessage(Streams.XmppStream stream, Message msg, XmppHandlerContext context)
        {
            User user = (User)msg.SelectSingleElement(typeof(User));
            if (user != null)
            {
                HandleUserMessage(msg, user, stream);
            }
            else
            {
                //Groupchat message
                MucRoomMember member = Room.GetRealMember(msg.From);
                if (member != null && ReferenceEquals(member.Stream, stream) && member.Role != Role.none)
                {
                    if (msg.Type == MessageType.groupchat)
                    {
                        if (msg.Subject != null)
                        {
                            Room.ChangeSubject(member, msg.Subject);
                        }
                        else
                        {
                            MessageBroadcast(msg, member);
                        }
                    }
                    else
                    {
                        msg.SwitchDirection();
                        msg.Type = MessageType.error;
                        msg.Error = new Error(ErrorCondition.NotAcceptable);
                        context.Sender.SendTo(stream, msg);
                    }
                }
                else
                {
                    msg.SwitchDirection();
                    msg.Type = MessageType.error;
                    msg.Error = new Error(ErrorCondition.Forbidden);
                    context.Sender.SendTo(stream, msg);
                }
            }

        }

        private void HandleUserMessage(Message msg, User user, XmppStream stream)
        {
            if (user.Invite != null)
            {
                Room.InviteUser(msg, user, stream);
            }
            else if (user.Decline != null)
            {
                Room.DeclinedUser(msg, user, stream);
            }
        }



        private void MessageBroadcast(Message msg, MucRoomMember member)
        {
            Room.BroadcastMessage(msg, member);
        }
    }
}