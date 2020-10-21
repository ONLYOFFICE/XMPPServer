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
using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Storage.Interface;
using System;
using System.Data;

namespace ASC.Xmpp.Server.Storage
{
    public class DbPrivateStore : DbStoreBase, IPrivateStore
    {
        protected override SqlCreate[] GetCreateSchemaScript()
        {
            var t1 = new SqlCreate.Table("jabber_private", true)
                .AddColumn("jid", DbType.String, 255, true)
                .AddColumn("tag", DbType.String, 255, true)
                .AddColumn("namespace", DbType.String, 255, true)
                .AddColumn("element", DbType.String, UInt16.MaxValue)
                .PrimaryKey("jid", "tag", "namespace");
            return new[] { t1 };
        }

        
        #region IPrivateStore Members

        public Element GetPrivate(Jid jid, Element element)
        {
            CheckArgs(jid, element);

            var elementStr = ExecuteScalar<string>(new SqlQuery("jabber_private").Select("element").Where("jid", jid.Bare).Where("tag", element.TagName).Where("namespace", element.Namespace));
            return !string.IsNullOrEmpty(elementStr) ? ElementSerializer.DeSerializeElement<Element>(elementStr) : null;
        }

        public void SetPrivate(Jid jid, Element element)
        {
            CheckArgs(jid, element);

            ExecuteNonQuery(
                new SqlInsert("jabber_private", true)
                .InColumnValue("jid", jid.Bare)
                .InColumnValue("tag", element.TagName)
                .InColumnValue("namespace", element.Namespace)
                .InColumnValue("element", ElementSerializer.SerializeElement(element)));
        }

        #endregion

        private void CheckArgs(Jid jid, Element element)
        {
            if (jid == null) throw new ArgumentNullException("jid");
            if (element == null) throw new ArgumentNullException("element");
            if (string.IsNullOrEmpty(element.TagName)) throw new ArgumentNullException("element.TagName");
            if (string.IsNullOrEmpty(element.Namespace)) throw new ArgumentNullException("element.Namespace");
        }
    }
}