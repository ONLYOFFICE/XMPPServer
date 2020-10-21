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

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Server
{
    public static class SignalRHelper
    {
        private const byte NO_STATE = 0;
        public const byte USER_ONLINE = 1;
        private const byte USER_AWAY = 2;
        private const byte USER_NOT_AVAILVABLE = 3;
        public const byte USER_OFFLINE = 4;
        public const int PRIORITY = 1;
        public const int NUMBER_OF_RECENT_MSGS = 10;
        public const string SIGNALR_RESOURCE = "SignalR";

        public static ShowType GetShowType(byte state)
        {
            switch (state)
            {
                case USER_ONLINE:
                    return ShowType.NONE;

                case USER_AWAY:
                    return ShowType.away;

                case USER_NOT_AVAILVABLE:
                    return ShowType.xa;

                default:
                    return ShowType.NONE;
            }
        }

        public static PresenceType GetPresenceType(byte state)
        {
            switch (state)
            {
                case USER_OFFLINE:
                case NO_STATE:
                    return PresenceType.unavailable;
                default:
                    return PresenceType.available;
            }
        }

        public static byte GetState(ShowType show, PresenceType type)
        {
            switch (show)
            {
                case ShowType.NONE:
                    switch (type)
                    {
                        case PresenceType.unavailable:
                            return USER_OFFLINE;
                        default:
                            return USER_ONLINE;
                    }
                case ShowType.chat:
                    return USER_ONLINE;
                case ShowType.away:
                    return USER_AWAY;
                case ShowType.dnd:
                case ShowType.xa:
                    return USER_NOT_AVAILVABLE;
                default:
                    return NO_STATE;
            }
        }
    }
}
