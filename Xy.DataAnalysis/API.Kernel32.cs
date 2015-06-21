using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Xy.DataAnalysis
{
    public class API
    {
        #region Access Right Consts
        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const int SYNCHRONIZE = 0x100000;
        private const int PROCESS_CREATE_THREAD = 0x2;
        private const int PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFF; //0x1F0FFF;

        private const int PROCESS_VM_OPERATION = 0x8;
        private const int PROCESS_VM_READ = 0x10;
        private const int PROCESS_VM_WRITE = 0x20;
        private const int PROCESS_VM_ALL = PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE;

        private const int MEM_RESERVE = 0x2000;
        private const int MEM_COMMIT = 0x1000;
        private const int MEM_RELEASE = 0x8000;

        private const int PAGE_EXECUTE_READWRITE = 0x4;


        public const int ALMOST_ALL_OF_THEM = PROCESS_ALL_ACCESS | PROCESS_VM_ALL | PROCESS_CREATE_THREAD;
        #endregion

        #region OpenProcess
        [DllImport("kernel32", SetLastError = true)]
        internal static extern IntPtr OpenProcess(
            int dwDesiredAccess,
            IntPtr bInheritHandle,
            IntPtr dwProcessId
            ); 
        #endregion
        #region ReadProcessMemory
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int ReadProcessMemory(
            IntPtr hProcess,
            int lpBase,
            ref int lpBuffer,
            int nSize,
            int lpNumberOfBytesRead
            );
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(
                    IntPtr hProcess,
                    int lpBase,
                    byte[] lpBuffer,
                    int nSize,
                    int lpNumberOfBytesRead
                    );
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int ReadProcessMemory(
            IntPtr hProcess,
            int lpBase,
            ref float lpBuffer,
            int nSize,
            int lpNumberOfBytesRead
            );
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int ReadProcessMemory(
            IntPtr hProcess,
            int lpBase,
            ref uint lpBuffer,
            int nSize,
            int lpNumberOfBytesRead
            );
        
        #endregion
        #region WriteProcessMemory
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int WriteProcessMemory(
            IntPtr hProcess,
            int lpBase,
            ref float lpBuffer,
            int nSize,
            int lpNumberOfBytesWritten
            );
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int WriteProcessMemory(
            IntPtr hProcess,
            int lpBase,
            ref int lpBuffer,
            int nSize,
            int lpNumberOfBytesWritten
            );
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int WriteProcessMemory(
            IntPtr hProcess,
            int lpBase,
            byte[] lpBuffer,
            int nSize,
            int lpNumberOfBytesWritten
            );
        #endregion
    }
}
