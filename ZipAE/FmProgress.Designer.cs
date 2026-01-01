namespace ZipAE
{
	partial class FmProgress
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            progBar = new ProgressBar();
            lblFileName = new Label();
            folderDlg = new FolderBrowserDialog();
            SuspendLayout();
            // 
            // progBar
            // 
            progBar.Location = new Point(13, 30);
            progBar.Margin = new Padding(4);
            progBar.Name = "progBar";
            progBar.Size = new Size(294, 25);
            progBar.TabIndex = 0;
            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Location = new Point(13, 9);
            lblFileName.Margin = new Padding(4, 0, 4, 0);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(24, 15);
            lblFileName.TabIndex = 1;
            lblFileName.Text = "file";
            // 
            // FmProgress
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(320, 66);
            Controls.Add(lblFileName);
            Controls.Add(progBar);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FmProgress";
            StartPosition = FormStartPosition.Manual;
            Text = "FmProgress";
            FormClosing += FmProgress_FormClosing;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progBar;
		private System.Windows.Forms.Label lblFileName;
		private System.Windows.Forms.FolderBrowserDialog folderDlg;
    }
}