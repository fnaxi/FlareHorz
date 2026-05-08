// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "Containers/FlareString.h"

class CEngine
{
public:
	CEngine() = default;
	virtual ~CEngine() = default;

	ENGINE_API int32 Initialize();
	ENGINE_API void Tick();

	ENGINE_API void Shutdown();
	
	void RequestExit(const TCHAR* Reason);
	FORCEINLINE bool IsRequestingExit() const { return bRequestingExit; }
	
private:
	
#if WITH_EDITOR
	class CEditor* Editor;
#endif
	
	struct SDL_Window* Window;
	struct SDL_Surface* WindowIcon;

	CString WindowTitle = FH_TEXT("FlareHorz Runtime");
	
	struct SDL_Renderer* Renderer;

	bool bRequestingExit = false;

	void Render();
};

/** Global engine pointer. Can be nullptr so don't use without checking. */
extern ENGINE_API CEngine* GEngine;
