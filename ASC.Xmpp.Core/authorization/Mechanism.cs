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

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.authorization
{
    /// <summary>
    ///   Summary description for Mechanism.
    /// </summary>
    public abstract class Mechanism
    {
        #region Members

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// </summary>
        public string Username
        { // lower case that until i implement our c# port of libIDN
            get; set;
        }

        //public XmppClientConnection XmppClientConnection { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <summary>
        /// </summary>
        /// <param name="con"> </param>
        public abstract void Init();

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public abstract void Parse(Node e);

        #endregion
    }
}