using Microsoft.VisualBasic;
using Save_Window_Position_and_Size.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Save_Window_Position_and_Size
{
    public partial class IgnoreForm : Form
    {
        private IgnoreListManager _ignoreListManager;
        
        public IgnoreForm(IgnoreListManager ignoreListManager)
        {
            InitializeComponent();
            _ignoreListManager = ignoreListManager;
        }
        
        private void IgnoreListBox_DoubleClick(object sender, EventArgs e)
        {
            if (IgnoreListBox.SelectedIndex == 0)
            {
                string newItem = Interaction.InputBox("Enter a new item:", "Add New Item");
                if (!string.IsNullOrEmpty(newItem))
                {
                    // Add to listbox for UI
                    IgnoreListBox.Items.Insert(1, newItem);
                    
                    // Add to manager (which will auto-save)
                    _ignoreListManager.AddToIgnoreList(newItem);
                }
            }
        }

        private void IgnoreListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (IgnoreListBox.SelectedIndex > 0 && e.KeyCode == Keys.Delete)
            {
                string itemToRemove = IgnoreListBox.Items[IgnoreListBox.SelectedIndex].ToString();
                
                // Remove from listbox for UI
                IgnoreListBox.Items.RemoveAt(IgnoreListBox.SelectedIndex);
                
                // Remove from manager (which will auto-save)
                _ignoreListManager.RemoveFromIgnoreList(itemToRemove);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Save changes and close
            SaveChangesToManager();
        }

        private void IgnoreForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save changes on form close
            SaveChangesToManager();
        }

        private void IgnoreForm_Load(object sender, EventArgs e)
        {
            // Clear any existing items except the "Add item..." entry
            IgnoreListBox.Items.Clear();
            IgnoreListBox.Items.Add("Add item...");
            
            // Add all items from the ignore list manager
            foreach (string item in _ignoreListManager.GetIgnoreList())
            {
                IgnoreListBox.Items.Add(item);
            }
        }
        
        private void SaveChangesToManager()
        {
            // Create a new list from the items in the listbox (excluding "Add item...")
            List<string> updatedList = new List<string>();
            foreach (var item in IgnoreListBox.Items)
            {
                string itemText = item.ToString();
                if (itemText != "Add item...")
                {
                    updatedList.Add(itemText);
                }
            }
            
            // Update the ignore list manager with the new list
            _ignoreListManager.UpdateIgnoreList(updatedList);
        }
    }
}
