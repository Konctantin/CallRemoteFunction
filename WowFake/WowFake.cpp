#include <stdio.h>
#include <tchar.h>
#include <Windows.h>

#define NOINLINE_C extern "C" __declspec(noinline)

NOINLINE_C DWORD LoopBody()
{
	int i = 5; // << inject jump
	int b = 9;
	printf(".");
	return i + b;
}

NOINLINE_C DWORD FrameScript_ExequteBuffer(char* src, char* path, DWORD state)
{
    printf("\ncall FrameScript_ExequteBuffer\n");
    printf("Patn: %s\n", path);
    printf("Src: %s\n", src);
    printf("State: %i\n\n", state);
    return 0;
}

NOINLINE_C DWORD Test()
{
	FrameScript_ExequteBuffer("print('test');", "lua.lua", 0);
	return 0;
}

NOINLINE_C DWORD Body()
{
	int arr[100] = { 0 }; // << inject executor

	arr[0] = 1;
	arr[1] = 2;
	arr[2] = 3;
	arr[3] = 4;
	arr[4] = 5;
	arr[5] = 6;
	arr[6] = 7;
	arr[7] = 8;
	arr[8] = 9;
	arr[9] = 10;
	arr[10] = 11;
	arr[11] = 12;
	arr[12] = 13;
	arr[13] = 14;
	arr[14] = 15;
	arr[15] = 16;
	arr[16] = 17;
	arr[17] = 18;
	arr[18] = 19;
	arr[19] = 20;
	arr[20] = 21;
	arr[21] = 22;
	arr[22] = 23;
	arr[23] = 24;
	arr[24] = 25;
	arr[25] = 26;
	arr[26] = 27;
	arr[27] = 28;
	arr[28] = 29;
	arr[29] = 30;
	arr[30] = 31;
	arr[31] = 32;
	arr[32] = 33;
	arr[33] = 34;
	arr[34] = 35;
	arr[35] = 36;
	arr[36] = 37;
	arr[37] = 38;
	arr[38] = 39;
	arr[39] = 40;
	arr[40] = 41;
	arr[41] = 42;
	arr[42] = 43;
	arr[43] = 44;
	arr[44] = 45;
	arr[45] = 46;
	arr[46] = 47;
	arr[47] = 48;
	arr[48] = 49;
	arr[49] = 50;
	arr[50] = 51;
	arr[51] = 52;
	arr[52] = 53;
	arr[53] = 54;
	arr[54] = 55;
	arr[55] = 56;
	arr[56] = 57;
	arr[57] = 58;
	arr[58] = 59;
	arr[59] = 60;
	arr[60] = 61;
	arr[61] = 62;
	arr[62] = 63;
	arr[63] = 64;
	arr[64] = 65;
	arr[65] = 66;
	arr[66] = 67;
	arr[67] = 68;
	arr[68] = 69;
	arr[69] = 70;
	arr[70] = 71;
	arr[71] = 72;
	arr[72] = 73;
	arr[73] = 74;
	arr[74] = 75;
	arr[75] = 76;
	arr[76] = 77;
	arr[77] = 78;
	arr[78] = 79;
	arr[79] = 80;
	arr[80] = 81;
	arr[81] = 82;
	arr[82] = 83;
	arr[83] = 84;
	arr[84] = 85;
	arr[85] = 86;
	arr[86] = 87;
	arr[87] = 88;
	arr[88] = 89;
	arr[89] = 90;
	arr[90] = 91;
	arr[91] = 92;
	arr[92] = 93;
	arr[93] = 94;
	arr[94] = 95;
	arr[95] = 96;
	arr[96] = 97;
	arr[97] = 98;
	arr[98] = 99;
	arr[99] = 100;

	printf("\narr[100] = { 0 }\n");

	return arr[98];
}

int _tmain(int argc, _TCHAR* argv[])
{
	Body();
	Test();

	while (true)
	{
		LoopBody();
		Sleep(100);
	}
	return 0;
}