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

using System.IO;
using System.Text;

#endregion

namespace ASC.Xmpp.Core.IO
{
    /// <summary>
    ///   This class is inherited from the StringWriter Class The standard StringWriter class supports no encoding With this Class we can set the Encoding of a StringWriter in the Constructor
    /// </summary>
    public class StringWriterWithEncoding : StringWriter
    {
        #region Members

        /// <summary>
        /// </summary>
        private readonly Encoding m_Encoding;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="encoding"> </param>
        public StringWriterWithEncoding(Encoding encoding)
        {
            m_Encoding = encoding;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public override Encoding Encoding
        {
            get { return m_Encoding; }
        }

        #endregion
    }
}