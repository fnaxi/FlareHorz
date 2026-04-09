// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

// Turns a preprocessor token into a real string
#define FH_STRINGIZE(Token) FH_STRINGIZE_PRIVATE(Token)
#define FH_STRINGIZE_PRIVATE(Token) #Token

// Concatenates two preprocessor tokens, performing macro expansion on them first
#define FH_JOIN(TokenA, TokenB) FH_JOIN_PRIVATE(TokenA, TokenB)
#define FH_JOIN_PRIVATE(TokenA, TokenB) TokenA##TokenB

#define FH_SOURCE_LOCATION TEXT(__FILE__ "(" FH_STRINGIZE(__LINE__) ")")

// Creates a string that can be used to include a header in the form like "Windows/WindowsPlatformFile.h".
#define FH_PLATFORM_HEADER(Suffix) FH_STRINGIZE(FH_JOIN(FH_PLATFORM_NAME/FH_PLATFORM_NAME, Suffix))
