// CopyRight FlareHorz Team. All Rights Reserved.


#include "EditorWindowBase.h"

void CEditorWindowBase::Draw()
{
	ImGui::Begin(*Name, &bOpen, Flags);
	{
		OnDraw();
	}
	ImGui::End();
}

void CEditorWindowBase::OnDraw()
{
}

