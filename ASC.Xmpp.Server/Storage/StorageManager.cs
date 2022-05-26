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
using System.Collections.Concurrent;
using System.Collections.Generic;

using ASC.Xmpp.Server.Storage.Interface;

namespace ASC.Xmpp.Server.Storage
{
    public class StorageManager : IDisposable
    {
        private readonly IDictionary<string, object> storages = new ConcurrentDictionary<string, object>();

        public IOfflineStore OfflineStorage
        {
            get { return GetStorage<IOfflineStore>("offline"); }
        }

        public IRosterStore RosterStorage
        {
            get { return GetStorage<IRosterStore>("roster"); }
        }

        public IVCardStore VCardStorage
        {
            get { return GetStorage<IVCardStore>("vcard"); }
        }

        public IPrivateStore PrivateStorage
        {
            get { return GetStorage<IPrivateStore>("private"); }
        }

        public IMucStore MucStorage
        {
            get { return GetStorage<IMucStore>("muc"); }
        }

        public IUserStore UserStorage
        {
            get { return GetStorage<IUserStore>("users"); }
        }

        public T GetStorage<T>(string storageName)
        {
            object storage;
            storages.TryGetValue(storageName, out storage);
            return (T)storage;
        }

        public void SetStorage(string storageName, object storage)
        {
            storages[storageName] = storage;
        }

        public void Dispose()
        {
            foreach (var s in storages.Values)
            {
                var disposable = s as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}