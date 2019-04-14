using System;
using System.Runtime.InteropServices;

namespace HGE.IO
{
    public unsafe class DynamicNativeLibrary : IDisposable
    {
        #region Constructor - fileName

        /// <summary>
        ///     Initializes a new instance of the NativeLibrary class from a native module stored on disk.
        /// </summary>
        /// <param name="lpLibFileName">Native module file name.</param>
        public DynamicNativeLibrary(string fileName)
        {
            _loadedModuleHandle = Win32.LoadLibrary(fileName);

            if (_loadedModuleHandle == IntPtr.Zero)
                throw new Exception("Module could not be loaded.");

            _loadedFromMemory = false;
        }

        #endregion

        #region Constructor - buffer

        /// <summary>
        ///     Initializes a new instance of the NativeLibrary class from a native module byte array.
        /// </summary>
        /// <param name="buffer">Native module byte array.</param>
        public DynamicNativeLibrary(byte[] buffer)
        {
            _loadedModuleHandle = MemoryLoadLibrary(buffer);

            if (_loadedModuleHandle == IntPtr.Zero)
                throw new Exception("Module could not be loaded.");

            _loadedFromMemory = true;
        }

        #endregion

        #region Destructor

        ~DynamicNativeLibrary()
        {
            Dispose(false);
        }

        #endregion

        #region MemoryLoadLibrary

        /// <summary>
        ///     Loads the specified native module from a byte array into the address space of the calling process.
        /// </summary>
        /// <param name="data">Native module byte array.</param>
        /// <returns>If the function succeeds, the return value is a handle to the module.</returns>
        private IntPtr MemoryLoadLibrary(byte[] data)
        {
            fixed (byte* ptr_data = data)
            {
                var dos_header = (Win32.IMAGE_DOS_HEADER*) ptr_data;

                if (dos_header->e_magic != Win32.IMAGE_DOS_SIGNATURE) throw new NotSupportedException();

                byte* ptr_old_header;
                uint old_header_oh_sizeOfImage;
                uint old_header_oh_sizeOfHeaders;
                int image_nt_headers_Size;
                IntPtr old_header_oh_imageBase;

                if (Environment.Is64BitProcess)
                {
                    var old_header = (Win32.IMAGE_NT_HEADERS64*) (ptr_data + dos_header->e_lfanew);
                    if (old_header->Signature != Win32.IMAGE_NT_SIGNATURE) throw new NotSupportedException();

                    old_header_oh_sizeOfImage = old_header->OptionalHeader.SizeOfImage;
                    old_header_oh_sizeOfHeaders = old_header->OptionalHeader.SizeOfHeaders;
                    old_header_oh_imageBase = old_header->OptionalHeader.ImageBase;
                    ptr_old_header = (byte*) old_header;

                    image_nt_headers_Size = sizeof(Win32.IMAGE_NT_HEADERS64);
                }
                else
                {
                    var old_header = (Win32.IMAGE_NT_HEADERS32*) (ptr_data + dos_header->e_lfanew);
                    if (old_header->Signature != Win32.IMAGE_NT_SIGNATURE) throw new NotSupportedException();

                    old_header_oh_sizeOfImage = old_header->OptionalHeader.SizeOfImage;
                    old_header_oh_sizeOfHeaders = old_header->OptionalHeader.SizeOfHeaders;
                    old_header_oh_imageBase = old_header->OptionalHeader.ImageBase;
                    ptr_old_header = (byte*) old_header;

                    image_nt_headers_Size = sizeof(Win32.IMAGE_NT_HEADERS32);
                }

                var codeBase = IntPtr.Zero;

                if (!Environment.Is64BitProcess)
                    codeBase = Win32.VirtualAlloc(old_header_oh_imageBase, old_header_oh_sizeOfImage, Win32.MEM_RESERVE,
                        Win32.PAGE_READWRITE);

                if (codeBase == IntPtr.Zero)
                    codeBase = Win32.VirtualAlloc(IntPtr.Zero, old_header_oh_sizeOfImage, Win32.MEM_RESERVE,
                        Win32.PAGE_READWRITE);

                if (codeBase == IntPtr.Zero)
                    return IntPtr.Zero;

                var memory_module = (MEMORY_MODULE*) Marshal.AllocHGlobal(sizeof(MEMORY_MODULE));
                memory_module->codeBase = (byte*) codeBase;
                memory_module->numModules = 0;
                memory_module->modules = null;
                memory_module->initialized = 0;

                Win32.VirtualAlloc(codeBase, old_header_oh_sizeOfImage, Win32.MEM_COMMIT, Win32.PAGE_READWRITE);

                var headers = Win32.VirtualAlloc(codeBase, old_header_oh_sizeOfHeaders, Win32.MEM_COMMIT,
                    Win32.PAGE_READWRITE);


                // copy PE header to code
                Win32.memcpy((byte*) headers, (byte*) dos_header, dos_header->e_lfanew + old_header_oh_sizeOfHeaders);

                memory_module->headers = &((byte*) headers)[dos_header->e_lfanew];

                if (Environment.Is64BitProcess)
                {
                    var mm_headers_64 = (Win32.IMAGE_NT_HEADERS64*) memory_module->headers;
                    mm_headers_64->OptionalHeader.ImageBase = codeBase;
                }
                else
                {
                    var mm_headers_32 = (Win32.IMAGE_NT_HEADERS32*) memory_module->headers;
                    mm_headers_32->OptionalHeader.ImageBase = codeBase;
                }

                CopySections(ptr_data, ptr_old_header, memory_module);

                var locationDelta = (ulong) codeBase - (ulong) old_header_oh_imageBase;

                if (locationDelta != 0) PerformBaseRelocation(memory_module, locationDelta);

                if (!BuildImportTable(memory_module)) goto error;

                FinalizeSections(memory_module);

                if (!CallDllEntryPoint(memory_module, Win32.DLL_PROCESS_ATTACH)) goto error;

                return (IntPtr) memory_module;

                error:
                MemoryFreeLibrary((IntPtr) memory_module);
                return IntPtr.Zero;
            }
        }

        #endregion

        #region CopySections

        /// <summary>
        ///     Copies sections from a native module file block to the new memory location.
        /// </summary>
        /// <param name="ptr_data">Pointer to a native module byte array.</param>
        /// <param name="ptr_old_headers">Pointer to a source native module headers.</param>
        /// <param name="memory_module">Pointer to a memory module.</param>
        private void CopySections(byte* ptr_data, byte* ptr_old_headers, MEMORY_MODULE* memory_module)
        {
            var codeBase = memory_module->codeBase;
            var section = Win32.IMAGE_FIRST_SECTION(memory_module->headers);

            ushort numberOfSections;
            uint sectionAlignment;

            if (Environment.Is64BitProcess)
            {
                var new_headers = (Win32.IMAGE_NT_HEADERS64*) memory_module->headers;
                numberOfSections = new_headers->FileHeader.NumberOfSections;

                var old_headers = (Win32.IMAGE_NT_HEADERS64*) ptr_old_headers;
                sectionAlignment = old_headers->OptionalHeader.SectionAlignment;
            }
            else
            {
                var new_headers = (Win32.IMAGE_NT_HEADERS32*) memory_module->headers;
                numberOfSections = new_headers->FileHeader.NumberOfSections;

                var old_headers = (Win32.IMAGE_NT_HEADERS32*) ptr_old_headers;
                sectionAlignment = old_headers->OptionalHeader.SectionAlignment;
            }

            uint index;
            byte* dest;

            for (index = 0; index < numberOfSections; index++, section++)
            {
                if (section->SizeOfRawData == 0)
                {
                    if (sectionAlignment > 0)
                    {
                        dest = (byte*) Win32.VirtualAlloc((IntPtr) (codeBase + section->VirtualAddress),
                            sectionAlignment, Win32.MEM_COMMIT, Win32.PAGE_READWRITE);
                        section->PhysicalAddress = (uint) dest;
                        Win32.memset(dest, 0, sectionAlignment);
                    }

                    continue;
                }

                // commit memory block and copy data from dll
                dest = (byte*) Win32.VirtualAlloc((IntPtr) (codeBase + section->VirtualAddress), section->SizeOfRawData,
                    Win32.MEM_COMMIT, Win32.PAGE_READWRITE);
                Win32.memcpy(dest, ptr_data + section->PointerToRawData, section->SizeOfRawData);

                section->PhysicalAddress = (uint) dest;
            }
        }

        #endregion

        #region PerformBaseRelocation

        /// <summary>
        ///     Adjusts base address of the imported data.
        /// </summary>
        /// <param name="memory_module">Pointer to a memory module.</param>
        /// <param name="delta">Adjustment delta value.</param>
        private void PerformBaseRelocation(MEMORY_MODULE* memory_module, ulong delta)
        {
            var directory = GET_HEADER_DIRECTORY(memory_module, Win32.IMAGE_DIRECTORY_ENTRY_BASERELOC);

            if (directory->Size > 0)
            {
                var relocation = (Win32.IMAGE_BASE_RELOCATION*) (memory_module->codeBase + directory->VirtualAddress);

                var sizeOfBaseRelocation = sizeof(Win32.IMAGE_BASE_RELOCATION);

                int index;

                for (; relocation->VirtualAddress > 0;)
                {
                    var dest = memory_module->codeBase + relocation->VirtualAddress;
                    var relInfo = (ushort*) ((byte*) relocation + sizeOfBaseRelocation);

                    for (index = 0; index < (relocation->SizeOfBlock - sizeOfBaseRelocation) / 2; index++, relInfo++)
                    {
                        uint* patchAddrHL32;
                        ulong* patchAddrHL64;

                        uint type, offset;

                        // the upper 4 bits define the type of relocation
                        type = (uint) (*relInfo >> 12);

                        // the lower 12 bits define the offset
                        offset = (uint) (*relInfo & 0xfff);

                        switch (type)
                        {
                            case Win32.IMAGE_REL_BASED_ABSOLUTE:
                                break;

                            case Win32.IMAGE_REL_BASED_HIGHLOW:
                                patchAddrHL32 = (uint*) ((uint) dest + offset);
                                *patchAddrHL32 += (uint) delta;
                                break;


                            case Win32.IMAGE_REL_BASED_DIR64:
                                patchAddrHL64 = (ulong*) ((ulong) dest + offset);
                                *patchAddrHL64 += delta;
                                break;

                            default:
                                break;
                        }
                    }

                    relocation = (Win32.IMAGE_BASE_RELOCATION*) ((byte*) relocation + relocation->SizeOfBlock);
                }
            }
        }

        #endregion

        #region BuildImportTable

        /// <summary>
        ///     Loads required dlls and adjust function table of the imports.
        /// </summary>
        /// <param name="memory_module">Pointer to a memory module.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        private bool BuildImportTable(MEMORY_MODULE* memory_module)
        {
            var result = true;

            var directory = GET_HEADER_DIRECTORY(memory_module, Win32.IMAGE_DIRECTORY_ENTRY_IMPORT);

            if (directory->Size > 0)
            {
                var importDesc = (Win32.IMAGE_IMPORT_DESCRIPTOR*) (memory_module->codeBase + directory->VirtualAddress);

                for (; importDesc->Name != 0; importDesc++)
                {
                    IntPtr* thunkRef;
                    IntPtr* funcRef;

                    var moduleName = Marshal.PtrToStringAnsi((IntPtr) (memory_module->codeBase + importDesc->Name));
                    var handle = Win32.LoadLibrary(moduleName);

                    if (handle == IntPtr.Zero)
                    {
                        result = false;
                        break;
                    }

                    var size_of_pointer = sizeof(IntPtr);

                    memory_module->modules = (IntPtr*) Win32.realloc((byte*) memory_module->modules,
                        (uint) (memory_module->numModules * size_of_pointer),
                        (uint) ((memory_module->numModules + 1) * size_of_pointer));


                    if (memory_module->modules == null)
                    {
                        result = false;
                        break;
                    }

                    memory_module->modules[memory_module->numModules++] = handle;

                    if (importDesc->Characteristics != 0)
                    {
                        thunkRef = (IntPtr*) (memory_module->codeBase + importDesc->Characteristics);
                        funcRef = (IntPtr*) (memory_module->codeBase + importDesc->FirstThunk);
                    }
                    else
                    {
                        thunkRef = (IntPtr*) (memory_module->codeBase + importDesc->FirstThunk);
                        funcRef = (IntPtr*) (memory_module->codeBase + importDesc->FirstThunk);
                    }

                    for (; *thunkRef != IntPtr.Zero; thunkRef++, funcRef++)
                    {
                        if (Win32.IMAGE_SNAP_BY_ORDINAL(thunkRef))
                        {
                            *funcRef = Win32.GetProcAddress(handle, (byte*) Win32.IMAGE_ORDINAL(thunkRef));
                        }
                        else
                        {
                            var thunkData = (Win32.IMAGE_IMPORT_BY_NAME*) (memory_module->codeBase + (ulong) *thunkRef);
                            var procName = Marshal.PtrToStringAnsi((IntPtr) (byte*) thunkData + 2);
                            *funcRef = Win32.GetProcAddress(handle, procName);
                        }

                        if (*funcRef == IntPtr.Zero)
                        {
                            result = false;
                            break;
                        }
                    }

                    if (!result)
                        break;
                }
            }

            return result;
        }

        #endregion

        #region FinalizeSections

        /// <summary>
        ///     Marks memory pages depending on section headers and release sections that are marked as "discardable".
        /// </summary>
        /// <param name="memory_module">Pointer to a memory module.</param>
        private void FinalizeSections(MEMORY_MODULE* memory_module)
        {
            var section = Win32.IMAGE_FIRST_SECTION(memory_module->headers);
            ;

            ushort number_of_sections;
            uint size_of_initialized_data;
            uint size_of_uninitialized_data;

            long image_offset = 0;

            if (Environment.Is64BitProcess)
            {
                var headers = (Win32.IMAGE_NT_HEADERS64*) memory_module->headers;
                number_of_sections = headers->FileHeader.NumberOfSections;
                size_of_initialized_data = headers->OptionalHeader.SizeOfInitializedData;
                size_of_uninitialized_data = headers->OptionalHeader.SizeOfUninitializedData;

                image_offset = (long) ((ulong) headers->OptionalHeader.ImageBase & 0xffffffff00000000);
            }
            else
            {
                var headers = (Win32.IMAGE_NT_HEADERS32*) memory_module->headers;
                number_of_sections = headers->FileHeader.NumberOfSections;
                size_of_initialized_data = headers->OptionalHeader.SizeOfInitializedData;
                size_of_uninitialized_data = headers->OptionalHeader.SizeOfUninitializedData;
            }

            for (var i = 0; i < number_of_sections; i++, section++)
            {
                uint protect, oldProtect, rawDataSize;
                var executable = Convert.ToUInt32((section->Characteristics & Win32.IMAGE_SCN_MEM_EXECUTE) != 0);
                var readable = Convert.ToUInt32((section->Characteristics & Win32.IMAGE_SCN_MEM_READ) != 0);
                var writeable = Convert.ToUInt32((section->Characteristics & Win32.IMAGE_SCN_MEM_WRITE) != 0);

                if ((section->Characteristics & Win32.IMAGE_SCN_MEM_DISCARDABLE) != 0)
                {
                    // section is not needed any more and can safely be freed
                    Win32.VirtualFree((IntPtr) (section->PhysicalAddress | image_offset), section->SizeOfRawData,
                        Win32.MEM_DECOMMIT);
                    continue;
                }

                protect = _protectionFlags[executable, readable, writeable];

                if ((section->Characteristics & Win32.IMAGE_SCN_MEM_NOT_CACHED) != 0)
                    protect |= Win32.PAGE_NOCACHE;

                // determine size of region
                rawDataSize = section->SizeOfRawData;

                if (rawDataSize == 0)
                    if ((section->Characteristics & Win32.IMAGE_SCN_CNT_INITIALIZED_DATA) != 0)
                        rawDataSize = size_of_initialized_data;

                    else if ((section->Characteristics & Win32.IMAGE_SCN_CNT_UNINITIALIZED_DATA) != 0)
                        rawDataSize = size_of_uninitialized_data;

                if (rawDataSize > 0)
                    Win32.VirtualProtect((IntPtr) (section->PhysicalAddress | image_offset), rawDataSize, protect,
                        &oldProtect);
            }
        }

        #endregion

        #region CallDllEntryPoint

        /// <summary>
        ///     Calls module entry point.
        /// </summary>
        /// <param name="memory_module">Pointer to a memory module.</param>
        /// <param name="fdwReason"></param>
        /// <returns>If the function succeeds or if there is no entry point, the return value is true.</returns>
        private bool CallDllEntryPoint(MEMORY_MODULE* memory_module, uint fdwReason)
        {
            uint addressOfEntryPoint;

            if (Environment.Is64BitProcess)
            {
                var headers = (Win32.IMAGE_NT_HEADERS64*) memory_module->headers;
                addressOfEntryPoint = headers->OptionalHeader.AddressOfEntryPoint;
            }
            else
            {
                var headers = (Win32.IMAGE_NT_HEADERS32*) memory_module->headers;
                addressOfEntryPoint = headers->OptionalHeader.AddressOfEntryPoint;
            }

            if (addressOfEntryPoint != 0)
            {
                var dllEntry = (IntPtr) (memory_module->codeBase + addressOfEntryPoint);

                if (dllEntry == IntPtr.Zero) return false;

                var dllEntryProc = (DllEntryProc) Marshal.GetDelegateForFunctionPointer(dllEntry, typeof(DllEntryProc));

                if (dllEntryProc((IntPtr) memory_module->codeBase, fdwReason, 0))
                {
                    if (fdwReason == Win32.DLL_PROCESS_ATTACH)
                        memory_module->initialized = 1;
                    else if (fdwReason == Win32.DLL_PROCESS_DETACH) memory_module->initialized = 0;

                    return true;
                }

                return false;
            }

            return true;
        }

        #endregion

        #region MemoryFreeLibrary

        /// <summary>
        ///     Deattach from the process and do a cleanup.
        /// </summary>
        /// <param name="hModule">Pointer to a memory module.</param>
        private void MemoryFreeLibrary(IntPtr hModule)
        {
            if (hModule == IntPtr.Zero)
                return;

            var memory_module = (MEMORY_MODULE*) hModule;

            if (memory_module != null)
            {
                if (memory_module->initialized != 0) CallDllEntryPoint(memory_module, Win32.DLL_PROCESS_DETACH);

                if (memory_module->modules != null)
                {
                    // free previously opened libraries
                    for (var index = 0; index < memory_module->numModules; index++)
                        if (memory_module->modules[index] != IntPtr.Zero)
                            Win32.FreeLibrary(memory_module->modules[index]);

                    Marshal.FreeHGlobal((IntPtr) memory_module->modules);
                }

                if ((IntPtr) memory_module->codeBase != IntPtr.Zero)
                    Win32.VirtualFree((IntPtr) memory_module->codeBase, 0, Win32.MEM_RELEASE);

                Marshal.FreeHGlobal((IntPtr) memory_module);
            }
        }

        #endregion

        #region GetDelegateForFunction

        /// <summary>
        ///     Retrieves a delegate of an exported function or variable from loaded module.
        /// </summary>
        /// <param name="procName">The function or variable name.</param>
        /// <param name="delegateType">The type of the delegate to be returned.</param>
        /// <returns>A delegate instance that can be cast to the appropriate delegate type.</returns>
        public Delegate GetDelegateForFunction(string procName, Type delegateType)
        {
            var procAddress = GetProcAddress(procName);

            if (procAddress != IntPtr.Zero) return Marshal.GetDelegateForFunctionPointer(procAddress, delegateType);

            return null;
        }

        #endregion

        #region GetProcAddress

        /// <summary>
        ///     Retrieves the address of an exported function or variable from loaded module.
        /// </summary>
        /// <param name="procName">The function or variable name.</param>
        /// <returns>
        ///     If the function succeeds, the return value is the address of the exported function or variable.
        ///     If the function fails, the return value is IntPtr.Zero.
        /// </returns>
        private IntPtr GetProcAddress(string procName)
        {
            if (_loadedModuleHandle == IntPtr.Zero)
                return IntPtr.Zero;

            if (!_loadedFromMemory) return Win32.GetProcAddress(_loadedModuleHandle, procName);

            var memory_module = (MEMORY_MODULE*) _loadedModuleHandle;

            var codeBase = memory_module->codeBase;

            var idx = -1;
            uint i;

            uint* nameRef;
            ushort* ordinal;


            var directory = GET_HEADER_DIRECTORY(memory_module, Win32.IMAGE_DIRECTORY_ENTRY_EXPORT);

            if (directory->Size == 0)
                // no export table found
                return IntPtr.Zero;

            var exports = (Win32.IMAGE_EXPORT_DIRECTORY*) (codeBase + directory->VirtualAddress);

            if (exports->NumberOfNames == 0 || exports->NumberOfFunctions == 0)
                // DLL doesn't export anything
                return IntPtr.Zero;

            // search function name in list of exported names
            nameRef = (uint*) (codeBase + exports->AddressOfNames);
            ordinal = (ushort*) (codeBase + exports->AddressOfNameOrdinals);

            for (i = 0; i < exports->NumberOfNames; i++, nameRef++, ordinal++)
            {
                var procNameHandle = (IntPtr) (byte*) ((uint) codeBase + *nameRef);
                var testProcName = Marshal.PtrToStringAnsi(procNameHandle);

                if (testProcName == procName)
                {
                    idx = *ordinal;
                    break;
                }
            }

            if (idx == -1)
                // exported symbol not found
                return IntPtr.Zero;

            if ((uint) idx > exports->NumberOfFunctions)
                // name <-> ordinal number don't match
                return IntPtr.Zero;

            // AddressOfFunctions contains the RVAs to the "real" functions
            //return (IntPtr)((uint)codeBase + *(uint*)((uint)codeBase + exports->AddressOfFunctions + (idx * 4)));
            return (IntPtr) (codeBase + *(uint*) (codeBase + exports->AddressOfFunctions + idx * 4));
        }

        #endregion

        #region GET_HEADER_DIRECTORY

        private Win32.IMAGE_DATA_DIRECTORY* GET_HEADER_DIRECTORY(MEMORY_MODULE* memory_module, uint index)
        {
            if (Environment.Is64BitProcess)
            {
                var headers = (Win32.IMAGE_NT_HEADERS64*) memory_module->headers;
                return (Win32.IMAGE_DATA_DIRECTORY*) &headers->OptionalHeader.DataDirectory[index];
            }
            else
            {
                var headers = (Win32.IMAGE_NT_HEADERS32*) memory_module->headers;
                return (Win32.IMAGE_DATA_DIRECTORY*) &headers->OptionalHeader.DataDirectory[index];
            }
        }

        #endregion

        #region MEMORY_MODULE

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct MEMORY_MODULE
        {
            public byte* headers;
            public byte* codeBase;
            public IntPtr* modules;
            public int numModules;
            public int initialized;
        }

        #endregion

        #region DllEntryProc

        private delegate bool DllEntryProc(IntPtr hinstDll, uint fdwReason, uint lpReserved);

        #endregion

        #region Private variables

        private IntPtr _loadedModuleHandle;
        private readonly bool _loadedFromMemory;
        private bool _disposed;

        private readonly uint[,,] _protectionFlags = new uint[2, 2, 2]
        {
            {
                /* not executable */ {Win32.PAGE_NOACCESS, Win32.PAGE_WRITECOPY},
                {Win32.PAGE_READONLY, Win32.PAGE_READWRITE}
            },
            {
                /* executable */ {Win32.PAGE_EXECUTE, Win32.PAGE_EXECUTE_WRITECOPY},
                {Win32.PAGE_EXECUTE_READ, Win32.PAGE_EXECUTE_READWRITE}
            }
        };

        #endregion

        #region Dispose

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Dispose - disposing

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // free managed resources
                }

                if (_loadedModuleHandle != IntPtr.Zero)
                {
                    if (_loadedFromMemory)
                        MemoryFreeLibrary(_loadedModuleHandle);
                    else
                        Win32.FreeLibrary(_loadedModuleHandle);

                    _loadedModuleHandle = IntPtr.Zero;
                }

                _disposed = true;
            }
        }

        #endregion

        #endregion
    }

    public static unsafe class Win32
    {
        /// <summary>
        ///     Copies bytes between buffers.
        /// </summary>
        /// <param name="dest">New buffer.</param>
        /// <param name="src">Buffer to copy from.</param>
        /// <param name="size">Number of characters to copy.</param>
        public static void memcpy(byte* dest, byte* src, uint count)
        {
            for (uint i = 0; i < count; i++) *(dest + i) = *(src + i);
        }

        /// <summary>
        ///     Sets buffers to a specified character.
        /// </summary>
        /// <param name="dest">Pointer to destination.</param>
        /// <param name="c">Character to set.</param>
        /// <param name="count">Number of characters.</param>
        public static void memset(byte* dest, byte c, uint count)
        {
            for (uint i = 0; i < count; i++) *dest = c;
        }

        /// <summary>
        ///     Reallocate memory blocks.
        /// </summary>
        /// <param name="memblock">Pointer to previously allocated memory block.</param>
        /// <param name="size">Previously allocated memory block size.</param>
        /// <param name="newsize">New size in bytes.</param>
        /// <returns></returns>
        public static byte* realloc(byte* memblock, uint size, uint newsize)
        {
            var newMemBlock = (byte*) Marshal.AllocHGlobal((int) newsize).ToPointer();

            memcpy(newMemBlock, memblock, size);

            Marshal.FreeHGlobal(new IntPtr(memblock));

            return newMemBlock;
        }

        #region Helpers

        #region IMAGE_FIRST_SECTION

        public static IMAGE_SECTION_HEADER* IMAGE_FIRST_SECTION(byte* ptr_image_nt_headers)
        {
            if (Environment.Is64BitProcess)
            {
                var image_nt_headers = (IMAGE_NT_HEADERS64*) ptr_image_nt_headers;
                return (IMAGE_SECTION_HEADER*) ((long) image_nt_headers +
                                                (long) Marshal.OffsetOf(typeof(IMAGE_NT_HEADERS64), "OptionalHeader") +
                                                image_nt_headers->FileHeader.SizeOfOptionalHeader);
            }
            else
            {
                var image_nt_headers = (IMAGE_NT_HEADERS32*) ptr_image_nt_headers;
                return (IMAGE_SECTION_HEADER*) ((long) image_nt_headers +
                                                (long) Marshal.OffsetOf(typeof(IMAGE_NT_HEADERS32), "OptionalHeader") +
                                                image_nt_headers->FileHeader.SizeOfOptionalHeader);
            }
        }

        #endregion

        #region IMAGE_SNAP_BY_ORDINAL

        public static bool IMAGE_SNAP_BY_ORDINAL(IntPtr* ordinal)
        {
            if (Environment.Is64BitProcess)
                return ((ulong) *ordinal & IMAGE_ORDINAL_FLAG64) != 0;
            return ((uint) *ordinal & IMAGE_ORDINAL_FLAG32) != 0;
        }

        #endregion

        #region IMAGE_ORDINAL

        public static ulong IMAGE_ORDINAL(IntPtr* ordinal)
        {
            return (ulong) *ordinal & 0xffff;
        }

        #endregion

        #endregion

        #region Structures

        #region IMAGE_DOS_HEADER

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_DOS_HEADER // DOS .EXE header
        {
            public ushort e_magic; // Magic number
            public ushort e_cblp; // Bytes on last page of file
            public ushort e_cp; // Pages in file
            public ushort e_crlc; // Relocations
            public ushort e_cparhdr; // Size of header in paragraphs
            public ushort e_minalloc; // Minimum extra paragraphs needed
            public ushort e_maxalloc; // Maximum extra paragraphs needed
            public ushort e_ss; // Initial (relative) SS value
            public ushort e_sp; // Initial SP value
            public ushort e_csum; // Checksum
            public ushort e_ip; // Initial IP value
            public ushort e_cs; // Initial (relative) CS value
            public ushort e_lfarlc; // File address of relocation table
            public ushort e_ovno; // Overlay number
            public fixed ushort e_res[4]; // Reserved ushorts
            public ushort e_oemid; // OEM identifier (for e_oeminfo)
            public ushort e_oeminfo; // OEM information; e_oemid specific
            public fixed ushort e_res2[10]; // Reserved ushorts
            public uint e_lfanew; // File address of new exe header
        }

        #endregion

        #region IMAGE_NT_HEADERS32

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_NT_HEADERS32
        {
            public uint Signature;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER32 OptionalHeader;
        }

        #endregion

        #region IMAGE_NT_HEADERS64

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_NT_HEADERS64
        {
            public uint Signature;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER64 OptionalHeader;
        }

        #endregion

        #region IMAGE_FILE_HEADER

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        }

        #endregion

        #region IMAGE_OPTIONAL_HEADER32

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_OPTIONAL_HEADER32
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public uint BaseOfData;
            public IntPtr ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public uint SizeOfStackReserve;
            public uint SizeOfStackCommit;
            public uint SizeOfHeapReserve;
            public uint SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            public fixed ulong DataDirectory[IMAGE_NUMBEROF_DIRECTORY_ENTRIES];
        }

        #endregion

        #region IMAGE_OPTIONAL_HEADER64

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_OPTIONAL_HEADER64
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public IntPtr ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public ulong SizeOfStackReserve;
            public ulong SizeOfStackCommit;
            public ulong SizeOfHeapReserve;
            public ulong SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            public fixed ulong DataDirectory[IMAGE_NUMBEROF_DIRECTORY_ENTRIES];
        }

        #endregion

        #region IMAGE_DATA_DIRECTORY

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_DATA_DIRECTORY
        {
            public uint VirtualAddress;
            public uint Size;
        }

        #endregion

        #region IMAGE_SECTION_HEADER

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_SECTION_HEADER
        {
            public fixed byte Name[IMAGE_SIZEOF_SHORT_NAME];
            public uint PhysicalAddress;
            public uint VirtualAddress;
            public uint SizeOfRawData;
            public uint PointerToRawData;
            public uint PointerToRelocations;
            public uint PointerToLinenumbers;
            public ushort NumberOfRelocations;
            public ushort NumberOfLinenumbers;
            public uint Characteristics;
        }

        #endregion

        #region IMAGE_BASE_RELOCATION

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_BASE_RELOCATION
        {
            public uint VirtualAddress;
            public uint SizeOfBlock;
        }

        #endregion

        #region IMAGE_IMPORT_DESCRIPTOR

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_IMPORT_DESCRIPTOR
        {
            public uint Characteristics;
            public uint TimeDateStamp;
            public uint ForwarderChain;
            public uint Name;
            public uint FirstThunk;
        }

        #endregion

        #region IMAGE_IMPORT_BY_NAME

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_IMPORT_BY_NAME
        {
            public ushort Hint;
            public fixed byte Name[1];
        }

        #endregion

        #region IMAGE_EXPORT_DIRECTORY

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_EXPORT_DIRECTORY
        {
            public uint Characteristics;
            public uint TimeDateStamp;
            public ushort MajorVersion;
            public ushort MinorVersion;
            public uint Name;
            public uint Base;
            public uint NumberOfFunctions;
            public uint NumberOfNames;
            public uint AddressOfFunctions; // RVA from base of image
            public uint AddressOfNames; // RVA from base of image
            public uint AddressOfNameOrdinals; // RVA from base of image
        }

        #endregion

        #endregion

        #region Constants

        public const uint IMAGE_DOS_SIGNATURE = 0x5A4D; // MZ
        public const uint IMAGE_OS2_SIGNATURE = 0x454E; // NE
        public const uint IMAGE_OS2_SIGNATURE_LE = 0x454C; // LE
        public const uint IMAGE_VXD_SIGNATURE = 0x454C; // LE
        public const uint IMAGE_NT_SIGNATURE = 0x00004550; // PE00

        public const int IMAGE_SIZEOF_SHORT_NAME = 8;

        public const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;

        public const ulong IMAGE_ORDINAL_FLAG64 = 0x8000000000000000;
        public const uint IMAGE_ORDINAL_FLAG32 = 0x80000000;

        public const uint IMAGE_SCN_TYPE_NO_PAD = 0x00000008; // Reserved.

        public const uint IMAGE_SCN_CNT_CODE = 0x00000020; // Section contains code.
        public const uint IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040; // Section contains initialized data.
        public const uint IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080; // Section contains uninitialized data.

        public const uint IMAGE_SCN_LNK_OTHER = 0x00000100; // Reserved.

        public const uint
            IMAGE_SCN_LNK_INFO = 0x00000200; // Section contains comments or some other type of information.

        public const uint IMAGE_SCN_LNK_REMOVE = 0x00000800; // Section contents will not become part of image.
        public const uint IMAGE_SCN_LNK_COMDAT = 0x00001000; // Section contents comdat.

        public const uint
            IMAGE_SCN_NO_DEFER_SPEC_EXC =
                0x00004000; // Reset speculative exceptions handling bits in the TLB entries for this section.

        public const uint IMAGE_SCN_GPREL = 0x00008000; // Section content can be accessed relative to GP
        public const uint IMAGE_SCN_MEM_FARDATA = 0x00008000;

        public const uint IMAGE_SCN_MEM_PURGEABLE = 0x00020000;
        public const uint IMAGE_SCN_MEM_16BIT = 0x00020000;
        public const uint IMAGE_SCN_MEM_LOCKED = 0x00040000;
        public const uint IMAGE_SCN_MEM_PRELOAD = 0x00080000;

        public const uint IMAGE_SCN_ALIGN_1BYTES = 0x00100000; //
        public const uint IMAGE_SCN_ALIGN_2BYTES = 0x00200000; //
        public const uint IMAGE_SCN_ALIGN_4BYTES = 0x00300000; //
        public const uint IMAGE_SCN_ALIGN_8BYTES = 0x00400000; //
        public const uint IMAGE_SCN_ALIGN_16BYTES = 0x00500000; // Default alignment if no others are specified.
        public const uint IMAGE_SCN_ALIGN_32BYTES = 0x00600000; //
        public const uint IMAGE_SCN_ALIGN_64BYTES = 0x00700000; //
        public const uint IMAGE_SCN_ALIGN_128BYTES = 0x00800000; //
        public const uint IMAGE_SCN_ALIGN_256BYTES = 0x00900000; //
        public const uint IMAGE_SCN_ALIGN_512BYTES = 0x00A00000; //
        public const uint IMAGE_SCN_ALIGN_1024BYTES = 0x00B00000; //
        public const uint IMAGE_SCN_ALIGN_2048BYTES = 0x00C00000; //
        public const uint IMAGE_SCN_ALIGN_4096BYTES = 0x00D00000; //

        public const uint IMAGE_SCN_ALIGN_8192BYTES = 0x00E00000; //

        // Unused                                    0x00F00000;
        public const uint IMAGE_SCN_ALIGN_MASK = 0x00F00000;

        public const uint IMAGE_SCN_LNK_NRELOC_OVFL = 0x01000000; // Section contains extended relocations.
        public const uint IMAGE_SCN_MEM_DISCARDABLE = 0x02000000; // Section can be discarded.
        public const uint IMAGE_SCN_MEM_NOT_CACHED = 0x04000000; // Section is not cachable.
        public const uint IMAGE_SCN_MEM_NOT_PAGED = 0x08000000; // Section is not pageable.
        public const uint IMAGE_SCN_MEM_SHARED = 0x10000000; // Section is shareable.
        public const uint IMAGE_SCN_MEM_EXECUTE = 0x20000000; // Section is executable.
        public const uint IMAGE_SCN_MEM_READ = 0x40000000; // Section is readable.
        public const uint IMAGE_SCN_MEM_WRITE = 0x80000000; // Section is writeable.

        public const uint PAGE_NOACCESS = 0x01;
        public const uint PAGE_READONLY = 0x02;
        public const uint PAGE_READWRITE = 0x04;
        public const uint PAGE_WRITECOPY = 0x08;
        public const uint PAGE_EXECUTE = 0x10;
        public const uint PAGE_EXECUTE_READ = 0x20;
        public const uint PAGE_EXECUTE_READWRITE = 0x40;
        public const uint PAGE_EXECUTE_WRITECOPY = 0x80;
        public const uint PAGE_GUARD = 0x100;
        public const uint PAGE_NOCACHE = 0x200;
        public const uint PAGE_WRITECOMBINE = 0x400;

        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RESERVE = 0x2000;
        public const uint MEM_DECOMMIT = 0x4000;
        public const uint MEM_RELEASE = 0x8000;
        public const uint MEM_FREE = 0x10000;
        public const uint MEM_PRIVATE = 0x20000;
        public const uint MEM_MAPPED = 0x40000;
        public const uint MEM_RESET = 0x80000;
        public const uint MEM_TOP_DOWN = 0x100000;
        public const uint MEM_WRITE_WATCH = 0x200000;
        public const uint MEM_PHYSICAL = 0x400000;
        public const uint MEM_ROTATE = 0x800000;
        public const uint MEM_LARGE_PAGES = 0x20000000;
        public const uint MEM_4MB_PAGES = 0x80000000;
        public const uint MEM_IMAGE = SEC_IMAGE;

        public const uint SEC_FILE = 0x800000;
        public const uint SEC_IMAGE = 0x1000000;
        public const uint SEC_PROTECTED_IMAGE = 0x2000000;
        public const uint SEC_RESERVE = 0x4000000;
        public const uint SEC_COMMIT = 0x8000000;
        public const uint SEC_NOCACHE = 0x10000000;
        public const uint SEC_WRITECOMBINE = 0x40000000;
        public const uint SEC_LARGE_PAGES = 0x80000000;

        public const int WRITE_WATCH_FLAG_RESET = 0x01;

        // Directory Entries

        public const int IMAGE_DIRECTORY_ENTRY_EXPORT = 0; // Export Directory
        public const int IMAGE_DIRECTORY_ENTRY_IMPORT = 1; // Import Directory
        public const int IMAGE_DIRECTORY_ENTRY_RESOURCE = 2; // Resource Directory
        public const int IMAGE_DIRECTORY_ENTRY_EXCEPTION = 3; // Exception Directory
        public const int IMAGE_DIRECTORY_ENTRY_SECURITY = 4; // Security Directory
        public const int IMAGE_DIRECTORY_ENTRY_BASERELOC = 5; // Base Relocation Table
        public const int IMAGE_DIRECTORY_ENTRY_DEBUG = 6; // Debug Directory
        public const int IMAGE_DIRECTORY_ENTRY_ARCHITECTURE = 7; // Architecture Specific Data
        public const int IMAGE_DIRECTORY_ENTRY_GLOBALPTR = 8; // RVA of GP
        public const int IMAGE_DIRECTORY_ENTRY_TLS = 9; // TLS Directory
        public const int IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG = 10; // Load Configuration Directory
        public const int IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT = 11; // Bound Import Directory in headers
        public const int IMAGE_DIRECTORY_ENTRY_IAT = 12; // Import Address Table
        public const int IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT = 13; // Delay Load Import Descriptors
        public const int IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14; // COM Runtime descriptor

        public const int IMAGE_REL_BASED_ABSOLUTE = 0;
        public const int IMAGE_REL_BASED_HIGH = 1;
        public const int IMAGE_REL_BASED_LOW = 2;
        public const int IMAGE_REL_BASED_HIGHLOW = 3;
        public const int IMAGE_REL_BASED_HIGHADJ = 4;
        public const int IMAGE_REL_BASED_MIPS_JMPADDR = 5;
        public const int IMAGE_REL_BASED_MIPS_JMPADDR16 = 9;
        public const int IMAGE_REL_BASED_IA64_IMM64 = 9;
        public const int IMAGE_REL_BASED_DIR64 = 10;


        public const uint DLL_PROCESS_ATTACH = 1;
        public const uint DLL_THREAD_ATTACH = 2;
        public const uint DLL_THREAD_DETACH = 3;
        public const uint DLL_PROCESS_DETACH = 0;

        #endregion

        #region Enums

        public enum ShowWindowCommand
        {
            SW_HIDE = 0,
            SW_SHOW = 5,
            SW_SHOWNA = 8,
            SW_SHOWNORMAL = 1,
            SW_SHOWMAXIMIZED = 3
        }

        public enum SetWindowPosFlags : uint
        {
            SWP_SHOWWINDOW = 0x0400,
            SWP_NOSIZE = 0x0001
        }

        #endregion

        #region kernel32.dll

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);


        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect,
            uint* lpflOldProtect);


        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, byte* lpProcName);

        #endregion

        #region user32.dll

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr handle, ShowWindowCommand command);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr handle, IntPtr handleAfter, int x, int y, int cx, int cy,
            SetWindowPosFlags flags);

        #endregion
    }
}