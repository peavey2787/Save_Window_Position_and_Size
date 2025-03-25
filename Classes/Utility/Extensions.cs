using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Save_Window_Position_and_Size.Classes;

namespace Save_Window_Position_and_Size.Classes
{
    internal class Extensions
    {
    }

    /// <summary>
    /// Provides thread-safe extension methods for ListBox controls
    /// </summary>
    public static class ListBoxExtensions
    {
        /// <summary>
        /// Adds a string item to a ListBox in a thread-safe manner
        /// </summary>
        public static void AddItemThreadSafe(this ListBox listBox, string item)
        {
            try
            {
                if (listBox == null || item == null) return;

                if (listBox.InvokeRequired)
                {
                    listBox.Invoke(new MethodInvoker(() =>
                    {
                        try
                        {
                            listBox.Items.Add(item);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error adding string item on UI thread: {ex.Message}");
                        }
                    }));
                }
                else
                {
                    listBox.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in AddItemThreadSafe(string): {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a Window object to a ListBox in a thread-safe manner
        /// </summary>
        internal static void AddItemThreadSafe(this ListBox listBox, Window item)
        {
            try
            {
                if (listBox == null || item == null) return;

                if (listBox.InvokeRequired)
                {
                    listBox.Invoke(new MethodInvoker(() =>
                    {
                        try
                        {
                            listBox.Items.Add(item);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error adding Window object on UI thread: {ex.Message}");
                        }
                    }));
                }
                else
                {
                    listBox.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in AddItemThreadSafe(Window): {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all items from a ListBox in a thread-safe manner
        /// </summary>
        public static void ClearItemsThreadSafe(this ListBox listBox)
        {
            try
            {
                if (listBox == null) return;

                if (listBox.InvokeRequired)
                {
                    listBox.Invoke(new MethodInvoker(() =>
                    {
                        try
                        {
                            listBox.Items.Clear();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error clearing items on UI thread: {ex.Message}");
                        }
                    }));
                }
                else
                {
                    listBox.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ClearItemsThreadSafe: {ex.Message}");
            }
        }
    }
}
