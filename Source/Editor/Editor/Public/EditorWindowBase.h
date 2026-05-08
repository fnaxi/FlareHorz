// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "Containers/FlareString.h"
#include "imgui.h"

class CEditorWindowBase
{
public:
	void Draw();

protected:
	CString Name = FH_TEXT("Untitled Window");
	
	virtual void OnDraw();

private:
	ImGuiWindowFlags Flags = 0;
	bool bOpen = true;
	
};
