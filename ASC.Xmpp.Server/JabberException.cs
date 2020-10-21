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
using System.Runtime.Serialization;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.utils.Xml.Dom;
using StanzaError = ASC.Xmpp.Core.protocol.client.Error;
using StreamError = ASC.Xmpp.Core.protocol.Error;

namespace ASC.Xmpp.Server
{
    public class JabberException : Exception
    {
        public StreamErrorCondition StreamErrorCondition
        {
            get;
            private set;
        }


        public ErrorCode ErrorCode
        {
            get;
            private set;
        }

        public bool CloseStream
        {
            get;
            private set;
        }

        public bool StreamError
        {
            get;
            private set;
        }

        public JabberException(string message, Exception innerException)
            : base(message, innerException)
        {
            StreamError = false;
            ErrorCode = ErrorCode.InternalServerError;
        }

        public JabberException(StreamErrorCondition streamErrorCondition)
            : this(streamErrorCondition, true)
        {

        }

        public JabberException(StreamErrorCondition streamErrorCondition, bool closeStream)
            : base()
        {
            StreamError = true;
            CloseStream = closeStream;
            this.StreamErrorCondition = streamErrorCondition;
        }

        public JabberException(ErrorCode errorCode)
            : base()
        {
            StreamError = false;
            CloseStream = false;
            this.ErrorCode = errorCode;
        }

        protected JabberException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public virtual Element ToElement()
        {
            return StreamError ? (Element)new StreamError(StreamErrorCondition) : (Element)new StanzaError(ErrorCode);
        }
    }
}
