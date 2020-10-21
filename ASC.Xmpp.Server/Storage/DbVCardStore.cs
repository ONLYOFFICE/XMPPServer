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

using ASC.Common.Data.Sql;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Server.Storage.Interface;
using System;
using System.Collections.Generic;
using System.Data;

namespace ASC.Xmpp.Server.Storage
{
    public class DbVCardStore : DbStoreBase, IVCardStore
    {
        private IDictionary<string, Vcard> vcardsCache = new Dictionary<string, Vcard>();


        protected override SqlCreate[] GetCreateSchemaScript()
        {
            var t1 = new SqlCreate.Table("jabber_vcard", true)
                .AddColumn("jid", DbType.String, 255, true)
                .AddColumn("vcard", DbType.String, UInt16.MaxValue, true)
                .PrimaryKey("jid");
            return new[] { t1 };
        }


        public virtual void SetVCard(Jid jid, Vcard vcard)
        {
            if (jid == null) throw new ArgumentNullException("jid");
            if (vcard == null) throw new ArgumentNullException("vcard");

            try
            {
                ExecuteNonQuery(
                    new SqlInsert("jabber_vcard", true)
                    .InColumnValue("jid", jid.Bare.ToLowerInvariant())
                    .InColumnValue("vcard", ElementSerializer.SerializeElement(vcard)));
                lock (vcardsCache)
                {
                    vcardsCache[jid.Bare.ToLowerInvariant()] = vcard;
                }
            }
            catch (Exception e)
            {
                throw new JabberException("Could not save VCard.", e);
            }
        }

        public virtual Vcard GetVCard(Jid jid, string id = "")
        {
            if (jid == null) throw new ArgumentNullException("jid");

            try
            {
                var bareJid = jid.Bare.ToLowerInvariant();
                var vcardStr = ExecuteScalar<string>(new SqlQuery("jabber_vcard").Select("vcard").Where("jid", bareJid));
                lock (vcardsCache)
                {
                    if (!vcardsCache.ContainsKey(bareJid))
                    {
                        vcardsCache[bareJid] = !string.IsNullOrEmpty(vcardStr) ? ElementSerializer.DeSerializeElement<Vcard>(vcardStr) : null;
                    }
                    return vcardsCache[bareJid];
                }
            }
            catch (Exception e)
            {
                throw new JabberException("Could not get VCard.", e);
            }
        }

        public virtual ICollection<Vcard> Search(Vcard pattern)
        {
            return new Vcard[0];
        }
    }
}
