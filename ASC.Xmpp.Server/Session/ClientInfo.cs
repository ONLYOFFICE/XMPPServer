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

using System.Collections.Concurrent;
using System.Collections.Generic;

using ASC.Xmpp.Core.protocol.iq.disco;

namespace ASC.Xmpp.Server.Session
{
    public class ClientInfo
    {
        private const string DEFAULT_NODE = "DEFAULT_NODE";

        private readonly IDictionary<string, DiscoInfo> discoCache = new ConcurrentDictionary<string, DiscoInfo>();

        public DiscoInfo GetDiscoInfo(string node)
        {
            if (string.IsNullOrEmpty(node))
            {
                node = DEFAULT_NODE;
            }

            DiscoInfo info;
            discoCache.TryGetValue(node, out info);
            return info;
        }

        public void SetDiscoInfo(DiscoInfo discoInfo)
        {
            if (discoInfo == null)
            {
                return;
            }
            var node = !string.IsNullOrEmpty(discoInfo.Node) ? discoInfo.Node : DEFAULT_NODE;
            discoCache[node] = discoInfo;
        }
    }
}
