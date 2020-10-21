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

namespace ASC.Xmpp.Core.IO.Compression.Checksums
{
    /// <summary>
    ///   Interface to compute a data checksum used by checked input/output streams. A data checksum can be updated by one byte or with a byte array. After each update the value of the current checksum can be returned by calling <code>getValue</code> . The complete checksum object can also be reset so it can be used again with new data.
    /// </summary>
    public interface IChecksum
    {
        #region Properties

        /// <summary>
        ///   Returns the data checksum computed so far.
        /// </summary>
        long Value { get; }

        #endregion

        #region Methods

        /// <summary>
        ///   Resets the data checksum as if no update was ever called.
        /// </summary>
        void Reset();

        /// <summary>
        ///   Adds one byte to the data checksum.
        /// </summary>
        /// <param name="bval"> the data value to add. The high byte of the int is ignored. </param>
        void Update(int bval);

        /// <summary>
        ///   Updates the data checksum with the bytes taken from the array.
        /// </summary>
        /// <param name="buffer"> buffer an array of bytes </param>
        void Update(byte[] buffer);

        /// <summary>
        ///   Adds the byte array to the data checksum.
        /// </summary>
        /// <param name="buf"> the buffer which contains the data </param>
        /// <param name="off"> the offset in the buffer where the data starts </param>
        /// <param name="len"> the length of the data </param>
        void Update(byte[] buf, int off, int len);

        #endregion
    }
}