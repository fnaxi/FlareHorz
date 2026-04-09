// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include <algorithm>
#include <initializer_list>

#include "Math/Math.h"
#include "Platform/Platform.h"

__pragma(warning(disable: 4251))

using UArraySizeType = int32;

/**
 * A dynamically sized array of typed elements.
 */
template <typename TElementType>
class TArray
{
	enum : int8  { INDEX_NONE			= -1 };
	enum : uint8 { ARRAY_INITIAL_SIZE	=  4 };
	
public:
	explicit TArray(const UArraySizeType Size = ARRAY_INITIAL_SIZE) : ArrayMax(Size), ArrayNum(0)
	{
		Data = new TElementType[ArrayMax];
	}

	/** Copy constructor. */
	TArray(const TArray& Other) : ArrayMax(Other.ArrayMax), ArrayNum(Other.ArrayNum)
	{
		Data = new TElementType[ArrayMax];
		for (UArraySizeType i = 0; i < ArrayNum; ++i)
		{
			Data[i] = Other.Data[i];
		}
	}
	
	/** Move constructor. */
	TArray(TArray&& Other) noexcept
	{
		Data = Other.Data;
		ArrayNum = Other.ArrayNum;
		ArrayMax = Other.ArrayMax;

		Other.Data = nullptr;
		Other.ArrayNum = 0;
		Other.ArrayMax = 0;
	}
	
	TArray(std::initializer_list<TElementType> InitList)
	{
		ArrayMax = static_cast<UArraySizeType>(InitList.size() > 0 ? InitList.size() : 4);
		ArrayNum = static_cast<UArraySizeType>(InitList.size());
		
		Data = new TElementType[ArrayMax];

		UArraySizeType Index = 0;
		for (const TElementType& Element : InitList)
		{
			Data[Index++] = Element;
		}
	}

	~TArray()
	{
		delete[] Data;
	}

	/**
	 * Adds a new item to the end of the array, possibly reallocating the whole array to fit.
	 * 
	 * @param Item The item to add.
	 * @return Index to the new item.
	 */
	UArraySizeType Add(const TElementType& Item)
	{
		if (ArrayNum >= ArrayMax)
		{
			Reallocate(CMath::Max(ArrayMax * 2, ArrayNum + 1));
		}
		
		Data[ArrayNum++] = Item;
		return ArrayNum - 1;
	}

	/**
	 * Adds a raw array of elements to the end of the array.
	 *
	 * @param Ptr A pointer to an array of elements to add.
	 * @param Count The number of elements to insert from Ptr.
	 */
	void Append(const TElementType* Ptr, const UArraySizeType Count)
	{
		// TODO: Check that array to append has valid type

		if (Ptr == nullptr || Count == 0)
		{
			// TODO: Error "Array to append is invalid or empty!"
			return;
		}
		
		ReallocateIfNeeded(Count);
		
		for (UArraySizeType i = 0; i < Count; ++i)
		{
			Data[ArrayNum++] = Ptr[i];
		}
	}
	
	/**
	 * Adds an initializer list of elements to the end of the array.
	 * 
	 * @param InitList The initializer list of elements to add.
	 */
	void Append(std::initializer_list<TElementType> InitList)
	{
		const UArraySizeType ElementsNum = static_cast<UArraySizeType>(InitList.size());
		ReallocateIfNeeded(ElementsNum);
		
		for (const TElementType& Element : InitList)
		{
			Data[ArrayNum++] = Element;
		}
	}
	
	/**
	 * Removes the first occurrence of the specified item in the array, maintaining order but not indices.
	 *
	 * @param Item The item to remove.
	 */
	void RemoveFirst(const TElementType& Item)
	{
		RemoveAt(First());
	}
	
	/**
	 * Removes as many instances of Item as there are in the array, maintaining order but not indices.
	 *
	 * @param Item Item to remove from array.
	 * @returns The number of items removed.
	 */
	UArraySizeType Remove(const TElementType& Item)
	{
		UArraySizeType RemovedCount = 0;
		for (UArraySizeType i = 0; i < ArrayNum; )
		{
			if (Data[i] == Item)
			{
				RemoveAt(i);
				++RemovedCount;
			}
			else
			{
				++i;
			}
		}
		
		return RemovedCount;
	}

	// TODO: RemoveLast()

	/**
	 * Removes an element at given location.
	 *
	 * @param Index Location in array of the element to remove.
	 * @returns True if element was removed, false otherwise.
	 */
	bool RemoveAt(const int32 Index)
	{
		if (Index < 0 || Index >= ArrayNum)
		{
			return false;
		}

		if constexpr (std::is_pointer_v<TElementType>)
		{
			delete Data[Index];
		}
		
		// Shift elements to the left
		for (UArraySizeType i = Index; i < ArrayNum - 1; ++i)
		{
			Data[i] = std::move(Data[i + 1]);
		}
		
		--ArrayNum;		
		return true;
	}

	/**
	 * Finds element within the array.
	 *
	 * @param Item Item to look for.
	 * @param Index Will contain the found index.
	 * @returns True if found. False otherwise.
	 */
	[[nodiscard]] bool Find(const TElementType& Item, UArraySizeType& Index) const
	{
		Index = Find(Item);
		return Index != INDEX_NONE;
	}

	/**
	 * Finds element within the array.
	 *
	 * @param Item Item to look for.
	 * @returns Index of the found element. INDEX_NONE otherwise.
	 */
	[[nodiscard]] UArraySizeType Find(const TElementType& Item) const
	{
		for (UArraySizeType i = 0; i < ArrayNum; ++i)
		{
			if (Data[i] == Item)
			{
				return i;
			}
		}
		return INDEX_NONE;
	}
	
	/**
	 * Finds an element which matches a predicate functor.
	 *
	 * @param Predicate The functor to apply to each element. true, or nullptr if none is found.
	 */
	template <typename TPredicate>
	[[nodiscard]] TElementType* FindByPredicate(TPredicate Predicate) const
	{
		for (UArraySizeType i = 0; i < ArrayNum; ++i)
		{
			if (Predicate(Data[i]))
			{
				return Data[i];
			}
		}
		return nullptr;
	}

	/**
	 * Checks if this array contains the element.
	 *
	 * @returns	True if found. False otherwise.
	 */
	[[nodiscard]] bool Contains(const TElementType& Value) const
	{
		for (UArraySizeType i = 0; i < ArrayNum; ++i)
		{
			if (Data[i] == Value)
			{
				return true;
			}
		}
		return false;
	}
	
	/** 
	 * Helper function returning the size of the inner type.
	 *
	 * @returns Size in bytes of array type.
	 */
	[[nodiscard]] FORCEINLINE static constexpr uint32 GetTypeSize()
	{
		return sizeof(TElementType);
	}
	
	/** Returns the amount of slack in this array in elements. */
	[[nodiscard]] FORCEINLINE UArraySizeType GetSlack() const
	{
		return ArrayMax - ArrayNum;
	}

	/** Shrinks the array's used memory to the smallest possible to store elements currently in it. */
	void Shrink()
	{
		if (ArrayNum < ArrayMax)
		{
			Reallocate(ArrayNum);
		}
	}
	
	/** Clears the array. */
	void Clear()
	{
		if constexpr (std::is_pointer_v<TElementType>)
		{
			for (UArraySizeType i = 0; i < ArrayNum; ++i)
			{
				delete Data[i];
			}
		}
		
		ArrayNum = 0;
		Reallocate(ARRAY_INITIAL_SIZE);
	}

	/** Returns true if the array is empty and contains no elements. */
	[[nodiscard]] bool IsEmpty() const
	{
		return ArrayNum == 0;
	}

	/**
	 * Tests if index is valid, i.e. greater than or equal to zero, and less than the number of elements in the array.
	 *
	 * @param Index Index to test.
	 * @returns True if index is valid. False otherwise.
	 */
	[[nodiscard]] FORCEINLINE bool IsValidIndex(const UArraySizeType Index) const
	{
		return Index >= 0 && Index < ArrayNum;
	}

	/** Returns the first element. */
	[[nodiscard]] TElementType& First()
	{
		return GetElement(0);
	}

	/** Returns the last element. */
	[[nodiscard]] TElementType& Last()
	{
		return GetElement(ArrayNum - 1);
	}

	/** Returns current number of elements in the array. */
	[[nodiscard]] FORCEINLINE UArraySizeType Num() const
	{
		return ArrayNum;
	}

	/** Returns number of bytes used. */
	[[nodiscard]] SIZE_T NumBytes() const
	{
		return static_cast<SIZE_T>(ArrayNum) * sizeof(TElementType);
	}
	
	/** Returns maximum number of elements in array. */
	[[nodiscard]] FORCEINLINE UArraySizeType Max() const
	{
		return ArrayMax;
	}
	
	/**
	 * Resizes array to given number of elements.
	 *
	 * @param NewNum New size of the array.
	 */
	void SetNum(const UArraySizeType NewNum)
	{
		if (NewNum > ArrayMax)
		{
			Reallocate(NewNum);
		}
		ArrayNum = NewNum;
	}
	
	/** Returns a typed pointer to the first array entry. */
	[[nodiscard]] FORCEINLINE TElementType* GetData() const { return Data; }
	
private:
	TElementType* Data;

	/** Current allocated size of the array. */
	UArraySizeType ArrayMax;
	
	/** Current number of elements in the array. */
	UArraySizeType ArrayNum;

	void Reallocate(const UArraySizeType NewSize)
	{
		if (NewSize == ArrayMax) return;
		
		TElementType* NewData = new TElementType[NewSize];
		for (UArraySizeType i = 0; i < ArrayNum; i++)
		{
			NewData[i] = std::move(Data[i]);
		}
		
		delete[] Data;
		
		Data = NewData;
		ArrayMax = NewSize;
	}
	
	void ReallocateIfNeeded(const UArraySizeType AdditionalNum)
	{
		if (ArrayNum + AdditionalNum > ArrayMax)
		{
			Reallocate(ArrayMax + AdditionalNum);
		}
	}

	[[nodiscard]] TElementType& GetElement(UArraySizeType Index) const
	{
		if (!IsValidIndex(Index))
		{
			// TODO: Assertion "Index out of range!"
		}
		return Data[Index];
	}

public:
	/*----------------------------------------------------------------------------
		Operators
	----------------------------------------------------------------------------*/

	TElementType*		begin()			{ return GetData(); }
	TElementType*		end()			{ return GetData() + ArrayNum; }
	const TElementType* begin() const	{ return GetData(); }
	const TElementType* end() const		{ return GetData() + ArrayNum; }
	
	TArray& operator=(const TArray& Other)
	{
		if (this != &Other)
		{
			delete[] Data;

			ArrayMax = Other.ArrayMax;
			ArrayNum = Other.ArrayNum;
			
			Data = new TElementType[ArrayMax];
			for (UArraySizeType i = 0; i < ArrayNum; ++i)
			{
				Data[i] = Other.Data[i];
			}
		}
		return *this;
	}

	TArray& operator=(TArray&& Other) noexcept
	{
		if (this != &Other)
		{
			delete[] Data;

			Data = Other.Data;
			ArrayNum = Other.ArrayNum;
			ArrayMax = Other.ArrayMax;

			Other.Data = nullptr;
			Other.ArrayNum = 0;
			Other.ArrayMax = 0;
		}
		return *this;
	}

	// TODO: Other operators (like assign a raw array or assign initializer_list)
	
	bool operator==(const TArray& Other) const
	{
		if (this == &Other)
		{
			return true;
		}
		if (ArrayNum != Other.ArrayNum)
		{
			return false;
		}

		for (UArraySizeType i = 0; i < ArrayNum; ++i)
		{
			if (Data[i] != Other.Data[i])
			{
				return false;
			}
		}

		return true;
	}
	bool operator!=(const TArray& Other) const
	{
		return !(this == &Other);
	}

	TElementType& operator[](UArraySizeType Index)
	{
		return GetElement(Index);
	}
	
	const TElementType& operator[](UArraySizeType Index) const
	{
		return GetElement(Index);
	}
};
