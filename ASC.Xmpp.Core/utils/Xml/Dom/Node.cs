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
using System.Text;
using System.Xml;

using ASC.Xmpp.Core.IO;

#endregion

namespace ASC.Xmpp.Core.utils.Xml.Dom
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// </summary>
        Document, // xmlDocument
        /// <summary>
        /// </summary>
        Element, // normal Element
        /// <summary>
        /// </summary>
        Text, // Textnode
        /// <summary>
        /// </summary>
        Cdata, // CDATA Section
        /// <summary>
        /// </summary>
        Comment, // comment
        /// <summary>
        /// </summary>
        Declaration // processing instruction
    }

    /// <summary>
    /// </summary>
    public abstract class Node : ICloneable
    {
        #region Members

        /// <summary>
        /// </summary>
        internal Node Parent;

        /// <summary>
        /// </summary>
        private NodeList m_ChildNodes;

        /// <summary>
        /// </summary>
        internal int m_Index;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        public Node()
        {
            m_ChildNodes = new NodeList(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public NodeList ChildNodes
        {
            get { return m_ChildNodes; }
        }

        /// <summary>
        /// </summary>
        public int Index
        {
            get { return m_Index; }
        }

        /// <summary>
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>
        /// </summary>
        public virtual string Value { get; set; }

        #endregion

        #region Methods

        public virtual object Clone()
        {
            var clone = (Node)MemberwiseClone();
            clone.m_ChildNodes = new NodeList();
            foreach (Node a in m_ChildNodes)
            {
                clone.m_ChildNodes.Add((Node)a.Clone());
            }
            return clone;
        }

        /// <summary>
        /// </summary>
        public void Remove()
        {
            if (Parent != null)
            {
                int idx = m_Index;
                Parent.ChildNodes.RemoveAt(idx);
                Parent.ChildNodes.RebuildIndex(idx);
            }
        }

        /// <summary>
        /// </summary>
        public void RemoveAllChildNodes()
        {
            m_ChildNodes.Clear();
        }

        /// <summary>
        ///   Appends the given Element as child element
        /// </summary>
        /// <param name="e"> </param>
        public virtual void AddChild(Node e)
        {
            m_ChildNodes.Add(e);
        }

        /// <summary>
        ///   Returns the Xml of the current Element (Node) as string
        /// </summary>
        /// <returns> </returns>
        public override string ToString()
        {
            return BuildXml(this, Formatting.None, 0, ' ');
        }

        /// <summary>
        /// </summary>
        /// <param name="enc"> </param>
        /// <returns> </returns>
        public string ToString(Encoding enc)
        {
            if (this != null)
            {
                var tw = new StringWriterWithEncoding(enc);

                // System.IO.StringWriter tw = new StringWriter();
                var w = new XmlTextWriter(tw);

                // Format the Output. So its human readable in notepad
                // Without that everyting is in one line
                w.Formatting = Formatting.Indented;
                w.Indentation = 3;

                WriteTree(this, w, null);

                return tw.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   returns the Xml, difference to the Xml property is that you can set formatting porperties
        /// </summary>
        /// <param name="format"> </param>
        /// <returns> </returns>
        public string ToString(Formatting format)
        {
            return BuildXml(this, format, 3, ' ');
        }

        /// <summary>
        ///   returns the Xml, difference to the Xml property is that you can set formatting properties
        /// </summary>
        /// <param name="format"> </param>
        /// <param name="indent"> </param>
        /// <returns> </returns>
        public string ToString(Formatting format, int indent)
        {
            return BuildXml(this, format, indent, ' ');
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        /// <param name="format"> </param>
        /// <param name="indent"> </param>
        /// <param name="indentchar"> </param>
        /// <returns> </returns>
        private string BuildXml(Node e, Formatting format, int indent, char indentchar)
        {
            if (e != null)
            {
                var tw = new StringWriter();
                var w = new XmlTextWriter(tw);
                w.Formatting = format;
                w.Indentation = indent;
                w.IndentChar = indentchar;

                WriteTree(this, w, null);

                return tw.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        /// <param name="tw"> </param>
        /// <param name="parent"> </param>
        private void WriteTree(Node e, XmlTextWriter tw, Node parent)
        {
            if (e.NodeType == NodeType.Document)
            {
                // Write the ProcessingInstruction node.
                // <?xml version="1.0" encoding="windows-1252"?> ...
                var doc = e as Document;
                string pi = null;

                if (doc.Version != null)
                {
                    pi += "version='" + doc.Version + "'";
                }

                if (doc.Encoding != null)
                {
                    if (pi != null)
                    {
                        pi += " ";
                    }

                    pi += "encoding='" + doc.Encoding + "'";
                }

                if (pi != null)
                {
                    tw.WriteProcessingInstruction("xml", pi);
                }

                foreach (Node n in e.ChildNodes)
                {
                    WriteTree(n, tw, e);
                }
            }
            else if (e.NodeType == NodeType.Text)
            {
                tw.WriteString(e.Value);
            }
            else if (e.NodeType == NodeType.Comment)
            {
                tw.WriteComment(e.Value);
            }
            else if (e.NodeType == NodeType.Element)
            {
                var el = e as Element;

                if (el.Prefix == null)
                {
                    tw.WriteStartElement(el.TagName);
                }
                else
                {
                    tw.WriteStartElement(el.Prefix + ":" + el.TagName);
                }

                // Write Namespace
                if ((parent == null || parent.Namespace != el.Namespace) && el.Namespace != null &&
                    el.Namespace.Length != 0)
                {
                    if (el.Prefix == null)
                    {
                        tw.WriteAttributeString("xmlns", el.Namespace);
                    }
                    else
                    {
                        tw.WriteAttributeString("xmlns:" + el.Prefix, el.Namespace);
                    }
                }

                foreach (string attName in el.Attributes.Keys)
                {
                    tw.WriteAttributeString(attName, el.Attribute(attName));
                }

                // tw.WriteString(el.Value);
                if (el.ChildNodes.Count > 0)
                {
                    foreach (Node n in el.ChildNodes)
                    {
                        WriteTree(n, tw, e);
                    }

                    tw.WriteEndElement();
                }
                else
                {
                    tw.WriteEndElement();
                }
            }
        }

        #endregion
    }
}