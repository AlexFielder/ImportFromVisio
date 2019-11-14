namespace ImportFromVisio
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.m_visioFileTextBox = new System.Windows.Forms.TextBox();
            this.m_visioFileButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.m_lcDefNameTextBox = new System.Windows.Forms.TextBox();
            this.m_defaultStateComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.m_createButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.m_lcDescTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Visio File:";
            // 
            // m_visioFileTextBox
            // 
            this.m_visioFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_visioFileTextBox.Location = new System.Drawing.Point(69, 12);
            this.m_visioFileTextBox.Name = "m_visioFileTextBox";
            this.m_visioFileTextBox.Size = new System.Drawing.Size(189, 20);
            this.m_visioFileTextBox.TabIndex = 1;
            this.m_visioFileTextBox.TextChanged += new System.EventHandler(this.m_visioFileTextBox_TextChanged);
            // 
            // m_visioFileButton
            // 
            this.m_visioFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_visioFileButton.Location = new System.Drawing.Point(264, 12);
            this.m_visioFileButton.Name = "m_visioFileButton";
            this.m_visioFileButton.Size = new System.Drawing.Size(24, 21);
            this.m_visioFileButton.TabIndex = 2;
            this.m_visioFileButton.Text = "...";
            this.m_visioFileButton.UseVisualStyleBackColor = true;
            this.m_visioFileButton.Click += new System.EventHandler(this.m_visioFileButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Lifecycle Definition Name:";
            // 
            // m_lcDefNameTextBox
            // 
            this.m_lcDefNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lcDefNameTextBox.Location = new System.Drawing.Point(15, 62);
            this.m_lcDefNameTextBox.Name = "m_lcDefNameTextBox";
            this.m_lcDefNameTextBox.Size = new System.Drawing.Size(273, 20);
            this.m_lcDefNameTextBox.TabIndex = 3;
            // 
            // m_defaultStateComboBox
            // 
            this.m_defaultStateComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_defaultStateComboBox.FormattingEnabled = true;
            this.m_defaultStateComboBox.Location = new System.Drawing.Point(15, 208);
            this.m_defaultStateComboBox.Name = "m_defaultStateComboBox";
            this.m_defaultStateComboBox.Size = new System.Drawing.Size(273, 21);
            this.m_defaultStateComboBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Default Lifecycle State:";
            // 
            // m_createButton
            // 
            this.m_createButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_createButton.Location = new System.Drawing.Point(91, 252);
            this.m_createButton.Name = "m_createButton";
            this.m_createButton.Size = new System.Drawing.Size(197, 23);
            this.m_createButton.TabIndex = 6;
            this.m_createButton.Text = "Create Lifecycle Definition";
            this.m_createButton.UseVisualStyleBackColor = true;
            this.m_createButton.Click += new System.EventHandler(this.m_createButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Lifecycle Definition Description:";
            // 
            // m_lcDescTextBox
            // 
            this.m_lcDescTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lcDescTextBox.Location = new System.Drawing.Point(15, 108);
            this.m_lcDescTextBox.Multiline = true;
            this.m_lcDescTextBox.Name = "m_lcDescTextBox";
            this.m_lcDescTextBox.Size = new System.Drawing.Size(273, 74);
            this.m_lcDescTextBox.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 287);
            this.Controls.Add(this.m_lcDescTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_createButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.m_defaultStateComboBox);
            this.Controls.Add(this.m_lcDefNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_visioFileButton);
            this.Controls.Add(this.m_visioFileTextBox);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(308, 218);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Import From Visio";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox m_visioFileTextBox;
        private System.Windows.Forms.Button m_visioFileButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox m_lcDefNameTextBox;
        private System.Windows.Forms.ComboBox m_defaultStateComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button m_createButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox m_lcDescTextBox;
    }
}

