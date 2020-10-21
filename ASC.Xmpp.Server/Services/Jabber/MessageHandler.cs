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
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web.Script.Serialization;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Core.Jabber;
using ASC.Web.Core.Users;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Storage.Interface;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Jabber
{
	[XmppHandler(typeof(Message))]
	class MessageHandler : XmppStanzaHandler
	{
        private DbPushStore pushStore;

        private static readonly ILog log = LogManager.GetLogger("ASC");
        
		public override void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context)
		{
            
			if (!message.HasTo || message.To.IsServer)
			{
				context.Sender.SendTo(stream, XmppStanzaError.ToServiceUnavailable(message));
				return;
			}

			var sessions = context.SessionManager.GetBareJidSessions(message.To);
            if (0 < sessions.Count)
            {
                foreach(var s in sessions)
                {
                    try
                    {
                        context.Sender.SendTo(s, message);
                    }
                    catch
                    {
                        context.Sender.SendToAndClose(s.Stream, message);
                    }
                }
            }
            else
            {
                pushStore = new DbPushStore();
                var properties = new Dictionary<string, string>(1);
                properties.Add("connectionStringName", "default");
                pushStore.Configure(properties);

                if (message.HasTag("active"))
                {
                    var fromFullName = message.HasAttribute("username") ? 
                                        message.GetAttribute("username") : message.From.ToString();


                    var messageToDomain = message.To.ToString().Split(new char[] { '@' })[1];
                    var tenant = CoreContext.TenantManager.GetTenant(messageToDomain);
                    var tenantId = tenant != null ? tenant.TenantId : -1;

                    var userPushList = new List<UserPushInfo>();
                    userPushList = pushStore.GetUserEndpoint(message.To.ToString().Split(new char[] { '@' })[0]);

                    var firebaseAuthorization = "";
                    try
                    {
                        CallContext.SetData(TenantManager.CURRENT_TENANT, new Tenant(tenantId, ""));
                        firebaseAuthorization = FireBase.Instance.Authorization;
                    }
                    catch (Exception exp)
                    {
                        log.DebugFormat("firebaseAuthorizationERROR: {0}", exp);
                    }
                    foreach (var user in userPushList)
                    {
                        try
                        {
                            var from = message.From.ToString().Split(new char[] {'@'})[0];
                            List<string> userId;
                            string photoPath = "";
                            using (var db = new DbManager("core"))
                            using (var command = db.Connection.CreateCommand())
                            {
                                var q = new SqlQuery("core_user").Select("id").Where(Exp.Like("username", from)).Where("tenant", tenantId);

                                userId = command.ExecuteList(q, DbRegistry.GetSqlDialect(db.DatabaseId))
                                    .ConvertAll(r => Convert.ToString(r[0]))
                                    .ToList();
                            }
                            if (userId.Count != 0)
                            {
                                var guid = new Guid(userId[0]);
                                photoPath = UserPhotoManager.GetBigPhotoURL(guid);
                            }
                           
                            var tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                            tRequest.Method = "post";
                            tRequest.ContentType = "application/json";
                            var data = new
                            {
                                to = user.endpoint,
                                data = new
                                {
                                    msg = message.Body,
                                    fromFullName = fromFullName,
                                    photoPath = photoPath
                                }    
                            };       
                            var serializer = new JavaScriptSerializer();
                            var json = serializer.Serialize(data);
                            var byteArray = Encoding.UTF8.GetBytes(json);
                            tRequest.Headers.Add(string.Format("Authorization: key={0}", firebaseAuthorization));
                            tRequest.ContentLength = byteArray.Length; 
                            using (var dataStream = tRequest.GetRequestStream())
                            {
                                dataStream.Write(byteArray, 0, byteArray.Length);   
                                using (var tResponse = tRequest.GetResponse())
                                {
                                    using (var dataStreamResponse = tResponse.GetResponseStream())
                                    {
                                        using (var tReader = new StreamReader(dataStreamResponse))
                                        {
                                            var sResponseFromServer = tReader.ReadToEnd();
                                            var str = sResponseFromServer;
                                        }    
                                    }    
                                }    
                            }    
                        }        
                        catch (Exception ex)
                        {
                            var str = ex.Message;
                            log.DebugFormat("PushRequestERROR: {0}", str);
                        }          
                    }
                }
                StoreOffline(message, context.StorageManager.OfflineStorage);
            }
		}

		private void StoreOffline(Message message, IOfflineStore offlineStore)
		{
			if ((message.Type == MessageType.normal || message.Type == MessageType.chat) && !string.IsNullOrEmpty(message.To.User))
			{
				offlineStore.SaveOfflineMessages(message);
			}
		}
	}
}