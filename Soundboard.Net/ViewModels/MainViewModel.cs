using ReactiveUI;
using Soundboard.Net.Views;
using System.Windows.Input;

namespace Soundboard.Net.ViewModels;

public class MainViewModel : ViewModelBase
{
	public int VolumePercentage { get; set; } = 50;
}
