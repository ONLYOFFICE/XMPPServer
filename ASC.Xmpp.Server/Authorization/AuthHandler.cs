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

using System.Collections.Generic;

using ASC.ActiveDirectory.Base.Settings;
using ASC.ActiveDirectory.Novell;
using ASC.Common.Logging;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.sasl;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Users;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Authorization
{
    [XmppHandler(typeof(Auth))]
    [XmppHandler(typeof(Response))]
    [XmppHandler(typeof(Abort))]
    class AuthHandler : XmppStreamHandler
    {
        private readonly ILog log = LogManager.GetLogger("ASC");
        private readonly IDictionary<string, AuthData> authData = new Dictionary<string, AuthData>();

        public override void StreamEndHandle(XmppStream stream, ICollection<Node> notSendedBuffer, XmppHandlerContext context)
        {
            lock (authData)
            {
                authData.Remove(stream.Id);
            }
        }

        public override void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
        {
            if (stream.Authenticated) return;

            if (element is Auth) ProcessAuth(stream, (Auth)element, context);
            if (element is Response) ProcessResponse(stream, (Response)element, context);
            if (element is Abort) ProcessAbort(stream, (Abort)element, context);
        }

        private void ProcessAuth(XmppStream stream, Auth auth, XmppHandlerContext context)
        {
            AuthData authStep;
            lock (authData)
            {
                authData.TryGetValue(stream.Id, out authStep);
            }

            if (auth.MechanismType == MechanismType.DIGEST_MD5)
            {
                if (authStep != null)
                {
                    context.Sender.SendToAndClose(stream, XmppFailureError.TemporaryAuthFailure);
                }
                else
                {
                    lock (authData)
                    {
                        authData[stream.Id] = new AuthData();
                    }
                    var challenge = GetChallenge(stream.Domain);
                    context.Sender.SendTo(stream, challenge);
                }
            }
            else if (auth.MechanismType == MechanismType.PLAIN)
            {
                if (auth.TextBase64 == null)
                {
                    context.Sender.SendToAndClose(stream, XmppFailureError.TemporaryAuthFailure);
                }
                else
                {
                    string[] array = auth.TextBase64.Split('\0');
                    if (array.Length == 3)
                    {
                        string userName = array[1];
                        string password = array[2];
                        bool isAuth = false;

                        User user = context.UserManager.GetUser(new Jid(userName, stream.Domain, null));


                        if (user != null)
                        {
                            if (user.Sid != null)
                            {
                                var settings = LdapSettings.Load();

                                if (settings.EnableLdapAuthentication)
                                {
                                    using (var ldapHelper = new NovellLdapHelper(settings))
                                    {
                                        var ldapUser = ldapHelper.GetUserBySid(user.Sid);

                                        if (ldapUser != null)
                                        {
                                            ldapHelper.CheckCredentials(ldapUser.DistinguishedName, password,
                                                settings.Server,
                                                settings.PortNumber, settings.StartTls, settings.Ssl,
                                                settings.AcceptCertificate, settings.AcceptCertificateHash);

                                            // ldap user
                                            isAuth = true;
                                        }
                                    }
                                }
                            }
                        }
                        if (isAuth)
                        {
                            log.DebugFormat("User {0} authorized, Domain = {1}", userName, stream.Domain);
                            context.Sender.ResetStream(stream);
                            stream.Authenticate(userName);
                            context.Sender.SendTo(stream, new Success());
                        }
                        else
                        {
                            log.DebugFormat("User {0} not authorized, Domain = {1}", userName, stream.Domain);
                            context.Sender.SendToAndClose(stream, XmppFailureError.NotAuthorized);
                        }
                    }
                    else
                    {
                        context.Sender.SendToAndClose(stream, XmppFailureError.TemporaryAuthFailure);
                    }
                }
            }
            else
            {
                context.Sender.SendToAndClose(stream, XmppFailureError.InvalidMechanism);
            }
        }

        private void ProcessResponse(XmppStream stream, Response response, XmppHandlerContext context)
        {
            AuthData authStep;
            lock (authData)
            {
                authData.TryGetValue(stream.Id, out authStep);
            }

            if (authStep == null)
            {
                context.Sender.SendToAndClose(stream, XmppFailureError.TemporaryAuthFailure);
                return;
            }

            if (authStep.Step == AuthStep.Step1)
            {
                context.Sender.SendToAndClose(stream, XmppFailureError.NotAuthorized);
            }
            else if (authStep.Step == AuthStep.Step2)
            {
                var success = ProcessStep2(stream, context);
                context.Sender.SendTo(stream, success);
            }
            else
            {
                context.Sender.SendToAndClose(stream, XmppFailureError.TemporaryAuthFailure);
            }
        }

        private void ProcessAbort(XmppStream stream, Abort abort, XmppHandlerContext context)
        {
            context.Sender.SendToAndClose(stream, XmppFailureError.Aborted);
        }

        private Challenge GetChallenge(string domain)
        {
            var challenge = new Challenge();
            challenge.TextBase64 = string.Format("realm=\"{0}\",nonce=\"{1}\",qop=\"auth\",charset=utf-8,algorithm=md5-sess", domain, UniqueId.CreateNewId());
            return challenge;
        }


        private Success ProcessStep2(XmppStream stream, XmppHandlerContext ctx)
        {
            lock (authData)
            {
                stream.Authenticate(authData[stream.Id].UserName);
                authData.Remove(stream.Id);
            }
            ctx.Sender.ResetStream(stream);
            return new Success();
        }

        private enum AuthStep
        {
            Step1,
            Step2
        }

        private class AuthData
        {
            public string UserName
            {
                get;
                set;
            }

            public AuthStep Step
            {
                get;
                private set;
            }

            public void DoStep()
            {
                Step++;
            }
        }
    }
}