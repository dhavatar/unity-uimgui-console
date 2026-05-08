using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace UImGuiConsole
{
    public class Script
    {
        /// <summary>
        /// Path to the script file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Commands in the script.
        /// </summary>
        public List<string> Data { get; private set; } = new();

        /// <summary>
        /// Unity TextAsset associated with this script.
        /// </summary>
        public TextAsset Asset { get; private set; } = null;

        protected bool fromMemory;   // Flag to specify if script was loaded from file or memory

        public Script(TextAsset textAsset)
        {
            Asset = textAsset;
            fromMemory = false;
            Load();
        }

        public Script(List<string> data)
        {
            Data = data;
            fromMemory = true;
        }

        public Script(string path, bool loadOnInit)
        {
            Path = path;
            fromMemory = false;

            if (loadOnInit)
            {
                Load();
            }
        }

        public void Load()
        {
            if (Asset != null)
            {
                using (StringReader reader = new StringReader(Asset.text))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Data.Add(line);
                    }
                }
            }
            else
            {
                if (!File.Exists(Path))
                {
                    throw new System.Exception($"Failed to load script {Path}");
                }

                using (var fs = File.OpenRead(Path))
                {
                    using (var streamReader = new StreamReader(fs, Encoding.UTF8, true))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            Data.Add(line);
                        }
                    }
                }
            }
        }

        public void Reload()
        {
            if (fromMemory) return;

            Unload();
            Load();
        }

        public void Unload()
        {
            Data.Clear();
        }
    }
}