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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.x.muc.iq.admin
{

    #region usings

    #endregion

    /*
        <query xmlns='http://jabber.org/protocol/muc#admin'>
            <item nick='pistol' role='none'>
              <reason>Avaunt, you cullion!</reason>
            </item>
        </query>
    */

    /// <summary>
    /// </summary>
    public class Admin : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Admin()
        {
            TagName = "query";
            Namespace = Uri.MUC_ADMIN;
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="item"> </param>
        public void AddItem(Item item)
        {
            AddChild(item);
        }

        /// <summary>
        /// </summary>
        /// <param name="items"> </param>
        public void AddItems(Item[] items)
        {
            foreach (Item itm in items)
            {
                AddItem(itm);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public Item[] GetItems()
        {
            ElementList nl = SelectElements(typeof(Item));
            var items = new Item[nl.Count];
            int i = 0;
            foreach (Item itm in nl)
            {
                items[i] = itm;
                i++;
            }

            return items;
        }

        #endregion
    }
}