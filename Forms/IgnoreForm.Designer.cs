namespace Save_Window_Position_and_Size
{
    partial class IgnoreForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            IgnoreListBox = new ListBox();
            button1 = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // IgnoreListBox
            // 
            IgnoreListBox.BackColor = Color.FromArgb(11, 83, 144);
            IgnoreListBox.ForeColor = SystemColors.Window;
            IgnoreListBox.FormattingEnabled = true;
            IgnoreListBox.ItemHeight = 15;
            IgnoreListBox.Items.AddRange(new object[] { "Add item..." });
            IgnoreListBox.Location = new Point(6, 28);
            IgnoreListBox.Name = "IgnoreListBox";
            IgnoreListBox.Size = new Size(231, 229);
            IgnoreListBox.TabIndex = 0;
            IgnoreListBox.DoubleClick += IgnoreListBox_DoubleClick;
            IgnoreListBox.KeyDown += IgnoreListBox_KeyDown;
            // 
            // button1
            // 
            button1.DialogResult = DialogResult.OK;
            button1.Location = new Point(80, 263);
            button1.Name = "button1";
            button1.Size = new Size(82, 26);
            button1.TabIndex = 1;
            button1.Text = "Close";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Arial", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.Window;
            label1.Location = new Point(59, 3);
            label1.Name = "label1";
            label1.Size = new Size(109, 22);
            label1.TabIndex = 2;
            label1.Text = "Ignore List";
            // 
            // IgnoreForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(11, 83, 144);
            ClientSize = new Size(242, 295);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(IgnoreListBox);
            Name = "IgnoreForm";
            Text = "Ignore List";
            FormClosing += IgnoreForm_FormClosing;
            Load += IgnoreForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox IgnoreListBox;
        private Button button1;
        private Label label1;
    }
}