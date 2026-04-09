// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

/**
 * Interface class that all module implementations should derive from. This is used to initialize
 * a module after it's been loaded, and also to clean it up before the module is unloaded.
 */
class IModuleInterface
{
public:
	/**
	 * Note: Even though this is an interface class we need a virtual destructor here because modules are deleted via a pointer to this interface.              
	 */
	virtual ~IModuleInterface()
	{
	}

	/**
	 * Called right after the module DLL has been loaded and the module object has been created.
	 * Load dependent modules here, and they will be guaranteed to be available during ShutdownModule. ie:
	 *
	 * CModuleManager::Get().LoadModuleChecked(TEXT("SomeModule"));
	 */
	virtual void StartupModule()
	{
	}

	/**
	 * Called before the module is unloaded, right before the module object is destroyed.
	 * During normal shutdown, this is called in reverse order that modules finish StartupModule().
	 * This means that, as long as a module references dependent modules in it's StartupModule(), it
	 * can safely reference those dependencies in ShutdownModule() as well.
	 */
	virtual void ShutdownModule()
	{
	}
};
