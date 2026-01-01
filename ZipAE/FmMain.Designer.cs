namespace ZipAE
{
	partial class FmMain
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
            menuStrip = new MenuStrip();
            tsmFile = new ToolStripMenuItem();
            tsmFileExit = new ToolStripMenuItem();
            tsmHelp = new ToolStripMenuItem();
            tsmHelpInfo = new ToolStripMenuItem();
            grpExtractDist = new GroupBox();
            txtExtPath = new TextBox();
            cbExtFolder = new ComboBox();
            btnExtRef = new Button();
            grpCompDist = new GroupBox();
            txtCompPath = new TextBox();
            cbCompFolder = new ComboBox();
            btnCompRef = new Button();
            chkExtractAllZip = new CheckBox();
            grpCompLv = new GroupBox();
            cbCompLv = new ComboBox();
            folderDlg = new FolderBrowserDialog();
            grpExtract = new GroupBox();
            grpCompress = new GroupBox();
            grpEncryptLv = new GroupBox();
            cbEncryptLv = new ComboBox();
            grpOther = new GroupBox();
            btnSaveSetting = new Button();
            grpExtPswd = new GroupBox();
            rbPswdNone = new RadioButton();
            rbPswdUse = new RadioButton();
            txtPswd = new MaskedTextBox();
            menuStrip.SuspendLayout();
            grpExtractDist.SuspendLayout();
            grpCompDist.SuspendLayout();
            grpCompLv.SuspendLayout();
            grpExtract.SuspendLayout();
            grpCompress.SuspendLayout();
            grpEncryptLv.SuspendLayout();
            grpOther.SuspendLayout();
            grpExtPswd.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { tsmFile, tsmHelp });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(8, 3, 0, 3);
            menuStrip.Size = new Size(648, 25);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // tsmFile
            // 
            tsmFile.DropDownItems.AddRange(new ToolStripItem[] { tsmFileExit });
            tsmFile.Name = "tsmFile";
            tsmFile.Size = new Size(53, 19);
            tsmFile.Text = "ファイル";
            // 
            // tsmFileExit
            // 
            tsmFileExit.Name = "tsmFileExit";
            tsmFileExit.Size = new Size(180, 22);
            tsmFileExit.Text = "終了";
            tsmFileExit.Click += tsmFileExit_Click;
            // 
            // tsmHelp
            // 
            tsmHelp.DropDownItems.AddRange(new ToolStripItem[] { tsmHelpInfo });
            tsmHelp.Name = "tsmHelp";
            tsmHelp.Size = new Size(48, 19);
            tsmHelp.Text = "ヘルプ";
            // 
            // tsmHelpInfo
            // 
            tsmHelpInfo.Name = "tsmHelpInfo";
            tsmHelpInfo.Size = new Size(180, 22);
            tsmHelpInfo.Text = "バージョン情報";
            tsmHelpInfo.Click += tsmHelpInfo_Click;
            // 
            // grpExtractDist
            // 
            grpExtractDist.Controls.Add(txtExtPath);
            grpExtractDist.Controls.Add(cbExtFolder);
            grpExtractDist.Controls.Add(btnExtRef);
            grpExtractDist.Location = new Point(8, 25);
            grpExtractDist.Margin = new Padding(5, 4, 5, 4);
            grpExtractDist.Name = "grpExtractDist";
            grpExtractDist.Padding = new Padding(5, 4, 5, 4);
            grpExtractDist.Size = new Size(294, 102);
            grpExtractDist.TabIndex = 0;
            grpExtractDist.TabStop = false;
            grpExtractDist.Text = "出力先";
            // 
            // txtExtPath
            // 
            txtExtPath.Location = new Point(8, 62);
            txtExtPath.Margin = new Padding(5, 4, 5, 4);
            txtExtPath.Name = "txtExtPath";
            txtExtPath.ReadOnly = true;
            txtExtPath.Size = new Size(276, 24);
            txtExtPath.TabIndex = 5;
            // 
            // cbExtFolder
            // 
            cbExtFolder.DropDownStyle = ComboBoxStyle.DropDownList;
            cbExtFolder.FormattingEnabled = true;
            cbExtFolder.Items.AddRange(new object[] { "入力ファイルと同じフォルダ", "指定したフォルダ" });
            cbExtFolder.Location = new Point(8, 25);
            cbExtFolder.Margin = new Padding(5, 4, 5, 4);
            cbExtFolder.Name = "cbExtFolder";
            cbExtFolder.Size = new Size(202, 25);
            cbExtFolder.TabIndex = 8;
            cbExtFolder.SelectedIndexChanged += cbExtractFolder_SelectedIndexChanged;
            // 
            // btnExtRef
            // 
            btnExtRef.Location = new Point(218, 24);
            btnExtRef.Margin = new Padding(5, 4, 5, 4);
            btnExtRef.Name = "btnExtRef";
            btnExtRef.Size = new Size(66, 31);
            btnExtRef.TabIndex = 4;
            btnExtRef.Text = "参照";
            btnExtRef.UseVisualStyleBackColor = true;
            btnExtRef.Click += btnExtRef_Click;
            // 
            // grpCompDist
            // 
            grpCompDist.Controls.Add(txtCompPath);
            grpCompDist.Controls.Add(cbCompFolder);
            grpCompDist.Controls.Add(btnCompRef);
            grpCompDist.Location = new Point(8, 25);
            grpCompDist.Margin = new Padding(5, 4, 5, 4);
            grpCompDist.Name = "grpCompDist";
            grpCompDist.Padding = new Padding(5, 4, 5, 4);
            grpCompDist.Size = new Size(294, 102);
            grpCompDist.TabIndex = 6;
            grpCompDist.TabStop = false;
            grpCompDist.Text = "出力先";
            // 
            // txtCompPath
            // 
            txtCompPath.Location = new Point(8, 62);
            txtCompPath.Margin = new Padding(5, 4, 5, 4);
            txtCompPath.Name = "txtCompPath";
            txtCompPath.ReadOnly = true;
            txtCompPath.Size = new Size(276, 24);
            txtCompPath.TabIndex = 5;
            // 
            // cbCompFolder
            // 
            cbCompFolder.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCompFolder.FormattingEnabled = true;
            cbCompFolder.Items.AddRange(new object[] { "入力ファイルと同じフォルダ", "指定したフォルダ" });
            cbCompFolder.Location = new Point(8, 25);
            cbCompFolder.Margin = new Padding(5, 4, 5, 4);
            cbCompFolder.Name = "cbCompFolder";
            cbCompFolder.Size = new Size(202, 25);
            cbCompFolder.TabIndex = 8;
            cbCompFolder.SelectedIndexChanged += cbArchiveFolder_SelectedIndexChanged;
            // 
            // btnCompRef
            // 
            btnCompRef.Location = new Point(218, 24);
            btnCompRef.Margin = new Padding(5, 4, 5, 4);
            btnCompRef.Name = "btnCompRef";
            btnCompRef.Size = new Size(66, 31);
            btnCompRef.TabIndex = 4;
            btnCompRef.Text = "参照";
            btnCompRef.UseVisualStyleBackColor = true;
            btnCompRef.Click += btnArcRef_Click;
            // 
            // chkExtractAllZip
            // 
            chkExtractAllZip.AutoSize = true;
            chkExtractAllZip.Location = new Point(8, 24);
            chkExtractAllZip.Margin = new Padding(5, 4, 5, 4);
            chkExtractAllZip.Name = "chkExtractAllZip";
            chkExtractAllZip.Size = new Size(316, 21);
            chkExtractAllZip.TabIndex = 5;
            chkExtractAllZip.Text = "ZIPファイルのみを複数入力した場合は展開処理とする。";
            chkExtractAllZip.UseVisualStyleBackColor = true;
            chkExtractAllZip.CheckedChanged += chkExtractAllZip_CheckedChanged;
            // 
            // grpCompLv
            // 
            grpCompLv.Controls.Add(cbCompLv);
            grpCompLv.Location = new Point(8, 135);
            grpCompLv.Margin = new Padding(5, 4, 5, 4);
            grpCompLv.Name = "grpCompLv";
            grpCompLv.Padding = new Padding(5, 4, 5, 4);
            grpCompLv.Size = new Size(294, 47);
            grpCompLv.TabIndex = 4;
            grpCompLv.TabStop = false;
            grpCompLv.Text = "圧縮レベル";
            // 
            // cbCompLv
            // 
            cbCompLv.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCompLv.FormattingEnabled = true;
            cbCompLv.Items.AddRange(new object[] { "圧縮なし", "圧縮率優先で圧縮", "処理速度優先で圧縮", "適度なバランスで圧縮" });
            cbCompLv.Location = new Point(82, 14);
            cbCompLv.Margin = new Padding(5, 4, 5, 4);
            cbCompLv.Name = "cbCompLv";
            cbCompLv.Size = new Size(202, 25);
            cbCompLv.TabIndex = 3;
            cbCompLv.SelectedIndexChanged += cbCompressionLevel_SelectedIndexChanged;
            // 
            // grpExtract
            // 
            grpExtract.Controls.Add(grpExtractDist);
            grpExtract.Location = new Point(6, 31);
            grpExtract.Margin = new Padding(5, 4, 5, 4);
            grpExtract.Name = "grpExtract";
            grpExtract.Padding = new Padding(5, 4, 5, 4);
            grpExtract.Size = new Size(312, 137);
            grpExtract.TabIndex = 2;
            grpExtract.TabStop = false;
            grpExtract.Text = "展開の設定";
            // 
            // grpCompress
            // 
            grpCompress.Controls.Add(grpEncryptLv);
            grpCompress.Controls.Add(grpCompDist);
            grpCompress.Controls.Add(grpCompLv);
            grpCompress.Location = new Point(328, 31);
            grpCompress.Margin = new Padding(5, 4, 5, 4);
            grpCompress.Name = "grpCompress";
            grpCompress.Padding = new Padding(5, 4, 5, 4);
            grpCompress.Size = new Size(312, 247);
            grpCompress.TabIndex = 3;
            grpCompress.TabStop = false;
            grpCompress.Text = "圧縮の設定";
            // 
            // grpEncryptLv
            // 
            grpEncryptLv.Controls.Add(cbEncryptLv);
            grpEncryptLv.Location = new Point(8, 190);
            grpEncryptLv.Margin = new Padding(5, 4, 5, 4);
            grpEncryptLv.Name = "grpEncryptLv";
            grpEncryptLv.Padding = new Padding(5, 4, 5, 4);
            grpEncryptLv.Size = new Size(294, 47);
            grpEncryptLv.TabIndex = 5;
            grpEncryptLv.TabStop = false;
            grpEncryptLv.Text = "暗号強度";
            // 
            // cbEncryptLv
            // 
            cbEncryptLv.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEncryptLv.FormattingEnabled = true;
            cbEncryptLv.Items.AddRange(new object[] { "強い (AES256)", "弱い (ZipCrypto)" });
            cbEncryptLv.Location = new Point(82, 14);
            cbEncryptLv.Margin = new Padding(5, 4, 5, 4);
            cbEncryptLv.Name = "cbEncryptLv";
            cbEncryptLv.Size = new Size(202, 25);
            cbEncryptLv.TabIndex = 3;
            cbEncryptLv.SelectedIndexChanged += cbEncryptLv_SelectedIndexChanged;
            // 
            // grpOther
            // 
            grpOther.Controls.Add(chkExtractAllZip);
            grpOther.Location = new Point(6, 286);
            grpOther.Margin = new Padding(5, 4, 5, 4);
            grpOther.Name = "grpOther";
            grpOther.Padding = new Padding(5, 4, 5, 4);
            grpOther.Size = new Size(478, 51);
            grpOther.TabIndex = 6;
            grpOther.TabStop = false;
            grpOther.Text = "その他の設定";
            // 
            // btnSaveSetting
            // 
            btnSaveSetting.Location = new Point(494, 298);
            btnSaveSetting.Margin = new Padding(5, 4, 5, 4);
            btnSaveSetting.Name = "btnSaveSetting";
            btnSaveSetting.Size = new Size(146, 33);
            btnSaveSetting.TabIndex = 7;
            btnSaveSetting.Text = "設定を保存する";
            btnSaveSetting.UseVisualStyleBackColor = true;
            btnSaveSetting.Click += btnSaveSetting_Click;
            // 
            // grpExtPswd
            // 
            grpExtPswd.Controls.Add(rbPswdNone);
            grpExtPswd.Controls.Add(rbPswdUse);
            grpExtPswd.Controls.Add(txtPswd);
            grpExtPswd.Location = new Point(6, 176);
            grpExtPswd.Margin = new Padding(5, 4, 5, 4);
            grpExtPswd.Name = "grpExtPswd";
            grpExtPswd.Padding = new Padding(5, 4, 5, 4);
            grpExtPswd.Size = new Size(312, 102);
            grpExtPswd.TabIndex = 8;
            grpExtPswd.TabStop = false;
            grpExtPswd.Text = "パスワードの設定";
            // 
            // rbPswdNone
            // 
            rbPswdNone.AutoSize = true;
            rbPswdNone.Checked = true;
            rbPswdNone.Location = new Point(205, 24);
            rbPswdNone.Name = "rbPswdNone";
            rbPswdNone.Size = new Size(72, 21);
            rbPswdNone.TabIndex = 12;
            rbPswdNone.TabStop = true;
            rbPswdNone.Text = "使わない";
            rbPswdNone.UseVisualStyleBackColor = true;
            rbPswdNone.CheckedChanged += rbPswd_CheckedChanged;
            // 
            // rbPswdUse
            // 
            rbPswdUse.AutoSize = true;
            rbPswdUse.Location = new Point(8, 24);
            rbPswdUse.Name = "rbPswdUse";
            rbPswdUse.Size = new Size(191, 21);
            rbPswdUse.TabIndex = 10;
            rbPswdUse.Text = "展開/暗号化圧縮で下記を使う";
            rbPswdUse.UseVisualStyleBackColor = true;
            rbPswdUse.CheckedChanged += rbPswd_CheckedChanged;
            // 
            // txtPswd
            // 
            txtPswd.Enabled = false;
            txtPswd.Location = new Point(8, 59);
            txtPswd.Name = "txtPswd";
            txtPswd.Size = new Size(284, 24);
            txtPswd.TabIndex = 0;
            txtPswd.UseSystemPasswordChar = true;
            txtPswd.Enter += txtPswd_Enter;
            txtPswd.Leave += txtPswd_Leave;
            txtPswd.MouseEnter += txtPswd_MouseEnter;
            txtPswd.MouseLeave += txtPswd_MouseLeave;
            // 
            // FmMain
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(648, 345);
            Controls.Add(grpExtPswd);
            Controls.Add(btnSaveSetting);
            Controls.Add(grpOther);
            Controls.Add(grpCompress);
            Controls.Add(grpExtract);
            Controls.Add(menuStrip);
            Font = new Font("Meiryo UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip;
            Margin = new Padding(5, 4, 5, 4);
            MaximizeBox = false;
            Name = "FmMain";
            StartPosition = FormStartPosition.Manual;
            Text = "FmMain";
            Load += FmMain_Load;
            DragDrop += FmMain_DragDrop;
            DragEnter += FmMain_DragEnter;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            grpExtractDist.ResumeLayout(false);
            grpExtractDist.PerformLayout();
            grpCompDist.ResumeLayout(false);
            grpCompDist.PerformLayout();
            grpCompLv.ResumeLayout(false);
            grpExtract.ResumeLayout(false);
            grpCompress.ResumeLayout(false);
            grpEncryptLv.ResumeLayout(false);
            grpOther.ResumeLayout(false);
            grpOther.PerformLayout();
            grpExtPswd.ResumeLayout(false);
            grpExtPswd.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem tsmFile;
		private System.Windows.Forms.ToolStripMenuItem tsmHelp;
		private System.Windows.Forms.ToolStripMenuItem tsmHelpInfo;
		private System.Windows.Forms.ToolStripMenuItem tsmFileExit;
		private System.Windows.Forms.GroupBox grpExtractDist;
		private System.Windows.Forms.TextBox txtExtPath;
		private System.Windows.Forms.ComboBox cbExtFolder;
		private System.Windows.Forms.Button btnExtRef;
		private System.Windows.Forms.GroupBox grpCompDist;
		private System.Windows.Forms.TextBox txtCompPath;
		private System.Windows.Forms.ComboBox cbCompFolder;
		private System.Windows.Forms.Button btnCompRef;
		private System.Windows.Forms.CheckBox chkExtractAllZip;
		private System.Windows.Forms.GroupBox grpCompLv;
		private System.Windows.Forms.ComboBox cbCompLv;
		private System.Windows.Forms.FolderBrowserDialog folderDlg;
		private System.Windows.Forms.GroupBox grpExtract;
		private System.Windows.Forms.GroupBox grpCompress;
		private System.Windows.Forms.GroupBox grpOther;
		private System.Windows.Forms.Button btnSaveSetting;
        private GroupBox grpExtPswd;
        private RadioButton rbPswdUse;
        private MaskedTextBox txtPswd;
        private RadioButton rbPswdNone;
        private GroupBox grpEncryptLv;
        private ComboBox cbEncryptLv;
    }
}

