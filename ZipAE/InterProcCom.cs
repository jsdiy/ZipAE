using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace InterProcCom
{
	//プロセス間通信クラス
	public static partial class IPC
	{
		//アプリケーション固有の識別子
		public static string AppIdHash { get; private set; } = string.Empty;
		public static uint AppIdValue { get; private set; } = 0;

		//初期化
		public static void Initialize(string appName)
		{
			//文字列をハッシュ化してbyte配列を取得する
			byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(appName));

			//byte配列を16進数文字列に変換する
			string hexString = Convert.ToHexString(hashBytes);
			AppIdHash = hexString;

			//ハッシュ化byte配列の先頭4バイトをuint型数値に変換する（リトルエンディアンで解釈される：[0][1][2][3] → 0x[3][2][1][0]）
			uint u32value = BitConverter.ToUInt32(hashBytes, 0);
			AppIdValue = u32value;
		}

		#region 既に起動されている同じアプリのハンドルとプロセスを取得する

		//既に起動されている同じアプリのハンドル（とプロセス）を取得する
		//戻り値：	メインウインドウのハンドル
		//引数(out)：	目的のアプリのプロセスが格納される（なければnull）
		public static IntPtr GetAlreadyStartedApplication(out Process? targetProc)
		{
			//目的のアプリのプロセスを取得する
			targetProc = GetAlreadyStartedProcess();
			if (targetProc == null) { return IntPtr.Zero; }

			//目的のアプリのForm.ShowInTaskbarがfalseだった場合（例：タスクトレイに格納するときfalseにした）、
			//Process.MainWindowHandleは0なので、Process.Idを手掛かりにウインドウ総当たりで検出する。
			IntPtr hWnd = targetProc.MainWindowHandle;
			if (hWnd == IntPtr.Zero) { hWnd = GetWindowHandleByProcessId(targetProc.Id); }
			return hWnd;
		}

		//既に起動されている同じアプリのプロセスを取得する
		private static Process? GetAlreadyStartedProcess()
		{
			Process currentProc = Process.GetCurrentProcess();  //呼び出し元である自分自身
			Process[] procs = Process.GetProcessesByName(currentProc.ProcessName);  //戻り値にはcurrentProcも含まれる

			string? currentFileName = currentProc.MainModule?.FileName;
			if (string.IsNullOrEmpty(currentFileName)) { return null; }
			try
			{
				foreach (Process targetProc in procs)
				{
					//現在この関数を実行中である自分自身は判定対象から除く
					if (targetProc.Id == currentProc.Id) { continue; }

					//プロセスのフルパス名を比較して同じアプリであるか判定する
					if (currentFileName.Equals(targetProc.MainModule?.FileName)) { return targetProc; }
				}
			}
			catch { return null; }

			//起動中の同じアプリはない
			return null;
		}
		#endregion

		#region プロセスIDを指定してウインドウハンドルを取得する

		//EnumWindows()のコールバック関数に渡す引数
		public struct EnumWindowsCbParam
		{
			public int ProcessId;
			public IntPtr hWnd;
		}

		//プロセスIDを指定してウインドウハンドルを取得する
		public static IntPtr GetWindowHandleByProcessId(int procId)
		{
			EnumWindowsCbParam tParam = new EnumWindowsCbParam();
			tParam.hWnd = IntPtr.Zero;
			tParam.ProcessId = procId;

			DlgtEnumWindowsProc dlgtEnumWindowsProc = EnumWindowsProc;
			EnumWindows(dlgtEnumWindowsProc, ref tParam);

			return tParam.hWnd;
		}

		//EnumWindows()のコールバック関数（検索処理の本体）
		//戻り値	true:列挙続行, false:列挙中止
		private static bool EnumWindowsProc(IntPtr hWnd, ref EnumWindowsCbParam lParam)
		{
			if ((GetParent(hWnd) == IntPtr.Zero) && IsWindowVisible(hWnd))
			{
				GetWindowThreadProcessId(hWnd, out int procIdOfArgWnd);
				if (procIdOfArgWnd == lParam.ProcessId)
				{
					lParam.hWnd = hWnd;
					return false;
				}
			}
			return true;
		}

		[LibraryImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static partial bool EnumWindows(DlgtEnumWindowsProc dlgtEnumWindowsProc, ref EnumWindowsCbParam lParam);
		public delegate bool DlgtEnumWindowsProc(IntPtr hWnd, ref EnumWindowsCbParam lParam);
		/*
		https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-enumwindows
		構文:
		BOOL EnumWindows(
		  WNDENUMPROC lpEnumFunc,  // コールバック関数
		  LPARAM lParam            // アプリケーション定義の値
		);
		戻り値:	関数が成功すると、戻り値は 0 以外になります。
				関数が失敗した場合は、0 を返します。
		
		https://learn.microsoft.com/ja-jp/previous-versions/windows/desktop/legacy/ms633498(v=vs.85)
		※EnumWindows()のリファレンスページの末尾にEnumWindowsProc()のリファレンスページへのリンクあり。
		構文:
		BOOL CALLBACK EnumWindowsProc(
		  HWND hwnd,      // 親ウィンドウのハンドル
		  LPARAM lParam   // アプリケーション定義の値
		);
		戻り値:	列挙を続行する場合は、0 以外の値（TRUE）を返してください。
				列挙を中止する場合は、0（FALSE）を返してください。
		*/

		[LibraryImport("user32.dll")]
		private static partial IntPtr GetParent(IntPtr hwnd);
		/*
		https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-getparent
		構文:
		HWND GetParent(
		  [in] HWND hWnd	//親ウィンドウ ハンドルを取得するウィンドウのハンドル
		);
		戻り値:
			指定された子ウィンドウの親ウィンドウまたはオーナーウィンドウのハンドルを返します。
			指定したウィンドウがトップレベルのオーナーを持たないウィンドウだった場合、
			および関数が失敗した場合は NULL が返ります。
		備考:
			publicにすると「CA1401: P/Invoke は参照可能になりません」の警告。
			→ https://learn.microsoft.com/ja-jp/dotnet/fundamentals/code-analysis/quality-rules/ca1401
		*/

		[LibraryImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static partial bool IsWindowVisible(IntPtr hWnd);
		/*
		https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-iswindowvisible
		構文:
		BOOL IsWindowVisible(
		  [in] HWND hWnd	//テストするウィンドウへのハンドル
		);
		戻り値:
			指定したウィンドウ、その親ウィンドウ、親の親ウィンドウなどが WS_VISIBLE スタイルの場合、戻り値は 0 以外になります。
			それ以外の場合、戻り値は 0 です。
			戻り値は、ウィンドウに WS_VISIBLE スタイルがあるかどうかを指定するため、
			ウィンドウが他のウィンドウによって完全に隠されている場合でも、0 以外の場合があります。←※

			→	Form.Hide()やForm.Visible=falseでないならtrue。
				タスクバー上にアイコン化されれていようと、タスクトレイに格納されていようと、
				他のウインドウの背面に隠れていようと(*)、IsWindowVisible()の評価には関係ない。	*…例外があるらしい。※を参照。
		*/

		[LibraryImport("user32.dll")]
		public static partial uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
		/*
		https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-getwindowthreadprocessid
		構文:
		DWORD GetWindowThreadProcessId(
		  HWND hWnd,             // ウィンドウのハンドル
		  LPDWORD lpdwProcessId  // プロセス ID
		);
		引数:	lpdwProcessId
			プロセス ID を受け取る変数へのポインタを指定します。
			ポインタを指定すると、それが指す変数にプロセス ID がコピーされます。
			NULL を指定した場合は、プロセス ID の取得は行われません。
		戻り値:
			ウィンドウを作成したスレッドの ID が返ります。
		*/

		#endregion

		#region 既に起動されているこのアプリケーションへ、後から起動したこのアプリから文字列を渡す

		//SendMessageで送る構造体
		[StructLayout(LayoutKind.Sequential)]
		private struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public int cbData;
			public IntPtr lpData;
		}
		/*
		http://msdn.microsoft.com/ja-jp/library/ms649010(v=VS.80).aspx
		構文:
		typedef struct tagCOPYDATASTRUCT {
		  ULONG_PTR dwData;		//識別子（任意）
		  DWORD     cbData;		//データ長(byte)
		  PVOID     lpData;		//データへのポインタ
		} COPYDATASTRUCT, *PCOPYDATASTRUCT;

		文字列のマーシャリングについて
		http://msdn.microsoft.com/ja-jp/library/s9ts558h(v=vs.90).aspx
		*/

		//起動済みの自アプリ（インスタンス）へアプリの引数を渡す
		//戻り値	true:	送信した
		//		false:	送信しなかった
		public static bool SendArgs(IntPtr targetWindowHandle, string[] args)
		{
			if (args == null || args.Length ==0) { return false; }
			IPC.SendString(targetWindowHandle, string.Join("\t", args));
			return true;
		}

		//起動済みの自アプリ（インスタンス）へ文字列を渡す
		public static void SendString(IntPtr targetWindowHandle, string message)
		{
			IntPtr pszText = IntPtr.Zero;

			try
			{
				//文字列を Unicode (UTF-16, 1文字2byte) としてアンマネージド空間のメモリにコピーし、そのポインタを取得する
				pszText = Marshal.StringToHGlobalUni(message);

				// COPYDATASTRUCT の準備
				COPYDATASTRUCT cds = new COPYDATASTRUCT();
				cds.dwData = IntPtr.Zero;
				cds.cbData = (message.Length + 1) * sizeof(char);  //データ長：(文字数 + ヌル終端) * 2(byte)
				cds.lpData = pszText;   //文字列へのポインタ

				//構造体のポインタを送信
				unsafe
				{
					IntPtr wParam = (IntPtr)AppIdValue;	//このアプリから送ったという目印
					IntPtr lParam = (IntPtr)(&cds);
					SendMessage(targetWindowHandle, WM_COPYDATA, wParam, lParam);
					/*	'&cds'について
					SendMessageは同期関数なので受信完了までここで処理は止まる。	その間、スタック領域上のcdsはメモリ移動/消滅することはない。
					よってcdsはMarshal.StructureToPtr()を使わず単純にnewし（スタック上に実体化）、
					fixed()は不要で、'(IntPtr)(&cds)'としてSendMessage()の引数にしている。
					*/
				}
			}
			finally
			{
				// 送信が終わった段階（SendMessage()実行後）でネイティブメモリ（アンマネージド空間のメモリ）を解放する
				if (pszText != IntPtr.Zero) { Marshal.FreeHGlobal(pszText); }
			}
		}

		//SendString()で送信された文字列を取り出す（文字列配列）
		//・該当するメッセージを受信したWindProc()でこのメソッドを呼び出す。
		public static string[] ReceiveArgs(System.Windows.Forms.Message m)
		{
			string? rcvStr = ReceiveString(m);
			if (string.IsNullOrEmpty(rcvStr)) { return [""]; }   //string[0]="" という、要素が一つの配列を返す
			string[] args = rcvStr.Split(['\t']);   //元のデータとしてはコマンドライン引数を想定
			return args;
		}

		//SendString()で送信された文字列を取り出す（単一の文字列）
		//・該当するメッセージを受信したWindProc()でこのメソッドを呼び出す。
		public static string? ReceiveString(System.Windows.Forms.Message m)
		{
			object? obj = m.GetLParam(typeof(COPYDATASTRUCT));
			if (obj == null) { return null; }
			COPYDATASTRUCT cds = (COPYDATASTRUCT)obj;
			string? rcvStr = Marshal.PtrToStringUni(cds.lpData);
			return rcvStr;
		}

		[LibraryImport("user32.dll", EntryPoint = "SendMessageW")]  //末尾'W'はUnicode版、'A'はANSI版。
		private static partial IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		public const int WM_COPYDATA = 0x004A;
		private const int WM_SYSCOMMAND = 0x0112, SC_RESTORE = 0xF120;
		/*
		https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-sendmessage
		構文:
		LRESULT SendMessage(
		  HWND hWnd,      // 送信先ウィンドウのハンドル（ウィンドウプロシージャがメッセージを受信するウィンドウのハンドル）
		  UINT Msg,       // 送信されるメッセージ
		  WPARAM wParam,  // 追加のメッセージ固有情報
		  LPARAM lParam   // 追加のメッセージ固有情報
		);
		戻り値:
			戻り値は、メッセージ処理の結果を指定します。送信されたメッセージによって異なります。
		備考:
			・SendMessage()は同期処理。PostMessage()は非同期処理。
			・LRESULTは、64bit環境ではInt64、32bit環境ではInt32。このような型は整数型ではなくIntPtr型で置き換えるのがC#のセオリー。
			・publicにすると「CA1401: P/Invoke は参照可能になりません」の警告。
				→ https://learn.microsoft.com/ja-jp/dotnet/fundamentals/code-analysis/quality-rules/ca1401
		*/

		//アプリを最前面に呼び出す（タスクバーにいたら出す）
		public static void WakeupApplication(IntPtr targetWindowHandle)
		{
			//SetForegroundWindow(targetWindowHandle);	//タスクバーから出す機能はない
			//ShowWindow(targetWindowHandle, SW_RESTORE);	//最前面にする機能はない
			SendMessage(targetWindowHandle, WM_SYSCOMMAND, (IntPtr)SC_RESTORE, IntPtr.Zero);	//標準的な方法
		}

		[LibraryImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool SetForegroundWindow(IntPtr hWnd);
		/*
		https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-setforegroundwindow
		構文:
		BOOL SetForegroundWindow(
			[in] HWND hWnd
		);
		戻り値:
			ウィンドウがフォアグラウンドに移動された場合、戻り値は 0 以外です。
			ウィンドウがフォアグラウンドに移動されなかった場合、戻り値は 0 になります。
		備考:
			デスクトップに表示されている状態のアプリを最前面に表示し直す。
			最小化されているアプリをタスクバーから出す機能はない。　※タスクバー上でフォーカスはあたる。
		*/

		[LibraryImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);
		private const int SW_RESTORE = 9;
		/*
		https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-showwindow
		構文:
		BOOL ShowWindow(
			[in] HWND hWnd,
			[in] int  nCmdShow
		);
		戻り値:
			ウィンドウが以前に表示されていた場合、戻り値は 0 以外です。
			ウィンドウが以前に非表示だった場合、戻り値は 0 になります。
		備考:
			最小化されているアプリをタスクバーから出す。
			最前面にする機能はないので、デスクトップに復帰したとき他のウインドウの背後にいる場合もある。
		*/

		#endregion

	}   //class
}   //namespace

#if (false)    //Form側サンプル
	・WndProc()を即座に抜ける必要があるので、重い処理を実行する場合は非同期メソッド(async-await)で処理する。
	・軽い処理ならそのメソッドを直接実行するか、BeginInvoke()で呼び出す。
		→Control.BeginInvoke()はUIスレッドのメッセージキューにデリゲート（引数の関数）を配置する。　※関数の実行はしない。
		 その後、UIスレッドが次に利用可能になったとき、指定の関数が実行される。　※重い処理だとUIが固まってしまうことに注意。
	・重い処理の場合、WndProc()自体にはasyncは書けない（overrideなので）。
		→非同期メソッド内で確実に例外処理(try-catch)するのであれば、戻り値Taskは破棄して構わない。

	protected override void WndProc(ref Message m)
	{
		switch (m.Msg)
		{
		case IPC.WM_COPYDATA:
			if (m.WParam != (IntPtr)IPC.AppIdValue) { break; }
			string[] args = IPC.ReceiveArgs(m);
			_ = AppExecute(args);	//非同期でメソッドを実行する
			m.Result = (IntPtr)1;
			return;

		default:
			break;
		}

		base.WndProc(ref m);
	}

#endif
