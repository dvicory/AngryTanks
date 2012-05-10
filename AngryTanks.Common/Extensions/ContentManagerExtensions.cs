using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using log4net;

namespace AngryTanks.Common.Extensions
{
    namespace ContentManagerExtensions
    {
        public static class ContentManagerExtensionsClass
        {
            private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            public static Dictionary<String, T> LoadDirectory<T>(this ContentManager contentManager, String contentFolder)
            {
                // load directory info, abord if none
                DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory +
                                                      Path.DirectorySeparatorChar +
                                                      contentFolder);
                if (!dir.Exists)
                    throw new DirectoryNotFoundException();

                // init the resulting dictionary
                Dictionary<String, T> result = new Dictionary<String, T>();

                // load all files that match the filter *.xnb
                FileInfo[] files = dir.GetFiles("*.xnb");
                foreach (FileInfo file in files)
                {
                    String key = Path.GetFileNameWithoutExtension(file.Name);
                    try
                    {
                        result[key] = contentManager.Load<T>(contentManager.RootDirectory +
                                                             Path.DirectorySeparatorChar +
                                                             contentFolder +
                                                             Path.DirectorySeparatorChar +
                                                             key);
                    }
                    catch (ContentLoadException e)
                    {
                        Log.Warn(e.Message);
                    }
                }

                // return result
                return result;
            }
        }
    }
}
