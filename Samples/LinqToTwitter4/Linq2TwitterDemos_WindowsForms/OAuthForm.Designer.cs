namespace Linq2TwitterDemos_WindowsForms
{
    partial class OAuthForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.SubmitPinButton = new System.Windows.Forms.Button();
            this.PinTextBox = new System.Windows.Forms.TextBox();
            this.PinLabel = new System.Windows.Forms.Label();
            this.OAuthWebBrowser = new System.Windows.Forms.WebBrowser();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SubmitPinButton);
            this.panel1.Controls.Add(this.PinTextBox);
            this.panel1.Controls.Add(this.PinLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 319);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(584, 42);
            this.panel1.TabIndex = 1;
            // 
            // SubmitPinButton
            // 
            this.SubmitPinButton.Location = new System.Drawing.Point(497, 11);
            this.SubmitPinButton.Name = "SubmitPinButton";
            this.SubmitPinButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitPinButton.TabIndex = 2;
            this.SubmitPinButton.Text = "Submit Pin";
            this.SubmitPinButton.UseVisualStyleBackColor = true;
            this.SubmitPinButton.Click += new System.EventHandler(this.SubmitPinButton_Click);
            // 
            // PinTextBox
            // 
            this.PinTextBox.Location = new System.Drawing.Point(38, 13);
            this.PinTextBox.Name = "PinTextBox";
            this.PinTextBox.Size = new System.Drawing.Size(453, 20);
            this.PinTextBox.TabIndex = 1;
            // 
            // PinLabel
            // 
            this.PinLabel.AutoSize = true;
            this.PinLabel.Location = new System.Drawing.Point(4, 16);
            this.PinLabel.Name = "PinLabel";
            this.PinLabel.Size = new System.Drawing.Size(28, 13);
            this.PinLabel.TabIndex = 0;
            this.PinLabel.Text = "PIN:";
            // 
            // OAuthWebBrowser
            // 
            this.OAuthWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OAuthWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.OAuthWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.OAuthWebBrowser.Name = "OAuthWebBrowser";
            this.OAuthWebBrowser.Size = new System.Drawing.Size(584, 319);
            this.OAuthWebBrowser.TabIndex = 2;
            // 
            // OAuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.OAuthWebBrowser);
            this.Controls.Add(this.panel1);
            this.Name = "OAuthForm";
            this.Text = "Authorize this Application";
            this.Load += new System.EventHandler(this.OAuthForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button SubmitPinButton;
        private System.Windows.Forms.TextBox PinTextBox;
        private System.Windows.Forms.Label PinLabel;
        private System.Windows.Forms.WebBrowser OAuthWebBrowser;

    }
}