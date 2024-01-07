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
		private DirectSoundOut _output;
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

			InitAudioEngine().Wait();
		}
		public IEnumerable<Sound> GetAllSounds()
		{
			return _allSounds;
		}
		private async Task InitAudioEngine()
		{
			await GetOutputDevices();
			
			_output = new DirectSoundOut(ExtractGuidFromSoundObject(GetDefaultDevice()));
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
		public async Task<IEnumerable<SoundOutputDevices>> GetOutputDevices()
		{
			using(MMDeviceEnumerator DeviceEnum = new MMDeviceEnumerator())
			{

				MMDevice DefaultDevice = DeviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
				if(_allDevices.Where(SoundDevice => SoundDevice.ID.Equals(DefaultDevice.ID)).Count() == 0){
					_allDevices.Add(new SoundOutputDevices()
					{
						Name = DefaultDevice.FriendlyName,
						ID = DefaultDevice.ID,
						IsDefault = true
					});
				}
				
				MMDeviceCollection AudioDevices = DeviceEnum.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
				foreach(MMDevice Device in AudioDevices)
				{
					Device.GetPropertyInformation();
					if(_allDevices.Where(SoundDevice => SoundDevice.ID.Equals(Device.ID)).Count() == 0)
					{
						_allDevices.Add(new SoundOutputDevices()
						{
							Name = Device.FriendlyName,
							ID = Device.ID,
							IsDefault = false
						});
					}
				}
			}

			return _allDevices;
		}
		public async Task ChangeOutputDevice(SoundOutputDevices Output)
		{
			_output.Dispose();
			_output = new DirectSoundOut(ExtractGuidFromSoundObject(Output));
			_output.Init(_mixer);
			_output.Play();
		}
		private Guid ExtractGuidFromSoundObject(SoundOutputDevices Device)
		{
			string DefaultGuid = Device.ID;
			string PreparedGuid = DefaultGuid.Split(".")[4].Replace("{", "").Replace("}", "");
			Guid Guid = Guid.Parse(PreparedGuid);
			return Guid;
		}
		private SoundOutputDevices GetDefaultDevice()
		{
			return _allDevices.Where(Device => Device.IsDefault == true).First();
		}
	}
}
