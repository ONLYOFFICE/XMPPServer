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
    ///   Represents information returned by <code>Encoding.tokenizeContent</code> . @see Encoding#tokenizeContent
    /// </summary>
    public class ContentToken : Token
    {
        #region Constants

        /// <summary>
        /// </summary>
        private const int INIT_ATT_COUNT = 8;

        #endregion

        #region Members

        /// <summary>
        /// </summary>
        private int attCount;

        /// <summary>
        /// </summary>
        private int[] attNameEnd = new int[INIT_ATT_COUNT];

        /// <summary>
        /// </summary>
        private int[] attNameStart = new int[INIT_ATT_COUNT];

        /// <summary>
        /// </summary>
        private bool[] attNormalized = new bool[INIT_ATT_COUNT];

        /// <summary>
        /// </summary>
        private int[] attValueEnd = new int[INIT_ATT_COUNT];

        /// <summary>
        /// </summary>
        private int[] attValueStart = new int[INIT_ATT_COUNT];

        #endregion

        #region Methods

        /// <summary>
        ///   Returns the number of attributes specified in the start-tag or empty element tag.
        /// </summary>
        /// <returns> </returns>
        public int getAttributeSpecifiedCount()
        {
            return attCount;
        }

        /// <summary>
        ///   Returns the index of the first character of the name of the attribute index <code>i</code> .
        /// </summary>
        /// <param name="i"> </param>
        /// <returns> </returns>
        public int getAttributeNameStart(int i)
        {
            if (i >= attCount)
            {
                throw new IndexOutOfRangeException();
            }

            return attNameStart[i];
        }

        /**
		 * Returns the index following the last character of the name of the
		 * attribute index <code>i</code>.
		 */

        /// <summary>
        /// </summary>
        /// <param name="i"> </param>
        /// <returns> </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public int getAttributeNameEnd(int i)
        {
            if (i >= attCount)
            {
                throw new IndexOutOfRangeException();
            }

            return attNameEnd[i];
        }

        /**
		 * Returns the index of the character following the opening quote of
		 * attribute index <code>i</code>.
		 */

        /// <summary>
        /// </summary>
        /// <param name="i"> </param>
        /// <returns> </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public int getAttributeValueStart(int i)
        {
            if (i >= attCount)
            {
                throw new IndexOutOfRangeException();
            }

            return attValueStart[i];
        }

        /**
		 * Returns the index of the closing quote attribute index <code>i</code>.
		 */

        /// <summary>
        /// </summary>
        /// <param name="i"> </param>
        /// <returns> </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public int getAttributeValueEnd(int i)
        {
            if (i >= attCount)
            {
                throw new IndexOutOfRangeException();
            }

            return attValueEnd[i];
        }

        /**
		 * Returns true if attribute index <code>i</code> does not need to
		 * be normalized.  This is an optimization that allows further processing
		 * of the attribute to be avoided when it is known that normalization
		 * cannot change the value of the attribute.
		 */

        /// <summary>
        /// </summary>
        /// <param name="i"> </param>
        /// <returns> </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool isAttributeNormalized(int i)
        {
            if (i >= attCount)
            {
                throw new IndexOutOfRangeException();
            }

            return attNormalized[i];
        }

        /// <summary>
        ///   Clear out all of the current attributes
        /// </summary>
        public void clearAttributes()
        {
            attCount = 0;
        }

        /// <summary>
        ///   Add a new attribute
        /// </summary>
        /// <param name="nameStart"> </param>
        /// <param name="nameEnd"> </param>
        /// <param name="valueStart"> </param>
        /// <param name="valueEnd"> </param>
        /// <param name="normalized"> </param>
        public void appendAttribute(int nameStart, int nameEnd, int valueStart, int valueEnd, bool normalized)
        {
            if (attCount == attNameStart.Length)
            {
                attNameStart = grow(attNameStart);
                attNameEnd = grow(attNameEnd);
                attValueStart = grow(attValueStart);
                attValueEnd = grow(attValueEnd);
                attNormalized = grow(attNormalized);
            }

            attNameStart[attCount] = nameStart;
            attNameEnd[attCount] = nameEnd;
            attValueStart[attCount] = valueStart;
            attValueEnd[attCount] = valueEnd;
            attNormalized[attCount] = normalized;
            ++attCount;
        }

        /// <summary>
        ///   Is the current attribute unique?
        /// </summary>
        /// <param name="buf"> </param>
        public void checkAttributeUniqueness(byte[] buf)
        {
            for (int i = 1; i < attCount; i++)
            {
                int len = attNameEnd[i] - attNameStart[i];
                for (int j = 0; j < i; j++)
                {
                    if (attNameEnd[j] - attNameStart[j] == len)
                    {
                        int n = len;
                        int s1 = attNameStart[i];
                        int s2 = attNameStart[j];
                        do
                        {
                            if (--n < 0)
                            {
                                throw new InvalidTokenException(attNameStart[i],
                                                                InvalidTokenException.DUPLICATE_ATTRIBUTE);
                            }
                        } while (buf[s1++] == buf[s2++]);
                    }
                }
            }
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// </summary>
        /// <param name="v"> </param>
        /// <returns> </returns>
        private static int[] grow(int[] v)
        {
            int[] tem = v;
            v = new int[tem.Length << 1];
            Array.Copy(tem, 0, v, 0, tem.Length);
            return v;
        }

        /// <summary>
        /// </summary>
        /// <param name="v"> </param>
        /// <returns> </returns>
        private static bool[] grow(bool[] v)
        {
            bool[] tem = v;
            v = new bool[tem.Length << 1];
            Array.Copy(tem, 0, v, 0, tem.Length);
            return v;
        }

        #endregion
    }
}