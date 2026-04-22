// CopyRight FlareHorz Team. All Rights Reserved.


#include "Engine.h"

#include "Module/ModuleManager.h"
#include "SDL3/SDL_init.h"
#include "SDL3/SDL_render.h"

ENGINE_API CEngine* GEngine = nullptr;

DEFINE_LOG_CATEGORY_STATIC(LogEngine, Log)
IMPLEMENT_MODULE(CDefaultModuleImpl, Engine)

int32 CEngine::Initialize()
{
	// TODO: Renderer/RHI/D3D12 modules
	
	if (!SDL_Init(SDL_INIT_VIDEO))
	{
		FH_LOG(LogEngine, Error, FH_TEXT("SDL couldn't initialize! %s"), SDL_GetError())

		Shutdown();
		return FH_FAILURE;
	}
	FH_LOG(LogEngine, Log, FH_TEXT("Initialized SDL video subsystem"))
	
	Window = SDL_CreateWindow(*WindowTitle, 1280, 720, 0);
	if (!Window)
	{
		FH_LOG(LogEngine, Error, FH_TEXT("Failed to create window! %s"), SDL_GetError())

		Shutdown();
		return FH_FAILURE;
	}

	WindowIcon = SDL_LoadSurface("../../../Images/Logo.png"); // TODO: Paths
	if (!WindowIcon)
	{
		FH_LOG(LogEngine, Error, FH_TEXT("Failed to load window icon! %s"), SDL_GetError())

		Shutdown();
		return FH_FAILURE;
	}
	else
	{
		SDL_SetWindowIcon(Window, WindowIcon);
	}

	Renderer = SDL_CreateRenderer(Window, NULL);
	if (!Renderer)
	{
		FH_LOG(LogEngine, Error, FH_TEXT("Failed to create SDL renderer! %s"), SDL_GetError())

		Shutdown();
		return FH_FAILURE;
	}
	
	return FH_SUCCESS;
}

void CEngine::Tick()
{
	SDL_Event Event;
	while (SDL_PollEvent(&Event))
	{
		switch (Event.type)
		{
		// TODO: handling SDL_EVENT_QUIT causes RequestExit() to be called twice
		/*case SDL_EVENT_QUIT:
			RequestExit(FH_TEXT("SDL requested to exit"));
			break;*/
		case SDL_EVENT_WINDOW_CLOSE_REQUESTED:
			RequestExit(FH_TEXT("Window was closed"));
			break;
		default: break;
		}
	}

	if (Renderer) // TODO: replace with assertion
	{
		SDL_RenderClear(Renderer);
		{
			Render();
		}
		SDL_RenderPresent(Renderer);
	}
}

void CEngine::Shutdown()
{
	SDL_DestroyRenderer(Renderer);
	SDL_DestroyWindow(Window);
	SDL_DestroySurface(WindowIcon);
	SDL_Quit();
}

void CEngine::RequestExit(const TCHAR* Reason)
{
	FH_LOG(LogEngine, Log, FH_TEXT("Engine exit requested (reason: %s%s)"), Reason, bRequestingExit ? FH_TEXT("; note: exit was already requested") : FH_TEXT(""))
	bRequestingExit = true;
}

void CEngine::Render()
{
	SDL_SetRenderDrawColor(Renderer, 170, 219, 30, 255);
}
