using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace TestGetContext
{
    class Program
    {

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr OpenThread(int DesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwThreadId);
        [DllImport("kernel32", SetLastError = true)]
        public static extern uint SuspendThread(IntPtr thandle);
        [DllImport("kernel32", SetLastError = true)]
        public static extern uint ResumeThread(IntPtr thandle);
        [DllImport("kernel32", SetLastError = true)]
        public unsafe static extern bool GetThreadContext(IntPtr thandle, void* context);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool GetThreadContext(IntPtr thandle, ref CONTEXT context);

        static unsafe void Main(string[] args)
        {
            var s = "AsFesgRT".Count(n => char.IsLower(n));
            Console.Title = "CONTEXT Sixe = " + Marshal.SizeOf(typeof(CONTEXT)); 

            Console.Write("Plese enter the thread Id: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
                return;

            // open thread
            var handle = OpenThread(0x1FFFFF, false, id);

            //var context = new CONTEXT { ContextFlags = 0x100001u /* CONTROL */ };

            var ctx_size = Marshal.SizeOf(typeof(CONTEXT));
            fixed (byte* ptr = new byte[ctx_size])
            {
                var context = (CONTEXT*)Marshal.AllocHGlobal(ctx_size);
                for (int i = 0; i < ctx_size; ++i)
                    Marshal.WriteByte((IntPtr)(context + i), 0);

                context->ContextFlags = 0x100001u;

                var ctx_size2 = Marshal.SizeOf(typeof(CONTEXT2));
                var ptr2 = (CONTEXT*)Marshal.AllocHGlobal(ctx_size2);
                ptr2->ContextFlags = 0x100001u;

                try
                {
                    if (handle == IntPtr.Zero)
                        throw new Win32Exception();

                    if (SuspendThread(handle) == 0xFFFFFFFF)
                        throw new Win32Exception();

                    if (!GetThreadContext(handle, context))
                        throw new Win32Exception();
                    Console.WriteLine("ContextFlags (alloc): 0x{0:X}", context->ContextFlags);
                    Console.WriteLine("Rip (alloc): 0x{0:X}", context->Rip);


                    if (!GetThreadContext(handle, ptr2))
                        throw new Win32Exception();
                    Console.WriteLine("ContextFlags (alloc): 0x{0:X}", ptr2->ContextFlags);
                    Console.WriteLine("Rip (alloc): 0x{0:X}", ptr2->Rip);


                    //if (!GetThreadContext(handle, ref context))
                    //    throw new Win32Exception();
                    //Console.WriteLine("Rip (ref): 0x{0:X}", context.Rip);
                }
                catch (Win32Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Message: " + ex.Message);
                    Console.WriteLine("Error code: 0x{0:X} ({0})", ex.ErrorCode);
                    Console.WriteLine("Native error code: 0x{0:X} ({0})", ex.NativeErrorCode);
                }
                finally
                {
                    Marshal.FreeHGlobal((IntPtr)context);
                    Marshal.FreeHGlobal((IntPtr)ptr2);
                    ResumeThread(handle);
                    Console.ReadLine();
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 1232, Pack = 16)]
    public struct CONTEXT
    {
        /// <summary>
        /// Context flag.
        /// </summary>
        [FieldOffset(0x30)]
        public uint ContextFlags;

        /// <summary>
        /// Next instruction pointer.
        /// </summary>
        [FieldOffset(0xF8)]
        public ulong Rip;
    };

    [StructLayout(LayoutKind.Sequential, Size = 1232, Pack = 16)]
    public struct CONTEXT2
    {
        public ulong P1Home; // + 0
        public ulong P2Home; // + 16
        public ulong P3Home; // + 32
        public ulong P4Home; // + 48
        public ulong P5Home; // + 64
        public ulong P6Home; // + 80
        public uint ContextFlags;
        public uint MxCsr;
        public ushort SegCs;
        public ushort SegDs;
        public ushort SegEs;
        public ushort SegFs;
        public ushort SegGs;
        public ushort SegSs;
        public uint EFlags;
        public ulong Dr0;
        public ulong Dr1;
        public ulong Dr2;
        public ulong Dr3;
        public ulong Dr6;
        public ulong Dr7;
        public ulong Rax;
        public ulong Rcx;
        public ulong Rdx;
        public ulong Rbx;
        public ulong Rsp;
        public ulong Rbp;
        public ulong Rsi;
        public ulong Rdi;
        public ulong R8;
        public ulong R9;
        public ulong R10;
        public ulong R11;
        public ulong R12;
        public ulong R13;
        public ulong R14;
        public ulong R15;
        public ulong Rip;
    };
}