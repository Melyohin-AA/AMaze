using System.Runtime.InteropServices;
using Vortice.Multimedia;

namespace AMaze.Sound;

internal class AudioMono : IDisposable
{
	private readonly byte[] data;
	private readonly GCHandle pinned;

	public uint Size => (uint)data.Length;
	public IntPtr Ptr => pinned.AddrOfPinnedObject();
	public WaveFormat Format { get; }

	public AudioMono(WaveFormat format, byte[] data)
	{
		if (format.Channels != 1)
			throw new ArgumentException();
		Format = format;
		this.data = data;
		pinned = GCHandle.Alloc(data, GCHandleType.Pinned);
	}

	~AudioMono() => Dispose();
	public void Dispose()
	{
		if (!pinned.IsAllocated) return;
		lock (this)
		{
			if (pinned.IsAllocated)
				pinned.Free();
		}
	}
}
