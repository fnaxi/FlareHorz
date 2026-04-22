// CopyRight FlareHorz Team. All Rights Reserved.


#include "LaunchEngineLoop.h"

#include "Engine.h"
#include "Module/ModuleManager.h"

DEFINE_LOG_CATEGORY_STATIC(LogEngineLoop, Log)

#define FH_LOAD_REQUIRED_MODULE(ModuleName) \
	{ \
		if (!CModuleManager::Get().LoadModule(FH_TEXT( ModuleName ))) \
		{ \
			FH_LOG(LogEngineLoop, Error, FH_TEXT("Failed to load required module " ModuleName "!")) \
			return FH_FAILURE; \
		} \
	}

int32 CEngineLoop::PreInit(const TCHAR* CmdLine)
{
	FH_LOAD_REQUIRED_MODULE("Core")
	FH_LOAD_REQUIRED_MODULE("Engine")

	return FH_SUCCESS;
}

int32 CEngineLoop::Init()
{
	GEngine = new CEngine();
	if (GEngine)
	{
		GEngine->Initialize();
	}
	else
	{
		FH_LOG(LogEngineLoop, Error, FH_TEXT("Failed to create global engine instance!"))

		Exit();
		return FH_FAILURE;
	}
	
	return FH_SUCCESS;
}

void CEngineLoop::Tick()
{
	GEngine->Tick();
}

void CEngineLoop::Exit()
{
	CModuleManager::Get().UnloadModulesAtShutdown();

	if (GEngine)
	{
		GEngine->Shutdown();
	}
	delete GEngine;
}

