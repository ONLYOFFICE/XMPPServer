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

namespace ASC.Xmpp.Core.utils.Xml.Dom
{
    /// <summary>
    ///   Summary description for Comment.
    /// </summary>
    public class Comment : Node
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Comment()
        {
            NodeType = NodeType.Comment;
        }

        /// <summary>
        /// </summary>
        /// <param name="text"> </param>
        public Comment(string text) : this()
        {
            Value = text;
        }

        #endregion
    }
}