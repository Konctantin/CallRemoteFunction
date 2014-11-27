#include <stdio.h>
#include <tchar.h>
#include <Windows.h>
#include <setjmp.h>
#include <string>

//#pragma comment(lib, "FASM")

typedef struct FasmLineHeader
{
    char * file_path;
    DWORD line_number;
    union
    {
        DWORD file_offset;
        DWORD macro_offset_line;
    };
    FasmLineHeader * macro_line;
} FASM_LINE_HEADER;

typedef struct FasmState
{
    int condition;
    union
    {
        int error_code;
        DWORD output_length;
    };
    union
    {
        BYTE * output_data;
        FasmLineHeader * error_data;
    };
} FASM_STATE;

extern "C" DWORD fasm_GetVersion();
extern "C" DWORD fasm_AssembleFile(char* szFileName);
extern "C" DWORD fasm_Assemble(char* szSource, BYTE* lpMemory, int nSize, int nPassesLimit, int hDisplayPipe);

void PrintContextFieldInfo()
{
#if !WIN32
    CONTEXT context;
    context.ContextFlags = 0xff;
    context.Rip = 0xFFEEDDCCBBAA2211;

    printf("Info of structure 'CONTEXT' for x64 mode\n");
    printf(" * Size: %i\n", sizeof(CONTEXT));
    printf(" * ContextFlag offset: +0x%X\n", (DWORD64)&context.ContextFlags - (DWORD64)&context);
    printf(" * Rip offset: +0x%X\n",         (DWORD64)&context.Rip - (DWORD64)&context);
    printf("\n\n");

    WOW64_CONTEXT context32;
    context32.ContextFlags = 0xff;
    context32.Eip = 0xDEADBEEF;

    printf("Info of structure 'CONTEXT' for x32 mode\n");
    printf(" * Size: %i\n", sizeof(WOW64_CONTEXT));
    printf(" * ContextFlag offset: +0x%X\n", (DWORD64)&context32.ContextFlags - (DWORD64)&context32);
    printf(" * Eip offset: +0x%X\n",         (DWORD64)&context32.Eip - (DWORD64)&context32);
    printf("\n\n");
#endif
}

int _tmain(int argc, _TCHAR* argv[])
{
    printf("ver: %X\n", fasm_GetVersion());

    BYTE buf[0x1000];
    char* src = "use32\n pop eax\n mov ecx, eax";
    fasm_Assemble(src, buf, 0x1000, 0x100, 0);

    FasmState* state = reinterpret_cast<FasmState*>(buf);
    printf("Size: %i\n", state->output_length);
    for (int i = 0; i < state->output_length; ++i)
    {
        printf("0x%2X ", state->output_data[i]);
    }
    printf("\n");


    PrintContextFieldInfo();

    unsigned char remoteCallEntryBase[256] =
    {
        0x40, 0x57,                                                 /*push rdi*/
        0x48, 0x83, 0xEC, 0x40,                                     /*sub rsp, 0x40*/
        0x48, 0x8B, 0xFC,                                           /*mov rdi, rsp*/
        0x50,                                                       /*push rax*/
        0x51,                                                       /*push rcx*/
        0x52,                                                       /*push rdx*/
        0x41, 0x50,                                                 /*push r8*/
        0x41, 0x51,                                                 /*push r9*/
    };
    unsigned char remoteCallArgBase1stArg[] =
    {
        0x48, 0xB9, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, /*mov rcx, 0xAAAAAAAAAAAAAAAA*/
    };

    printf("remoteCallEntryBase: %i\n\n", sizeof(remoteCallEntryBase));
    printf("remoteCallArgBase1stArg: %i\n\n", sizeof(remoteCallArgBase1stArg));

    system("pause");
    return 0;
}