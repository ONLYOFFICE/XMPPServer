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

#region Using directives

using ASC.Xmpp.Core.protocol.client;

#endregion

namespace ASC.Xmpp.Core.protocol.component
{
    /// <summary>
    ///   Summary description for Presence.
    /// </summary>
    public class Presence : client.Presence
    {
        #region << Constructors >>

        public Presence()
        {
            Namespace = Uri.ACCEPT;
        }

        public Presence(ShowType show, string status) : this()
        {
            Show = show;
            Status = status;
        }

        public Presence(ShowType show, string status, int priority) : this(show, status)
        {
            Priority = priority;
        }

        #endregion

        /// <summary>
        ///   Error Child Element
        /// </summary>
        public new Error Error
        {
            get { return SelectSingleElement(typeof(Error)) as Error; }
            set
            {
                if (HasTag(typeof(Error)))
                    RemoveTag(typeof(Error));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}