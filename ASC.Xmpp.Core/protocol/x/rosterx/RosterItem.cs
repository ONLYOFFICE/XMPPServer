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

namespace ASC.Xmpp.Core.protocol.x.rosterx
{
    /// <summary>
    /// </summary>
    public enum Action
    {
        /// <summary>
        /// </summary>
        NONE = -1,

        /// <summary>
        /// </summary>
        add,

        /// <summary>
        /// </summary>
        remove,

        /// <summary>
        /// </summary>
        modify
    }

    /// <summary>
    ///   Summary description for RosterItem.
    /// </summary>
    public class RosterItem : Base.RosterItem
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public RosterItem()
        {
            Namespace = Uri.X_ROSTERX;
        }

        /// <summary>
        /// </summary>
        /// <param name="jid"> </param>
        public RosterItem(Jid jid) : this()
        {
            Jid = jid;
        }

        /// <summary>
        /// </summary>
        /// <param name="jid"> </param>
        /// <param name="name"> </param>
        public RosterItem(Jid jid, string name) : this(jid)
        {
            Name = name;
        }

        /// <summary>
        /// </summary>
        /// <param name="jid"> </param>
        /// <param name="name"> </param>
        /// <param name="action"> </param>
        public RosterItem(Jid jid, string name, Action action) : this(jid, name)
        {
            Action = action;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public Action Action
        {
            get { return (Action) GetAttributeEnum("action", typeof (Action)); }

            set { SetAttribute("action", value.ToString()); }
        }

        #endregion

        /*
		<item action='delete' jid='rosencrantz@denmark' name='Rosencrantz'>   
			<group>Visitors</group>   
		</item> 
		*/
    }
}