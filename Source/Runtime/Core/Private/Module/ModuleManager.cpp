// CopyRight FlareHorz Team. All Rights Reserved.


#include <algorithm>

#include "Module/ModuleManager.h"

int32 CModuleManager::CModuleInfo::CurrentLoadOrder = 1;

CModuleManager& CModuleManager::Get()
{
	static CModuleManager Singleton = CModuleManager(SPrivateToken{});
	return Singleton;
}

IModuleInterface* CModuleManager::LoadModule(const CString& InModuleName)
{
	TSharedPtr<CModuleInfo> ModuleInfo = FindModule(InModuleName);
	if (!ModuleInfo)
	{
		FH_LOG(TEXT("Module %s is not registered!"), *InModuleName)
		return nullptr;
	}

	if (ModuleInfo->bWasUnloadedAtShutdown)
	{
		FH_LOG(TEXT("Module %s was unloaded at shutdown!"), *InModuleName)
		return nullptr;
	}

	IModuleInterface* Interface = ModuleInfo->Interface.PTR_Get();
	if (!Interface) return nullptr;
	
	if (ModuleInfo->bIsReady)
	{
		return ModuleInfo->Interface.PTR_Get();
	}
	else
	{
		FH_LOG(TEXT("Loaded %s module"), *InModuleName)
		
		Interface->StartupModule();
		ModuleInfo->bIsReady = true;
		
		return Interface;
	}
}

IModuleInterface* CModuleManager::LoadModuleChecked(const CString& InModuleName)
{
	// TODO: LoadModuleChecked()
	return nullptr;
}

IModuleInterface* CModuleManager::GetModule(const CString& InModuleName)
{
	TSharedPtr<CModuleInfo> ModuleInfo = FindModule(InModuleName);
	if (!ModuleInfo || !ModuleInfo->bIsReady)
	{
		FH_LOG(TEXT("Module %s is not loaded!"), *InModuleName)
		return nullptr;
	}
	
	return ModuleInfo->Interface.PTR_Get();
}

void CModuleManager::UnloadModulesAtShutdown()
{
	int32 MaxLoadOrder = 0;
	for (const TPair<CString, TSharedPtr<CModuleInfo>>& Module : Modules)
	{
		MaxLoadOrder = CMath::Max(Module.Value->LoadOrder, MaxLoadOrder);
	}

	TArray<CString> ModuleNames;
	for (TPair<CString, TSharedPtr<CModuleInfo>>& Module : Modules)
	{
		ModuleNames.Add(Module.Key);
	}
	
	for (int32 Order = MaxLoadOrder; Order >= 0; --Order) // TODO: TMap::CreateIterator() or TMap::Sort()
	{
		for (int32 i = 0; i < ModuleNames.Num(); ++i)
		{
			const CString& Name = ModuleNames[i];
			if (Modules[Name]->LoadOrder == Order)
			{
				UnloadModule(Name);
				if (TSharedPtr<CModuleInfo> ModuleInfo = FindModule(Name))
				{
					ModuleInfo->bWasUnloadedAtShutdown = true;
				}
			}
		}
	}
}

void CModuleManager::UnloadModule(const CString& InModuleName)
{
	TSharedPtr<CModuleInfo> ModuleInfo = FindModule(InModuleName);
	if (!ModuleInfo || !ModuleInfo->bIsReady)
	{
		FH_LOG(TEXT("Module %s is already unloaded!"), *InModuleName)
		return;
	}

	ModuleInfo->bIsReady = false;
	ModuleInfo->Interface->ShutdownModule();
	ModuleInfo.PTR_Reset();

	FH_LOG(TEXT("Unloaded %s module"), *InModuleName)
}

bool CModuleManager::IsModuleLoaded(const CString& InModuleName) const
{
	return Modules.Contains(InModuleName) && FindModule(InModuleName)->bIsReady;
}

int32 CModuleManager::GetModuleCount() const
{
	return Modules.Num();
}

void CModuleManager::AddModule_Internal(const TCHAR* InModuleName, const UModuleInitializer& ModuleInitializer)
{
	if (Modules.Contains(InModuleName)) return;

	IModuleInterface* ModuleInterface = ModuleInitializer();
	if (ModuleInterface)
	{
		Modules.Add( InModuleName, MakeShared<CModuleInfo>(ModuleInterface) );
	}
	else
	{
		FH_LOG(TEXT("The module %s was not constructed properly!"), InModuleName)
	}
}

TSharedPtr<CModuleManager::CModuleInfo> CModuleManager::FindModule(const CString& InModuleName) const
{
	if (const TSharedPtr<CModuleInfo>* FoundModule = Modules.Find(InModuleName))
	{
		return *FoundModule;
	}
	return nullptr;
}

