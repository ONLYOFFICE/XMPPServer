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

namespace ASC.Xmpp.Server.Streams
{
    public class XmppStreamManager
    {
        private readonly ConcurrentDictionary<string, XmppStream> streams = new ConcurrentDictionary<string, XmppStream>();

        public XmppStream GetStream(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException("connectionId");
            }

            XmppStream stream;
            streams.TryGetValue(connectionId, out stream);
            return stream;
        }

        public XmppStream GetOrCreateNewStream(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException("connectionId");
            }

            var stream = new XmppStream(connectionId);
            streams.AddOrUpdate(connectionId, stream, (id, old) =>
            {
                if (old.Authenticated)
                {
                    stream.Authenticate(old.User);
                }
                return stream;
            });
            return stream;
        }

        public void RemoveStream(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException("connectionId");
            }

            XmppStream stream;
            streams.TryRemove(connectionId, out stream);
        }
    }
}
