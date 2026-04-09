// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include <algorithm>

template <typename TKeyType, typename TValueType>
class TPair
{
public:
	TPair() = default;

	TPair(const TKeyType& InKey, const TValueType& InValue)
		: Key(InKey), Value(InValue) {}

	TPair(TKeyType&& InKey, TValueType&& InValue)
		: Key(std::move(InKey)), Value(std::move(InValue)) {}

	/** Copy constructor. */
	TPair(const TPair& Other)
		: Key(Other.Key), Value(Other.Value) {}

	/** Move constructor. */
	TPair(TPair&& Other) noexcept
		: Key(std::move(Other.Key)), Value(std::move(Other.Value)) {}

	// Assignment operators
	TPair& operator=(const TPair& Other)
	{
		if (this != &Other)
		{
			Key = Other.Key;
			Value = Other.Value;
		}
		return *this;
	}

	TPair& operator=(TPair&& Other) noexcept
	{
		if (this != &Other)
		{
			Key = std::move(Other.Key);
			Value = std::move(Other.Value);
		}
		return *this;
	}

	TKeyType Key;
	TValueType Value;
};

/**
 * Array that maps keys to values.
 *
 * TODO: TMap is only partly implemented
 */
template <typename TKeyType, typename TValueType>
class TMap
{
	using UPairType = TPair<TKeyType, TValueType>;

public:
	/**
	 * Set the value associated with a key.
	 *
	 * @param Key The key to associate the value with.
	 * @param Value The value to associate with the key.
	 * @return A reference to the value as stored in the map. The reference is only valid until the next change to any key in the map.
	 */
	void Add(const TKeyType& Key, const TValueType& Value)
	{
		for (UArraySizeType i = 0; i < Pairs.Num(); ++i)
		{
			if (Pairs[i].Key == Key)
			{
				Pairs[i].Value = Value;
				return;
			}
		}
		Pairs.Add(UPairType(Key, Value));
	}

	// TODO: doc
	FORCEINLINE void Remove(const TKeyType& Key)
	{
		for (UArraySizeType i = 0; i < Pairs.Num(); )
		{
			if (Pairs[i].Key == Key)
			{
				Pairs.RemoveAt(i);
			}
			else
			{
				++i;
			}
		}
	}

	// TODO: doc
	FORCEINLINE void RemoveAt(const int32 Index)
	{
		Pairs.RemoveAt(Index);
	}

	/** Removes all elements from the map. */
	FORCEINLINE void Clear()
	{
		Pairs.Clear();
	}

	/** Shrinks the pair set to avoid slack. */
	FORCEINLINE void Shrink()
	{
		Pairs.Shrink();
	}
	
	/** @return he number of elements in the map. */
	[[nodiscard]] FORCEINLINE int32 Num() const
	{
		return Pairs.Num();
	}

	/**
	 * Check if map contains the specified key.
	 *
	 * @param Key The key to check for.
	 * @return true if the map contains the key.
	 */
	[[nodiscard]] FORCEINLINE bool Contains(const TKeyType& Key) const
	{
		return Find(Key) != nullptr;
	}

	/**
	 * Find the value associated with a specified key.
	 *
	 * @param Key The key to search for.
	 */
	[[nodiscard]] FORCEINLINE const TValueType* Find(const TKeyType& Key) const
	{
		for (UArraySizeType i = 0; i < Pairs.Num(); ++i)
		{
			if (Pairs[i].Key == Key)
			{
				return &Pairs[i].Value;
			}
		}
		return nullptr;
	}

private:
	/** A set of the key-value pairs in the map. */
	TArray<UPairType> Pairs;
	
public:
	/*----------------------------------------------------------------------------
		Operators
	----------------------------------------------------------------------------*/

	FORCEINLINE TValueType& operator[](const TKeyType& Key)
	{
		for (UArraySizeType i = 0; i < Pairs.Num(); ++i)
		{
			if (Pairs[i].Key == Key)
			{
				return Pairs[i].Value;
			}
		}

		Pairs.Add(UPairType(Key, TValueType()));
		return Pairs.Last().Value;
	}
	
	UPairType* begin()				{ return Pairs.begin(); }
	UPairType* end()				{ return Pairs.end(); }
	const UPairType* begin() const	{ return Pairs.begin(); }
	const UPairType* end() const	{ return Pairs.end(); }

};
