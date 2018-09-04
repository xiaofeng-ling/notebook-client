using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace notebook
{
    class Config
    {
        public string token = "";

        private readonly static string configPath = "config.json";

        public static Config LoadConfig()
        {
            try
            {
                string configText = File.ReadAllText(Config.configPath);

                Config config = JsonConvert.DeserializeObject<Config>(configText);

                return config;
            }
            catch (Exception)
            {
                return new Config();
            }
        }

        public bool saveConfig()
        {
            string configText = JsonConvert.SerializeObject(this);
            File.WriteAllText(Config.configPath, configText);

            return true;
        }
    }
}
