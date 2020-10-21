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

using ASC.Xmpp.Core.protocol;

namespace ASC.Xmpp.Server
{
	public static class XmppStreamError
	{
		public static Error BadFormat
		{
			get { return new Error(StreamErrorCondition.BadFormat) { Prefix = Uri.PREFIX }; }
		}

		public static Error Conflict
		{
			get { return new Error(StreamErrorCondition.Conflict) { Prefix = Uri.PREFIX }; }
		}


        public static Error HostGone
        {
            get { return new Error(StreamErrorCondition.HostGone) { Prefix = Uri.PREFIX }; }
        }

		public static Error HostUnknown
		{
			get { return new Error(StreamErrorCondition.HostUnknown) { Prefix = Uri.PREFIX }; }
		}

		public static Error BadNamespacePrefix
		{
			get { return new Error(StreamErrorCondition.BadNamespacePrefix) { Prefix = Uri.PREFIX }; }
		}

		public static Error NotAuthorized
		{
			get { return new Error(StreamErrorCondition.NotAuthorized) { Prefix = Uri.PREFIX }; }
		}

		public static Error InvalidFrom
		{
			get { return new Error(StreamErrorCondition.InvalidFrom) { Prefix = Uri.PREFIX }; }
		}

		public static Error ImproperAddressing
		{
			get { return new Error(StreamErrorCondition.ImproperAddressing) { Prefix = Uri.PREFIX }; }
		}

		public static Error InternalServerError
		{
			get { return new Error(StreamErrorCondition.InternalServerError) { Prefix = Uri.PREFIX }; }
		}
	}
}
