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

using ASC.Xmpp.Core.protocol.sasl;

namespace ASC.Xmpp.Server.Authorization
{
	static class XmppFailureError
	{
		/// <summary>
		/// The receiving entity acknowledges an <abort/> element sent by the initiating entity; sent in reply to the <abort/> element.
		/// </summary>
		public static Failure Aborted;

		/// <summary>
		/// The data provided by the initiating entity could not be processed because the [BASE64] (Josefsson, S., “The Base16, Base32, and Base64 Data Encodings,” July 2003.) encoding is incorrect (e.g., because the encoding does not adhere to the definition in Section 3 of [BASE64] (Josefsson, S., “The Base16, Base32, and Base64 Data Encodings,” July 2003.)); sent in reply to a <response/> element or an <auth/> element with initial response data.
		/// </summary>
		public static Failure IncorrectEncoding;

		/// <summary>
		/// The authzid provided by the initiating entity is invalid, either because it is incorrectly formatted or because the initiating entity does not have permissions to authorize that ID; sent in reply to a <response/> element or an <auth/> element with initial response data.
		/// </summary>
		public static Failure InvalidAuthzid;

		/// <summary>
		/// The initiating entity did not provide a mechanism or requested a mechanism that is not supported by the receiving entity; sent in reply to an <auth/> element.
		/// </summary>
		public static Failure InvalidMechanism;

		/// <summary>
		/// The mechanism requested by the initiating entity is weaker than server policy permits for that initiating entity; sent in reply to a <response/> element or an <auth/> element with initial response data.
		/// </summary>
		public static Failure MechanismTooWeak;

		/// <summary>
		/// The authentication failed because the initiating entity did not provide valid credentials (this includes but is not limited to the case of an unknown username); sent in reply to a <response/> element or an <auth/> element with initial response data.
		/// </summary>
		public static Failure NotAuthorized;

		/// <summary>
		/// The authentication failed because of a temporary error condition within the receiving entity; sent in reply to an <auth/> element or <response/> element.
		/// </summary>
		public static Failure TemporaryAuthFailure;

		public static Failure UnknownCondition;

		static XmppFailureError()
		{
			Aborted = new Failure(FailureCondition.aborted);
			IncorrectEncoding = new Failure(FailureCondition.incorrect_encoding);
			InvalidAuthzid = new Failure(FailureCondition.invalid_authzid);
			InvalidMechanism = new Failure(FailureCondition.invalid_mechanism);
			MechanismTooWeak = new Failure(FailureCondition.mechanism_too_weak);
			NotAuthorized = new Failure(FailureCondition.not_authorized);
			TemporaryAuthFailure = new Failure(FailureCondition.temporary_auth_failure);
			UnknownCondition = new Failure(FailureCondition.UnknownCondition);
		}
	}
}
