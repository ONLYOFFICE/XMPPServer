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

namespace ASC.Xmpp.Core.IO.Compression
{
    /// <summary>
    ///   SharpZipBaseException is the base exception class for the SharpZipLibrary. All library exceptions are derived from this.
    /// </summary>
    public class SharpZipBaseException : ApplicationException
    {
        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the SharpZipLibraryException class.
        /// </summary>
        public SharpZipBaseException()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the SharpZipLibraryException class with a specified error message.
        /// </summary>
        /// <param name="msg"> </param>
        public SharpZipBaseException(string msg) : base(msg)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the SharpZipLibraryException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message"> Error message string </param>
        /// <param name="innerException"> The inner exception </param>
        public SharpZipBaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }
}