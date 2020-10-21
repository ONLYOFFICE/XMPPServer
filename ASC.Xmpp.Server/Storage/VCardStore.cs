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
using System.Configuration;
using ASC.Xmpp.protocol.iq.vcard;
using ASC.Xmpp.Server.storage.Interface;

namespace ASC.Xmpp.Server.storage
{
	class VCardStore : DbStoreBase, IVCardStore
	{
		private IDictionary<string, Vcard> vcardsCache = new Dictionary<string, Vcard>();

		protected override string[] CreateSchemaScript
		{
			get
			{
				return new[]{
					"create table VCard(UserName TEXT NOT NULL primary key, VCard TEXT)"
				};
			}
		}

		protected override string[] DropSchemaScript
		{
			get
			{
				return new[]{
					"drop table VCard"
				};
			}
		}

		public VCardStore(ConnectionStringSettings connectionSettings)
			: base(connectionSettings)
		{
			InitializeDbSchema(false);
		}

		public VCardStore(string provider, string connectionString)
			: base(provider, connectionString)
		{
			InitializeDbSchema(false);
		}

		#region IVCardStore Members

		public void SetVCard(string user, Vcard vcard)
		{
			if (string.IsNullOrEmpty(user)) throw new ArgumentNullException("user");
			if (vcard == null) throw new ArgumentNullException("vcard");

			lock (vcardsCache)
			{
				using (var connect = GetDbConnection())
				using (var command = connect.CreateCommand())
				{
					command.CommandText = "insert or replace into VCard(UserName, VCard) values (@userName, @vCard)";
					AddCommandParameter(command, "userName", user);
					AddCommandParameter(command, "vCard", ElementSerializer.SerializeElement(vcard));
					command.ExecuteNonQuery();
				}
				vcardsCache[user] = vcard;
			}
		}

		public Vcard GetVCard(string user)
		{
			if (string.IsNullOrEmpty(user)) throw new ArgumentNullException("user");

			lock (vcardsCache)
			{
				if (!vcardsCache.ContainsKey(user))
				{
					using (var connect = GetDbConnection())
					using (var command = connect.CreateCommand())
					{
						command.CommandText = "select VCard from VCard where UserName = @userName";
						AddCommandParameter(command, "userName", user);
						var vcardStr = command.ExecuteScalar() as string;
						vcardsCache[user] = !string.IsNullOrEmpty(vcardStr) ? ElementSerializer.DeSerializeElement<Vcard>(vcardStr) : null;
					}
				}
				return vcardsCache[user];
			}
		}

		#endregion
	}
}
