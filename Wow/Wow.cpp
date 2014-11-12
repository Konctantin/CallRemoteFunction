#include <stdio.h>
#include <tchar.h>
#include <Windows.h>

char locale[] = { 'r', 'u', 'R', 'U' };

typedef struct {
    void* vTable;
    BYTE* buffer;
    DWORD base;
    DWORD alloc;
    DWORD size;
    DWORD read;
} CDataStore;

#define NOINLINE_C extern "C" __declspec(noinline)

DWORD MainLoop();

NOINLINE_C DWORD Test();
NOINLINE_C DWORD Test2();
NOINLINE_C DWORD NetClient__Send(CDataStore* packet);
NOINLINE_C DWORD FrameScript_ExequteBuffer(char* src, char* path, DWORD state);
NOINLINE_C DWORD FrameScript_ExequteBuffer2(char* src, char* path, DWORD state, DWORD lol1, DWORD lol2, DWORD lol3, DWORD64 lol4);
NOINLINE_C DWORD __fastcall NetClient_Send2(void* thisPTR, void* dummy, CDataStore* ds, DWORD connectionId);
NOINLINE_C DWORD __fastcall NetClient_ProcessMessage(void* thisPTR, void* dummy, void* param1, void* param2, CDataStore* ds, void* param4);

int _tmain(int argc, _TCHAR* argv[])
{
    printf("CONTEXT = %i\n\n", sizeof(CONTEXT));

    CONTEXT context;
    context.ContextFlags = 0xAABBCC;
    context.Rip = 0xDDFF110011;

    void* ptr = &context;
    void* cf  = &context.ContextFlags;
    void* rip = &context.Rip;

    printf("context: 0x%X, ContextFlags: 0x%X, Rip: 0x%X\n\n\n", ptr, cf, rip);
    printf("context: 0x%X, ContextFlags: +0x%X, Rip: +0x%X\n\n\n", ptr, (DWORD64)cf - (DWORD64)ptr, (DWORD64)rip - (DWORD64)ptr);
    FlushInstructionCache(0, 0, 0);

    CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)&MainLoop, NULL, 0, NULL);

    Test();

    FrameScript_ExequteBuffer("print('test');", "lua.lua", 1);

    FrameScript_ExequteBuffer2("src", "path", 0, 1, 2, 3, 4);

    while (true);

    return 0;
}

DWORD MainLoop()
{
    //printf("\nthread: %i\n", *(DWORD*)param);
    CDataStore ds;
    CDataStore ds2;

    GetThreadContext(0, 0);

    int i = 0;
    while (1)
    {
        printf("%i\n", ++i);

        int s = i > 10000 ? 1 : 0;

        if (i > 10000000)
        {
            CONTEXT ctx;
            ctx.ContextFlags = 0x100001;
            ctx.Rip = 0xFFAADD;
            Test2();
            locale[0] = 'x';
            FrameScript_ExequteBuffer("print('test');", "lua.lua", i);

            NetClient__Send(nullptr);
            NetClient_Send2((void*)&ctx, (void*)56, &ds, s);

            FrameScript_ExequteBuffer2("src", "path", 0, 1, 2, 3, 4);
        }

        ds.size = 44 + i;
        NetClient_Send2((void*)5, (void*)56, &ds, s);

        ds2.size = 155 + i;
        NetClient_ProcessMessage((void*)12, (void*)66, (void*)100, (void*)200, &ds2, (void*)s);

        for (int j = 0; j < 200; ++j)
            Sleep(10);
    }
}

NOINLINE_C DWORD Test()
{
    FrameScript_ExequteBuffer("print('test');", "lua.lua", 0);
    return 0;
}

NOINLINE_C DWORD Test2()
{
    FrameScript_ExequteBuffer2("src", "path", 0, 1, 0xddaaffee, 3, 0xddaaffeeccccdd);
    return 0;
}

NOINLINE_C DWORD NetClient__Send(CDataStore* packet)
{
    if (packet != nullptr)
    {
        WCHAR ch[0x1000] = { 0 };
        auto code = GetLastError();
        FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, NULL, code, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), ch, 0x1000, NULL);
        printf("Error Code: %d\nError Name: %s", code, ch);

        printf("DS ->: ", packet->size);
        packet->size = 5000;
        printf("DS <-: ", packet->size);
    }
    return 0;
}

NOINLINE_C DWORD FrameScript_ExequteBuffer(char* src, char* path, DWORD state)
{
    printf("\ncall FrameScript_ExequteBuffer\n");
    printf("Patn: %s\n", path);
    printf("Src: %s\n", src);
    printf("State: %i\n\n", state);
    return 0;
}

NOINLINE_C DWORD FrameScript_ExequteBuffer2(char* src, char* path, DWORD state, DWORD lol1, DWORD lol2, DWORD lol3, DWORD64 lol4)
{
    printf("\ncall FrameScript_ExequteBuffer2\n");
    printf("Patn: %s\n", path);
    printf("Src: %s\n", src);
    printf("State: %i\n", state);

    printf("lol1: %i\n", lol1);
    printf("lol2: %i\n", lol2);
    printf("lol3: %i\n", lol3);
    printf("lol4: %i\n\n", lol4);

    return 0;
}

NOINLINE_C DWORD __fastcall NetClient_Send2(void* thisPTR, void* dummy, CDataStore* ds, DWORD connectionId)
{
    if (connectionId)
        printf("CMSG Size: %i\n", ds->size);
    return 0;
}

NOINLINE_C DWORD __fastcall NetClient_ProcessMessage(void* thisPTR, void* dummy, void* param1, void* param2, CDataStore* ds, void* param4)
{
    if ((DWORD)param4)
        printf("SMSG Size: %i Locale: %s\n", ds->size, locale);
    return 0;
}