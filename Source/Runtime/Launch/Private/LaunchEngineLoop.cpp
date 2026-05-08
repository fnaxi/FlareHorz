// CopyRight FlareHorz Team. All Rights Reserved.


#include "LaunchEngineLoop.h"

#include "Engine.h"
#include "Module/ModuleManager.h"

DEFINE_LOG_CATEGORY_STATIC(L_EngineLoop, Log)

#define FH_LOAD_REQUIRED_MODULE(ModuleName) \
	{ \
		if (!CModuleManager::Get().LoadModule(FH_TEXT( ModuleName ))) \
		{ \
			FH_LOG(L_EngineLoop, Error, FH_TEXT("Failed to load required module " ModuleName "!")) \
			return FH_FAILURE; \
		} \
	}

int32 CEngineLoop::PreInit(const TCHAR* CmdLine)
{
	// TODO: use Startup() and Shutdown() in modules
	/*FH_LOAD_REQUIRED_MODULE("Core")
	FH_LOAD_REQUIRED_MODULE("Editor")
	FH_LOAD_REQUIRED_MODULE("Engine")*/

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
		FH_LOG(L_EngineLoop, Error, FH_TEXT("Failed to create global engine instance!"))
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

