namespace quizGame
{
    partial class SpecialQuizForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAnswer1 = new System.Windows.Forms.Button();
            this.btnAnswer2 = new System.Windows.Forms.Button();
            this.btnAnswer3 = new System.Windows.Forms.Button();
            this.btnAnswer4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(36, 23);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(44, 16);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "label1";
            // 
            // lblQuestion
            // 
            this.lblQuestion.AutoSize = true;
            this.lblQuestion.Location = new System.Drawing.Point(139, 63);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(44, 16);
            this.lblQuestion.TabIndex = 1;
            this.lblQuestion.Text = "label1";
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.Location = new System.Drawing.Point(139, 89);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(44, 16);
            this.lblTimer.TabIndex = 2;
            this.lblTimer.Text = "label1";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(609, 32);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(77, 33);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "sluiten";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAnswer1
            // 
            this.btnAnswer1.Location = new System.Drawing.Point(92, 191);
            this.btnAnswer1.Name = "btnAnswer1";
            this.btnAnswer1.Size = new System.Drawing.Size(125, 80);
            this.btnAnswer1.TabIndex = 4;
            this.btnAnswer1.Text = "button1";
            this.btnAnswer1.UseVisualStyleBackColor = true;
            this.btnAnswer1.Click += new System.EventHandler(this.btnAnswer1_Click);
            // 
            // btnAnswer2
            // 
            this.btnAnswer2.Location = new System.Drawing.Point(232, 191);
            this.btnAnswer2.Name = "btnAnswer2";
            this.btnAnswer2.Size = new System.Drawing.Size(125, 80);
            this.btnAnswer2.TabIndex = 5;
            this.btnAnswer2.Text = "button2";
            this.btnAnswer2.UseVisualStyleBackColor = true;
            this.btnAnswer2.Click += new System.EventHandler(this.btnAnswer2_Click);
            // 
            // btnAnswer3
            // 
            this.btnAnswer3.Location = new System.Drawing.Point(376, 191);
            this.btnAnswer3.Name = "btnAnswer3";
            this.btnAnswer3.Size = new System.Drawing.Size(125, 80);
            this.btnAnswer3.TabIndex = 6;
            this.btnAnswer3.Text = "button3";
            this.btnAnswer3.UseVisualStyleBackColor = true;
            this.btnAnswer3.Click += new System.EventHandler(this.btnAnswer3_Click);
            // 
            // btnAnswer4
            // 
            this.btnAnswer4.Location = new System.Drawing.Point(520, 191);
            this.btnAnswer4.Name = "btnAnswer4";
            this.btnAnswer4.Size = new System.Drawing.Size(125, 80);
            this.btnAnswer4.TabIndex = 7;
            this.btnAnswer4.Text = "button4";
            this.btnAnswer4.UseVisualStyleBackColor = true;
            this.btnAnswer4.Click += new System.EventHandler(this.btnAnswer4_Click);
            // 
            // SpecialQuizForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnAnswer4);
            this.Controls.Add(this.btnAnswer3);
            this.Controls.Add(this.btnAnswer2);
            this.Controls.Add(this.btnAnswer1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.lblQuestion);
            this.Controls.Add(this.lblTitle);
            this.Name = "SpecialQuizForm";
            this.Text = "SpecialQuizForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAnswer1;
        private System.Windows.Forms.Button btnAnswer2;
        private System.Windows.Forms.Button btnAnswer3;
        private System.Windows.Forms.Button btnAnswer4;
    }
}