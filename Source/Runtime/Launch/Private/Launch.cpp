// CopyRight FlareHorz Team. All Rights Reserved.


#include "Launch.h"

#include "CoreMinimal.h"
#include "Module/ModuleManager.h"

int32 main(int32 ArgC, ANSICHAR** ArgV)
{
	CModuleManager::Get().LoadModule(TEXT("Core"));

	CModuleManager::Get().UnloadModulesAtShutdown();
	
	return 0;
}

