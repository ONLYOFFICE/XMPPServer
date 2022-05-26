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

namespace ASC.Xmpp.Core.protocol.x.data
{

    #region usings

    #endregion

    /*
	<x xmlns='jabber:x:data'
		type='{form-type}'>
		<title/>
		<instructions/>
		<field var='field-name'
				type='{field-type}'
				label='description'>
			<desc/>
			<required/>
			<value>field-value</value>
			<option label='option-label'><value>option-value</value></option>
			<option label='option-label'><value>option-value</value></option>
		</field>
	</x>
	
	
	<xs:element name='option'>
    <xs:complexType>
      <xs:sequence>
        <xs:element ref='value'/>
      </xs:sequence>
      <xs:attribute name='label' type='xs:string' use='optional'/>
    </xs:complexType>
	</xs:element>
	*/

    /// <summary>
    ///   Field Option.
    /// </summary>
    public class Option : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Option()
        {
            TagName = "option";
            Namespace = Uri.X_DATA;
        }

        /// <summary>
        /// </summary>
        /// <param name="label"> </param>
        /// <param name="val"> </param>
        public Option(string label, string val) : this()
        {
            Label = label;
            SetValue(val);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Label of the option
        /// </summary>
        public string Label
        {
            get { return GetAttribute("label"); }

            set { SetAttribute("label", value); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Value of the Option
        /// </summary>
        /// <returns> </returns>
        public string GetValue()
        {
            return GetTag(typeof(Value));
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        public void SetValue(string val)
        {
            SetTag(typeof(Value), val);
        }

        #endregion
    }
}