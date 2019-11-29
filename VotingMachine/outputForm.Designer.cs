namespace VotingMachine
{
    partial class outputForm
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
            this.statesDropdown = new System.Windows.Forms.ComboBox();
            this.goButton = new System.Windows.Forms.Button();
            this.sortByBox = new System.Windows.Forms.ComboBox();
            this.resultsBox = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 28.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(272, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(272, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = "ELECTION";
            // 
            // statesDropdown
            // 
            this.statesDropdown.FormattingEnabled = true;
            this.statesDropdown.Location = new System.Drawing.Point(272, 84);
            this.statesDropdown.Name = "statesDropdown";
            this.statesDropdown.Size = new System.Drawing.Size(177, 24);
            this.statesDropdown.TabIndex = 2;
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(466, 83);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(54, 24);
            this.goButton.TabIndex = 3;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButtonClick);
            // 
            // sortByBox
            // 
            this.sortByBox.FormattingEnabled = true;
            this.sortByBox.Location = new System.Drawing.Point(272, 114);
            this.sortByBox.Name = "sortByBox";
            this.sortByBox.Size = new System.Drawing.Size(177, 24);
            this.sortByBox.TabIndex = 4;
            // 
            // resultsBox
            // 
            this.resultsBox.HideSelection = false;
            this.resultsBox.Location = new System.Drawing.Point(12, 160);
            this.resultsBox.Name = "resultsBox";
            this.resultsBox.Size = new System.Drawing.Size(776, 278);
            this.resultsBox.TabIndex = 5;
            this.resultsBox.UseCompatibleStateImageBehavior = false;
            this.resultsBox.View = System.Windows.Forms.View.Details;
            // 
            // outputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.resultsBox);
            this.Controls.Add(this.sortByBox);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.statesDropdown);
            this.Controls.Add(this.label1);
            this.Name = "outputForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox statesDropdown;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.ComboBox sortByBox;
        private System.Windows.Forms.ListView resultsBox;
    }
}

