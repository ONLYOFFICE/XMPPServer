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
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

using ASC.Common.Logging;
using ASC.Common.Module;
using ASC.Core;

namespace ASC.Jabber.Service
{
    class Program : ServiceBase
    {
        private static readonly ILog log;
        IServiceController jabberService;

        static Program()
        {
            log = LogManager.GetLogger("ASC.JabberSvc");
        }

        static void Main(string[] args)
        {
            // Catch a background thread exception 
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                log.ErrorFormat("Fatal Exception : " + Environment.NewLine +
                                e.ExceptionObject);

                if (!WorkContext.IsMono)
                {
                    EventLog.WriteEntry("Jabber Service",
                        "Fatal Exception : " + Environment.NewLine +
                        e.ExceptionObject, EventLogEntryType.Error);
                }
            };

            var program = new Program();
            if (Environment.UserInteractive || args.Contains("--console") || args.Contains("-c"))
            {
                program.OnStart(args);

                Console.WriteLine("\r\nPress any key to stop...\r\n");
                Console.ReadKey();

                program.OnStop();
            }
            else
            {
                Run(program);
            }
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                // start all services from config or start only one service from parameter -t ServiceType
                if (args.Length == 0)
                {
                    args = Environment.GetCommandLineArgs();
                }

                var serviceType = string.Empty;
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-s" || args[i] == "--service")
                    {
                        if (string.IsNullOrEmpty(args[i + 1]))
                        {
                            throw new ArgumentNullException("--service", "Type of service not found.");
                        }
                        serviceType = args[i + 1].Trim().Trim('"');
                    }
                }
            }
            catch (Exception error)
            {
                log.ErrorFormat("Can not start services: {0}", error);
                ExitCode = 1064; // An exception occurred in the service when handling the control request
                throw;
            }

            jabberService = (IServiceController)Activator.CreateInstance(Type.GetType("ASC.Xmpp.Host.XmppServerLauncher, ASC.Xmpp.Host", true));

            try
            {
                jabberService.Start();
                log.InfoFormat("Jabber service started.");
            }
            catch (Exception error)
            {
                log.ErrorFormat("Can not start jabber service {0}", error);
                ExitCode = 1064; // An exception occurred in the service when handling the control request
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                jabberService.Stop();
                log.InfoFormat("Jabber service stopped.");
            }
            catch (Exception error)
            {
                log.ErrorFormat("Can not stop jabber service", error);
                ExitCode = 1064; // An exception occurred in the service when handling the control request
                throw;
            }

        }
    }
}
