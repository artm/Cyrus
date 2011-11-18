using System.Text;
using System.Collections;
using System;
using System.Net;

public class Osc
{
	// read and advance the offset
	public static System.String ReadString(byte[] data, ref int offset)
	{
		int count = 0;
		for(; (offset+count)<data.Length && data[offset+count]!=0;count++);
		System.String result = Encoding.UTF8.GetString(data, offset, count);
		offset += count;
		offset += offset % 4 > 0 ? 4 - offset % 4 : 0;
		return result;
	}

	public static int ReadInt32(byte[] data, ref int offset)
	{
		int res = IPAddress.NetworkToHostOrder( BitConverter.ToInt32(data, offset) );
		offset += 4;
		return res;
	}

	public static float ReadFloat32(byte[] data, ref int offset)
	{
		float res;
		if (!BitConverter.IsLittleEndian)
			res = BitConverter.ToSingle(data, offset);
		else {
			byte[] sub = new byte[4];
			Array.Copy(data, offset, sub, 0, 4);
			Array.Reverse(sub);
			res = BitConverter.ToSingle(sub, 0);
		}
		offset += 4;
		return res;
	}

	public static IEnumerable ParseArgs(byte[] data, int offset, string types)
	{
		foreach(char t in types.Substring(1)) {
			switch(t) {
			case 'i':
				yield return ReadInt32(data, ref offset);
				break;
			case 'f':
				yield return ReadFloat32(data, ref offset);
				break;
			case 's':
				yield return ReadString(data, ref offset);
				break;
			default:
				throw new NotImplementedException("OSC type '" + t + "' not implemented");
			}
		}
	}

	public static object[] ToArray(byte[] data, int offset, string types)
	{
		object[] res = new object[types.Length-1];
		int i = 0;
		foreach(object o in ParseArgs(data,offset,types)) {
			res[i++] = o;
		}
		return res;
	}
}

