using Ionic.Zip;
using System.Text;

namespace ZipAE
{
	public partial class FmProgress : Form
	{
		private volatile bool reqZipCancel;
		private bool isZipWorking;
		private readonly ZipJobParam jobParam;

		//プログレスバーへ送信するデータ
		public class ZipStatus
		{
			public int PrevPercent { get; set; } = 0;
			public int Percent { get; set; } = 0;
			public string FileName { get; private set; } = string.Empty;

			public void Reset(string fileName) { PrevPercent = Percent = 0; FileName = fileName; }
		}

		private ZipStatus zipStatus;
		private IProgress<ZipStatus> progress;  //ここはProgressではなくIProgressとする。インスタンスはProgressでnewする。

		//コンストラクタ
		public FmProgress(FmMain fmMain, ZipJobParam param)
		{
			InitializeComponent();
			this.Owner = fmMain;
			this.Icon = fmMain.Icon;
			this.Location = fmMain.Location;
			this.Text = (param.Mode == EZipMode.Extract) ? "展開" : "圧縮";

			reqZipCancel = false;
			isZipWorking = false;
			jobParam = param;

			zipStatus = new ZipStatus();
			progress = new Progress<ZipStatus>((status) =>
			{
				progBar.Value = status.Percent;
				lblFileName.Text = status.FileName;
			});
		}

		//圧縮/展開の処理を開始する
		public async Task StartZipJobAsync(string[] files)
		{
			string br = Environment.NewLine;

			try
			{
				isZipWorking = true;

				await Task.Run(() =>
				{
					switch (jobParam.Mode)
					{
					case EZipMode.Compress: Compress(files); break;
					case EZipMode.Extract: Extract(files); break;
					default: break;
					}
				});
			}
			catch (Ionic.Zip.BadPasswordException)	//Zipパスワードエラー
			{
				string mes = $"ファイルは暗号化されています。パスワードを指定してください。{br}" +
					$"またはパスワードが違います。{br}処理を中止します。";
				MessageBox.Show(mes, "パスワードエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (Ionic.Zip.ZipException ze)	//zip処理の例外全般
			{
				string mes = $"圧縮/展開処理のエラーが発生しました。{br}処理を中止します。{br}{br}" + ze.Message;
				MessageBox.Show(mes, "ZIP処理エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception e)	//全ての例外
			{
				string mes = $"エラーが発生しました。{br}処理を中止します。{br}{br}" + e.Message;
				MessageBox.Show(mes, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				isZipWorking = false;
			}

			//タスク（非同期処理）が完了：zip処理が正常終了、ユーザーによるキャンセル、例外発生
			this.Close();
		}

		//フォームがクローズ中
		private void FmProgress_FormClosing(object sender, FormClosingEventArgs e)
		{
			//zip処理を中止するためのユーザー操作によるクローズだった場合
			if (isZipWorking /*&& (e.CloseReason == CloseReason.UserClosing) この条件は不要*/)
			{
				reqZipCancel = true;    //Event_SaveProgress()/Event_ExtractProgress()にzip処理中止を要求する
				e.Cancel = true;        //zip処理中止完了まで待つため、FormClosing()を取り消す
				this.Enabled = false;   //フォームの閉じるボタン[X]を連打されないよう対策する
			}
		}

		#region 圧縮

		//圧縮処理呼び出し
		public void Compress(string[] fileList)
		{
			//圧縮時の出力ファイル(xxxxx(n).zip)を作成する
			string fileBaseName = Path.GetFileNameWithoutExtension(fileList.First());
			string fileName = GetUniqueFileName(jobParam.DistinationDirectory, fileBaseName, @".zip");
			string compressedFilePath = Path.Combine(jobParam.DistinationDirectory, fileName);

			CompressWorker(fileList, compressedFilePath);
		}

		//作成しようとするファイル名の重複を検索し、重複があれば「カッコ＋数字」を付けたファイル名を返す
		private string GetUniqueFileName(string directoryPath, string fileBaseName, string fileExt)
		{
			string fileName = Path.ChangeExtension(fileBaseName, fileExt);
			string filePath = Path.Combine(directoryPath, fileName);
			if (!File.Exists(filePath)) { return fileName; }

			int count = 2;
			string numberedFileName = string.Empty;
			while (File.Exists(filePath))
			{
				numberedFileName = Path.ChangeExtension(fileBaseName + $"({count})", fileExt);
				filePath = Path.Combine(directoryPath, numberedFileName);
				count++;
			}

			return numberedFileName;
		}

		//zip圧縮処理
		private void CompressWorker(string[] fileList, string compressedFilePath)
		{
			using (var zipfile = new Ionic.Zip.ZipFile(Encoding.Default))
			{
				//zip処理のパラメータ
				//・Zipfile.AddXxx()より前に設定すること
				zipfile.CompressionLevel = jobParam.CompressLevel;	//圧縮レベル（設定しない場合はDefaultとなる）
				zipfile.Password = jobParam.Password;		//パスワード（nullまたは文字列がセットされる）
				zipfile.Encryption = jobParam.EncryptType;	//暗号アルゴリズム

				//エントリーを追加する
				foreach (string filePath in fileList)
				{
					if (File.Exists(filePath)) { zipfile.AddFile(filePath, string.Empty); }
					else if (Directory.Exists(filePath)) { zipfile.AddDirectory(filePath, Path.GetFileName(filePath)); }
				}

				//圧縮を実行する
				zipfile.SaveProgress += this.Event_SaveProgress;	//イベントを登録する
				zipfile.UseZip64WhenSaving = Zip64Option.AsNecessary;	//圧縮後、4GiB以上になる場合はこの指定が必要
				zipfile.Save(compressedFilePath);	//登録したエントリーを書庫ファイルへ保存する
			}
		}

		//zip圧縮イベント
		private void Event_SaveProgress(object? sender, SaveProgressEventArgs ea)
		{
			//キャンセル要求があった場合はzip処理を中止する
			if (reqZipCancel)
			{
				ea.Cancel = true;
				return;
			}

			//イベント処理
			switch (ea.EventType)
			{
			//zipファイル作成開始
			case ZipProgressEventType.Saving_Started: break;

			//書き込み開始
			case ZipProgressEventType.Saving_BeforeWriteEntry:
				zipStatus.Reset(ea.CurrentEntry.FileName);
				progress.Report(zipStatus);
				break;

			//書き込み中（このイベントは頻繁に発生する。使用する場合はUI更新の負荷を考慮する）
			case ZipProgressEventType.Saving_EntryBytesRead:
				zipStatus.Percent = (int)(100L * ea.BytesTransferred / ea.TotalBytesToTransfer);
				if ((zipStatus.PrevPercent < zipStatus.Percent) && (zipStatus.Percent % 5 == 0))
				{
					zipStatus.PrevPercent = zipStatus.Percent;
					progress.Report(zipStatus);
				}
				break;

			//書き込み終了
			case ZipProgressEventType.Saving_AfterWriteEntry: break;

			//zipファイル作成完了
			case ZipProgressEventType.Saving_Completed: break;

			default: break;
			}
		}

		#endregion

		#region 展開

		//展開処理呼び出し
		public void Extract(string[] fileList)
		{
			foreach (string filePath in fileList)
			{
				//出力先フォルダに展開元ファイルのベース名のフォルダを作成し、そこを真の出力先とする
				string fileBaseName = Path.GetFileNameWithoutExtension(filePath);
				string subDirectoryName = GetUniqueDirectoryName(jobParam.DistinationDirectory, fileBaseName);
				string subDirectoryPath = Path.Combine(jobParam.DistinationDirectory, subDirectoryName);

				ExtractWorker(filePath, subDirectoryPath);
			}
		}

		//作成しようとするフォルダ名の重複を検索し、重複があれば「カッコ＋数字」を付けたフォルダ名を返す
		private string GetUniqueDirectoryName(string baseDirectoryPath, string subDirectoryName)
		{
			string subDirectoryPath = Path.Combine(baseDirectoryPath, subDirectoryName);
			if (!Directory.Exists(subDirectoryPath)) { return subDirectoryName; }

			int count = 2;
			string numberedSubDirectoryName = string.Empty;
			while (Directory.Exists(subDirectoryPath))
			{
				numberedSubDirectoryName = subDirectoryName + $"({count})";
				subDirectoryPath = Path.Combine(baseDirectoryPath, numberedSubDirectoryName);
				count++;
			}

			return numberedSubDirectoryName;
		}

		//zip展開処理
		private void ExtractWorker(string filePath, string extractedDirectoryPath)
		{
			//展開を実行する
			ReadOptions readOpt = new ReadOptions();
			readOpt.Encoding = Encoding.Default;
			using (var zipfile = Ionic.Zip.ZipFile.Read(filePath, readOpt))
			{
				zipfile.Password = jobParam.Password;   //展開パスワード（nullまたは文字列がセットされる）
				zipfile.ExtractProgress += this.Event_ExtractProgress;  //イベントを登録する
				zipfile.ExtractAll(extractedDirectoryPath);	//展開する（引数のフォルダは作成される）
			}

			/*	いらないな…
			//二重フォルダを解消する（ユーザー指定のオプションにより）
			*/
		}

		//zip展開イベント
		private void Event_ExtractProgress(object? sender, ExtractProgressEventArgs ea)
		{
			//キャンセル要求があった場合はzip処理を中止する
			if (this.reqZipCancel)
			{
				ea.Cancel = true;
				return;
			}

			switch (ea.EventType)
			{
			//書庫内の一つのエントリーについて、展開開始
			case ZipProgressEventType.Extracting_BeforeExtractEntry:
				zipStatus.Reset(ea.CurrentEntry.FileName);
				progress.Report(zipStatus);
				break;

			//書庫内の一つのエントリーについて、展開中（このイベントは頻繁に発生する。使用する場合はUI更新の負荷を考慮する）
			case ZipProgressEventType.Extracting_EntryBytesWritten:
				zipStatus.Percent = (int)(100L * ea.BytesTransferred / ea.TotalBytesToTransfer);
				if ((zipStatus.PrevPercent < zipStatus.Percent) && (zipStatus.Percent % 5 == 0))
				{
					zipStatus.PrevPercent = zipStatus.Percent;
					progress.Report(zipStatus);
				}
				break;

			//出力先に同名エントリーが存在した（上書き、スキップ、処理中止の判断）
			case ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite: break;

			//書庫内の一つのエントリーについて、展開完了
			case ZipProgressEventType.Extracting_AfterExtractEntry: break;

			//書庫内の全てのエントリーについて、展開完了
			case ZipProgressEventType.Extracting_AfterExtractAll: break;

			default: break;
			}
		}

		#endregion

	}	//class
}	//namespace
