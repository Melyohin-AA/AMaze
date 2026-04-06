using Vortice.XAudio2;

namespace AMaze.Sound;

internal class AudioPanned : IDisposable
{
	private static readonly HashSet<AudioPanned> played = new HashSet<AudioPanned>();

	private bool disposed;

	// owns
	public IXAudio2SourceVoice SourceVoice { get; }
	// uses
	private readonly IXAudio2MasteringVoice masterVoice;
	private readonly AudioMono audioMono;

	public AudioPanned(
		IXAudio2 xaudio2, IXAudio2MasteringVoice masterVoice, AudioMono audioMono)
	{
		this.masterVoice = masterVoice;
		this.audioMono = audioMono;
		SourceVoice = xaudio2.CreateSourceVoice(audioMono.Format, true);
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
				Console.Title = ex.Message;
			}
			SourceVoice.DestroyVoice();
			SourceVoice.Dispose();
		}
	}

	public void Pan(float leftVolume, float rightVolume)
	{
		SourceVoice.SetOutputMatrix(masterVoice, 1, 2, [leftVolume, rightVolume]);
	}
	public void SubmitSourceBuffer(uint loopCount)
	{
		var buffer = new AudioBuffer {
			AudioDataPointer = audioMono.Ptr,
			AudioBytes = audioMono.Size,
			LoopCount = loopCount,
			Flags = BufferFlags.EndOfStream,
		};
		SourceVoice.SubmitSourceBuffer(buffer);
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
		var result = SourceVoice.Start();
		if (result.Failure)
		{
			Dispose();
			throw new Exception(result.Description);
		}
	}

	public void Stop()
	{
		SourceVoice.Stop();
		PlayEndedCb();
	}

	private void PlayEndedCb()
	{
		lock (played)
		{
			played.Remove(this);
		}
	}
}
