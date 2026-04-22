// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "Platform/Platform.h"

/**
 * Implements the main engine loop.	
 */
class CEngineLoop
{
public:
	CEngineLoop() = default;
	~CEngineLoop() = default;

	/**
	 * Pre-Initialize the main loop - parse command line, load core modules, etc.
	 *
	 * @param CmdLine The command line.
	 * @return The error level; 0 if successful, > 0 if there were errors.
	 */
	int32 PreInit(const TCHAR* CmdLine);
	
	/**
	 * Initialize the main loop.
	 *
	 * @return The error level; 0 if successful, > 0 if there were errors.
	 */
	int32 Init();

	/** Updates the main loop. */
	void Tick();

	/** Performs shut down. */
	void Exit();
	
};
