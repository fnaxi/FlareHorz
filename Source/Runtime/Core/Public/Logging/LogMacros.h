// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include <cstdio>

// TODO: Logging
#define FH_LOG(...) \
	{ \
		wprintf(__VA_ARGS__); \
		wprintf(TEXT("\n")); \
	}
