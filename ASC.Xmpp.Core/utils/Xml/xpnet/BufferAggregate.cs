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
using System.IO;

#endregion

namespace ASC.Xmpp.Core.utils.Xml.xpnet
{

    #region usings

    #endregion

    /// <summary>
    ///   Aggregate byte arrays together, so we can parse across IP packet boundaries
    /// </summary>
    public class BufferAggregate
    {
        #region Members

        /// <summary>
        /// </summary>
        private readonly MemoryStream m_stream = new MemoryStream();

        /// <summary>
        /// </summary>
        private BufNode m_head;

        /// <summary>
        /// </summary>
        private BufNode m_tail;

        #endregion

        #region Methods

        /// <summary>
        ///   Write to the buffer. Please make sure that you won't use this memory any more after you hand it in. It will get mangled.
        /// </summary>
        /// <param name="buf"> </param>
        public void Write(byte[] buf)
        {
            m_stream.Write(buf, 0, buf.Length);
            if (m_tail == null)
            {
                m_head = m_tail = new BufNode();
                m_head.buf = buf;
            }
            else
            {
                var n = new BufNode();
                n.buf = buf;
                m_tail.next = n;
                m_tail = n;
            }
        }

        /// <summary>
        ///   Get the current aggregate contents of the buffer.
        /// </summary>
        /// <returns> </returns>
        public byte[] GetBuffer()
        {
            return m_stream.ToArray();
        }

        /// <summary>
        ///   Clear the first "offset" bytes of the buffer, so they won't be parsed again.
        /// </summary>
        /// <param name="offset"> </param>
        public void Clear(int offset)
        {
            int s = 0;
            int save = -1;

            BufNode bn = null;
            for (bn = m_head; bn != null; bn = bn.next)
            {
                if (s + bn.buf.Length <= offset)
                {
                    if (s + bn.buf.Length == offset)
                    {
                        bn = bn.next;
                        break;
                    }

                    s += bn.buf.Length;
                }
                else
                {
                    save = s + bn.buf.Length - offset;
                    break;
                }
            }

            m_head = bn;
            if (m_head == null)
            {
                m_tail = null;
            }

            if (save > 0)
            {
                var buf = new byte[save];
                Buffer.BlockCopy(m_head.buf, m_head.buf.Length - save, buf, 0, save);
                m_head.buf = buf;
            }

            m_stream.SetLength(0);
            for (bn = m_head; bn != null; bn = bn.next)
            {
                m_stream.Write(bn.buf, 0, bn.buf.Length);
            }
        }

        /// <summary>
        ///   UTF8 encode the current contents of the buffer. Just for prettiness in the debugger.
        /// </summary>
        /// <returns> </returns>
        public override string ToString()
        {
            byte[] b = GetBuffer();
            return System.Text.Encoding.UTF8.GetString(b, 0, b.Length);
        }

        #endregion

        // RingBuffer of the Nieblung

        #region Nested type: BufNode

        /// <summary>
        /// </summary>
        private class BufNode
        {
            #region Members

            /// <summary>
            /// </summary>
            public byte[] buf;

            /// <summary>
            /// </summary>
            public BufNode next;

            #endregion
        }

        #endregion
    }
}