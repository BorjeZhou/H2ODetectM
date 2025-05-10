using System;
using System.Collections.Generic;

namespace GCProject.Main.Protocol;

public class ProtocolDataQueue
{
	private int _index;

	private byte[] Data;

	public ProtocolDataQueue(byte[] Data)
	{
		this.Data = Data;
	}

	public void SetData(byte[] data)
	{
		Data = data;
	}

	public byte[] TakeBytes(int takeCount)
	{
		if (takeCount + _index > Data.Length)
		{
			throw new IndexOutOfRangeException();
		}
		byte[] retByte = new byte[takeCount];
		Array.Copy(Data, _index, retByte, 0, takeCount);
		_index += takeCount;
		return retByte;
	}

	public byte[] TakeAllBytesRemainSome(int remainCount = 0)
	{
		int takeCount = Data.Length - _index - remainCount;
		if (takeCount < 0)
		{
			throw new IndexOutOfRangeException();
		}
		return TakeBytes(takeCount);
	}

	public byte TakeByte()
	{
		if (1 + _index > Data.Length)
		{
			throw new IndexOutOfRangeException();
		}
		int index = _index;
		_index++;
		return Data[index];
	}

	public void SkipBytes(int count = 1)
	{
		if (count + _index > Data.Length)
		{
			throw new IndexOutOfRangeException();
		}
		_index += count;
	}

	public bool EqualBytes(byte[] compareBytes)
	{
		byte[] takeBytes = TakeBytes(compareBytes.Length);
		if (takeBytes.Length != compareBytes.Length)
		{
			return false;
		}
		for (int i = 0; i < takeBytes.Length; i++)
		{
			if (takeBytes[i] != compareBytes[i])
			{
				return false;
			}
		}
		return true;
	}

	public bool EqualByte(byte compareByte)
	{
		if (1 + _index > Data.Length)
		{
			throw new IndexOutOfRangeException();
		}
		int index = _index;
		_index++;
		return compareByte == Data[index];
	}

	public List<byte[]> TakeBytesSplitwith(byte Spliter1, byte Spliter2)
	{
		int dataLen = Data.Length;
		List<byte[]> Result = new List<byte[]>();
		int lastSectionIndex = 0;
		for (int i = 0; i < dataLen - 1; i++)
		{
			if (Data[i] == Spliter1 && Data[i + 1] == Spliter2 && lastSectionIndex != i)
			{
				int sectionLen = i - lastSectionIndex;
				byte[] OneSection = new byte[sectionLen];
				Array.Copy(Data, lastSectionIndex, OneSection, 0, sectionLen);
				Result.Add(OneSection);
				lastSectionIndex = i;
			}
		}
		int lastlen = Data.Length - lastSectionIndex;
		byte[] LastSection = new byte[lastlen];
		Array.Copy(Data, lastSectionIndex, LastSection, 0, lastlen);
		Result.Add(LastSection);
		return Result;
	}
}
