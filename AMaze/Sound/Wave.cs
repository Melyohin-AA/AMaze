using Vortice.Multimedia;

namespace AMaze.Sound;

internal class Wave
{
	public WaveFormat Format { get; }
	public byte[] Data { get; }

	public Wave(WaveFormat format, byte[] data)
	{
		Format = format;
		Data = data;
	}

	public AudioMono ToAudioMono()
	{
		return new AudioMono(Format, Data);
	}

	public static Wave Read(string filepath)
	{
		using var stream = new FileStream(filepath, FileMode.Open);
		using var reader = new BinaryReader(stream);
		if (!SkipUntil(reader, ('f' << 24) | ('m' << 16) | ('t' << 8) | ' '))
			throw new FormatException();
		int bits = reader.ReadInt32();
		reader.ReadInt16();
		short channels = reader.ReadInt16();
		int sampleRate = reader.ReadInt32();
		if (!SkipUntil(reader, ('d' << 24) | ('a' << 16) | ('t' << 8) | 'a'))
			throw new FormatException();
		uint size = reader.ReadUInt32();
		if (size > 0xFFFFFF)
			throw new FormatException();
		byte[] data = reader.ReadBytes((int)size);
		return new Wave(new WaveFormat(sampleRate, bits, channels), data);
	}

	private static bool SkipUntil(BinaryReader reader, uint mark)
	{
		const int limit = 256;
		if (reader.BaseStream.Length - reader.BaseStream.Position < limit)
			return false;
		uint markBuff = 0;
		for (int i = 0; i < limit; i++)
		{
			markBuff = (markBuff << 8) | reader.ReadByte();
			if (markBuff == mark)
				return true;
		}
		return false;
	}
}
