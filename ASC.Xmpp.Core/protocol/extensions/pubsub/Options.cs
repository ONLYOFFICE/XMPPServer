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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    /*
        <xs:element name='options'>
            <xs:complexType>
              <xs:sequence minOccurs='0'>
                <xs:any namespace='jabber:x:data'/>
              </xs:sequence>
              <xs:attribute name='jid' type='xs:string' use='required'/>
              <xs:attribute name='node' type='xs:string' use='optional'/>
              <xs:attribute name='subid' type='xs:string' use='optional'/>
            </xs:complexType>
        </xs:element>
     
        <iq type='get'
            from='francisco@denmark.lit/barracks'
            to='pubsub.shakespeare.lit'
            id='options1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub'>
            <options node='blogs/princely_musings' jid='francisco@denmark.lit'/>
          </pubsub>
        </iq>
    */

    public class Options : Element
    {
        #region << Constructors >>

        public Options()
        {
            TagName = "options";
            Namespace = Uri.PUBSUB;
        }

        public Options(Jid jid) : this()
        {
            Jid = jid;
        }

        public Options(Jid jid, string node) : this(jid)
        {
            Node = node;
        }

        public Options(Jid jid, string node, string subId) : this(jid, node)
        {
            SubId = subId;
        }

        #endregion

        public Jid Jid
        {
            get
            {
                if (HasAttribute("jid"))
                    return new Jid(GetAttribute("jid"));
                else
                    return null;
            }
            set
            {
                if (value != null)
                    SetAttribute("jid", value.ToString());
            }
        }

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        public string SubId
        {
            get { return GetAttribute("subid"); }
            set { SetAttribute("subid", value); }
        }

        /// <summary>
        ///   The X-Data Element/Form
        /// </summary>
        public x.data.Data Data
        {
            get { return SelectSingleElement(typeof(x.data.Data)) as x.data.Data; }
            set
            {
                if (HasTag(typeof(x.data.Data)))
                    RemoveTag(typeof(x.data.Data));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}