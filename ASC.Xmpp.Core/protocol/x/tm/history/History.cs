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

#region using

using System;
using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.tm.history
{
    public class History : Element
    {
        public History()
        {
            TagName = "query";
            Namespace = Uri.X_TM_IQ_HISTORY;
        }

        public DateTime From
        {
            get { return Time.Date(GetAttribute("from")); }
            set { SetAttribute("from", Time.Date(value)); }
        }

        public DateTime To
        {
            get
            {
                DateTime to = Time.Date(GetAttribute("to"));
                return to != DateTime.MinValue ? to : DateTime.MaxValue;
            }
            set { SetAttribute("to", Time.Date(value)); }
        }

        public int StartIndex
        {
            get { return GetAttributeInt("startindex"); }
            set { SetAttribute("startindex", value); }
        }

        public int Count
        {
            get { return GetAttributeInt("count"); }
            set { SetAttribute("count", value); }
        }

        public string Text
        {
            get { return GetAttribute("text"); }
            set { SetAttribute("text", value); }
        }
    }
}