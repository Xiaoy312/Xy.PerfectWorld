using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Xy.DataAnalysis
{
    public static class Core
    {
        public static IntPtr Handle { get; private set; }

        public static void Attach(Process process)
        {
            var handle = API.OpenProcess(API.ALMOST_ALL_OF_THEM, IntPtr.Zero, new IntPtr(process.Id));
            if (handle == IntPtr.Zero)
            {
                throw new UnauthorizedAccessException(
                    "Unable to attach to process. Please make sure the application is running with elevated rights. See innner exception for details.",
                    new Win32Exception());
            }

            Handle = handle;
        }

        public static float ReadFloat(int address)
        {
            float value = default(float);
            API.ReadProcessMemory(Handle, address, ref value, sizeof(float), 0);

            return value;
        }
        public static int ReadInt(int address)
        {
            int value = 0;
            API.ReadProcessMemory(Handle, address, ref value, sizeof(int), 0);

            return value;
        }
        public static byte[] ReadBytes(int address, int length)
        {
            var buffer = new byte[length];
            API.ReadProcessMemory(Handle, address, buffer, length, 0);

            return buffer;
        }
        public static int WriteBytes(int address, byte[] buffer)
        {
            return API.WriteProcessMemory(Handle, address, buffer, buffer.Length, 0);
        }
    }
}
