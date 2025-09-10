namespace Quizmester
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txbSearch = new TextBox();
            btnSearch = new Button();
            Reset = new Button();
            dgvInformation = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvInformation).BeginInit();
            SuspendLayout();
            // 
            // txbSearch
            // 
            txbSearch.Location = new Point(13, 12);
            txbSearch.Name = "txbSearch";
            txbSearch.Size = new Size(135, 27);
            txbSearch.TabIndex = 0;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(154, 12);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(110, 27);
            btnSearch.TabIndex = 1;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // Reset
            // 
            Reset.Location = new Point(270, 12);
            Reset.Name = "Reset";
            Reset.Size = new Size(104, 27);
            Reset.TabIndex = 2;
            Reset.Text = "Reset";
            Reset.UseVisualStyleBackColor = true;
            // 
            // dgvInformation
            // 
            dgvInformation.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInformation.Location = new Point(13, 68);
            dgvInformation.Name = "dgvInformation";
            dgvInformation.RowHeadersWidth = 51;
            dgvInformation.Size = new Size(770, 363);
            dgvInformation.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgvInformation);
            Controls.Add(Reset);
            Controls.Add(btnSearch);
            Controls.Add(txbSearch);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dgvInformation).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txbSearch;
        private Button btnSearch;
        private Button Reset;
        private DataGridView dgvInformation;
    }
}
