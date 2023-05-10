using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Window_Position_and_Size
{
    internal class Extensions
    {
    }
    public static class ListBoxExtensions
    {
        public static void AddItemThreadSafe(this ListBox listBox, string item)
        {
            if (listBox.InvokeRequired)
            {
                listBox.Invoke(new MethodInvoker(() => listBox.Items.Add(item)));
            }
            else
            {
                listBox.Items.Add(item);
            }
        }
        public static void ClearItemsThreadSafe(this ListBox listBox)
        {
            if (listBox.InvokeRequired)
            {
                listBox.Invoke(new MethodInvoker(() => listBox.Items.Clear()));
            }
            else
            {
                listBox.Items.Clear();
            }
        }
    }
}
