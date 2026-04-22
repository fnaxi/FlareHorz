// CopyRight FlareHorz Team. All Rights Reserved.


#include "CoreTypes.h"
#include "Engine.h"
#include "LaunchEngineLoop.h"

static CEngineLoop GEngineLoop;

int32 GuardedMain(const TCHAR* CmdLine)
{
	// Make sure CEngineLoop::Exit() is always called
	struct SEngineLoopCleanupGuard 
	{ 
		~SEngineLoopCleanupGuard()
		{
			GEngineLoop.Exit();
		}
	} CleanupGuard;

	int32 ExitResult = GEngineLoop.PreInit(CmdLine);
	if (ExitResult != FH_SUCCESS)
	{
		return ExitResult;
	}

	ExitResult = GEngineLoop.Init();
	if (ExitResult != FH_SUCCESS)
	{
		return ExitResult;
	}
	
	while (GEngine && !GEngine->IsRequestingExit())
	{
		GEngineLoop.Tick();
	}
	
	return FH_SUCCESS;
}

IMPLEMENT_ENTRY_POINT()

