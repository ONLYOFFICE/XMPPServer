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

using ASC.Core.Users;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Server;
using ASC.Xmpp.Server.Storage.Interface;
using ASC.Xmpp.Server.Users;

namespace ASC.Xmpp.Host
{
    class ASCUserStore : IUserStore
    {
        #region IUserStore Members

        public ICollection<User> GetUsers(string domain)
        {
            ASCContext.SetCurrentTenant(domain);
            var users = new List<User>();
            foreach (var ui in ASCContext.UserManager.GetUsers())
            {
                var u = ToUser(ui, domain);
                if (u != null) users.Add(u);
            }
            return users;
        }

        public User GetUser(Jid jid)
        {
            ASCContext.SetCurrentTenant(jid.Server);
            var u = ASCContext.UserManager.GetUserByUserName(jid.User);
            if (Constants.LostUser.Equals(u) || u.Status == EmployeeStatus.Terminated) return null;
            return ToUser(u, jid.Server);
        }

        public void SaveUser(User user)
        {
            throw new JabberException(ErrorCode.Forbidden);
        }

        public void RemoveUser(Jid jid)
        {
            throw new JabberException(ErrorCode.Forbidden);
        }

        #endregion

        private User ToUser(UserInfo userInfo, string domain)
        {

            try
            {
                if (string.IsNullOrEmpty(userInfo.UserName)) return null;
                return new User(
                    new Jid(userInfo.UserName.ToLowerInvariant() + "@" + domain.ToLowerInvariant()),
                    ASCContext.UserManager.IsUserInGroup(userInfo.ID, Constants.GroupAdmin.ID),
                    userInfo.Sid
                );
            }
            catch { }
            return null;
        }
    }
}