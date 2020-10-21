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

using System;

namespace ASC.Xmpp.Core.utils.Idn
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class StringprepException : Exception
    {
        #region Members

        /// <summary>
        /// </summary>
        public static string BIDI_BOTHRAL = "Contains both R and AL code points.";

        /// <summary>
        /// </summary>
        public static string BIDI_LTRAL = "Leading and trailing code points not both R or AL.";

        /// <summary>
        /// </summary>
        public static string CONTAINS_PROHIBITED = "Contains prohibited code points.";

        /// <summary>
        /// </summary>
        public static string CONTAINS_UNASSIGNED = "Contains unassigned code points.";

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="message"> </param>
        public StringprepException(string message) : base(message)
        {
        }

        #endregion
    }
}