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
        #region Asm save/restore flags and registers

        private byte[] pushafq = {
            // push xmm registers to stack
            0x48, 0x81, 0xEC, 0x00, 0x01, 0x00, 0x00,                   // sub rsp, 16*16

            0xF3,       0x0F, 0x7F, 0x04, 0x24,                         // movdqu [rsp+16*00], xmm0
            0xF3,       0x0F, 0x7F, 0x4C, 0x24, 0x10,                   // movdqu [rsp+16*01], xmm1
            0xF3,       0x0F, 0x7F, 0x4C, 0x24, 0x20,                   // movdqu [rsp+16*02], xmm2
            0xF3,       0x0F, 0x7F, 0x4C, 0x24, 0x30,                   // movdqu [rsp+16*03], xmm3
            0xF3,       0x0F, 0x7F, 0x4C, 0x24, 0x40,                   // movdqu [rsp+16*04], xmm4
            0xF3,       0x0F, 0x7F, 0x4C, 0x24, 0x50,                   // movdqu [rsp+16*05], xmm5
            0xF3,       0x0F, 0x7F, 0x4C, 0x24, 0x60,                   // movdqu [rsp+16*06], xmm6
            0xF3,       0x0F, 0x7F, 0x4C, 0x24, 0x70,                   // movdqu [rsp+16*07], xmm7
            0xF3, 0x44, 0x0F, 0x7F, 0x84, 0x24, 0x80, 0x00, 0x00, 0x00, // movdqu [rsp+16*08], xmm8
            0xF3, 0x44, 0x0F, 0x7F, 0x8C, 0x24, 0x90, 0x00, 0x00, 0x00, // movdqu [rsp+16*09], xmm9
            0xF3, 0x44, 0x0F, 0x7F, 0x94, 0x24, 0xA0, 0x00, 0x00, 0x00, // movdqu [rsp+16*10], xmm10
            0xF3, 0x44, 0x0F, 0x7F, 0x9C, 0x24, 0xB0, 0x00, 0x00, 0x00, // movdqu [rsp+16*11], xmm11
            0xF3, 0x44, 0x0F, 0x7F, 0xA4, 0x24, 0xC0, 0x00, 0x00, 0x00, // movdqu [rsp+16*12], xmm12
            0xF3, 0x44, 0x0F, 0x7F, 0xAC, 0x24, 0xD0, 0x00, 0x00, 0x00, // movdqu [rsp+16*13], xmm13
            0xF3, 0x44, 0x0F, 0x7F, 0xB4, 0x24, 0xE0, 0x00, 0x00, 0x00, // movdqu [rsp+16*14], xmm14
            0xF3, 0x44, 0x0F, 0x7F, 0xBC, 0x24, 0xF0, 0x00, 0x00, 0x00, // movdqu [rsp+16*15], xmm15

            // save registers
            0x40, 0x50, // push rax
            0x40, 0x54, // push rsp
            0x40, 0x51, // push rcx
            0x40, 0x52, // push rdx
            0x40, 0x53, // push rbx
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

            0x9C,       // pushfq
        };

        private byte[] popafq = {
            // restore flags
            0x9D,       // popfq

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
            0x40, 0x5B, // pop rbx
            0x40, 0x5A, // pop rdx
            0x40, 0x59, // pop rcx
            0x40, 0x5C, // pop rsp
            0x40, 0x58, // pop rax

            // pop xmm registers from stack
            0xF3,       0x0F, 0x6F, 0x04, 0x24,                         // movdqu xmm0, [rsp+16*00]
            0xF3,       0x0F, 0x6F, 0x4C, 0x24, 0x10,                   // movdqu xmm1, [rsp+16*01]
            0xF3,       0x0F, 0x6F, 0x54, 0x24, 0x20,                   // movdqu xmm2, [rsp+16*02]
            0xF3,       0x0F, 0x6F, 0x5C, 0x24, 0x30,                   // movdqu xmm3, [rsp+16*03]
            0xF3,       0x0F, 0x6F, 0x5C, 0x24, 0x40,                   // movdqu xmm4, [rsp+16*04]
            0xF3,       0x0F, 0x6F, 0x5C, 0x24, 0x50,                   // movdqu xmm5, [rsp+16*05]
            0xF3,       0x0F, 0x6F, 0x5C, 0x24, 0x60,                   // movdqu xmm6, [rsp+16*06]
            0xF3,       0x0F, 0x6F, 0x5C, 0x24, 0x70,                   // movdqu xmm7, [rsp+16*07]

            0xF3, 0x44, 0x0F, 0x6F, 0x84, 0x24, 0x80, 0x00, 0x00, 0x00, // movdqu xmm8,  [rsp+16*08]
            0xF3, 0x44, 0x0F, 0x6F, 0x8C, 0x24, 0x90, 0x00, 0x00, 0x00, // movdqu xmm9,  [rsp+16*09]
            0xF3, 0x44, 0x0F, 0x6F, 0x94, 0x24, 0xA0, 0x00, 0x00, 0x00, // movdqu xmm10, [rsp+16*10]
            0xF3, 0x44, 0x0F, 0x6F, 0x9C, 0x24, 0xB0, 0x00, 0x00, 0x00, // movdqu xmm11, [rsp+16*11]
            0xF3, 0x44, 0x0F, 0x6F, 0xA4, 0x24, 0xC0, 0x00, 0x00, 0x00, // movdqu xmm12, [rsp+16*12]
            0xF3, 0x44, 0x0F, 0x6F, 0xAC, 0x24, 0xD0, 0x00, 0x00, 0x00, // movdqu xmm13, [rsp+16*13]
            0xF3, 0x44, 0x0F, 0x6F, 0xB4, 0x24, 0xE0, 0x00, 0x00, 0x00, // movdqu xmm14, [rsp+16*14]
            0xF3, 0x44, 0x0F, 0x6F, 0xBC, 0x24, 0xF0, 0x00, 0x00, 0x00, // movdqu xmm15, [rsp+16*15]

            0x48, 0x81, 0xC4, 0x00, 0x01, 0x00, 0x00, // add rsp, 16*16
        };

        #endregion

        #region API

        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAccess DesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwThreadId);
        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, FreeType dwFreeType);
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32", SetLastError = true)]
        public static unsafe extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, void* lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32", SetLastError = true)]
        public static extern uint SuspendThread(IntPtr thandle);
        [DllImport("kernel32", SetLastError = true)]
        public static extern uint ResumeThread(IntPtr thandle);
        [DllImport("kernel32", SetLastError = true)]
        public static unsafe extern bool GetThreadContext(IntPtr thandle, CONTEXT* context);
        [DllImport("kernel32", SetLastError = true)]
        public static unsafe extern bool SetThreadContext(IntPtr thandle, CONTEXT* context);
        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool FlushInstructionCache(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr dwSize);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern IntPtr NtSuspendProcess(IntPtr ProcessHandle);
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern IntPtr NtResumeProcess(IntPtr ProcessHandle);

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
            Process = process;
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

            var address = VirtualAllocEx(Process.Handle, IntPtr.Zero, size, allocType, memProtect);

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

            if (!VirtualFreeEx(Process.Handle, address, 0, freeType))
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
            if(!ReadProcessMemory(Process.Handle, address, bytes, count, IntPtr.Zero))
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
            ReadProcessMemory(Process.Handle, address, result, result.Length, IntPtr.Zero);
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
            if (!ReadProcessMemory(Process.Handle, addess, result, length, IntPtr.Zero))
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
                if (!WriteProcessMemory(Process.Handle, address, buffer, buffer.Length, IntPtr.Zero))
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
                if (!WriteProcessMemory(Process.Handle, address, buffer, buffer.Length, IntPtr.Zero))
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
            var addr = Alloc(buffer.Length);
            if (addr == IntPtr.Zero)
                throw new Win32Exception();
            Write(addr, buffer);
            return addr;
        }

        /// <summary>
        /// Затисывает массив байт в память процесса.
        /// </summary>
        /// <param name="address">Указатель на участок памяти куда надо записать массив.</param>
        /// <param name="buffer">Массив байт.</param>
        public void Write(IntPtr address, byte[] buffer)
        {
            if (!WriteProcessMemory(Process.Handle, address, buffer, buffer.Length, IntPtr.Zero))
                throw new Win32Exception();
        }

        public IntPtr WriteArray<T>(T[] array) where T : struct, IConvertible
        {
            var elementSize = Marshal.SizeOf(typeof(T));
            var arraySize = array.Length * elementSize;
            var ptr = Alloc(arraySize, AllocationType.Reserve | AllocationType.Commit);

            for (int offset = 0, i = 0; offset < arraySize; offset += elementSize, ++i)
            {
                Write<T>(IntPtr.Add(ptr, offset), array[i]);
            }

            return ptr;
        }

        /// <summary>
        /// Записывает в память процесса строку по указанному аддрессу в кодировке utf-8.
        /// </summary>
        /// <param name="address">Указатель на участок памяти куда надо записать строку.</param>
        /// <param name="str">Записываемая строка.</param>
        public void WriteCString(IntPtr address, string str)
        {
            var buffer = Encoding.UTF8.GetBytes(str + '\0');
            if (!WriteProcessMemory(Process.Handle, address, buffer, buffer.Length, IntPtr.Zero))
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
            if (!WriteProcessMemory(Process.Handle, address, buffer, buffer.Length, IntPtr.Zero))
                throw new Win32Exception();
            return address;
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
            var threadId = Process.Threads[0].Id;
            var tHandle = OpenThread(ThreadAccess.All, false, threadId);

            //if (SuspendThread(tHandle) == 0xFFFFFFFF)
            //    throw new Win32Exception();

            // don't use new CONTEXT
            // because __declspec(align(16)) _CONTEXT
            var context = (CONTEXT*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CONTEXT)));
            //context->ContextFlags = 0x100001u; // CONTEXT_CONTROL
            context->ContextFlags = 0x10001F; // CONTEXT_ALL

            if (NtSuspendProcess(Process.Handle) != IntPtr.Zero)
                throw new Win32Exception();

            if (!GetThreadContext(tHandle, context))
                throw new Win32Exception();

            var checkAddr = Write<uint>(0);
            var bytes = new List<byte>();

            #region ASM

            byte minStackSize = 0x20;
            byte reservStack  = (byte)Math.Max(funcArgs.Length * IntPtr.Size, minStackSize);

            // save flags and registers
            bytes.AddRange(pushafq);

            // align stack ???? mabe
            // and rsp, not 0x10
            bytes.Add(0x48, 0x83, 0xE4, 0xEF);

            // sub rsp, reservStack
            bytes.Add(0x48, 0x83, 0xEC, reservStack);

            #region Function arguments

            // 4 first argument of the function is stored in the registers
            if (funcArgs.Length > 0)
            {
                // mov rcx, funcArgs[0]
                bytes.AddRange(new byte[] { 0x48, 0xB9 });
                bytes.AddRange(BitConverter.GetBytes(funcArgs[0]));
                // lea rcx, [rcx]
                bytes.AddRange(new byte[] { 0x48, 0x8D, 0x09 });
            }

            if (funcArgs.Length > 1)
            {
                // mov rdx, funcArgs[1]
                bytes.AddRange(new byte[] { 0x48, 0xBA });
                bytes.AddRange(BitConverter.GetBytes(funcArgs[1]));
                // lea rdx, [rdx]
                bytes.AddRange(new byte[] { 0x48, 0x8D, 0x12 });
            }

            if (funcArgs.Length > 2)
            {
                // mov r8, funcArgs[2]
                bytes.AddRange(new byte[] { 0x49, 0xB8 });
                bytes.AddRange(BitConverter.GetBytes(funcArgs[2]));
                // lea r8, [r8]
                bytes.AddRange(new byte[] { 0x4D, 0x8D, 0x00 });
            }

            if (funcArgs.Length > 3)
            {
                // mov r9, funcArgs[3]
                bytes.AddRange(new byte[] { 0x49, 0xB9 });
                bytes.AddRange(BitConverter.GetBytes(funcArgs[3]));
                // lea r9, [r9]
                bytes.AddRange(new byte[] { 0x4D, 0x8D, 0x09 });
            }

            // other arguments on the stack
            byte displacement = minStackSize;
            for (int i = 4; i < funcArgs.Length; ++i)
            {
                // mov rax, param
                bytes.AddRange(new byte[] { 0x48, 0xB8 });
                bytes.AddRange(BitConverter.GetBytes(funcArgs[i]));
                // lea rax, [rax]
                bytes.AddRange(new byte[] { 0x48, 0x8D, 0x00 });
                // mov qword ptr [rsp+i*8], rax
                bytes.AddRange(new byte[] { 0x48, 0x89, 0x44, 0x24, displacement });

                //
                displacement += (byte)IntPtr.Size;
            }

            #endregion

            // mov rax, funcPtr
            bytes.AddRange(new byte[] { 0x48, 0xB8 });
            bytes.AddRange(BitConverter.GetBytes(funcAddress.ToInt64()));

            // lea rax, rax
            bytes.AddRange(new byte[] { 0x48, 0x8D, 0x00 });

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

            // jump to original address
            // jmp [rsp]
            bytes.AddRange(new byte[]{ 0xFF, 0x25, 0x00, 0x00, 0x00, 0x00 });
            // orig: dq rip
            bytes.AddRange(BitConverter.GetBytes(context->Rip));

            // retn
            bytes.Add(0xC3);

            #endregion

            // Save original code and disable protect
            var oldCode = ReadBytes(injAddress, bytes.Count);

            var oldProtect = MemoryProtection.ReadOnly;
            if (!VirtualProtectEx(Process.Handle, injAddress, bytes.Count, MemoryProtection.ExecuteReadWrite, out oldProtect))
                throw new Win32Exception();

            Debug.WriteLine("Shell code size: {0}", bytes.Count);

            // write shell code
            Write(injAddress, bytes.ToArray());

            // set next instruction pointer
            context->Rip = (ulong)injAddress.ToInt64();

            if (!SetThreadContext(tHandle, context))
                throw new Win32Exception();

            if (NtResumeProcess(Process.Handle) != IntPtr.Zero)
                throw new Win32Exception();

            //if (ResumeThread(tHandle) == 0xFFFFFFFF)
            //    throw new Win32Exception();

            for (int i = 0; i < 0x100; ++i)
            {
                System.Threading.Thread.Sleep(15);
                if (Read<uint>(checkAddr) == 0xDEADBEEF)
                {
                    Debug.WriteLine("iter: " + i);
                    break;
                }
            }

            Marshal.FreeHGlobal((IntPtr)context);
            Free(checkAddr);

            // original code
            Write(injAddress, oldCode);

            if (!FlushInstructionCache(Process.Handle, injAddress, (IntPtr)oldCode.Length))
                throw new Win32Exception();

            // restore protection
            if (!VirtualProtectEx(Process.Handle, injAddress, bytes.Count, oldProtect, out oldProtect))
                throw new Win32Exception();
        }

        public void Call_x32(int injAddress, int funcAddress, params int[] funcArgs)
        {
        }

        /// <summary>
        /// Возвращает абсолютный аддресс в процессе.
        /// </summary>
        /// <param name="offset">Относительный аддресс.</param>
        /// <returns>абсолютный аддресс в процессе.</returns>
        public IntPtr Rebase(long offset)
        {
            return new IntPtr(offset + Process.MainModule.BaseAddress.ToInt64());
        }

        /// <summary>
        /// Указывает что главное окно процесса находится на переднем плане.
        /// </summary>
        public bool IsFocusWindow
        {
            get { return Process.MainWindowHandle == GetForegroundWindow(); }
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