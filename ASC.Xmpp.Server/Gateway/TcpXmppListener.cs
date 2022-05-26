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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

using ASC.Common.Logging;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace ASC.Xmpp.Server.Gateway
{
    class TcpXmppListener : XmppListenerBase
    {
        private IPEndPoint bindEndPoint = new IPEndPoint(IPAddress.Any, 5222);
        private TcpListener tcpListener;

        private static readonly ILog log = LogManager.GetLogger("ASC");
        private long maxPacket = 1048576; //1024 kb

        public X509Certificate2 Certificate { get; private set; }
        public XmppStartTlsOption StartTls { get; private set; }

        public override void Configure(IDictionary<string, string> properties)
        {
            try
            {
                if (properties.ContainsKey("bindPort"))
                {
                    bindEndPoint = new IPEndPoint(IPAddress.Any, int.Parse(properties["bindPort"]));
                }

                Certificate = InitCertificate(properties);
                StartTls = XmppStartTlsOption.None;
                if (properties.ContainsKey("startTls"))
                {
                    StartTls = String.Equals(properties["startTls"], "optional", StringComparison.OrdinalIgnoreCase) ?
                        XmppStartTlsOption.Optional : (String.Equals(properties["startTls"], "required", StringComparison.OrdinalIgnoreCase) ?
                        XmppStartTlsOption.Required : XmppStartTlsOption.None);
                }
                if (StartTls != XmppStartTlsOption.None && Certificate == null)
                {
                    throw new ConfigurationErrorsException("Wrong configuration of TcpXmppListener! StartTls is Optional or Required but Certificate is null.");
                }
                if (properties.ContainsKey("maxpacket"))
                {
                    long.TryParse(properties["maxpacket"], out maxPacket);
                }
                log.InfoFormat("Configure listener '{0}' on {1}", Name, bindEndPoint);
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error configure listener '{0}': {1}", Name, e);
                throw;
            }
        }

        protected override void DoStart()
        {
            tcpListener = new TcpListener(bindEndPoint);
            tcpListener.Start();
            tcpListener.BeginAcceptSocket(BeginAcceptCallback, null);
        }

        protected override void DoStop()
        {
            tcpListener.Stop();
        }

        private X509Certificate2 InitCertificate(IDictionary<string, string> properties)
        {
            if (!CheckPropertyAndFile(properties, "certificate")) return null;

            var result = !properties.ContainsKey("certificatePassword") ?
                new X509Certificate2(properties["certificate"]) :
                new X509Certificate2(properties["certificate"], properties["certificatePassword"]);

            if (!CheckPropertyAndFile(properties, "certificatePrivateKey")) return result;

            using (var reader = File.OpenText(properties["certificatePrivateKey"]))
            {
                var pemObject = new PemReader(reader).ReadObject();

                var rsaPrivateCrtKeyParameters = pemObject as RsaPrivateCrtKeyParameters;
                if (rsaPrivateCrtKeyParameters == null)
                {
                    var keyPair = pemObject as AsymmetricCipherKeyPair;
                    if (keyPair != null)
                    {
                        rsaPrivateCrtKeyParameters = keyPair.Private as RsaPrivateCrtKeyParameters;
                    }
                }

                var privateKey = DotNetUtilities.ToRSA(rsaPrivateCrtKeyParameters);
                if (rsaPrivateCrtKeyParameters != null)
                {
                    result.PrivateKey = privateKey;
                }
            }

            return result;
        }

        private bool CheckPropertyAndFile(IDictionary<string, string> properties, string property)
        {
            return properties.ContainsKey(property) && File.Exists(properties[property]);
        }

        private void BeginAcceptCallback(IAsyncResult asyncResult)
        {
            try
            {
                var socket = tcpListener.EndAcceptSocket(asyncResult);
                if (Started)
                {
                    var nossl = Certificate == null || StartTls != XmppStartTlsOption.None;
                    AddNewXmppConnection(nossl ? new TcpXmppConnection(socket, maxPacket) : new TcpSslXmppConnection(socket, maxPacket, Certificate));
                }
            }
            catch (ObjectDisposedException) { return; }
            catch (Exception e)
            {
                log.ErrorFormat("Error listener '{0}' on AcceptCallback: {1}", Name, e);
            }
            finally
            {
                if (Started)
                {
                    try
                    {
                        tcpListener.BeginAcceptSocket(BeginAcceptCallback, null);
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("Error listener '{0}' on AcceptCallback: {1}", Name, e);
                    }
                }
            }
        }
    }
}
