using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace Launcher
{
    public sealed unsafe class WseLoader : LoaderBase
    {
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper disable InconsistentNaming
        private const uint LMEM_FIXED = 0;
        private const uint HIGH_PRIORITY_CLASS = 128;
        private const uint INFINITE = 0xFFFFFFFFu;

        [DllImport("Kernel32.dll")]
        private static extern IntPtr LocalAlloc(
            uint uFlags,
            UIntPtr uBytes
        );

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern IntPtr LocalFree(
            IntPtr hMem
        );

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW")]
        private static extern IntPtr GetModuleHandle(
            IntPtr lpModuleName
        );

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleFileNameW")]
        private static extern uint GetModuleFileName(
            IntPtr hModule,
            char* lpFilename,
            uint nSize
        );

        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode, EntryPoint = "PathRemoveFileSpecW")]
        private static extern bool PathRemoveFileSpec(
            char* pszPath
        );

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateProcessW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            [MarshalAs(UnmanagedType.Bool)]bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            IntPtr lpCurrentDirectory,
            [In] ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation
        );

        [DllImport("User32.dll")]
        private static extern uint WaitForInputIdle(
            IntPtr hProcess,
            uint dwMilliseconds
        );

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public uint cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [DllImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore InconsistentNaming

        public override void LoadGame()
        {
            SetActiveMod("Native");

            var szPath = (char*)LocalAlloc(LMEM_FIXED, (UIntPtr)1024u);
            GetModuleFileName(GetModuleHandle(IntPtr.Zero), szPath, 1024u);
            PathRemoveFileSpec(szPath);
            var pathToExe = new string(szPath);

            var warbandPath = FindWarbandPath(pathToExe, out var warbandFound);

            if (!warbandFound)
            {
                MessageBox.Show("Could not find mb_warband.exe");
                goto clear_mem;
            }

            string wseLoader;
            if (File.Exists(wseLoader = Path.Combine(pathToExe, "WSELoader.exe")))
                goto wse_founded;
            if (File.Exists(wseLoader = warbandPath.Replace("\\mb_warband.exe", "\\WSELoader.exe")))
                goto wse_founded;

            //TODO: Download WSE
            MessageBox.Show("Could not find WSELoader.exe");
            goto clear_mem;

        wse_founded:
            var si = new STARTUPINFO();
            si.cb = (uint)Marshal.SizeOf(si);
            var cmdLineArgs = $"\"{wseLoader}\" --path \"{warbandPath}\"\0";

            var created = CreateProcess(wseLoader, cmdLineArgs, IntPtr.Zero, IntPtr.Zero, false, HIGH_PRIORITY_CLASS, IntPtr.Zero,
                IntPtr.Zero, ref si, out var pi);

            if (!created)
            {
                MessageBox.Show("CreateProcess failed " + Marshal.GetLastWin32Error());
                goto clear_mem;
            }

            WaitForInputIdle(pi.hProcess, INFINITE);
            
            PushPlayButton();

        clear_mem:
            LocalFree((IntPtr)szPath);
        }
    }
}
