using System;

/*
 * Some library provides you data as a buffer of `ushort`.
 * According to documentation, you know that in that case, staring from index 4
 * there is a `float` value of room temperature encoded.
 *
 * Decode this value.
 */

var data = new ushort[]
{
    0xff01,
    0xff02,
    0x0001,
    0x007f,
    0x3789,
    0xa441,
    0xf4ff,
    0x0101
};

var value = ReadFloat(data, 4);
Console.WriteLine(value);

float ReadFloat(ushort[] buffer, int index)
{
	var raw = buffer[index..(index+2)];
	var bytes = new byte[4];
	
	bytes[0]= BitConverter.GetBytes(raw[0])[1];
	bytes[1]= BitConverter.GetBytes(raw[0])[0];
	bytes[2]= BitConverter.GetBytes(raw[1])[1];
	bytes[3]= BitConverter.GetBytes(raw[1])[0];
	
	float value = BitConverter.ToSingle( bytes);
    return value;
}