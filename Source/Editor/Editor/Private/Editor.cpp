// CopyRight FlareHorz Team. All Rights Reserved.


#include "Editor.h"

#include "CoreTypes.h"
#include "ViewportEditorWindow.h"
#include "Logging/Logger.h"

#include "imgui_impl_sdl3.h"
#include "imgui_impl_sdlrenderer3.h"
#include "Module/ModuleManager.h"

DEFINE_LOG_CATEGORY_STATIC(L_Editor, Log)

IMPLEMENT_MODULE(CDefaultModuleImpl, Editor)

int32 CEditor::Initialize(SDL_Window* Window, SDL_Renderer* Renderer)
{
	IMGUI_CHECKVERSION();
	ImGui::CreateContext();
	
	ImGuiIO& IO = ImGui::GetIO();
	IO.ConfigFlags |= ImGuiConfigFlags_NavEnableKeyboard;
	IO.ConfigFlags |= ImGuiConfigFlags_DockingEnable;

	// TODO: replace with path
	IO.IniFilename = "../../../ImGui.ini";

	ImGui::StyleColorsDark(); // ImGui::StyleColorsLight();
	
	if (!ImGui_ImplSDL3_InitForSDLRenderer(Window, Renderer))
	{
		FH_LOG(L_Editor, Error, FH_TEXT("Failed to initialize ImGui implementation for SDL!"))
		return FH_FAILURE;
	}
	if (!ImGui_ImplSDLRenderer3_Init(Renderer))
	{
		FH_LOG(L_Editor, Error, FH_TEXT("Failed to initialize ImGui implementation for SDL renderer!"))
		return FH_FAILURE;
	}
	
	Windows.Add( new CViewportEditorWindow() );
	
	return FH_SUCCESS;
}

void CEditor::Shutdown()
{
	// TODO: Revisit this. If ImGui has failed to initialize this will result in assertion:
	// Assertion failed: bd != nullptr && "No renderer backend to shutdown, or already shutdown?"
	ImGui_ImplSDLRenderer3_Shutdown();
	ImGui_ImplSDL3_Shutdown();
	ImGui::DestroyContext();
}

void CEditor::ProcessEvent(SDL_Event* Event)
{
	ImGui_ImplSDL3_ProcessEvent(Event);
}

void CEditor::PrepareImGui(SDL_Renderer* Renderer)
{
	ImGui_ImplSDLRenderer3_NewFrame();
	ImGui_ImplSDL3_NewFrame();

	ImGui::NewFrame();
	DrawGUI();
	ImGui::EndFrame();
	
	ImGui::Render();
}

void CEditor::DrawGUI()
{
	ImGui::DockSpaceOverViewport();
	for (CEditorWindowBase* Window : Windows)
	{
		if (!Window) continue;

		Window->Draw();
	}

	static bool bShowDemoWindow = true;
	ImGui::ShowDemoWindow(&bShowDemoWindow);
}

void CEditor::RenderImGui(SDL_Renderer* Renderer)
{
	ImGui_ImplSDLRenderer3_RenderDrawData(ImGui::GetDrawData(), Renderer);
}

