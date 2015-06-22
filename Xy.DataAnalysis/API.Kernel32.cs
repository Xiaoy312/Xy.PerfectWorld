using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Xy.DataAnalysis
{
    internal class API
    {
        #region Access Right Consts
        public const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        public const int SYNCHRONIZE = 0x100000;
        public const int PROCESS_CREATE_THREAD = 0x2;
        public const int PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFF; //0x1F0FFF;

        public const int PROCESS_VM_OPERATION = 0x8;
        public const int PROCESS_VM_READ = 0x10;
        public const int PROCESS_VM_WRITE = 0x20;
        public const int PROCESS_VM_ALL = PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE;

        public const int MEM_RESERVE = 0x2000;
        public const int MEM_COMMIT = 0x1000;
        public const int MEM_RELEASE = 0x8000;

        public const int PAGE_EXECUTE_READWRITE = 0x4;


        public const int ALMOST_ALL_OF_THEM = PROCESS_ALL_ACCESS | PROCESS_VM_ALL | PROCESS_CREATE_THREAD;
        #endregion

        #region OpenProcess
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            int dwDesiredAccess,
            IntPtr bInheritHandle,
            IntPtr dwProcessId
            );
        #endregion
        #region ReadProcessMemory
        [DllImport("kernel32", SetLastError = true)]
        public static extern int ReadProcessMemory(
            IntPtr hProcess,
            int lpBase,
            ref int lpBuffer,
            int nSize,
            int lpNumberOfBytesRead
            );
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
                    IntPtr hProcess,
                    int lpBase,
                    byte[] lpBuffer,
                    int nSize,
                    int lpNumberOfBytesRead
                    );
        [DllImport("kernel32", SetLastError = true)]
        public static extern int ReadProcessMemory(
            IntPtr hProcess,
            int lpBase,
            ref float lpBuffer,
            int nSize,
            int lpNumberOfBytesRead
            );
        [DllImport("kernel32", SetLastError = true)]
        public static extern int ReadProcessMemory(
            IntPtr hProcess,
            int lpBase,
            ref uint lpBuffer,
            int nSize,
            int lpNumberOfBytesRead
            );

        #endregion
        #region WriteProcessMemory
        [DllImport("kernel32", SetLastError = true)]
        public static extern int WriteProcessMemory(
            IntPtr hProcess,
            int lpBase,
            ref float lpBuffer,
            int nSize,
            int lpNumberOfBytesWritten
            );
        [DllImport("kernel32", SetLastError = true)]
        public static extern int WriteProcessMemory(
            IntPtr hProcess,
            int lpBase,
            ref int lpBuffer,
            int nSize,
            int lpNumberOfBytesWritten
            );
        [DllImport("kernel32", SetLastError = true)]
        public static extern int WriteProcessMemory(
            IntPtr hProcess,
            int lpBase,
            byte[] lpBuffer,
            int nSize,
            int lpNumberOfBytesWritten
            );
        #endregion

        #region VirtualAllocEx
        [DllImport("Kernel32.dll")]
        public static extern int VirtualAllocEx(
            IntPtr hProcess,
            int lpAddress,
            int dwSize,
            int flAllocationType,
            int flProtect
            );
        #endregion
        #region VirtualFreeEx
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(
            IntPtr hProcess,
            int lpAddress,
            int dwSize,
            FreeType dwFreeType
            );

        [Flags]
        public enum FreeType
        {
            Decommit = 0x4000,
            Release = 0x8000,
        }
        #endregion
        #region CreateRemoteThread
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            int lpThreadAttributes,
            int dwStackSize,
            int lpStartAddress,
            int lpParameter,
            int dwCreationFlags,
            int lpThreadId
            );
        #endregion
        #region WaitForSingleObject
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        #endregion
        #region CloseHandle
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject); 
        #endregion
    }
}
