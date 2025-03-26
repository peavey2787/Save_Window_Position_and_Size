using Newtonsoft.Json;
using Save_Window_Position_and_Size.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Save_Window_Position_and_Size.Classes
{
    internal class QuickLayoutManager
    {
        private WindowManager windowManager;
        private List<QuickLayoutForm> activeLayoutForms;

        public QuickLayoutManager(WindowManager windowManager)
        {
            this.windowManager = windowManager;
            this.activeLayoutForms = new List<QuickLayoutForm>();
        }

        public QuickLayoutForm CreateQuickLayoutForm()
        {
            try
            {
                // Determine next available index
                int nextIndex = 1;
                while (activeLayoutForms.Exists(f => ((string)f.Text).EndsWith(nextIndex.ToString())) &&
                       nextIndex <= Constants.Defaults.MaxQuickLayouts)
                {
                    nextIndex++;
                }

                // Check if we've hit the maximum number of layouts
                if (nextIndex > Constants.Defaults.MaxQuickLayouts)
                {
                    MessageBox.Show($"Maximum of {Constants.Defaults.MaxQuickLayouts} quick layouts reached. Please close one before creating another.",
                        "Maximum Layouts Reached", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }

                // Choose an icon based on the index (cycle through available icons)
                Icon formIcon = GetIconForIndex(nextIndex);

                // Create new form
                var form = new QuickLayoutForm(windowManager, nextIndex, formIcon);

                // Add to active forms
                activeLayoutForms.Add(form);

                // Show the form (it will properly show in taskbar then minimize itself)
                form.Show();

                // Ensure it's registered with Windows
                Application.DoEvents();

                return form;
            }
            catch (Exception ex)
            {
                // Show a message but don't expose the exception details
                MessageBox.Show("Could not create quick layout. Please try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private Icon GetIconForIndex(int index)
        {
            // Cycle through a set of available icons
            switch (index % 5)
            {
                case 1:
                    return Icon.FromHandle(new Bitmap(Properties.Resources.pin).GetHicon());
                case 2:
                    return Icon.FromHandle(new Bitmap(Properties.Resources.redo).GetHicon());
                case 3:
                    return Icon.FromHandle(new Bitmap(Properties.Resources.play_button).GetHicon());
                case 4:
                    return Icon.FromHandle(new Bitmap(Properties.Resources.stop_button).GetHicon());
                case 0: // or case 5
                    return Icon.FromHandle(new Bitmap(Properties.Resources.floppy_disk).GetHicon());
                default:
                    return Icon.FromHandle(new Bitmap(Properties.Resources.refresh_blue_arrows).GetHicon());
            }
        }

        public void CloseAll()
        {
            // Create a copy of the list to avoid modification during enumeration
            var formsToClose = new List<QuickLayoutForm>(activeLayoutForms);

            foreach (var form in formsToClose)
            {
                try
                {
                    // Remove from our tracking list first
                    activeLayoutForms.Remove(form);

                    // Then close the form
                    form.Close();

                    // Dispose the form
                    form.Dispose();
                }
                catch
                {
                    // Ignore errors when closing forms
                }
            }

            // Clear the list to be sure
            activeLayoutForms.Clear();
        }
    }
}