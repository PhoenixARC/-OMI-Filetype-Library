using System;
using System.IO;
using System.Text;

internal class ArraySupport
{

	private bool UseLittleEndian = true;

	public ArraySupport()
	{

	}

	public ArraySupport(bool LittleEndian)
	{
		UseLittleEndian = LittleEndian;
	}

	internal short GetInt16(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 2);
		if (UseLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToInt16(array, 0);
	}

	internal float Getfloat(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 4);
		if (UseLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToSingle(array, 0);
	}

	internal ushort GetUInt16(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 2);
		if (UseLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToUInt16(array, 0);
	}

	internal int GetInt32(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 4);
		if (UseLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToInt32(array, 0);
	}

	internal uint GetUInt32(Stream InputStream)
	{
		byte[] array = ReadStreamBytes(InputStream, 4);
		if (UseLittleEndian)
		{
			Array.Reverse(array);
		}
		return BitConverter.ToUInt32(array, 0);
	}

	internal byte[] ReadStreamBytes(Stream InputStream, int ReadNumber)
	{
		byte[] array = new byte[ReadNumber];
		InputStream.Read(array, 0, ReadNumber);
		return array;
	}

	internal byte[] endianReverseUnicode(byte[] str)
	{
		byte[] newStr = new byte[str.Length];
		for (int i = 0; i < str.Length; i += 2)
		{
			newStr[i] = str[i + 1];
			newStr[i + 1] = str[i];
		}
		return newStr;
	}

	internal string GetString(Stream InputStream)
	{
		ushort num = GetUInt16(InputStream);
		byte[] array = new byte[(int)num];
		for (int i = 0; i < (int)num; i++)
		{
			byte b = (byte)InputStream.ReadByte();
			array[i] = b;
		}
		Encoding utf = Encoding.UTF8;
		return utf.GetString(array);
	}

	internal string GetStringInt32(Stream InputStream)
	{
		int num = GetInt32(InputStream);
		byte[] array = new byte[(int)num];
		for (int i = 0; i < (int)num; i++)
		{
			byte b = (byte)InputStream.ReadByte();
			array[i] = b;
		}
		Encoding utf = Encoding.UTF8;
		return utf.GetString(array);
	}

	internal string GetStringUTF16(Stream InputStream)
	{
		int num = GetInt32(InputStream) * 2;
		byte[] array = new byte[(int)num];
		for (int i = 0; i < (int)num; i++)
		{
			byte b = (byte)InputStream.ReadByte();
			array[i] = b;
		}
		array = endianReverseUnicode(array);
		Encoding utf = Encoding.Unicode;
		return utf.GetString(array);
	}

	internal char[] GetCharArray(Stream InputStream, int Size)
	{
		byte[] array = new byte[Size];
		for (int i = 0; i < Size; i++)
		{
			byte b = (byte)InputStream.ReadByte();
			array[i] = b;
		}
		Encoding utf = Encoding.UTF8;
		return utf.GetString(array).ToCharArray();
	}

	internal void WriteShortToStream(short short_0, Stream InputStream)
	{
		byte[] bytes = BitConverter.GetBytes(short_0);
		if (UseLittleEndian)
		{
			Array.Reverse(bytes);
		}
		InputStream.Write(bytes, 0, bytes.Length);
	}

	internal void WriteIntToStream(int Number, Stream InputStream)
	{
		byte[] bytes = BitConverter.GetBytes(Number);
		if (UseLittleEndian)
		{
			Array.Reverse(bytes);
		}
		InputStream.Write(bytes, 0, bytes.Length);
	}

	internal void WriteUIntToStream(uint uint_0, Stream InputStream)
	{
		byte[] bytes = BitConverter.GetBytes(uint_0);
		if (UseLittleEndian)
		{
			Array.Reverse(bytes);
		}
		InputStream.Write(bytes, 0, bytes.Length);
	}

	internal void WriteStringToStream(string string_0, MemoryStream memoryInputStream)
	{
		Encoding utf = Encoding.UTF8;
		byte[] bytes = utf.GetBytes(string_0);
		WriteShortToStream((short)bytes.Length, memoryInputStream);
		memoryInputStream.Write(bytes, 0, bytes.Length);
	}

	internal int method(Stream InputStream)
	{
		int result = GetInt32(InputStream);
		InputStream.Seek(-4L, SeekOrigin.Current);
		return result;
	}
}
