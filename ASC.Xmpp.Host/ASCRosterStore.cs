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

using ASC.Core.Users;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.roster;
using ASC.Xmpp.Server;
using ASC.Xmpp.Server.Configuration;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Storage.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ASC.Xmpp.Host
{
    class ASCRosterStore : DbRosterStore, IRosterStore
    {
        #region IRosterStore Members

        public override List<UserRosterItem> GetRosterItems(Jid rosterJid)
        {
            try
            {
                ASCContext.SetCurrentTenant(rosterJid.Server);
                var items = GetASCRosterItems(rosterJid);
                items.AddRange(base.GetRosterItems(rosterJid));
                SortRoster(items);
                return items;
            }
            catch (Exception e)
            {
                throw new JabberException("Could not get roster items.", e);
            }
        }

        public override UserRosterItem GetRosterItem(Jid rosterJid, Jid itemJid)
        {
            try
            {
                ASCContext.SetCurrentTenant(rosterJid.Server);
                var u = ASCContext.UserManager.GetUserByUserName(itemJid.User);
                return !string.IsNullOrEmpty(u.UserName) ?
                    ToUserRosterItem(u, itemJid.Server) :
                    base.GetRosterItem(rosterJid, itemJid);
            }
            catch (Exception e)
            {
                throw new JabberException("Could not get roster item.", e);
            }
        }

        public override UserRosterItem SaveRosterItem(Jid rosterJid, UserRosterItem item)
        {
            if (item == null) throw new ArgumentNullException("item");

            ASCContext.SetCurrentTenant(rosterJid.Server);
            if (IsASCRosterItem(rosterJid, item.Jid)) throw new JabberException(ErrorCode.Forbidden);

            return base.SaveRosterItem(rosterJid, item);
        }

        public override void RemoveRosterItem(Jid rosterJid, Jid itemJid)
        {
            ASCContext.SetCurrentTenant(rosterJid.Server);
            if (IsASCRosterItem(rosterJid, itemJid)) throw new JabberException(ErrorCode.Forbidden);

            base.RemoveRosterItem(rosterJid, itemJid);
        }

        #endregion

        private List<UserRosterItem> GetASCRosterItems(Jid jid)
        {
            var items = new List<UserRosterItem>();
            foreach (var u in ASCContext.UserManager.GetUsers())
            {
                if (string.IsNullOrEmpty(u.UserName) || string.Compare(jid.User, u.UserName, true) == 0) continue;
                items.Add(ToUserRosterItem(u, jid.Server));
            }
            // for migration from teamlab.com to onlyoffice.com
            string domain = jid.Server;
            if (JabberConfiguration.ReplaceDomain && domain.EndsWith(JabberConfiguration.ReplaceFromDomain))
            {
                int place = domain.LastIndexOf(JabberConfiguration.ReplaceFromDomain);
                if (place >= 0)
                {
                    domain = domain.Remove(place, JabberConfiguration.ReplaceFromDomain.Length).Insert(place, JabberConfiguration.ReplaceToDomain);
                }
            }
            //add server
            items.Add(new UserRosterItem(new Jid(jid.Server)) { Name = domain, Subscribtion = SubscriptionType.both, Ask = AskType.NONE });
            return items;
        }

        private bool IsASCRosterItem(Jid rosterJid, Jid itemJid)
        {
            return ASCContext.UserManager.IsUserNameExists(itemJid.User);
        }

        private void SortRoster(List<UserRosterItem> roster)
        {
            roster.Sort((x, y) => string.Compare(!string.IsNullOrEmpty(x.Name) ? x.Name : x.Jid.ToString(), !string.IsNullOrEmpty(y.Name) ? y.Name : y.Jid.ToString(), true));
        }

        private UserRosterItem ToUserRosterItem(UserInfo u, string server)
        {
            var item = new UserRosterItem(new Jid(u.UserName + "@" + server))
            {
                Name = UserFormatter.GetUserName(u),
                Subscribtion = SubscriptionType.both,
                Ask = AskType.NONE,
            };
            foreach (var g in ASCContext.UserManager.GetUserGroups(u.ID))
            {
                item.Groups.Add(g.Name);
            }
            return item;
        }
    }
}