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
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Storage.Interface;

namespace ASC.Xmpp.Server.Users
{
    public class UserManager
    {
        private readonly StorageManager storageManager;

        private IUserStore userStore;

        private IUserStore UserStore
        {
            get
            {
                if (userStore == null)
                {
                    lock (this)
                    {
                        if (userStore == null) userStore = storageManager.UserStorage;
                    }
                }
                return userStore;
            }
        }

        public UserManager(StorageManager storageManager)
        {
            if (storageManager == null) throw new ArgumentNullException("storageManager");
            this.storageManager = storageManager;
        }

        public bool IsUserExists(Jid jid)
        {
            return GetUser(jid) != null;
        }

        public User GetUser(Jid jid)
        {
            return UserStore.GetUser(jid);
        }

        public ICollection<User> GetUsers(string domain)
        {
            return UserStore.GetUsers(domain);
        }

        public void SaveUser(User user)
        {
            UserStore.SaveUser(user);
        }

        public void RemoveUser(Jid jid)
        {
            UserStore.RemoveUser(jid);
        }
    }
}