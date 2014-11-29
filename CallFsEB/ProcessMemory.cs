using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CallFsEB
{
    public class ProcessMemory
    {
        #region Asm Stuff

        private byte[] pushafq = {
            // save flags
            0x9C,       // pushfq

            // save registers
            0x40, 0x50, // push rax
            0x40, 0x51, // push rcx
            0x40, 0x52, // push rdx
            0x40, 0x53, // push rbx
            0x40, 0x54, // push rsp <>
            0x40, 0x55, // push rbp
            0x40, 0x56, // push rsi
            0x40, 0x57, // push rdi
            0x41, 0x50, // push r8
            0x41, 0x51, // push r9
            0x41, 0x52, // push r10
            0x41, 0x53, // push r11
            0x41, 0x54, // push r12
            0x41, 0x55, // push r13
            0x41, 0x56, // push r14
            0x41, 0x57, // push r15
        };

        private byte[] popafq = {
            // restore registers
            0x41, 0x5F, // pop r15
            0x41, 0x5E, // pop r14
            0x41, 0x5D, // pop r13
            0x41, 0x5C, // pop r12
            0x41, 0x5B, // pop r11
            0x41, 0x5A, // pop r10
            0x41, 0x59, // pop r9
            0x41, 0x58, // pop r8
            0x40, 0x5F, // pop rdi
            0x40, 0x5E, // pop rsi
            0x40, 0x5D, // pop rbp
            0x40, 0x5C, // pop rsp <>
            0x40, 0x5B, // pop rbx
            0x40, 0x5A, // pop rdx
            0x40, 0x59, // pop rcx
            0x40, 0x58, // pop rax

            // restore flags
            0x9D,       // popfq
        };

        #endregion

        #region API

        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAccess DesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwThreadId);
        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, FreeType dwFreeType);
        [DllImport("kernel32", SetLastError = true)]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32", SetLastError = true)]
        public static extern uint SuspendThread(IntPtr thandle);
        [DllImport("kernel32", SetLastError = true)]
        public static extern uint ResumeThread(IntPtr thandle);
        [DllImport("kernel32", SetLastError = true)]
        public static unsafe extern bool GetThreadContext(IntPtr thandle, CONTEXT* context);
        [DllImport("kernel32", SetLastError = true)]
        public static unsafe extern bool SetThreadContext(IntPtr thandle, CONTEXT* context);
        [DllImport("user32", SetLastError = true)]
        static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32", SetLastError = true)]
        static extern bool FlushInstructionCache(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr dwSize);

        [DllImport("ntdll.dll", SetLastError = true)]
        static extern IntPtr NtSuspendProcess(IntPtr ProcessHandle);
        [DllImport("ntdll.dll", SetLastError = true)]
        static extern IntPtr NtResumeProcess(IntPtr ProcessHandle);

        #endregion

        
        /// <summary>
        /// Возвращает текущий процесс.
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Yanitta.ProcessMemory"/>.
        /// </summary>
        /// <param name="process"></param>
        public ProcessMemory(Process process)
        {
            this.Process = process;
        }

        /// <summary>
        /// Выделяет в процессе участок памяти.
        /// </summary>
        /// <param name="size">Размер выделяемой памяти.</param>
        /// <param name="allocType">Тип выделяемой памяти.</param>
        /// <param name="memProtect">Тип защиты памяти.</param>
        /// <returns>Указатель на выделенный участок памяти.</returns>
        public IntPtr Alloc(int size, AllocationType allocType = AllocationType.Commit, MemoryProtection memProtect = MemoryProtection.ExecuteReadWrite)
        {
            if (size <= 0)
                throw new ArgumentNullException("size");

            var address = VirtualAllocEx(this.Process.Handle, IntPtr.Zero, size, allocType, memProtect);

            if (address == IntPtr.Zero)
                throw new Win32Exception();

            return address;
        }

        /// <summary>
        /// Осводождает ранее выделенный участок памяти.
        /// </summary>
        /// <param name="address">Указатель на выделенный участок памяти.</param>
        /// <param name="freeType">Тип осводождения памяти.</param>
        public void Free(IntPtr address, FreeType freeType = FreeType.Release)
        {
            if (address == IntPtr.Zero)
                throw new ArgumentNullException("address");

            if (!VirtualFreeEx(this.Process.Handle, address, 0, freeType))
                throw new Win32Exception();
        }

        /// <summary>
        /// Считывает массив байт из текущего процесса.
        /// </summary>
        /// <param name="address">Указатель на участок памяти с которого надо начать считывание.</param>
        /// <param name="count">Размер считываемого массива.</param>
        /// <returns>Считанный из процесса масив.</returns>
        public unsafe byte[] ReadBytes(IntPtr address, int count)
        {
            var bytes = new byte[count];
            if(!ReadProcessMemory(this.Process.Handle, address, bytes, count, IntPtr.Zero))
                throw new Win32Exception();
            return bytes;
        }

        /// <summary>
        /// Считывает из процесса значение указанного типа.
        /// </summary>
        /// <typeparam name="T">Тип считываемого значения.</typeparam>
        /// <param name="address">Указатель на участок памяти от куда надо считать значение.</param>
        /// <returns>Значение указанного типа.</returns>
        public unsafe T Read<T>(IntPtr address) where T : struct
        {
            var result = new byte[Marshal.SizeOf(typeof(T))];
            ReadProcessMemory(this.Process.Handle, address, result, result.Length, IntPtr.Zero);
            var handle = GCHandle.Alloc(result, GCHandleType.Pinned);
            T returnObject = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return returnObject;
        }

        /// <summary>
        /// Считывает из процесса строку заканчивающуюся 0 в кодировке utf-8.
        /// </summary>
        /// <param name="addess">Указатель на участок памяти от куда надо считать значение.</param>
        /// <param name="length">Длинна строки (ограничение).</param>
        /// <returns>Считанная строка</returns>
        public string ReadString(IntPtr addess, int length = 100)
        {
            var result = new byte[length];
            if (!ReadProcessMemory(this.Process.Handle, addess, result, length, IntPtr.Zero))
                throw new Win32Exception();
            return Encoding.UTF8.GetString(result.TakeWhile(ret => ret != 0).ToArray());
        }

        /// <summary>
        /// Записывает в память процесса значение указанного типа.
        /// </summary>
        /// <typeparam name="T">Тип записываемого значения.</typeparam>
        /// <param name="value">Значение, которое надо записать в память процесса.</param>
        /// <returns>Указатель на участок памяти куда записано значение.</returns>
        public IntPtr Write<T>(T value) where T : struct
        {
            var buffer  = new byte[Marshal.SizeOf(value)];
            var hObj    = Marshal.AllocHGlobal(buffer.Length);
            var address = Alloc(buffer.Length);
            if (address == IntPtr.Zero)
                throw new Win32Exception();
            try
            {
                Marshal.StructureToPtr(value, hObj, false);
                Marshal.Copy(hObj, buffer, 0, buffer.Length);
                if (!WriteProcessMemory(this.Process.Handle, address, buffer, buffer.Length, IntPtr.Zero))
                    throw new Win32Exception();
            }
            catch
            {
                Free(address);
            }
            finally
            {
                Marshal.FreeHGlobal(hObj);
            }

            return address;
        }

        /// <summary>
        /// Записывает в память процесса значение указанного типа.
        /// </summary>
        /// <typeparam name="T">Тип записываемого значения.</typeparam>
        /// <param name="address">Указатель на участок памяти куда надо записать значение.</param>
        /// <param name="value">Значение, которое надо записать в память процесса.</param>
        public void Write<T>(IntPtr address, T value) where T : struct
        {
            var buffer = new byte[Marshal.SizeOf(value)];
            var hObj   = Marshal.AllocHGlobal(buffer.Length);
            try
            {
                Marshal.StructureToPtr(value, hObj, false);
                Marshal.Copy(hObj, buffer, 0, buffer.Length);
                if (!WriteProcessMemory(this.Process.Handle, address, buffer, buffer.Length, IntPtr.Zero))
                    throw new Win32Exception();
            }
            finally
            {
                Marshal.FreeHGlobal(hObj);
            }
        }

        /// <summary>
        /// Затисывает массив байт в память процесса.
        /// </summary>
        /// <param name="buffer">Массив байт.</param>
        /// <returns>Указатель на участок памяти куда записан массив.</returns>
        public IntPtr Write(byte[] buffer)
        {
            var addr = this.Alloc(buffer.Length);
            if (addr == IntPtr.Zero)
                throw new Win32Exception();
            this.Write(addr, buffer);
            return addr;
        }

        /// <summary>
        /// Затисывает массив байт в память процесса.
        /// </summary>
        /// <param name="address">Указатель на участок памяти куда надо записать массив.</param>
        /// <param name="buffer">Массив байт.</param>
        public void Write(IntPtr address, byte[] buffer)
        {
            if (!WriteProcessMemory(this.Process.Handle, address, buffer, buffer.Length, IntPtr.Zero))
                throw new Win32Exception();
        }

        /// <summary>
        /// Записывает в память процесса строку по указанному аддрессу в кодировке utf-8.
        /// </summary>
        /// <param name="address">Указатель на участок памяти куда надо записать строку.</param>
        /// <param name="str">Записываемая строка.</param>
        public void WriteCString(IntPtr address, string str)
        {
            var buffer = Encoding.UTF8.GetBytes(str + '\0');
            if (!WriteProcessMemory(this.Process.Handle, address, buffer, buffer.Length, IntPtr.Zero))
                throw new Win32Exception();
        }

        /// <summary>
        /// Записывает в память процесса указанную строку.
        /// </summary>
        /// <param name="str">Строка для записи в память.</param>
        /// <returns>Указатель на строку в памяти.</returns>
        public IntPtr WriteCString(string str)
        {
            var buffer = Encoding.UTF8.GetBytes(str + '\0');
            var address = Alloc(buffer.Length);
            if (!WriteProcessMemory(this.Process.Handle, address, buffer, buffer.Length, IntPtr.Zero))
                throw new Win32Exception();
            return address;
        }

        public unsafe CONTEXT GetContext(uint contextFlag = 0x100001u)
        {
            var tHandle = OpenThread(ThreadAccess.All, false, this.Process.Threads[0].Id);
            if (NtSuspendProcess(this.Process.Handle) != IntPtr.Zero)
                throw new Win32Exception();

            // don't use new CONTEXT
            // because __declspec(align(16)) _CONTEXT
            var context = (CONTEXT*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CONTEXT)));
            context->ContextFlags = contextFlag; // CONTEXT_CONTROL

            if (!GetThreadContext(tHandle, context))
                throw new Win32Exception();

            if (NtResumeProcess(this.Process.Handle) != IntPtr.Zero)
                throw new Win32Exception();

            var rcont = new CONTEXT { ContextFlags = context->ContextFlags, Rip = context->Rip };

            Marshal.FreeHGlobal((IntPtr)context);

            return rcont;
        }

        /// <summary>
        /// Выполняет функцию по указанному адрессу с указанным списком аргуметов.
        /// </summary>
        /// <param name="address">Относительный адресс выполняемой функции.</param>
        /// <param name="funcArgs">
        /// Параметры функции.
        /// Параметрами могут выступать как и значения так и указатели на значения.
        /// </param>
        public unsafe void Call(IntPtr injAddress, IntPtr funcAddress, params long[] funcArgs)
        {
            var tHandle = OpenThread(ThreadAccess.All, false, this.Process.Threads[0].Id);

            //if (SuspendThread(tHandle) == 0xFFFFFFFF)
            //    throw new Win32Exception();

            if (NtSuspendProcess(this.Process.Handle) != IntPtr.Zero)
                throw new Win32Exception();

            // don't use new CONTEXT
            // because __declspec(align(16)) _CONTEXT
            var context = (CONTEXT*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CONTEXT)));
            context->ContextFlags = 0x100001u; // CONTEXT_CONTROL

            if (!GetThreadContext(tHandle, context))
                throw new Win32Exception();

            var checkAddr = this.Write<uint>(0);
            var bytes = new List<byte>();

            #region ASM

            byte minStackSize = 0x20;
            var reservStack  = (byte)Math.Max(funcArgs.Length * IntPtr.Size, minStackSize);

            // save flags and registers
            bytes.AddRange(pushafq);

            // code

            // sub rsp, reservStack
            bytes.AddRange(new byte[] { 0x48, 0x83, 0xEC, reservStack });

            #region Function arguments

            // 4 first argument of the function is stored in the registers
            if (funcArgs.Length > 0)
            {
                // mov rcx, funcArgs[0]
                bytes.AddRange(new byte[] { 0x48, 0xB9 });
                bytes.AddRange(BitConverter.GetBytes(funcArgs[0]));
            }

            if (funcArgs.Length > 1)
            {
                // mov rdx, funcArgs[1]
                bytes.AddRange(new byte[] { 0x48, 0xBA });
                bytes.AddRange(BitConverter.GetBytes(funcArgs[1]));
            }

            if (funcArgs.Length > 2)
            {
                // mov r8, funcArgs[2]
                bytes.AddRange(new byte[] { 0x49, 0xB8 });
                bytes.AddRange(BitConverter.GetBytes(funcArgs[2]));
            }

            if (funcArgs.Length > 3)
            {
                // mov r9, funcArgs[3]
                bytes.AddRange(new byte[] { 0x49, 0xB9 });
                bytes.AddRange(BitConverter.GetBytes(funcArgs[3]));
            }

            // other arguments on the stack
            byte displacement = minStackSize;
            for (int i = 4; i < funcArgs.Length; ++i)
            {
                // mov rax, param
                bytes.AddRange(new byte[] { 0x48, 0xB8 });
                bytes.AddRange(BitConverter.GetBytes(funcArgs[i]));

                // mov qword ptr [rsp+i], rax
                bytes.AddRange(new byte[] { 0x48, 0x89, 0x44, 0x24, displacement });

                //
                displacement += (byte)IntPtr.Size;
            }

            #endregion

            // mov rax, funcPtr
            bytes.AddRange(new byte[] { 0x48, 0xB8 });
            bytes.AddRange(BitConverter.GetBytes(funcAddress.ToInt64()));

            // call rax
            bytes.AddRange(new byte[] { 0xFF, 0xD0 });

            // mov rax, checkAddr
            bytes.AddRange(new byte[] { 0x48, 0xB8 });
            bytes.AddRange(BitConverter.GetBytes(checkAddr.ToInt64()));

            // mov dword[rax], 0xDEADBEEF
            bytes.AddRange(new byte[] { 0xC7, 0x00, 0xEF, 0xBE, 0xAD, 0xDE });

            // add rsp, reservStack
            bytes.AddRange(new byte[] { 0x48, 0x83, 0xC4, reservStack });

            //restore registers and flags
            bytes.AddRange(popafq);

            #region push rip

            Console.WriteLine("Rip: 0x{0:X16}", context->Rip);

            var lorip = (uint)((context->Rip >> 00) & 0xFFFFFFFF);
            var hirip = (uint)((context->Rip >> 32) & 0xFFFFFFFF);

            // push to stack next instruction address
            bytes.Add(0x68); // push lo
            bytes.AddRange(BitConverter.GetBytes(lorip));
            
            // mov [rsp+4], hi
            bytes.AddRange(new byte[] { 0xC7, 0x44, 0x24, 0x04 });
            bytes.AddRange(BitConverter.GetBytes(hirip));

            #endregion

            if (context->Rip <= 0xFFFFFFFFUL)
            {
                // retn
                bytes.Add(0xC3);
            }
            else
            {
                // retfq
                bytes.AddRange(new byte[] { 0x48, 0xCE /*0xCB*/ });
            }

            #endregion

            // Save original code and disable protect
            var oldCode = this.ReadBytes(injAddress, bytes.Count);

            var oldProtect = MemoryProtection.ReadOnly;
            if (!VirtualProtectEx(this.Process.Handle, injAddress, bytes.Count, MemoryProtection.ExecuteReadWrite, out oldProtect))
                throw new Win32Exception();

            Debug.WriteLine("Shell code size: {0}", bytes.Count);
            
            // write shell code
            this.Write(injAddress, bytes.ToArray());

            // set next instruction pointer
            context->Rip = (ulong)injAddress.ToInt64();

            if (!SetThreadContext(tHandle, context))
                throw new Win32Exception();

            if (NtResumeProcess(this.Process.Handle) != IntPtr.Zero)
                throw new Win32Exception();

            //if (ResumeThread(tHandle) == 0xFFFFFFFF)
            //    throw new Win32Exception();

            for (int i = 0; i < 0x100; ++i)
            {
                System.Threading.Thread.Sleep(15);
                if (this.Read<uint>(checkAddr) == 0xDEADBEEF)
                {
                    Debug.WriteLine("iter: " + i);
                    break;
                }
            }

            Marshal.FreeHGlobal((IntPtr)context);
            this.Free(checkAddr);

            // original code
            this.Write(injAddress, oldCode);

            if (!FlushInstructionCache(this.Process.Handle, injAddress, (IntPtr)oldCode.Length))
                throw new Win32Exception();

            // restore protection
            if (!VirtualProtectEx(this.Process.Handle, injAddress, bytes.Count, oldProtect, out oldProtect))
                throw new Win32Exception();
        }

        /// <summary>
        /// Возвращает абсолютный аддресс в процессе.
        /// </summary>
        /// <param name="offset">Относительный аддресс.</param>
        /// <returns>абсолютный аддресс в процессе.</returns>
        public IntPtr Rebase(long offset)
        {
            return new IntPtr(offset + this.Process.MainModule.BaseAddress.ToInt64());
        }

        /// <summary>
        /// Указывает что главное окно процесса находится на переднем плане.
        /// </summary>
        public bool IsFocusWindow
        {
            get { return this.Process.MainWindowHandle == GetForegroundWindow(); }
        }
    }

    #region Enums

    /// <summary>
    /// Тип выделения памяти.
    /// </summary>
    [Flags]
    public enum AllocationType : uint
    {
        Commit     = 0x00001000,
        Reserve    = 0x00002000,
        Decommit   = 0x00004000,
        Release    = 0x00008000,
        Reset      = 0x00080000,
        TopDown    = 0x00100000,
        WriteWatch = 0x00200000,
        Physical   = 0x00400000,
        LargePages = 0x20000000,
    }

    /// <summary>
    /// Тип защиты памяти.
    /// </summary>
    [Flags]
    public enum MemoryProtection : uint
    {
        NoAccess                 = 0x001,
        ReadOnly                 = 0x002,
        ReadWrite                = 0x004,
        WriteCopy                = 0x008,
        Execute                  = 0x010,
        ExecuteRead              = 0x020,
        ExecuteReadWrite         = 0x040,
        ExecuteWriteCopy         = 0x080,
        GuardModifierflag        = 0x100,
        NoCacheModifierflag      = 0x200,
        WriteCombineModifierflag = 0x400,
    }

    /// <summary>
    /// Тип освобождения памяти.
    /// </summary>
    [Flags]
    public enum FreeType : uint
    {
        Decommit = 0x4000,
        Release  = 0x8000,
    }

    /// <summary>
    /// Тип доступа к процессу.
    /// </summary>
    [Flags]
    public enum ThreadAccess : uint
    {
        Terminate           = 0x00001,
        SuspendResume       = 0x00002,
        GetContext          = 0x00008,
        SetContext          = 0x00010,
        SetInformation      = 0x00020,
        QueryInformation    = 0x00040,
        SetThreadToken      = 0x00080,
        Impersonate         = 0x00100,
        DirectImpersonation = 0x00200,
        All                 = 0x1FFFFF
    }

    /// <summary>
    /// Contains processor-specific register data.
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

    /// <summary>
    /// Contains processor-specific register data.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 716)]
    public struct WOW64_CONTEXT
    {
        /// <summary>
        /// Context flag.
        /// </summary>
        [FieldOffset(0x00)]
        public uint ContextFlags;

        /// <summary>
        /// Next instruction pointer.
        /// </summary>
        [FieldOffset(0xB8)]
        public uint Eip;
    };

    #endregion
}