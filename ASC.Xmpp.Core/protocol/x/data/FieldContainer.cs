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

    /// <summary>
    ///   Bass class for all xdata classes that contain xData fields
    /// </summary>
    public abstract class FieldContainer : Element
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public Field AddField()
        {
            var f = new Field();
            AddChild(f);
            return f;
        }

        /// <summary>
        /// </summary>
        /// <param name="field"> </param>
        public Field AddField(Field field)
        {
            AddChild(field);
            return field;
        }

        /// <summary>
        ///   Retrieve a field with the given "var"
        /// </summary>
        /// <param name="var"> </param>
        /// <returns> </returns>
        public Field GetField(string var)
        {
            ElementList nl = SelectElements(typeof(Field));
            foreach (Element e in nl)
            {
                var f = e as Field;
                if (f.Var == var)
                {
                    return f;
                }
            }

            return null;
        }

        /// <summary>
        ///   Gets a list of all form fields
        /// </summary>
        /// <returns> </returns>
        public Field[] GetFields()
        {
            ElementList nl = SelectElements(typeof(Field));
            var fields = new Field[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                fields[i] = (Field)e;
                i++;
            }

            return fields;
        }

        #endregion
    }
}