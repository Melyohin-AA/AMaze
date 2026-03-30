using Vortice.XAudio2;

namespace AMaze.Sound;

internal class StereoPlayer : IDisposable
{
	private readonly IXAudio2 xaudio2;
	private readonly IXAudio2MasteringVoice masterVoice;
	private readonly Dictionary<string, AudioMono> monoSamples = new Dictionary<string, AudioMono>();

	public StereoPlayer()
	{
		xaudio2 = XAudio2.XAudio2Create();
		masterVoice = xaudio2.CreateMasteringVoice();
		foreach (FileInfo file in new DirectoryInfo("Resources").EnumerateFiles("*.wav"))
		{
			string name = file.Name.Remove(file.Name.Length - 4, 4);
			if (name == "") continue;
			AudioMono sample = Wave.Read(file.FullName).ToAudioMono();
			monoSamples.Add(name, sample);
		}
	}

	~StereoPlayer() => Dispose();
	public void Dispose()
	{
		foreach (AudioMono sample in monoSamples.Values)
			sample.Dispose();
		masterVoice.DestroyVoice();
		masterVoice.Dispose();
		xaudio2.Dispose();
	}

	public void PanAndPlay(string monoName, float leftVolume, float rightVolume)
	{
		AudioMono mono = monoSamples[monoName];
		var panned = new AudioPanned(xaudio2, masterVoice, mono, leftVolume, rightVolume);
		panned.Play();
	}
}
