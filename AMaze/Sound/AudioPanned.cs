using Vortice.XAudio2;

namespace AMaze.Sound;

internal class AudioPanned : IDisposable
{
	private static readonly HashSet<AudioPanned> played = new HashSet<AudioPanned>();

	private bool disposed;

	public IXAudio2SourceVoice SourceVoice { get; }

	public AudioPanned(
		IXAudio2 xaudio2, IXAudio2MasteringVoice masterVoice, AudioMono audioMono, float leftVolume, float rightVolume)
	{
		SourceVoice = xaudio2.CreateSourceVoice(audioMono.Format, true);
		SourceVoice.SetOutputMatrix(masterVoice, 1, 2, [leftVolume, rightVolume]);
		var buffer = new AudioBuffer {
			AudioDataPointer = audioMono.Ptr,
			AudioBytes = audioMono.Size,
			Flags = BufferFlags.EndOfStream,
		};
		SourceVoice.SubmitSourceBuffer(buffer);
	}

	~AudioPanned() => Dispose();
	public void Dispose()
	{
		if (disposed) return;
		lock (this)
		{
			if (disposed) return;
			disposed = true;
			try
			{
				SourceVoice.Stop();
				SourceVoice.FlushSourceBuffers();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			SourceVoice.DestroyVoice();
			SourceVoice.Dispose();
		}
	}

	public void Play()
	{
		lock (played)
		{
			played.Add(this);
		}
		SourceVoice.StreamEnd += PlayEndedCb;
		SourceVoice.VoiceError += (err) => {
			Console.Title = "VoiceError: " + err.Result.Description;
			PlayEndedCb();
		};
		Console.WriteLine("start");
		var result = SourceVoice.Start();
		if (result.Failure)
		{
			Dispose();
			throw new Exception(result.Description);
		}
	}

	private void PlayEndedCb()
	{
		lock (played)
		{
			played.Remove(this);
		}
	}
}
