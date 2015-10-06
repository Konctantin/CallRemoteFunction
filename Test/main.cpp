#include <stdio.h>
#include <tchar.h>
#include <Windows.h>
#include <setjmp.h>
#include <string>

//#include <ntstatus.h>
//#include <ntdef.h>

//#pragma comment(lib, "FASM")

#define OFFSET(f) printf(" * "#f" offset: +0x%X\n", (DWORD64)&(context.#f) - (DWORD64)&context)

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

//extern "C" DWORD fasm_GetVersion();
//extern "C" DWORD fasm_AssembleFile(char* szFileName);
//extern "C" DWORD fasm_Assemble(char* szSource, BYTE* lpMemory, int nSize, int nPassesLimit, int hDisplayPipe);

void PrintContextFieldInfo()
{
    CONTEXT context;
    context.ContextFlags = 0xff;
    context.Rip = 0xFFEEDDCCBBAA2211;

    printf("Info of structure 'CONTEXT' for x64 mode\n");
    printf(" * Size: %i\n", sizeof(CONTEXT));

    BOOLEAN b;

    //RTL_PAGED_CODE();

    /*
    Info of structure 'CONTEXT' for x64 mode
     * Size: 1232
     * P1Home offset: +0x0
     * P2Home offset: +0x8
     * P3Home offset: +0x10
     * P4Home offset: +0x18
     * P5Home offset: +0x20
     * P6Home offset: +0x28
     * ContextFlags offset: +0x30
     * MxCsr offset: +0x34
     * SegCs offset: +0x38
     * SegDs offset: +0x3A
     * SegEs offset: +0x3C
     * SegFs offset: +0x3E
     * SegGs offset: +0x40
     * SegSs offset: +0x42
     * EFlags offset: +0x44
     * Dr0 offset: +0x48
     * Dr1 offset: +0x50
     * Dr2 offset: +0x58
     * Dr3 offset: +0x60
     * Dr6 offset: +0x68
     * Dr7 offset: +0x70
     * Rax offset: +0x78
     * Rcx offset: +0x80
     * Rdx offset: +0x88
     * Rbx offset: +0x90
     * Rsp offset: +0x98
     * Rbp offset: +0xA0
     * Rsi offset: +0xA8
     * Rdi offset: +0xB0
     * R8 offset: +0xB8
     * R9 offset: +0xC0
     * R10 offset: +0xC8
     * R11 offset: +0xD0
     * R12 offset: +0xD8
     * R13 offset: +0xE0
     * R14 offset: +0xE8
     * R15 offset: +0xF0
    */
    offsetof(CONTEXT, P1Home);
    printf(" * P1Home offset: +0x%X\n", (DWORD64)&(context.P1Home) - (DWORD64)&context);
    printf(" * P2Home offset: +0x%X\n", (DWORD64)&(context.P2Home) - (DWORD64)&context);
    printf(" * P3Home offset: +0x%X\n", (DWORD64)&(context.P3Home) - (DWORD64)&context);
    printf(" * P4Home offset: +0x%X\n", (DWORD64)&(context.P4Home) - (DWORD64)&context);
    printf(" * P5Home offset: +0x%X\n", (DWORD64)&(context.P5Home) - (DWORD64)&context);
    printf(" * P6Home offset: +0x%X\n", (DWORD64)&(context.P6Home) - (DWORD64)&context);
    printf(" * ContextFlags offset: +0x%X\n", (DWORD64)&(context.ContextFlags) - (DWORD64)&context);
    printf(" * MxCsr offset: +0x%X\n", (DWORD64)&(context.MxCsr) - (DWORD64)&context);
    printf(" * SegCs offset: +0x%X\n", (DWORD64)&(context.SegCs) - (DWORD64)&context);
    printf(" * SegDs offset: +0x%X\n", (DWORD64)&(context.SegDs) - (DWORD64)&context);
    printf(" * SegEs offset: +0x%X\n", (DWORD64)&(context.SegEs) - (DWORD64)&context);
    printf(" * SegFs offset: +0x%X\n", (DWORD64)&(context.SegFs) - (DWORD64)&context);
    printf(" * SegGs offset: +0x%X\n", (DWORD64)&(context.SegGs) - (DWORD64)&context);
    printf(" * SegSs offset: +0x%X\n", (DWORD64)&(context.SegSs) - (DWORD64)&context);
    printf(" * EFlags offset: +0x%X\n", (DWORD64)&(context.EFlags) - (DWORD64)&context);
    printf(" * Dr0 offset: +0x%X\n", (DWORD64)&(context.Dr0) - (DWORD64)&context);
    printf(" * Dr1 offset: +0x%X\n", (DWORD64)&(context.Dr1) - (DWORD64)&context);
    printf(" * Dr2 offset: +0x%X\n", (DWORD64)&(context.Dr2) - (DWORD64)&context);
    printf(" * Dr3 offset: +0x%X\n", (DWORD64)&(context.Dr3) - (DWORD64)&context);
    printf(" * Dr6 offset: +0x%X\n", (DWORD64)&(context.Dr6) - (DWORD64)&context);
    printf(" * Dr7 offset: +0x%X\n", (DWORD64)&(context.Dr7) - (DWORD64)&context);
    printf(" * Rax offset: +0x%X\n", (DWORD64)&(context.Rax) - (DWORD64)&context);
    printf(" * Rcx offset: +0x%X\n", (DWORD64)&(context.Rcx) - (DWORD64)&context);
    printf(" * Rdx offset: +0x%X\n", (DWORD64)&(context.Rdx) - (DWORD64)&context);
    printf(" * Rbx offset: +0x%X\n", (DWORD64)&(context.Rbx) - (DWORD64)&context);
    printf(" * Rsp offset: +0x%X\n", (DWORD64)&(context.Rsp) - (DWORD64)&context);
    printf(" * Rbp offset: +0x%X\n", (DWORD64)&(context.Rbp) - (DWORD64)&context);
    printf(" * Rsi offset: +0x%X\n", (DWORD64)&(context.Rsi) - (DWORD64)&context);
    printf(" * Rdi offset: +0x%X\n", (DWORD64)&(context.Rdi) - (DWORD64)&context);
    printf(" * R8  offset: +0x%X\n", (DWORD64)&(context.R8)  - (DWORD64)&context);
    printf(" * R9  offset: +0x%X\n", (DWORD64)&(context.R9)  - (DWORD64)&context);
    printf(" * R10 offset: +0x%X\n", (DWORD64)&(context.R10) - (DWORD64)&context);
    printf(" * R11 offset: +0x%X\n", (DWORD64)&(context.R11) - (DWORD64)&context);
    printf(" * R12 offset: +0x%X\n", (DWORD64)&(context.R12) - (DWORD64)&context);
    printf(" * R13 offset: +0x%X\n", (DWORD64)&(context.R13) - (DWORD64)&context);
    printf(" * R14 offset: +0x%X\n", (DWORD64)&(context.R14) - (DWORD64)&context);
    printf(" * R15 offset: +0x%X\n", (DWORD64)&(context.R15) - (DWORD64)&context);
    printf(" * R15 offset: +0x%X\n", (DWORD64)&(context.Rip) - (DWORD64)&context);

    printf("\n\n");

    printf("\n\n");
}

int _tmain(int argc, _TCHAR* argv[])
{
    //printf("ver: %X\n", fasm_GetVersion());

    //BYTE buf[0x1000];
    //char* src = "use32\n pop eax\n mov ecx, eax";
    //fasm_Assemble(src, buf, 0x1000, 0x100, 0);

    //FasmState* state = reinterpret_cast<FasmState*>(buf);
    //printf("Size: %i\n", state->output_length);
    //for (int i = 0; i < state->output_length; ++i)
    //{
    //    printf("0x%2X ", state->output_data[i]);
    //}
    //printf("\n");

    printf("sizeof(M128A) = %i\n", sizeof(M128A));
    printf("sizeof(XMM_SAVE_AREA32) = %i\n", sizeof(XMM_SAVE_AREA32));
    //printf("CONTEXT_ALL = 0x%X\n", CONTEXT_ALL, EFLAGS_);


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