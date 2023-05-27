using Microsoft.VisualBasic;
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
        List<string> IgnoreList { get; set; }
        public IgnoreForm(List<string> ignoreList)
        {
            InitializeComponent();
            IgnoreList = ignoreList;
        }
        public List<string> GetIgnoreList()
        {
            return IgnoreList;
        }
        private void IgnoreListBox_DoubleClick(object sender, EventArgs e)
        {
            if (IgnoreListBox.SelectedIndex == 0)
            {
                string newItem = Interaction.InputBox("Enter a new item:", "Add New Item");
                if (!string.IsNullOrEmpty(newItem))
                {
                    IgnoreListBox.Items.Insert(1, newItem);
                }
            }
        }

        private void IgnoreListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (IgnoreListBox.SelectedIndex >= 0 && e.KeyCode == Keys.Delete)
            {
                IgnoreListBox.Items.RemoveAt(IgnoreListBox.SelectedIndex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void IgnoreForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IgnoreList = new List<string>();
            foreach (string item in IgnoreListBox.Items)
                if (item != "Add item...")
                    IgnoreList.Add(item);
        }

        private void IgnoreForm_Load(object sender, EventArgs e)
        {
            foreach (string item in IgnoreList) IgnoreListBox.Items.Add(item);
        }
    }
}
