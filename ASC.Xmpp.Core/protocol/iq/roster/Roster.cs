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

namespace ASC.Xmpp.Core.protocol.iq.roster
{
    /// <summary>
    ///   Zusammenfassung f�r Roster.
    /// </summary>
    public class Roster : Element
    {
        // Request Roster:
        // <iq id='someid' to='myjabber.net' type='get'>
        //		<query xmlns='jabber:iq:roster'/>
        // </iq>
        public Roster()
        {
            TagName = "query";
            Namespace = Uri.IQ_ROSTER;
        }

        public RosterItem[] GetRoster()
        {
            ElementList nl = SelectElements(typeof(RosterItem));
            int i = 0;
            var result = new RosterItem[nl.Count];
            foreach (RosterItem ri in nl)
            {
                result[i] = ri;
                i++;
            }
            return result;
        }

        public void AddRosterItem(RosterItem r)
        {
            ChildNodes.Add(r);
        }
    }
}