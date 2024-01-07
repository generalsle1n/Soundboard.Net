using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Soundboard.Net.Manager.Model;
using System.Reflection;

namespace Soundboard.Net.Manager
{
	public class SoundManager
	{
		private const string _soundFolderName = "Sounds";
		private List<Sound> _allSounds = new List<Sound>();
		private List<SoundOutputDevices> _allDevices = new List<SoundOutputDevices>();
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

			_allDevices.Add(new SoundOutputDevices()
			{
				Name = "Default",
				ID = "-1"
			});
		}
		public async Task PlaySound(Sound Sound)
		{
			using(AudioFileReader AudioReader = new AudioFileReader(Sound.AbsolutePath))
			{
				_mixer.AddMixerInput(AudioReader.ToWaveProvider());
			}
		}
		public async Task<IEnumerable<SoundOutputDevices>> GetOutputDevices()
		{
			using(MMDeviceEnumerator DeviceEnum = new MMDeviceEnumerator())
			{
				MMDeviceCollection AudioDevices = DeviceEnum.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
				foreach(MMDevice Device in AudioDevices)
				{
					if(_allDevices.Where(SoundDevice => SoundDevice.ID.Equals(Device.ID)).Count() == 0)
					{
						_allDevices.Add(new SoundOutputDevices()
						{
							Name = Device.FriendlyName,
							ID = Device.ID
						});
					}
				}
			}

			return _allDevices;
		}
		public async Task ChangeOutputDevice(SoundOutputDevices Output)
		{
			_output.Dispose();
			if (Output.ID.Equals("-1"))
			{
				_output.DeviceNumber = -1;
			}
			else
			{
				_output.DeviceNumber = _allDevices.FindIndex(device => device.ID.Equals(Output.ID)) -1;
			}
			_output.Init(_mixer);
			_output.Play();
		}
	}
}
