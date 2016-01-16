using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talky.Configuration
{
    class ConfigurationFile
    {

        private const string CONFIG_PATH = "C:\\Talky\\";
        private const string CONFIG_EXT = ".tcfg";

        public string Path { get; private set; }

        public ConfigurationFile(string name)
        {
            Path = CONFIG_PATH + name + CONFIG_EXT;
        }

        public bool Exists()
        {
            return File.Exists(Path);
        }

        public void Write(Dictionary<string, string> values)
        {
            if (!Exists())
            {
                try
                {
                    if (!Directory.Exists(CONFIG_PATH))
                    {
                        Directory.CreateDirectory(CONFIG_PATH);
                    }

                    File.Create(Path).Close();
                } catch (System.Exception e)
                {
                    Program.Instance.OHGODNO("Couldn't make configuration file!", e);
                    return;
                }
            } else
            {
                File.WriteAllText(Path, string.Empty);
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(Path))
                {
                    foreach (string key in values.Keys)
                    {
                        writer.WriteLine(key + ":" + values[key]);
                    }
                }
            } catch (System.Exception e)
            {
                Program.Instance.OHGODNO("Couldn't write the configuration file!", e);
            }
        }

        public IReadOnlyDictionary<string, string> Values(Dictionary<string, string> defaults = null)
        {
            try
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                using (StreamReader reader = new StreamReader(Path))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] split = line.Split(new char[] { ':' }, 2);
                        if (split.Length == 2)
                        {
                            values.Add(split[0], split[1]);
                        }
                    }
                }
                return (values.Count == 0 ? defaults : values);
            } catch (System.Exception e)
            {
                if (defaults == null)
                {
                    Program.Instance.OHGODNO("Couldn't read the configuration file!", e);
                }
                return defaults;
            }
        }

    }
}
