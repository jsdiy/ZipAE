using InterProcCom;
using Ionic.Zip;

namespace ZipAE
{
	public partial class FmMain : Form
	{
		private const string SettingFileName = @"zipae.setting";
		private SettingData settingData;

		//コンストラクタ
		public FmMain()
		{
			InitializeComponent();
			this.Text = Application.ProductName!;

			settingData = new SettingData();
		}

		//Form起動
		/*
		様々な経路でファイルリストを取得する。そのうちコマンドライン引数から取得した場合のみ、アプリのフルパスがリスト先頭に格納されている。
		それは圧縮/展開対象ではないので排除する必要がある。

		①アプリを起動した際、それが多重起動ではなく最初の起動だった場合、ここForm_Load()へ来る。
			そのうえで、
			①-a:	アプリのexeファイル/ショートカットアイコンにファイルをD&Dした場合、コマンドライン引数args[1]以降にファイルが格納されている。
			①-b:	単なる起動だった場合、コマンドライン引数args[1]以降はない。
					そのうえで、
					①-c:	FormにファイルをD&Dした場合、FmMain_DragDrop()でstring[] filesが取得される。
							files[0]以降にファイルが格納されている。
			※①-a,bいずれもargs[0]にはアプリのフルパスが格納されている。
		
		②アプリを起動した際、それが多重起動だった場合、ここForm_Load()へは来ない。
			②-a:	アプリのexeファイル/ショートカットアイコンにファイルをD&Dした場合、WndProc()にstring[] filesが渡ってくる。
					files[0]以降にファイルが格納されている。
			②-b:	単なる多重起動だった場合、先行アプリがActivate()されるだけ。
		
		まとめ：	ファイルリストの先頭にアプリのフルパスが格納されている状態は、コマンドライン引数からD&Dファイルを取得する場合のみ。
		対策：	Form_Load()で、string[] args = Environment.GetCommandLineArgs(); args[0]="";	として潰す。
		*/
		private async void FmMain_Load(object sender, EventArgs e)
		{
			//設定ファイルを読み込んで値を復元する
			string settingFile = GetSettingFilePath();
			settingData = SettingData.LoadFromXml(settingFile);

			this.Location = settingData.FmMainLocation;
			chkExtractAllZip.Checked = settingData.IsExtractAllZip;

			cbExtFolder.SelectedIndex = settingData.Extract.CbIndexDirectory;
			txtExtPath.Text = settingData.Extract.TxtDirectoryPath;

			cbCompFolder.SelectedIndex = settingData.Compress.CbIndexDirectory;
			txtCompPath.Text = settingData.Compress.TxtDirectoryPath;
			cbCompLv.SelectedIndex = settingData.Compress.CbIndexCompressLevel;
			cbEncryptLv.SelectedIndex = settingData.Compress.CbIndexEncryptLevel;

			//起動時の引数のうち、args[0]に格納されているアプリのフルパスを削除する
			//args[1]以降があれば圧縮/展開処理を開始する
			string[] args = Environment.GetCommandLineArgs();
			args[0] = string.Empty;
			if (1 < args.Length)
			{
				this.WindowState = FormWindowState.Minimized;   //フォームを最小化する
				await AppExecute(args);		//この中でアプリは終了(this.close())する
			}

			//引数なしの場合は普通に起動（Formを表示）させる
		}

		//設定ファイルのフルパスを取得する
		private string GetSettingFilePath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingFileName);
		}

		//フォームへD&D
		private void FmMain_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false)
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		//フォームへD&D
		private async void FmMain_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data?.GetData(DataFormats.FileDrop) is string[] filePaths)
			{
				await AppExecute(filePaths);
			}
		}

		//ショートカットアイコンへD&Dされた場合の対応（外部プロセスからの呼び出しをキャッチする）
		//・ _ = AppExecute(args);		//Task結果を破棄する（呼び出し関数内で例外処理している場合）
		//・ AppExecuteForWndProc(args);	//Task結果に対応する（呼び出し関数内で例外処理していない場合）
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
			case IPC.WM_COPYDATA:
				if (m.WParam != (IntPtr)IPC.AppIdValue) { break; }
				string[] args = IPC.ReceiveArgs(m);
				_ = AppExecute(args);
				m.Result = (IntPtr)1;
				return;

			default:
				break;
			}

			base.WndProc(ref m);
		}
		/*
		private void AppExecuteForWndProc(string[] args)
		{
			//ラムダ式を使った書き方（参考 https://x.com/i/grok?conversation=2003773412377022472）
			_ = AppExecute(args).ContinueWith(t =>
			{
				if (t.IsFaulted) { タスクでエラー発生した場合の処理; }
				if (t.IsCanceled) { タスクがキャンセルされた場合の処理; }
				など
			},
			TaskScheduler.FromCurrentSynchronizationContext());
			
			//メソッドを呼び出す書き方
			_ = AppExecute(args).ContinueWith(OnAppExecuteCompleted,	//タスク完了して次に呼び出されるメソッド
				TaskScheduler.FromCurrentSynchronizationContext());		//それをUIスレッドで実行することの指示
		}
		//
		private void OnAppExecuteCompleted(Task t)
		{
			if (t.IsFaulted) { タスクでエラー発生した場合の処理; }
			if (t.IsCanceled) { タスクがキャンセルされた場合の処理; }
		}
		*/

		//メイン処理
		private async Task AppExecute(string[] fileList)
		{
			//ファイルチェック（存在するファイル/ディレクトリのみのリストにする）
			fileList = ValidateFileList(fileList);
			if (fileList.Length == 0)
			{
				string mes = $"有効なファイル/フォルダが見つかりませんでした。{Environment.NewLine}処理を中止します。";
				MessageBox.Show(mes, "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			//圧縮か展開かを判断する
			/*
			入力ファイルが1個で、zipなら展開
			入力ファイルが1個で、非zipなら圧縮
			入力ファイルが2個以上で、全部zipで、IsExtractAllZip=trueなら展開
			入力ファイルが2個以上で、それ以外なら圧縮
			*/
			EZipMode mode = (IsAllZipFile(fileList) && (fileList.Length == 1 || settingData.IsExtractAllZip))
				? EZipMode.Extract
				: EZipMode.Compress;

			//出力先ディレクトリを取得する
			string destDir = GetDestinationDirectoryPath(mode, fileList.First());
			if (destDir.Equals(string.Empty)) { return; }

			/*	同名ファイルがあったらxxxxx(2).zipのようにすることにした
				さらに、FmProgress.ExtractWorker()でやることにした

			//圧縮時の出力ファイル(xxxxx.zip)を作成する
			string compressedFileName = string.Empty, compressedFilePath = string.Empty;
			if (mode == EZipMode.Compress)
			{
				compressedFileName = $"{Path.GetFileNameWithoutExtension(fileList.First())}.zip";
				compressedFilePath = Path.Combine(destDir, compressedFileName);
				//出力先フォルダに同名ファイル(xxxxx.zip)がある場合は処理を中止する
				if (File.Exists(compressedFilePath))
				{
					string mes = compressedFilePath + Environment.NewLine +
						"は存在します。" + Environment.NewLine +
						"別のフォルダへ移動させるか、リネームするか、削除してください。" + Environment.NewLine +
						"処理を中止します。";
					MessageBox.Show(mes, "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
			}
			*/

			//圧縮/展開パラメータをセットする
			string? pswd = rbPswdNone.Checked ? null : txtPswd.Text;
			EncryptionAlgorithm encType = rbPswdNone.Checked ? EncryptionAlgorithm.None : settingData.Compress.EncryptType;
			ZipJobParam param = new ZipJobParam(
				Mode: mode,
				DistinationDirectory: destDir,
				CompressLevel: settingData.Compress.CompressionLevel,
				Password: pswd,
				EncryptType: encType
			);
			/*	暗号方式	(ZipJobParam.EncryptType)
			方式				特徴
			WinZipAes256	セキュリティ重視。 非常に強力な暗号化ですが、展開側（ソフト）がAES対応している必要があります。
			PkzipWeak		互換性重視。 Windows標準の展開機能や古いソフトでも解凍できますが、セキュリティ強度は低めです。
			*/

			//圧縮/展開を開始する
			FmProgress fmProgress = new FmProgress(this, param);
			fmProgress.Show();
			fmProgress.Activate();
			await fmProgress.StartZipJobAsync(fileList);

			//圧縮/展開完了後
			/*  ※…非同期処理中にユーザーがアプリを閉じていた場合、awaitから戻ってきたときの状態としては、
				Form機能（アプリのUI）は失われているが、
				メモリ上にインスタンスは残っている（OSがタスクを実行中なのでGC()で回収されない）、
				という中途半端な状態となっている。thisにアクセス可能だがUI操作でアプリがクラッシュする。
				よって、アプリが破棄されているか(this.IsDisposed)を確認し、UI含めアプリを操作させないよう対策する必要がある。
			*/
			if (this.IsDisposed) { return; }    //※
			if (this.OwnedForms.Length == 0) { this.Close(); }
		}

		//ファイルパスの妥当性を確認する
		//・不正なファイルパスをリストから取り除く。全部取り除かれた場合、長さゼロの配列が返る。
		private string[] ValidateFileList(string[] files)
		{
			List<String> fileList = new List<String>(files);
			if (fileList.Count == 0) { return Array.Empty<string>(); }
			for (int i = fileList.Count - 1; 0 <= i; i--)
			{
				if (!File.Exists(fileList[i]) && !Directory.Exists(fileList[i])) { fileList.RemoveAt(i); continue; }
				if (fileList[i].Equals(Path.GetPathRoot(fileList[i]))) { fileList.RemoveAt(i); continue; }
			}
			return fileList.ToArray();
		}

		//ファイルが全てzipか否かを判定する
		//戻り値	true:	全てzipファイル
		//		false:	ディレクトリや非zipファイルが含まれている
		private bool IsAllZipFile(string[] files)
		{
			foreach (string filePath in files)
			{
				if (Directory.Exists(filePath) || !filePath.EndsWith(@".zip", true, null)) { return false; }
			}
			return true;
		}

		//出力先ディレクトリを取得する
		//・圧縮：リスト先頭のファイルのディレクトリ, ユーザー指定のディレクトリ。
		//・展開：リスト先頭のファイルのディレクトリ, ユーザー指定のディレクトリ。
		private string GetDestinationDirectoryPath(EZipMode mode, string oneInputFile)
		{
			DirectoryProperty dirProp = (mode == EZipMode.Extract) ? settingData.Extract : settingData.Compress;

			switch (dirProp.DirectoryPlan)
			{
			case EDirectoryPlan.InputOrigin:
				return Path.GetDirectoryName(oneInputFile) ?? string.Empty;

			case EDirectoryPlan.UserSpecified:
				return dirProp.TxtDirectoryPath;

			default:
				break;
			}

			return string.Empty;
		}

		#region 展開の設定

		//フォルダ参照ボタン_押下
		private void btnExtRef_Click(object sender, EventArgs e)
		{
			DialogResult res = folderDlg.ShowDialog();
			if (res == DialogResult.OK) { txtExtPath.Text = settingData.Extract.TxtDirectoryPath = folderDlg.SelectedPath; }
		}

		//展開フォルダー_選択
		private void cbExtractFolder_SelectedIndexChanged(object sender, EventArgs e)
		{
			settingData.Extract.CbIndexDirectory = cbExtFolder.SelectedIndex;

			//コントロールの操作可否を変更する
			if (settingData.Extract.DirectoryPlan == EDirectoryPlan.UserSpecified)
			{
				btnExtRef.Enabled = txtExtPath.Enabled = true;
				txtExtPath.BackColor = SystemColors.Window;
			}
			else
			{
				btnExtRef.Enabled = txtExtPath.Enabled = false;
				txtExtPath.BackColor = SystemColors.Control;
			}
		}

		#endregion

		#region 圧縮の設定

		//フォルダ参照ボタン_押下
		private void btnArcRef_Click(object sender, EventArgs e)
		{
			DialogResult res = folderDlg.ShowDialog();
			if (res == DialogResult.OK) { txtCompPath.Text = settingData.Compress.TxtDirectoryPath = folderDlg.SelectedPath; }
		}

		//圧縮フォルダー_選択
		private void cbArchiveFolder_SelectedIndexChanged(object sender, EventArgs e)
		{
			settingData.Compress.CbIndexDirectory = cbCompFolder.SelectedIndex;

			//コントロールの操作可否を変更する
			if (settingData.Compress.DirectoryPlan == EDirectoryPlan.UserSpecified)
			{
				btnCompRef.Enabled = txtCompPath.Enabled = true;
				txtCompPath.BackColor = SystemColors.Window;
			}
			else
			{
				btnCompRef.Enabled = txtCompPath.Enabled = false;
				txtCompPath.BackColor = SystemColors.Control;
			}
		}

		//圧縮レベル_選択
		private void cbCompressionLevel_SelectedIndexChanged(object sender, EventArgs e)
		{
			settingData.Compress.CbIndexCompressLevel = cbCompLv.SelectedIndex;
		}

		//暗号化強度_選択
		private void cbEncryptLv_SelectedIndexChanged(object sender, EventArgs e)
		{
			settingData.Compress.CbIndexEncryptLevel = cbEncryptLv.SelectedIndex;
		}

		#endregion

		#region パスワードの設定

		//パスワード：用途選択
		private void rbPswd_CheckedChanged(object sender, EventArgs e)
		{
			txtPswd.Enabled = (sender != rbPswdNone);
			//※設定値をsettingDataに保存する必要はない
		}

		//パスワード：見せる
		//・MouseEnter()により表示が変更された瞬間にMouseLeave()が発生する（Windows特有の挙動）。
		//  それを回避するため、カーソル移動の有無により正しいイベント発生かを判定する。
		private Point pswdViewMouseCursorPos = Cursor.Position;
		private void txtPswd_MouseEnter(object sender, EventArgs e)
		{
			if (pswdViewMouseCursorPos != Cursor.Position) { txtPswd.UseSystemPasswordChar = false; }
			pswdViewMouseCursorPos = Cursor.Position;
		}

		//パスワード：隠す
		private void txtPswd_MouseLeave(object sender, EventArgs e)
		{
			if (pswdViewMouseCursorPos != Cursor.Position) { txtPswd.UseSystemPasswordChar = true; }
			pswdViewMouseCursorPos = Cursor.Position;
		}

		//パスワード：見せる（K/B対応）
		private void txtPswd_Enter(object sender, EventArgs e)
		{
			txtPswd.UseSystemPasswordChar = false;
		}

		//パスワード：隠す（K/B対応）
		private void txtPswd_Leave(object sender, EventArgs e)
		{
			txtPswd.UseSystemPasswordChar = true;
		}

		#endregion

		//その他の設定：入力が全部zipだったら展開処理とする
		private void chkExtractAllZip_CheckedChanged(object sender, EventArgs e)
		{
			settingData.IsExtractAllZip = chkExtractAllZip.Checked;
		}

		//設定を保存する
		private void btnSaveSetting_Click(object sender, EventArgs e)
		{
			settingData.FmMainLocation = this.Location;

			string settingFile = GetSettingFilePath();
			settingData.SaveToXml(settingFile);
		}

		//ヘルプ：バージョン情報
		private void tsmHelpInfo_Click(object sender, EventArgs e)
		{
			string br = Environment.NewLine;
			string title = $"{Application.ProductName}について";
			string mes = $"ZIPアーカイバ {Application.ProductName}{br}{Program.AppVer}{br}(c) @jsdiy, 2013-";
			MessageBox.Show(mes, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		//ファイル：終了
		private void tsmFileExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

	}	//class
}	//namespace
