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
using System.IO;

namespace ASC.Xmpp.Server.Utils
{
	static class PathUtils
	{
        private const string DATA_DIRECTORY = "|DataDirectory|";
        private const string DATA_DIRECTORY_KEY = "DataDirectory";


        public static string GetAbsolutePath(string path)
		{
            var currDir = AppDomain.CurrentDomain.BaseDirectory;
            if (path.Trim(Path.DirectorySeparatorChar).StartsWith(DATA_DIRECTORY, StringComparison.CurrentCultureIgnoreCase))
            {
                path = path.Substring(DATA_DIRECTORY.Length).Trim(Path.DirectorySeparatorChar);
                var dataDir = (string)AppDomain.CurrentDomain.GetData(DATA_DIRECTORY_KEY) ?? currDir;
                path = Path.Combine(dataDir, path);
            }
            return Path.IsPathRooted(path) ? path : Path.Combine(currDir, path);
        }
	}
}
