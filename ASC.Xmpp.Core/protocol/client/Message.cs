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
using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.protocol.extensions.chatstates;
using ASC.Xmpp.Core.protocol.extensions.html;
using ASC.Xmpp.Core.protocol.extensions.nickname;
using ASC.Xmpp.Core.protocol.extensions.shim;
using ASC.Xmpp.Core.protocol.x;

namespace ASC.Xmpp.Core.protocol.client
{

    #region usings

    #endregion

    /// <summary>
    ///   This class represents a XMPP message.
    /// </summary>
    public class Message : Stanza
    {
        #region << Constructors >>

        /// <summary>
        /// </summary>
        public Message()
        {
            TagName = "message";
            Namespace = Uri.CLIENT;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        public Message(Jid to) : this()
        {
            To = to;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="body"> </param>
        public Message(Jid to, string body) : this(to)
        {
            Body = body;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="from"> </param>
        public Message(Jid to, Jid from) : this()
        {
            To = to;
            From = from;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="body"> </param>
        public Message(string to, string body) : this()
        {
            To = new Jid(to);
            Body = body;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        public Message(Jid to, string body, string subject) : this()
        {
            To = to;
            Body = body;
            Subject = subject;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        public Message(string to, string body, string subject) : this()
        {
            To = new Jid(to);
            Body = body;
            Subject = subject;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        /// <param name="thread"> </param>
        public Message(string to, string body, string subject, string thread) : this()
        {
            To = new Jid(to);
            Body = body;
            Subject = subject;
            Thread = thread;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        /// <param name="thread"> </param>
        public Message(Jid to, string body, string subject, string thread) : this()
        {
            To = to;
            Body = body;
            Subject = subject;
            Thread = thread;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="type"> </param>
        /// <param name="body"> </param>
        public Message(string to, MessageType type, string body) : this()
        {
            To = new Jid(to);
            Type = type;
            Body = body;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="type"> </param>
        /// <param name="body"> </param>
        public Message(Jid to, MessageType type, string body) : this()
        {
            To = to;
            Type = type;
            Body = body;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="type"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        public Message(string to, MessageType type, string body, string subject) : this()
        {
            To = new Jid(to);
            Type = type;
            Body = body;
            Subject = subject;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="type"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        public Message(Jid to, MessageType type, string body, string subject) : this()
        {
            To = to;
            Type = type;
            Body = body;
            Subject = subject;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="type"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        /// <param name="thread"> </param>
        public Message(string to, MessageType type, string body, string subject, string thread) : this()
        {
            To = new Jid(to);
            Type = type;
            Body = body;
            Subject = subject;
            Thread = thread;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="type"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        /// <param name="thread"> </param>
        public Message(Jid to, MessageType type, string body, string subject, string thread) : this()
        {
            To = to;
            Type = type;
            Body = body;
            Subject = subject;
            Thread = thread;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="from"> </param>
        /// <param name="body"> </param>
        public Message(Jid to, Jid from, string body) : this()
        {
            To = to;
            From = from;
            Body = body;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="from"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        public Message(Jid to, Jid from, string body, string subject) : this()
        {
            To = to;
            From = from;
            Body = body;
            Subject = subject;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="from"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        /// <param name="thread"> </param>
        public Message(Jid to, Jid from, string body, string subject, string thread) : this()
        {
            To = to;
            From = from;
            Body = body;
            Subject = subject;
            Thread = thread;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="from"> </param>
        /// <param name="type"> </param>
        /// <param name="body"> </param>
        public Message(Jid to, Jid from, MessageType type, string body) : this()
        {
            To = to;
            From = from;
            Type = type;
            Body = body;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="from"> </param>
        /// <param name="type"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        public Message(Jid to, Jid from, MessageType type, string body, string subject) : this()
        {
            To = to;
            From = from;
            Type = type;
            Body = body;
            Subject = subject;
        }

        /// <summary>
        /// </summary>
        /// <param name="to"> </param>
        /// <param name="from"> </param>
        /// <param name="type"> </param>
        /// <param name="body"> </param>
        /// <param name="subject"> </param>
        /// <param name="thread"> </param>
        public Message(Jid to, Jid from, MessageType type, string body, string subject, string thread)
            : this()
        {
            To = to;
            From = from;
            Type = type;
            Body = body;
            Subject = subject;
            Thread = thread;
        }

        #endregion

        #region << Properties >>

        /// <summary>
        ///   The body of the message. This contains the message text.
        /// </summary>
        public string Body
        {
            get { return GetTag("body"); }

            set { SetTag("body", value); }
        }

        /// <summary>
        ///   subject of this message. Its like a subject in a email. The Subject is optional.
        /// </summary>
        public string Subject
        {
            get { return GetTag("subject"); }

            set { SetTag("subject", value); }
        }

        /// <summary>
        ///   messages and conversations could be threaded. You can compare this with threads in newsgroups or forums. Threads are optional.
        /// </summary>
        public string Thread
        {
            get { return GetTag("thread"); }

            set { SetTag("thread", value); }
        }

        /// <summary>
        ///   message type (chat, groupchat, normal, headline or error).
        /// </summary>
        public MessageType Type
        {
            get { return (MessageType) GetAttributeEnum("type", typeof (MessageType)); }

            set
            {
                if (value == MessageType.normal)
                {
                    RemoveAttribute("type");
                }
                else
                {
                    SetAttribute("type", value.ToString());
                }
            }
        }

        /// <summary>
        ///   Error Child Element
        /// </summary>
        public Error Error
        {
            get { return SelectSingleElement(typeof (Error)) as Error; }

            set
            {
                if (HasTag(typeof (Error)))
                {
                    RemoveTag(typeof (Error));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        ///   The html part of the message if you want to support the html-im Jep. This part of the message is optional.
        /// </summary>
        public Html Html
        {
            get { return (Html) SelectSingleElement(typeof (Html)); }

            set
            {
                RemoveTag(typeof (Html));
                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        ///   The event Element for JEP-0022 Message events
        /// </summary>
        public Event XEvent
        {
            get { return SelectSingleElement(typeof (Event)) as Event; }

            set
            {
                if (HasTag(typeof (Event)))
                {
                    RemoveTag(typeof (Event));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        ///   The event Element for JEP-0022 Message events
        /// </summary>
        public Delay XDelay
        {
            get { return SelectSingleElement(typeof (Delay)) as Delay; }

            set
            {
                if (HasTag(typeof (Delay)))
                {
                    RemoveTag(typeof (Delay));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        ///   Stanza Headers and Internet Metadata
        /// </summary>
        public Headers Headers
        {
            get { return SelectSingleElement(typeof (Headers)) as Headers; }

            set
            {
                if (HasTag(typeof (Headers)))
                {
                    RemoveTag(typeof (Headers));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        ///   Nickname Element
        /// </summary>
        public Nickname Nickname
        {
            get { return SelectSingleElement(typeof (Nickname)) as Nickname; }

            set
            {
                if (HasTag(typeof (Nickname)))
                {
                    RemoveTag(typeof (Nickname));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        #region << Chatstate Properties >>   

        /// <summary>
        /// </summary>
        public Chatstate Chatstate
        {
            get
            {
                if (HasTag(typeof (Active)))
                {
                    return Chatstate.active;
                }
                else if (HasTag(typeof (Inactive)))
                {
                    return Chatstate.inactive;
                }
                else if (HasTag(typeof (Composing)))
                {
                    return Chatstate.composing;
                }
                else if (HasTag(typeof (Paused)))
                {
                    return Chatstate.paused;
                }
                else if (HasTag(typeof (Gone)))
                {
                    return Chatstate.gone;
                }
                else
                {
                    return Chatstate.None;
                }
            }

            set
            {
                RemoveChatstate();
                switch (value)
                {
                    case Chatstate.active:
                        AddChild(new Active());
                        break;
                    case Chatstate.inactive:
                        AddChild(new Inactive());
                        break;
                    case Chatstate.composing:
                        AddChild(new Composing());
                        break;
                    case Chatstate.paused:
                        AddChild(new Paused());
                        break;
                    case Chatstate.gone:
                        AddChild(new Gone());
                        break;
                }
            }
        }

        /// <summary>
        /// </summary>
        private void RemoveChatstate()
        {
            RemoveTag(typeof (Active));
            RemoveTag(typeof (Inactive));
            RemoveTag(typeof (Composing));
            RemoveTag(typeof (Paused));
            RemoveTag(typeof (Gone));
        }

        #endregion

        #endregion

        #region << Methods and Functions >>

#if !CF

        /// <summary>
        ///   Create a new unique Thread indendifier
        /// </summary>
        /// <returns> </returns>
        public string CreateNewThread()
        {
            string guid = Guid.NewGuid().ToString().ToLower();
            Thread = guid;

            return guid;
        }

#endif

        #endregion
    }
}