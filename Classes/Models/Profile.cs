using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Window_Position_and_Size.Classes
{
    /// <summary>
    /// Represents a profile containing a collection of windows
    /// </summary>
    internal class Profile
    {
        public string Name { get; set; }
        public List<Window> Windows { get; set; } = new List<Window>();

        public Profile(string name)
        {
            Name = name;
        }

        public Profile Clone()
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
    internal class ProfileCollection
    {
        public List<Profile> Profiles { get; set; } = new List<Profile>();
        public Profile SelectedProfile { get; set; }
        public int SelectedProfileIndex
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

        public ProfileCollection(int maxProfiles)
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