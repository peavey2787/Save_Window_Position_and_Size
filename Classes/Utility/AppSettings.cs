using System.Configuration;
using System.Text;
using Newtonsoft.Json;

namespace Save_Window_Position_and_Size.Classes
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
            ConfigurationManager.RefreshSection(Constants.Defaults.ConfigManagerAppSettingsKey);
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



    public static class AppSettings<T>
    {
        public static void Save(string key, T value)
        {
            // Serialize the object to JSON using Newtonsoft.Json
            string jsonString = JsonConvert.SerializeObject(value);

            // Convert the JSON string to a Base64-encoded string
            string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

            // Save the Base64 string using AppSettings logic
            AppSettings.Save(key, base64String);
        }

        public static T Load(string key)
        {
            // Load the Base64 string
            string base64String = AppSettings.Load(key);

            if (string.IsNullOrEmpty(base64String) || base64String == "{}" || base64String == "0")
            {
                return default;
            }

            // Decode the Base64 string back to a JSON string
            string jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            // Deserialize the JSON string into the original object using Newtonsoft.Json
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }

}
