// CopyRight FlareHorz Team. All Rights Reserved.


#include "Engine.h"

#include "Editor.h"
#include "Module/ModuleManager.h"

#include "SDL3/SDL_init.h"
#include "SDL3/SDL_render.h"

#include "ViewportEditorWindow.h"

ENGINE_API CEngine* GEngine = nullptr;

DEFINE_LOG_CATEGORY_STATIC(L_Engine, Log)
IMPLEMENT_MODULE(CDefaultModuleImpl, Engine)

int32 CEngine::Initialize()
{
	// TODO: Renderer/RHI/D3D12 modules
	
	if (!SDL_InitSubSystem(SDL_INIT_VIDEO))
	{
		FH_LOG(L_Engine, Error, FH_TEXT("Couldn't initialize SDL video subsystem! %s"), SDL_GetError())
		return FH_FAILURE;
	}
	FH_LOG(L_Engine, Log, FH_TEXT("Initialized SDL video subsystem"))

	constexpr SDL_WindowFlags WindowFlags = SDL_WINDOW_RESIZABLE | SDL_WINDOW_HIDDEN | SDL_WINDOW_HIGH_PIXEL_DENSITY;
	
	Window = SDL_CreateWindow(*WindowTitle, 1280, 720, WindowFlags);
	if (!Window)
	{
		FH_LOG(L_Engine, Error, FH_TEXT("Failed to create window! %s"), SDL_GetError())
		return FH_FAILURE;
	}
	else
	{
		SDL_SetWindowPosition(Window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
	}

	WindowIcon = SDL_LoadSurface("../../../Images/Logo.png"); // TODO: Paths
	if (!WindowIcon)
	{
		FH_LOG(L_Engine, Error, FH_TEXT("Failed to load window icon! %s"), SDL_GetError())
		return FH_FAILURE;
	}
	else
	{
		SDL_SetWindowIcon(Window, WindowIcon);
	}
	
	Renderer = SDL_CreateRenderer(Window, nullptr);
	if (!Renderer)
	{
		FH_LOG(L_Engine, Error, FH_TEXT("Failed to create SDL renderer! %s"), SDL_GetError())
		return FH_FAILURE;
	}
	else
	{
		SDL_SetRenderVSync(Renderer, 1);
	}
	
	SDL_ShowWindow(Window);
	
#if WITH_EDITOR
	Editor = new CEditor();
	if (Editor)
	{
		if (const int32 ExitCode = Editor->Initialize(Window, Renderer); ExitCode != FH_SUCCESS)
		{
			return ExitCode;
		}

		GViewportTexture = SDL_CreateTexture(Renderer, SDL_PIXELFORMAT_RGBA8888, SDL_TEXTUREACCESS_TARGET, GViewportSizeX, GViewportSizeY);
		if (!GViewportTexture)
		{
			FH_LOG(L_Engine, Error, FH_TEXT("Can't create GViewportTexture!"))
			return FH_FAILURE;
		}
	}
	else // TODO: replace with assertion
	{
		FH_LOG(L_Engine, Error, FH_TEXT("Can't create Editor instance!"))
	}
#endif
	
	return FH_SUCCESS;
}

void CEngine::Tick()
{
	SDL_Event Event;
	while (SDL_PollEvent(&Event))
	{
#if WITH_EDITOR
		if (Editor) Editor->ProcessEvent(&Event);
#endif
		if (Event.type == SDL_EVENT_WINDOW_CLOSE_REQUESTED && Event.window.windowID == SDL_GetWindowID(Window))
		{
			RequestExit(FH_TEXT("Window was closed"));
		}
	}

	if (!Renderer) return; // TODO: replace with assertion

#if WITH_EDITOR
	if (Editor) Editor->PrepareImGui(Renderer);
#endif

	// TODO: verify GViewportTexture
	
#if WITH_EDITOR
	SDL_SetRenderTarget(Renderer, GViewportTexture);
#endif
	SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 255);
	SDL_RenderClear(Renderer);

	Render();
	
	SDL_RenderPresent(Renderer);
	
#if WITH_EDITOR
	SDL_SetRenderTarget(Renderer, nullptr);
	SDL_SetRenderDrawColor(Renderer, 170, 219, 30, 255);
	SDL_RenderClear(Renderer);
	if (Editor) Editor->RenderImGui(Renderer);
	
	SDL_RenderPresent(Renderer);
#endif
}

void CEngine::Shutdown()
{
#if WITH_EDITOR
	if (Editor)
	{
		Editor->Shutdown();
	}
	delete Editor;
#endif
	
	SDL_DestroyRenderer(Renderer);
	SDL_DestroyWindow(Window);
	SDL_DestroySurface(WindowIcon);
	SDL_Quit();
}

void CEngine::RequestExit(const TCHAR* Reason)
{
	FH_LOG(L_Engine, Log, FH_TEXT("Engine exit requested (reason: %s%s)"), Reason, bRequestingExit ? FH_TEXT("; note: exit was already requested") : FH_TEXT(""))
	bRequestingExit = true;
}

void CEngine::Render()
{
	SDL_FRect Square = { 100.0f, 100.0f, 100.0f, 100.0f };
	{
		SDL_SetRenderDrawColor(Renderer, 255, 0, 0, 255);
		SDL_RenderFillRect(Renderer, &Square);
	}
}

