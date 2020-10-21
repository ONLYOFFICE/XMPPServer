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
using System.Net;
using ASC.Common.Logging;

namespace ASC.Xmpp.Server.Gateway
{
    class BoshXmppListener : XmppListenerBase
    {
        private const string DEFAULT_BIND_URL = "http://*:5280/http-poll/";

        private readonly HttpListener httpListener = new HttpListener();

        private Uri bindUri;
        private long maxPacket = 1048576; //1024 kb

        private static readonly ILog log = LogManager.GetLogger("ASC");


        public override void Configure(IDictionary<string, string> properties)
        {
            try
            {
                string hostname = Dns.GetHostName().ToLowerInvariant();

                string bindPrefix = properties.ContainsKey("bind") ? properties["bind"] : DEFAULT_BIND_URL;
                bindUri = new Uri(bindPrefix.Replace("*", hostname));

                httpListener.Prefixes.Add(bindPrefix);

                log.InfoFormat("Configure listener {0} on {1}", Name, bindPrefix);

                if (properties.ContainsKey("maxpacket"))
                {
                    var value = 0L;
                    if (long.TryParse(properties["maxpacket"], out value)) maxPacket = value;
                }
            }
            catch (Exception e)
            {
                log.DebugFormat("Error configure listener {0}: {1}", Name, e);
                throw;
            }
        }

        protected override void DoStart()
        {
            httpListener.Start();
            BeginGetContext();
        }

        protected override void DoStop()
        {
            httpListener.Stop();
        }

        private void BeginGetContext()
        {
            if (httpListener != null && httpListener.IsListening)
            {
                httpListener.BeginGetContext(GetContextCallback, null);
            }
        }

        private void GetContextCallback(IAsyncResult asyncResult)
        {
            HttpListenerContext ctx = null;
            try
            {
                try
                {
                    ctx = httpListener.EndGetContext(asyncResult);
                }
                finally
                {
                    BeginGetContext();
                }

                if (maxPacket < ctx.Request.ContentLength64)
                {
                    BoshXmppHelper.TerminateBoshSession(ctx, "request-too-large");
                    return;
                }

                if (ctx.Request.Url.AbsolutePath == bindUri.AbsolutePath)
                {
                    var body = BoshXmppHelper.ReadBodyFromRequest(ctx);
                    if (body == null)
                    {
                        BoshXmppHelper.TerminateBoshSession(ctx, "bad-request");
                        return;
                    }

                    var connection = GetXmppConnection(body.Sid) as BoshXmppConnection;

                    if (!string.IsNullOrEmpty(body.Sid) && connection == null)
                    {
                        BoshXmppHelper.TerminateBoshSession(ctx, "item-not-found");
                        return;
                    }

                    if (connection == null)
                    {
                        connection = new BoshXmppConnection(body);
                        log.DebugFormat("Create new Bosh connection Id = {0}", connection.Id);

                        AddNewXmppConnection(connection);
                    }
                    connection.ProcessBody(body, ctx);
                }
                else
                {
                    BoshXmppHelper.TerminateBoshSession(ctx, "bad-request");
                    log.DebugFormat("{0}: Unknown uri request {1}", Name, ctx.Request.Url);
                }
            }
            catch (Exception e)
            {
                BoshXmppHelper.TerminateBoshSession(ctx, "internal-server-error");
                if (Started && !(e is ObjectDisposedException))
                {
                    log.ErrorFormat("{0}: Error GetContextCallback: {1}", Name, e);
                }
            }
        }
    }
}