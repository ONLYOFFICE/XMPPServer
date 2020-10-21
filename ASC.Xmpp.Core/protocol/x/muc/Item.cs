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

namespace ASC.Xmpp.Core.protocol.x.muc
{
    /// <summary>
    ///   Summary description for Item.
    /// </summary>
    public class Item : Base.Item
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Item()
        {
            TagName = "item";
            Namespace = Uri.MUC_USER;
        }

        /// <summary>
        /// </summary>
        /// <param name="affiliation"> </param>
        public Item(Affiliation affiliation) : this()
        {
            Affiliation = affiliation;
        }

        /// <summary>
        /// </summary>
        /// <param name="role"> </param>
        public Item(Role role) : this()
        {
            Role = role;
        }

        /// <summary>
        /// </summary>
        /// <param name="affiliation"> </param>
        /// <param name="role"> </param>
        public Item(Affiliation affiliation, Role role) : this(affiliation)
        {
            Role = role;
        }

        /// <summary>
        /// </summary>
        /// <param name="affiliation"> </param>
        /// <param name="role"> </param>
        /// <param name="reason"> </param>
        public Item(Affiliation affiliation, Role role, string reason) : this(affiliation, role)
        {
            Reason = reason;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public Actor Actor
        {
            get { return SelectSingleElement(typeof (Actor)) as Actor; }

            set
            {
                if (HasTag(typeof (Actor)))
                {
                    RemoveTag(typeof (Actor));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        /// </summary>
        public Affiliation Affiliation
        {
            get { return (Affiliation) GetAttributeEnum("affiliation", typeof (Affiliation)); }

            set { SetAttribute("affiliation", value.ToString()); }
        }

        /// <summary>
        /// </summary>
        public string Nickname
        {
            get { return GetAttribute("nick"); }

            set { SetAttribute("nick", value); }
        }

        /// <summary>
        /// </summary>
        public string Reason
        {
            get { return GetTag("reason"); }

            set { SetTag("reason", value); }
        }

        /// <summary>
        /// </summary>
        public Role Role
        {
            get { return (Role) GetAttributeEnum("role", typeof (Role)); }

            set { SetAttribute("role", value.ToString()); }
        }

        #endregion

        /*
        <x xmlns='http://jabber.org/protocol/muc#user'>
             <item affiliation='admin' role='moderator'/>
        </x>
         
        <item nick='pistol' role='none'>
            <reason>Avaunt, you cullion!</reason>
        </item>
        
        <presence
                from='darkcave@macbeth.shakespeare.lit/thirdwitch'
                to='crone1@shakespeare.lit/desktop'>
                <x xmlns='http://jabber.org/protocol/muc#user'>
                    <item   affiliation='none'
                            jid='hag66@shakespeare.lit/pda'
                            role='participant'/>
                </x>
        </presence>
        */
    }
}