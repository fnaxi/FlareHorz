// CopyRight FlareHorz Team. All Rights Reserved.


#include "Engine.h"

#include "Module/ModuleManager.h"

#include "SDL3/SDL_init.h"
#include "SDL3/SDL_render.h"

#include "imgui.h"
#include "imgui_impl_sdl3.h"
#include "imgui_impl_sdlrenderer3.h"

ENGINE_API CEngine* GEngine = nullptr;

DEFINE_LOG_CATEGORY_STATIC(LogEngine, Log)
IMPLEMENT_MODULE(CDefaultModuleImpl, Engine)

int32 CEngine::Initialize()
{
	// TODO: Renderer/RHI/D3D12 modules
	
	if (!SDL_InitSubSystem(SDL_INIT_VIDEO))
	{
		FH_LOG(LogEngine, Error, FH_TEXT("Couldn't initialize SDL video subsystem! %s"), SDL_GetError())
		return FH_FAILURE;
	}
	FH_LOG(LogEngine, Log, FH_TEXT("Initialized SDL video subsystem"))

	constexpr SDL_WindowFlags WindowFlags = SDL_WINDOW_RESIZABLE | SDL_WINDOW_HIDDEN | SDL_WINDOW_HIGH_PIXEL_DENSITY;
	
	Window = SDL_CreateWindow(*WindowTitle, 1280, 720, WindowFlags);
	if (!Window)
	{
		FH_LOG(LogEngine, Error, FH_TEXT("Failed to create window! %s"), SDL_GetError())
		return FH_FAILURE;
	}
	else
	{
		SDL_SetWindowPosition(Window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
	}

	WindowIcon = SDL_LoadSurface("../../../Images/Logo.png"); // TODO: Paths
	if (!WindowIcon)
	{
		FH_LOG(LogEngine, Error, FH_TEXT("Failed to load window icon! %s"), SDL_GetError())
		return FH_FAILURE;
	}
	else
	{
		SDL_SetWindowIcon(Window, WindowIcon);
	}
	
	Renderer = SDL_CreateRenderer(Window, nullptr);
	if (!Renderer)
	{
		FH_LOG(LogEngine, Error, FH_TEXT("Failed to create SDL renderer! %s"), SDL_GetError())
		return FH_FAILURE;
	}
	else
	{
		SDL_SetRenderVSync(Renderer, 1);
	}
	
	SDL_ShowWindow(Window);
	
	IMGUI_CHECKVERSION();
	ImGui::CreateContext();
	
	ImGuiIO& IO = ImGui::GetIO();
	IO.ConfigFlags |= ImGuiConfigFlags_NavEnableKeyboard;	// Enable Keyboard Controls
	IO.ConfigFlags |= ImGuiConfigFlags_DockingEnable;		// Enable Docking

	ImGui::StyleColorsDark(); // ImGui::StyleColorsLight();

	if (!ImGui_ImplSDL3_InitForSDLRenderer(Window, Renderer))
	{
		FH_LOG(LogEngine, Error, FH_TEXT("Failed to initialize ImGui implementation for SDL!"))
		return FH_FAILURE;
	}
	if (!ImGui_ImplSDLRenderer3_Init(Renderer))
	{
		FH_LOG(LogEngine, Error, FH_TEXT("Failed to initialize ImGui implementation for SDL renderer!"))
		return FH_FAILURE;
	}
	
	return FH_SUCCESS;
}

void CEngine::Tick()
{
	SDL_Event Event;
	while (SDL_PollEvent(&Event))
	{
		ImGui_ImplSDL3_ProcessEvent(&Event);
		if (Event.type == SDL_EVENT_WINDOW_CLOSE_REQUESTED && Event.window.windowID == SDL_GetWindowID(Window))
		{
			RequestExit(FH_TEXT("Window was closed"));
		}
	}

	// Start the Dear ImGui frame
	ImGui_ImplSDLRenderer3_NewFrame();
	ImGui_ImplSDL3_NewFrame();

	ImGui::NewFrame();
	{
		RenderImGui();
		ImGui::EndFrame();
	}
	ImGui::Render();

	// Start the SDL3 frame
	if (!Renderer) return; // TODO: replace with assertion
	
	SDL_RenderClear(Renderer);
	{
		ImGui_ImplSDLRenderer3_RenderDrawData(ImGui::GetDrawData(), Renderer);
		Render();
	}
	SDL_RenderPresent(Renderer);
}

void CEngine::Shutdown()
{
	// TODO: Revisit this. If ImGui has failed to initialize this will result in assertion:
	// Assertion failed: bd != nullptr && "No renderer backend to shutdown, or already shutdown?"
	ImGui_ImplSDLRenderer3_Shutdown();
	ImGui_ImplSDL3_Shutdown();
	ImGui::DestroyContext();

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

void CEngine::RenderImGui()
{
	static bool bShowDemoWindow = true;
	ImGui::ShowDemoWindow(&bShowDemoWindow);
}
