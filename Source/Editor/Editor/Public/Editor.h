// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "Containers/Array.h"

class EDITOR_API CEditor
{
public:
	int32 Initialize(struct SDL_Window* Window, struct SDL_Renderer* Renderer);
	void Shutdown();

	void ProcessEvent(union SDL_Event* Event);

	void PrepareImGui(SDL_Renderer* Renderer);
	void DrawGUI();
	void RenderImGui(SDL_Renderer* Renderer);
	
private:
	TArray<class CEditorWindowBase*> Windows;
};
