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

using System.Collections;
using System.Text;

#endregion

namespace ASC.Xmpp.Core.utils.Xml.xpnet
{

    #region usings

    #endregion

    /// <summary>
    ///   Namespace stack.
    /// </summary>
    public class NS
    {
        #region Members

        /// <summary>
        /// </summary>
        private readonly Stack m_stack = new Stack();

        #endregion

        #region Constructor

        /// <summary>
        ///   Create a new stack, primed with xmlns and xml as prefixes.
        /// </summary>
        public NS()
        {
            PushScope();
            AddNamespace("xmlns", "http://www.w3.org/2000/xmlns/");
            AddNamespace("xml", "http://www.w3.org/XML/1998/namespace");
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The current default namespace.
        /// </summary>
        public string DefaultNamespace
        {
            get { return LookupNamespace(string.Empty); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Declare a new scope, typically at the start of each element
        /// </summary>
        public void PushScope()
        {
            m_stack.Push(new Hashtable());
        }

        /// <summary>
        ///   Pop the current scope off the stack. Typically at the end of each element.
        /// </summary>
        public void PopScope()
        {
            m_stack.Pop();
        }

        /// <summary>
        ///   Add a namespace to the current scope.
        /// </summary>
        /// <param name="prefix"> </param>
        /// <param name="uri"> </param>
        public void AddNamespace(string prefix, string uri)
        {
            ((Hashtable) m_stack.Peek()).Add(prefix, uri);
        }

        /// <summary>
        ///   Lookup a prefix to find a namespace. Searches down the stack, starting at the current scope.
        /// </summary>
        /// <param name="prefix"> </param>
        /// <returns> </returns>
        public string LookupNamespace(string prefix)
        {
            foreach (Hashtable ht in m_stack)
            {
                if ((ht.Count > 0) && ht.ContainsKey(prefix))
                {
                    return (string) ht[prefix];
                }
            }

            return string.Empty;
        }

        /// <summary>
        ///   Debug output only.
        /// </summary>
        /// <returns> </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (Hashtable ht in m_stack)
            {
                sb.Append("---\n");
                foreach (string k in ht.Keys)
                {
                    sb.Append(string.Format("{0}={1}\n", k, ht[k]));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// </summary>
        public void Clear()
        {
#if !CF
            m_stack.Clear();
#else
			while (m_stack.Count > 0)
			    m_stack.Pop();
#endif
        }

        #endregion
    }
}