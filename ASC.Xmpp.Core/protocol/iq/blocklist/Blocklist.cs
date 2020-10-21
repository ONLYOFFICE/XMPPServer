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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.blocklist
{
    public class Blocklist : BlockBase
    {
        public Blocklist()
        {
            TagName = "blocklist";
        }
    }

    public class Block : BlockBase
    {
        public Block()
        {
            TagName = "block";
        }
    }

    public class Unblock : BlockBase
    {
        public Unblock()
        {
            TagName = "unblock";
        }
    }

    public class BlockBase : Element
    {
        public BlockBase()
        {
            Namespace = Uri.IQ_BLOCKLIST;
        }


        public BlockItem[] GetItems()
        {
            ElementList nl = SelectElements(typeof (BlockItem));
            int i = 0;
            var result = new BlockItem[nl.Count];
            foreach (BlockItem ri in nl)
            {
                result[i] = ri;
                i++;
            }
            return result;
        }

        public void AddBlockItem(BlockItem r)
        {
            ChildNodes.Add(r);
        }
    }
}