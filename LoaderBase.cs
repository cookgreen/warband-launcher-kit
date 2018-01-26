using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Launcher
{
    public abstract class LoaderBase : ILoader
    {
        protected void SetActiveMod(string mod)
        {
            var key = Registry.CurrentUser.OpenSubKey("Software\\MountAndBladeWarbandKeys", true) ??
                      Registry.CurrentUser.CreateSubKey("Software\\MountAndBladeWarbandKeys", RegistryKeyPermissionCheck.ReadWriteSubTree);
            key?.SetValue("last_module_warband", mod, RegistryValueKind.String);
        }

        // ReSharper disable InconsistentNaming
        [DllImport("User32.dll", SetLastError = true, EntryPoint = "FindWindowW", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(
            string lpClassName,
            string lpWindowName
        );

        [DllImport("User32.dll")]
        private static extern IntPtr GetDlgItem(
            IntPtr hDlg,
            int nIDDlgItem
        );

        [DllImport("User32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        private const uint WM_LBUTTONDOWN = 513;
        private const uint WM_LBUTTONUP = 514;
        private const int MK_LBUTTON = 1;
        private const int IDC_PLAY_BUTTON = 1029; //1040 is fake play button idc
        // ReSharper restore InconsistentNaming

        protected void PushPlayButton()
        {
            IntPtr hDialog, hPlayButton;
            do
            {
                hDialog = FindWindow("#32770", "Mount&Blade Warband");
            } while (hDialog == IntPtr.Zero);
            
            do
            {
                hPlayButton = GetDlgItem(hDialog, IDC_PLAY_BUTTON);
            } while (hPlayButton == IntPtr.Zero);

            SendMessage(hPlayButton, WM_LBUTTONDOWN, (IntPtr)MK_LBUTTON, IntPtr.Zero);
            SendMessage(hPlayButton, WM_LBUTTONUP, (IntPtr)MK_LBUTTON, IntPtr.Zero);
        }

        protected string FindWarbandPath(string pathToExe, out bool success)
        {
            string warbandPath;
            if (File.Exists(warbandPath = Path.Combine(pathToExe, "mb_warband.exe")))
                goto wb_founded;
            if (File.Exists(warbandPath = Path.Combine(Registry.LocalMachine.ReadString("SOFTWARE\\Mount&Blade Warband", ""),
                "mb_warband.exe")))
                goto wb_founded;
            if (File.Exists(warbandPath = Path.Combine(Registry.LocalMachine.ReadString("SOFTWARE\\Mount&Blade Warband", "Install_Path"),
                "mb_warband.exe")))
                goto wb_founded;

            success = false;
            return "";

        wb_founded:
            success = true;
            return warbandPath;
        }

        public abstract void LoadGame();
    }
}
