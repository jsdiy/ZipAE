using InterProcCom;
using System.Diagnostics;

namespace ZipAE
{
	internal static class Program
	{
		public static string AppVer { get; } =
			//"ver. 1.00";	//2013/07/29
			"ver. 2.00";    //2025/12/

		private const string MutexName = "Global\\ZipAE-{8A209FEA-4956-40A6-943B-BB5DEACA36EB}";

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			IPC.Initialize(MutexName + AppVer);

			//ミューテックスを生成する
			//・using利用により、Main()を抜けるときにmutexが解放される。
			using var mutex = new Mutex(true, MutexName, out bool createdNew);

			//多重起動チェックする
			if (createdNew)
			{
				//初回起動だった場合、通常のアプリ起動処理
				// To customize application configuration such as set high DPI settings or default font,
				// see https://aka.ms/applicationconfiguration.
				ApplicationConfiguration.Initialize();
				Application.Run(new FmMain());
			}
			else
			{
				//2回目の起動だった場合、起動済みインスタンスを呼び出す
				IntPtr handle = IPC.GetAlreadyStartedApplication(out Process? targetProc);
				if (handle == IntPtr.Zero) { return; }

				//引数ありの場合、引数を渡して呼び出す（ウインドウ表示状態はそのまま。タスクトレイにいても出さない）
				//引数なしの場合、アプリを最前面に呼び出す（タスクトレイにいたら出す）
				string[] args = Environment.GetCommandLineArgs();   //args[0]はアプリ自身(exe)のフルパス
				if (1 < args.Length)
					IPC.SendArgs(handle, args);
				else
					IPC.WakeupApplication(handle);
			}
		}

	}   //class
}   //namespace
