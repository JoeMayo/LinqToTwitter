namespace Linq2TwitterDemos_WindowsForms.StatusDemos
{
    partial class HomeTimelineForm
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
            this.TweetDataGridView = new System.Windows.Forms.DataGridView();
            this.UserImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.ScreenName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TweetText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.TweetDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // TweetDataGridView
            // 
            this.TweetDataGridView.AllowUserToAddRows = false;
            this.TweetDataGridView.AllowUserToDeleteRows = false;
            this.TweetDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TweetDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UserImage,
            this.ScreenName,
            this.TweetText});
            this.TweetDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TweetDataGridView.Location = new System.Drawing.Point(0, 0);
            this.TweetDataGridView.Name = "TweetDataGridView";
            this.TweetDataGridView.ReadOnly = true;
            this.TweetDataGridView.Size = new System.Drawing.Size(584, 361);
            this.TweetDataGridView.TabIndex = 0;
            // 
            // UserImage
            // 
            this.UserImage.HeaderText = "Image";
            this.UserImage.Name = "UserImage";
            this.UserImage.ReadOnly = true;
            // 
            // ScreenName
            // 
            this.ScreenName.HeaderText = "Screen Name";
            this.ScreenName.Name = "ScreenName";
            this.ScreenName.ReadOnly = true;
            // 
            // TweetText
            // 
            this.TweetText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TweetText.HeaderText = "Tweet";
            this.TweetText.Name = "TweetText";
            this.TweetText.ReadOnly = true;
            // 
            // HomeTimelineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.TweetDataGridView);
            this.Name = "HomeTimelineForm";
            this.Text = "Home Timeline";
            this.Load += new System.EventHandler(this.HomeTimelineForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TweetDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView TweetDataGridView;
        private System.Windows.Forms.DataGridViewImageColumn UserImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScreenName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TweetText;
    }
}