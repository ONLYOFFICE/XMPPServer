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

using ASC.Xmpp.Core.protocol.sasl;

#endregion

namespace ASC.Xmpp.Core.authorization
{
    /// <summary>
    /// </summary>
    /// <param name="sender"> </param>
    /// <param name="args"> </param>
    public delegate void SaslEventHandler(object sender, SaslEventArgs args);

    /// <summary>
    /// </summary>
    public class SaslEventArgs
    {
        #region Members

        /// <summary>
        /// </summary>
        private bool m_Auto = true;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        public SaslEventArgs()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="mechanisms"> </param>
        public SaslEventArgs(Mechanisms mechanisms)
        {
            Mechanisms = mechanisms;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Set Auto to true if the library should choose the mechanism Set it to false for choosing the authentication method yourself
        /// </summary>
        public bool Auto
        {
            get { return m_Auto; }

            set { m_Auto = value; }
        }

        /// <summary>
        ///   SASL Mechanism for authentication as string
        /// </summary>
        public string Mechanism { get; set; }

        /// <summary>
        /// </summary>
        public Mechanisms Mechanisms { get; set; }

        #endregion

        // by default the library chooses the auth method
    }
}