using System.Configuration;

namespace Save_Window_Position_and_Size
{
    public static class AppSettings
    {
        public static void Save(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Check if the key already exists in AppSettings
            KeyValueConfigurationElement key_config = config.AppSettings.Settings[key];

            // If the key is not found, add it to AppSettings
            if (key_config == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                key_config.Value = value;
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        public static string Load(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Check if the key exists in AppSettings
            KeyValueConfigurationElement key_config = config.AppSettings.Settings[key];

            if (key_config == null)
            {
                return null;
            }
            else
            {
                return key_config.Value;
            }
        }
    }
}
