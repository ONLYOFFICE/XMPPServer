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

using System.IO;

#endregion

namespace ASC.Xmpp.Core.utils.Xml.Dom
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class Document : Node
    {
        #region Members

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        public Document()
        {
            NodeType = NodeType.Document;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// </summary>
        public Element RootElement
        {
            get
            {
                foreach (Node n in ChildNodes)
                {
                    if (n.NodeType == NodeType.Element)
                    {
                        return n as Element;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// </summary>
        public string Version { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///   Clears the Document
        /// </summary>
        public void Clear()
        {
            ChildNodes.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="xml"> </param>
        public void LoadXml(string xml)
        {
            if (xml != string.Empty && xml != null)
            {
                new DomLoader(xml, this);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="filename"> </param>
        /// <returns> </returns>
        public bool LoadFile(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    var sr = new StreamReader(filename);
                    new DomLoader(sr, this);
                    sr.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="stream"> </param>
        /// <returns> </returns>
        public bool LoadStream(Stream stream)
        {
            try
            {
                var sr = new StreamReader(stream);
                new DomLoader(sr, this);
                sr.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="filename"> </param>
        public void Save(string filename)
        {
            var w = new StreamWriter(filename);

            w.Write(ToString(System.Text.Encoding.UTF8));
            w.Flush();
            w.Close();
        }

        #endregion
    }
}