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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Users;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Storage.Interface;

namespace ASC.Xmpp.Host
{
    class ASCVCardStore : DbVCardStore, IVCardStore
    {
        private static readonly int IMAGE_SIZE = 64;

        private TimeSpan CACHE_TIMEOUT = TimeSpan.FromHours(1);

        private readonly ICache cache = AscCache.Memory;


        public override void Configure(IDictionary<string, string> properties)
        {
            base.Configure(properties);
            if (properties.ContainsKey("cacheTimeout"))
            {
                var timeout = int.Parse(properties["cacheTimeout"]);
                CACHE_TIMEOUT = 0 < timeout ? TimeSpan.FromMinutes(timeout) : TimeSpan.MaxValue;
            }
        }

        public override void SetVCard(Jid jid, Vcard vcard)
        {
            ASCContext.SetCurrentTenant(jid.Server);
            var tenant = ASCContext.GetCurrentTenant();
            if (tenant == null || ASCContext.UserManager.IsUserNameExists(jid.User))
            {
                throw new JabberException(ErrorCode.Forbidden);
            }
            base.SetVCard(jid, vcard);
        }

        public override Vcard GetVCard(Jid jid, string id = "")
        {
            ASCContext.SetCurrentTenant(jid.Server);

            var tenant = ASCContext.GetCurrentTenant();
            if (tenant == null)
            {
                return new Vcard();
            }

            jid = new Jid(jid.Bare.ToLowerInvariant());
            var ui = ASCContext.UserManager.GetUserByUserName(jid.User);

            if (ui != null)
            {

                var vcard = cache.Get<Vcard>(jid.ToString());
                if (vcard != null)
                {
                    return vcard;
                }

                vcard = new Vcard();
                vcard.Name = new Name(ui.LastName, ui.FirstName, null);
                vcard.Fullname = UserFormatter.GetUserName(ui);
                vcard.Nickname = ui.UserName;
                vcard.Description = ui.Notes;
                if (ui.BirthDate != null) vcard.Birthday = ui.BirthDate.Value;
                vcard.JabberId = jid;
                if (ui.Sex.HasValue)
                {
                    vcard.Gender = ui.Sex.Value ? Gender.MALE : Gender.FEMALE;
                }

                var index = ui.Contacts.FindIndex(c => string.Compare(c, "phone", true) == 0) + 1;
                if (0 < index && index < ui.Contacts.Count)
                {
                    vcard.AddTelephoneNumber(new Telephone(TelephoneLocation.WORK, TelephoneType.NUMBER, ui.Contacts[index]));
                }
                vcard.AddEmailAddress(new Email(EmailType.INTERNET, ui.Email, true));

                var departments = string.Join(", ", CoreContext.UserManager.GetUserGroups(ui.ID).Select(d => HttpUtility.HtmlEncode(d.Name)).ToArray());
                if (tenant != null) vcard.Organization = new Organization(tenant.Name, departments);
                vcard.Title = ui.Title;
                if (id == null || id.IndexOf("tmtalk", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    var image = PreparePhoto(ASCContext.UserManager.GetUserPhoto(ui.ID));
                    if (image != null)
                    {
                        vcard.Photo = new Photo(image, ImageFormat.Png);
                        image.Dispose();
                    }
                }
                cache.Insert(jid.ToString(), vcard, CACHE_TIMEOUT);
                return vcard;
            }
            else
            {
                return base.GetVCard(jid);
            }
        }

        public override ICollection<Vcard> Search(Vcard pattern)
        {
            throw new NotImplementedException();
        }


        private Image PreparePhoto(byte[] photo)
        {
            if (photo == null || photo.Length == 0) return null;

            using (var stream = new MemoryStream(photo))
            using (var image = Image.FromStream(stream))
            {
                var imageMinSize = Math.Min(image.Width, image.Height);
                var size = IMAGE_SIZE;
                if (imageMinSize < 96) size = 64;
                if (imageMinSize < 64) size = 32;

                using (var bitmap = new Bitmap(size, size))
                using (var g = Graphics.FromImage(bitmap))
                {
                    var delta = (image.Width - image.Height) / 2;
                    var srcRect = new RectangleF(0f, 0f, imageMinSize, imageMinSize);
                    if (image.Width < image.Height) srcRect.Y = -delta;
                    else srcRect.X = delta;

                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    var gu = GraphicsUnit.Pixel;
                    var destRect = bitmap.GetBounds(ref gu);
                    g.DrawImage(image, destRect, srcRect, gu);

                    var saveStream = new MemoryStream();
                    bitmap.Save(saveStream, ImageFormat.Png);
                    return Image.FromStream(saveStream);
                }
            }
        }
    }
}