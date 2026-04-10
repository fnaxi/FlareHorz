// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "LogVerbosity.h"
#include "Containers/FlareString.h"

/** A macro to define a logging category as a C++ "static". This should ONLY be declared in a source file. Only accessible in that single file. */
#define DEFINE_LOG_CATEGORY_STATIC(CategoryName, DefaultVerbosity) \
	static struct SLogCategory##CategoryName : public SLogCategoryBase \
	{ \
	SLogCategory##CategoryName() : SLogCategoryBase(TEXT(#CategoryName), ELogVerbosity::DefaultVerbosity) {} \
	} CategoryName;

/** 
 * A macro to declare a logging category as a C++ "extern", declared in the header and paired
 * with DEFINE_LOG_CATEGORY in the source. Accessible by all files that include the header.
 */
#define DECLARE_LOG_CATEGORY(CategoryName, DefaultVerbosity) \
	extern struct SLogCategory##CategoryName : public SLogCategoryBase \
	{ \
		SLogCategory##CategoryName() : SLogCategoryBase(TEXT(#CategoryName), ELogVerbosity::DefaultVerbosity) {} \
	} CategoryName;

/** 
 * A macro to define a logging category, paired with DECLARE_LOG_CATEGORY from the header.
 */
#define DEFINE_LOG_CATEGORY(CategoryName) SLogCategory##CategoryName CategoryName;

/**
 * Base class for all log categories.
 */
struct SLogCategoryBase
{
	CORE_API SLogCategoryBase(const CString& InName, const ELogVerbosity InVerbosity)
	{
		Name = InName;
		Verbosity = InVerbosity;
	}

	[[nodiscard]] const CString& GetName() const { return Name; }

	[[nodiscard]] ELogVerbosity GetVerbosity() const { return Verbosity; }
	void SetVerbosity(const ELogVerbosity InVerbosity) { Verbosity = InVerbosity; }

private:
	CString Name;
	ELogVerbosity Verbosity;
};
