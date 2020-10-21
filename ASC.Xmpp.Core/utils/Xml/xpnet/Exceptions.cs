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

#endregion

namespace ASC.Xmpp.Core.utils.Xml.xpnet
{

    #region usings

    #endregion

    /// <summary>
    ///   Base class for other exceptions
    /// </summary>
    public class TokenException : Exception
    {
    }

    /// <summary>
    ///   An empty token was detected. This only happens with a buffer of length 0 is passed in to the parser.
    /// </summary>
    public class EmptyTokenException : TokenException
    {
    }

    /// <summary>
    ///   End of prolog.
    /// </summary>
    public class EndOfPrologException : TokenException
    {
    }

    /**
     * Thrown to indicate that the byte subarray being tokenized is a legal XML
     * token, but that subsequent bytes in the same entity could be part of
     * the token.  For example, <code>Encoding.tokenizeProlog</code>
     * would throw this if the byte subarray consists of a legal XML name.
     * @version $Revision: 1.3 $ $Date: 1998/02/17 04:24:06 $
     */

    /// <summary>
    /// </summary>
    public class ExtensibleTokenException : TokenException
    {
        #region Members

        /// <summary>
        /// </summary>
        private readonly TOK tokType;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="tokType"> </param>
        public ExtensibleTokenException(TOK tokType)
        {
            this.tokType = tokType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public TOK TokenType
        {
            get { return tokType; }
        }

        #endregion

        /**
         * Returns the type of token in the byte subarrary.
         */
    }

    /// <summary>
    ///   Several kinds of token problems.
    /// </summary>
    public class InvalidTokenException : TokenException
    {
        #region Constants

        /// <summary>
        ///   More than one attribute with the same name on the same element
        /// </summary>
        public const byte DUPLICATE_ATTRIBUTE = 2;

        /// <summary>
        ///   An illegal character
        /// </summary>
        public const byte ILLEGAL_CHAR = 0;

        /// <summary>
        ///   Doc prefix wasn't XML
        /// </summary>
        public const byte XML_TARGET = 1;

        #endregion

        #region Members

        /// <summary>
        /// </summary>
        private readonly int offset;

        /// <summary>
        /// </summary>
        private readonly byte type;

        #endregion

        #region Constructor

        /// <summary>
        ///   Some other type of bad token detected
        /// </summary>
        /// <param name="offset"> </param>
        /// <param name="type"> </param>
        public InvalidTokenException(int offset, byte type)
        {
            this.offset = offset;
            this.type = type;
        }

        /// <summary>
        ///   Illegal character detected
        /// </summary>
        /// <param name="offset"> </param>
        public InvalidTokenException(int offset)
        {
            this.offset = offset;
            type = ILLEGAL_CHAR;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Offset into the buffer where the problem ocurred.
        /// </summary>
        public int Offset
        {
            get { return offset; }
        }

        /// <summary>
        ///   Type of exception
        /// </summary>
        public int Type
        {
            get { return type; }
        }

        #endregion
    }

    /**
     * Thrown to indicate that the subarray being tokenized is not the
     * complete encoding of one or more characters, but might be if
     * more bytes were added.
     * @version $Revision: 1.2 $ $Date: 1998/02/17 04:24:11 $
     */

    /// <summary>
    /// </summary>
    public class PartialCharException : PartialTokenException
    {
        #region Members

        /// <summary>
        /// </summary>
        private readonly int leadByteIndex;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="leadByteIndex"> </param>
        public PartialCharException(int leadByteIndex)
        {
            this.leadByteIndex = leadByteIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public int LeadByteIndex
        {
            get { return leadByteIndex; }
        }

        #endregion

        /**
         * Returns the index of the first byte that is not part of the complete
         * encoding of a character.
         */
    }

    /// <summary>
    ///   A partial token was received. Try again, after you add more bytes to the buffer.
    /// </summary>
    public class PartialTokenException : TokenException
    {
    }
}