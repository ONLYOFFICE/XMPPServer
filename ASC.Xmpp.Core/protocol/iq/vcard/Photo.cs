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

#if !CF
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using ASC.Xmpp.Core.utils.Xml.Dom;

#if !SL
#endif

namespace ASC.Xmpp.Core.protocol.iq.vcard
{
    /// <summary>
    ///   Vcard Photo When you dont want System.Drawing in the Lib just remove the photo stuff
    /// </summary>
    public class Photo : Element
    {
        // <!-- Photograph property. Value is either a BASE64 encoded
        // binary value or a URI to the external content. -->
        // <!ELEMENT PHOTO ((TYPE, BINVAL) | EXTVAL)>	

        #region << Constructors >>

        public Photo()
        {
            TagName = "PHOTO";
            Namespace = Uri.VCARD;
        }

#if !SL
        public Photo(Image image, ImageFormat format)
            : this()
        {
            SetImage(image, format);
        }
#endif

        public Photo(string url)
            : this()
        {
            SetImage(url);
        }

        #endregion

        /// <summary>
        ///   The Media Type, Only available when BINVAL
        /// </summary>
        public string Type
        {
            //<TYPE>image</TYPE>
            get { return GetTag("TYPE"); }
            set { SetTag("TYPE", value); }
        }

        /// <summary>
        ///   Sets the URL of an external image
        /// </summary>
        /// <param name="url"> </param>
        public void SetImage(string url)
        {
            SetTag("EXTVAL", url);
        }

#if !SL
        public void SetImage(Image image, ImageFormat format)
        {
            // if we have no FOrmatprovider then we save the image as PNG
            if (format == null) format = ImageFormat.Png;

            string sType = "image";
            if (format == ImageFormat.Jpeg) sType = "image/jpeg";
            else if (format == ImageFormat.Png) sType = "image/png";
            else if (format == ImageFormat.Gif) sType = "image/gif";
#if!CF_2
            else if (format == ImageFormat.Tiff) sType = "image/tiff";
#endif
            SetTag("TYPE", sType);

            var ms = new MemoryStream();
            image.Save(ms, format);
            byte[] buf = ms.GetBuffer();
            SetTagBase64("BINVAL", buf);
        }

        /// <summary>
        ///   returns the image format or null for unknown formats or TYPES
        /// </summary>
        public ImageFormat ImageFormat
        {
            get
            {
                string sType = GetTag("TYPE");

                if (sType == "image/jpeg")
                    return ImageFormat.Jpeg;
                else if (sType == "image/png")
                    return ImageFormat.Png;
                else if (sType == "image/gif")
                    return ImageFormat.Gif;
#if!CF_2
                else if (sType == "image/tiff")
                    return ImageFormat.Tiff;
#endif
                else
                    return null;
            }
        }

        /// <summary>
        ///   gets or sets the from internal (binary) or external source When external then it trys to get the image with a Webrequest
        /// </summary>
        public Image Image
        {
            get
            {
                try
                {
                    if (HasTag("BINVAL"))
                    {
                        byte[] pic = Convert.FromBase64String(GetTag("BINVAL"));
                        var ms = new MemoryStream(pic, 0, pic.Length);
                        return new Bitmap(ms);
                    }
                    else if (HasTag("EXTVAL"))
                    {
                        var req = WebRequest.Create(GetTag("EXTVAL"));
                        using (var response = req.GetResponse())
                        using (var stream = response.GetResponseStream())
                        {
                            return new Bitmap(stream);
                        }

                    }
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }
            /*
            set
			{
				SetTag("TYPE", "image");
				MemoryStream ms = new MemoryStream();
				// Save the Image as PNG to the Memorystream
				value.Save(ms, ImageFormat.Png);
				byte[] buf = ms.GetBuffer();				
				SetTagBase64("BINVAL", buf);
			}
            */
        }
#endif
    }
}

#endif