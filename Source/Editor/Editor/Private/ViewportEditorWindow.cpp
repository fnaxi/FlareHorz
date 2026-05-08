// CopyRight FlareHorz Team. All Rights Reserved.


#include "ViewportEditorWindow.h"

#include "SDL3/SDL_render.h"

EDITOR_API SDL_Texture* GViewportTexture = nullptr;
EDITOR_API int32 GViewportSizeX = 640;
EDITOR_API int32 GViewportSizeY = 640;

CViewportEditorWindow::CViewportEditorWindow()
{
	Name = FH_TEXT("Viewport");
}

CViewportEditorWindow::~CViewportEditorWindow()
{
	SDL_DestroyTexture(GViewportTexture);
}

void CViewportEditorWindow::OnDraw()
{
	CEditorWindowBase::OnDraw();

	if (!GViewportTexture)
	{
		// TODO: revisit this
		ImGui::TextColored(ImVec4(1.0f, 0.0f, 0.0f, 1.0f), "GViewportTexture is nullptr!");
		return;
	}

	ImGui::Image(GViewportTexture, ImVec2(static_cast<float>(GViewportSizeX), static_cast<float>(GViewportSizeY)));
}

