using Ionic.Zip;
using Ionic.Zlib;
using System.Text;
using System.Xml.Serialization;
/*	Ionic.Zip(DotNetZip)
公式配布元	https://github.com/DinoChiesa/dotnetzip-2025
翻訳	https://github-com.translate.goog/DinoChiesa/dotnetzip-2025?_x_tr_sl=ja&_x_tr_tl=en&_x_tr_hl=ja&_x_tr_pto=wapp
*/

namespace ZipAE
{
	public enum EDirectoryPlan { InputOrigin, UserSpecified }

	public enum EZipMode { Compress, Extract }
	public record ZipJobParam
	(
		EZipMode Mode,
		string DistinationDirectory,	//展開：展開先、	圧縮：<ファイル>.zipの保存先
		CompressionLevel CompressLevel,	//展開：無効、	圧縮：Ionic.Zlib.CompressLevel
		string? Password,				//展開：有効、	圧縮：有効
		EncryptionAlgorithm EncryptType	//展開：無効、	圧縮：暗号アルゴリズム
	);

	//ディレクトリに関するプロパティ（圧縮/展開共通）
	public class DirectoryProperty
	{
		public int CbIndexDirectory { get; set; }
		public string TxtDirectoryPath { get; set; }

		public EDirectoryPlan DirectoryPlan { get { return (EDirectoryPlan)CbIndexDirectory; } }

		public DirectoryProperty()
		{
			CbIndexDirectory = 0;
			TxtDirectoryPath = string.Empty; ;
		}
	}

	//ディレクトリに関するプロパティ（展開）
	public class ExtractProperty : DirectoryProperty
	{
		//なし
	}

	//ディレクトリに関するプロパティ（圧縮）
	public class CompressProperty : DirectoryProperty
	{
		public int CbIndexCompressLevel { get; set; }
		public int CbIndexEncryptLevel { get; set; }

		public CompressionLevel CompressionLevel { get { return compressLevels[CbIndexCompressLevel]; } }
		private readonly CompressionLevel[] compressLevels =
			{ CompressionLevel.None, CompressionLevel.BestCompression, CompressionLevel.BestSpeed, CompressionLevel.Default };

		public EncryptionAlgorithm EncryptType { get { return encryptTypes[CbIndexEncryptLevel]; } }
		private readonly EncryptionAlgorithm[] encryptTypes =
			{ EncryptionAlgorithm.WinZipAes256, EncryptionAlgorithm.PkzipWeak };

		public CompressProperty()
		{
			CbIndexCompressLevel = 0;
			CbIndexEncryptLevel = 0;
		}
	}

	/// <summary>
	/// 設定値保存クラス
	/// </summary>
	[XmlRoot("SettingData")]
	public class SettingData
	{
		public Point FmMainLocation { get; set; }
		public bool IsExtractAllZip { get; set; }
		public ExtractProperty Extract { get; set; }
		public CompressProperty Compress { get; set; }

		//コンストラクタ
		public SettingData()
		{
			FmMainLocation = new Point(100, 100);
			IsExtractAllZip = false;
			Extract = new ExtractProperty();
			Compress = new CompressProperty();
		}

		//シリアライズ（保存）
		public bool SaveToXml(string filePath)
		{
			try
			{
				//「BOMなしUTF8」で出力する
				var serializer = new XmlSerializer(typeof(SettingData));
				using var writer = new StreamWriter(filePath, false, new UTF8Encoding(false));
				serializer.Serialize(writer, this);
			}
			catch { return false; }

			return true;
		}

		//デシリアライズ（読み込み）
		public static SettingData LoadFromXml(string filePath)
		{
			try
			{
				//エンコードが何であれ「BOMなしUTF8」として読む
				//・出力したファイル以外のエンコードを考慮する必要はない。
				var serializer = new XmlSerializer(typeof(SettingData));
				using TextReader reader = new StreamReader(filePath, new UTF8Encoding(false));
				if (serializer.Deserialize(reader) is SettingData settingData) { return settingData; }
			}
			catch { }

			return new SettingData();
		}

	}	//class
}	//namespace
