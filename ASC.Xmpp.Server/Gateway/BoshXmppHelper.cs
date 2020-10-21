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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using ASC.Common.Logging;
using ASC.Xmpp.Core.protocol.extensions.bosh;
using ASC.Xmpp.Core.utils;

namespace ASC.Xmpp.Server.Gateway
{
    static class BoshXmppHelper
    {
        private static readonly ILog log = LogManager.GetLogger("ASC");

        public static Body ReadBodyFromRequest(HttpListenerContext ctx)
        {
            if (ctx == null) throw new ArgumentNullException("ctx");

            try
            {
                if (!ctx.Request.HasEntityBody) return null;

                string body;
                using (var streamReader = new StreamReader(ctx.Request.InputStream))
                {
                    body = streamReader.ReadToEnd();
                }

                return ElementSerializer.DeSerializeElement<Body>(body);
            }
            catch (Exception e)
            {
                if (e is HttpListenerException || e is ObjectDisposedException)
                {
                    // ignore
                }
                else
                {
                    log.ErrorFormat("Error read body from request: {0}", e);
                }
            }
            return null;
        }

        public static void TerminateBoshSession(HttpListenerContext ctx, string condition)
        {
            TerminateBoshSession(ctx, null, condition);
        }

        public static void TerminateBoshSession(HttpListenerContext ctx, Body body)
        {
            TerminateBoshSession(ctx, body, null);
        }

        private static void TerminateBoshSession(HttpListenerContext ctx, Body body, string condition)
        {
            if (ctx == null || ctx.Response == null) return;

            if (body == null) body = new Body();
            try
            {
                body.Type = BoshType.terminate;
                if (!string.IsNullOrEmpty(condition)) body.SetAttribute("condition", condition);

                SendAndCloseResponse(ctx, body);
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error TerminateBoshSession body: {0}\r\n{1}", body, e);
            }
        }

        public static void SendAndCloseResponse(HttpListenerContext ctx, Body body)
        {
            var response = ctx.Response;
            try
            {
                var text = body.ToString();

                response.ContentType = "text/xml; charset=utf-8";
                var buffer = Encoding.UTF8.GetBytes(text);

                var headerValues = ctx.Request.Headers.GetValues("Accept-Encoding");
                if (headerValues != null && headerValues.Contains("gzip", StringComparer.InvariantCultureIgnoreCase))
                {
                    response.AddHeader("Content-Encoding", "gzip");
                    using (var ms = new MemoryStream())
                    {
                        using (var gzip = new GZipStream(ms, CompressionMode.Compress))
                        {
                            gzip.Write(buffer, 0, buffer.Length);
                        }
                        buffer = ms.ToArray();
                    }
                }

                response.ContentLength64 = buffer.Length;
                response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache");
                response.Headers.Add(HttpResponseHeader.Vary, "*");
                response.Headers.Add(HttpResponseHeader.Pragma, "no-cache");
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Flush();
            }
            finally
            {
                try
                {
                    response.Close();
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}