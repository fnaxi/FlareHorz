// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "EditorWindowBase.h"

extern EDITOR_API struct SDL_Texture* GViewportTexture;
extern EDITOR_API int32 GViewportSizeX;
extern EDITOR_API int32 GViewportSizeY;

class CViewportEditorWindow : public CEditorWindowBase
{
public:
	CViewportEditorWindow();
	virtual ~CViewportEditorWindow();
	
protected:
	virtual void OnDraw() override;
	
};
