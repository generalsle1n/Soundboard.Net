using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Soundboard.Net.Manager.Model;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Soundboard.Net.Manager
{
	public class SoundManager
	{
		private const string _soundFolderName = "Sounds";
		private List<Sound> _allSounds = new List<Sound>();
		private WaveOutEvent _output;
		private MixingSampleProvider _mixer;
		public SoundManager()
		{
			string BinaryPath = Assembly.GetExecutingAssembly().Location;
			string SoundFolderPath = Path.Combine(Path.GetDirectoryName(BinaryPath), _soundFolderName);
			string[] RawFiles = Directory.GetFiles(SoundFolderPath);

			foreach(string SingleFile in RawFiles)
			{
				_allSounds.Add(new Sound()
				{
					Name = Path.GetFileNameWithoutExtension(SingleFile),
					AbsolutePath = SingleFile
				});
			}

			InitAudioEngine();
		}
		public IEnumerable<Sound> GetAllSounds()
		{
			return _allSounds;
		}

		private void InitAudioEngine()
		{
			_output = new WaveOutEvent();
			_mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
			_mixer.ReadFully = true;
			_output.Init(_mixer);
			_output.Play();
		}

		public async Task PlaySound(Sound Sound)
		{
			using(AudioFileReader AudioReader = new AudioFileReader(Sound.AbsolutePath))
			{
				_mixer.AddMixerInput(AudioReader.ToWaveProvider());
			}
		}
	}
}
