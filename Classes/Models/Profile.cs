using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Save_Window_Position_and_Size.Classes
{
    /// <summary>
    /// Represents a profile containing a collection of windows
    /// </summary>
    [Serializable]
    internal class Profile
    {
        [JsonProperty] // Ensures the property is serialized
        internal string Name { get; set; }

        [JsonProperty] // Ensures the property is serialized
        internal List<Window> Windows { get; set; } = new List<Window>();

        // Parameterless constructor for JSON deserialization
        [JsonConstructor]
        internal Profile()
        {
            Windows = new List<Window>();
        }

        internal Profile(string name)
        {
            Name = name;
        }

        internal Profile Clone()
        {
            Profile clone = new Profile(this.Name);
            foreach (Window window in this.Windows.Where(w => w.IsValid()))
            {
                clone.Windows.Add(window.Clone());
            }
            return clone;
        }
    }

    /// <summary>
    /// Collection of profiles with active profile tracking
    /// </summary>
    [Serializable]
    internal class ProfileCollection
    {
        [JsonProperty] // Ensures the property is serialized
        internal List<Profile> Profiles { get; set; } = new List<Profile>();

        [JsonProperty] // Ensures the property is serialized
        internal Profile SelectedProfile { get; set; }

        [JsonConstructor]
        internal ProfileCollection(List<Profile> profiles, Profile selectedProfile)
        {
            Profiles = profiles ?? new List<Profile>();
            SelectedProfile = selectedProfile ?? Profiles.FirstOrDefault();
        }

        internal ProfileCollection()
        {
            Profiles = new List<Profile>();
            SelectedProfile = null;
        }

        [JsonIgnore] // Prevents this property from being serialized
        internal int SelectedProfileIndex
        {
            get
            {
                return Profiles.IndexOf(SelectedProfile);
            }
            set
            {
                if (value >= 0 && value < Profiles.Count)
                {
                    SelectedProfile = Profiles[value];
                }
            }
        }

        internal ProfileCollection(int maxProfiles)
        {
            // Initialize with default profiles
            for (int i = 0; i < maxProfiles; i++)
            {
                Profiles.Add(new Profile($"Profile {i + 1}"));
            }

            // Set first profile as selected by default
            SelectedProfile = Profiles.FirstOrDefault();
        }
    }
}
