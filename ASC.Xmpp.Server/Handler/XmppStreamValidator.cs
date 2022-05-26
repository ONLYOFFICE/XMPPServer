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
using ASC.Xmpp.Core.protocol.iq.auth;
using ASC.Xmpp.Core.protocol.iq.register;
using ASC.Xmpp.Core.utils.Idn;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Streams;

using Error = ASC.Xmpp.Core.protocol.Error;
using Uri = ASC.Xmpp.Core.protocol.Uri;

namespace ASC.Xmpp.Server.Handler
{
    class XmppStreamValidator
    {
        public bool ValidateStanza(Stanza stanza, XmppStream stream, XmppHandlerContext context)
        {
            Element result = null;
            if (stream.Namespace == Uri.CLIENT) result = ValidateClientStanza(stanza, stream);
            if (stream.Namespace == Uri.SERVER) result = ValidateServerStanza(stanza, stream);

            if (result == null) return true;

            if (result is Stanza)
            {
                context.Sender.SendTo(stream, result);
            }
            else if (result is Error)
            {
                context.Sender.SendToAndClose(stream, result);
            }
            else
            {
                return true;
            }
            return false;
        }

        private Element ValidateClientStanza(Stanza stanza, XmppStream stream)
        {
            if (!stream.Authenticated)
            {
                if (!(stanza is AuthIq) && (stanza is IQ && !(((IQ)stanza).Query is Register))) return XmppStanzaError.ToNotAuthorized(stanza);
            }

            //remove empty jids
            if (stanza.HasFrom && string.IsNullOrEmpty(stanza.From.ToString())) stanza.From = null;
            if (stanza.HasTo && string.IsNullOrEmpty(stanza.To.ToString())) stanza.To = null;

            //prep strings
            stanza.From = NodePrep(stanza.From);
            stanza.To = NodePrep(stanza.To);

            if (!ValidateJid(stanza.From) || !ValidateJid(stanza.To)) return XmppStanzaError.ToBadRequest(stanza);

            if (stanza.HasFrom)
            {
                if (!stream.JidBinded(stanza.From))
                {
                    // return null if we have from in bind iq (for qutIM 0.3 client)
                    if (!(stanza is IQ) || ((IQ)stanza).Bind == null || ((IQ)stanza).Bind.Resource != stanza.From.Resource)
                    {
                        return XmppStreamError.InvalidFrom;
                    }
                }
            }
            else
            {
                if (stream.MultipleResources) return XmppStanzaError.ToConflict(stanza);
                stanza.From = new Jid(string.Format("{0}@{1}/{2}", stream.User, stream.Domain, 0 < stream.Resources.Count ? stream.Resources[0] : null));
            }

            if (stanza is Message)
            {
                var message = (Message)stanza;
                if (message.Type == MessageType.chat && message.To == null) return XmppStanzaError.ToRecipientUnavailable(stanza);
            }
            return null;
        }

        private bool ValidateJid(Jid jid)
        {
            if (jid == null) return true;
            if (jid.HasUser)
            {
                if (jid.User != Jid.EscapeNode(jid.User)) return false;
                if (jid.User != Stringprep.NodePrep(jid.User)) return false;
            }
            return true;
        }

        private Jid NodePrep(Jid jid)
        {
            if (jid != null)
            {
                if (jid.HasUser)
                {
                    jid.User = Stringprep.NodePrep(jid.User);
                }
                if (jid.HasResource)
                {
                    jid.Resource = Stringprep.ResourcePrep(jid.Resource);
                }
                //BUG: Many faults here in log
                jid.Server = Stringprep.NamePrep(jid.Server);
            }
            return jid;
        }

        private Element ValidateServerStanza(Stanza stanza, XmppStream stream)
        {
            if (!stanza.HasTo || !stanza.HasFrom) return XmppStreamError.ImproperAddressing;
            return null;
        }
    }
}
