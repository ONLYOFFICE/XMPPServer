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
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Muc2.Room.Member
{
    class MucRoomMemberDiscoHandler : ServiceDiscoHandler
    {
        private readonly Jid realJid;

        public MucRoomMemberDiscoHandler(Jid jid, Jid realJid)
            : base(jid)
        {
            this.realJid = realJid;
        }

        protected override IQ GetDiscoItems(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            if (((DiscoItems)iq.Query).Node != null) return XmppStanzaError.ToServiceUnavailable(iq);

            var answer = new IQ(IqType.result);
            answer.Id = iq.Id;
            answer.From = Jid;
            answer.To = iq.From;
            var items = new DiscoItems();
            answer.Query = items;
            if (realJid != null)
            {
                items.AddDiscoItem().Jid = realJid;
            }
            return answer;
        }
    }
}