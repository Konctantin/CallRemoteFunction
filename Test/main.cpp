#include <stdio.h>
#include <tchar.h>
#include <Windows.h>

int _tmain(int argc, _TCHAR* argv[])
{
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

