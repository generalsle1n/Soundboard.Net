using Avalonia.Controls;
using Avalonia.Input;
using DynamicData;
using ReactiveUI;
using Soundboard.Net.Manager;
using Soundboard.Net.Manager.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Soundboard.Net.Views;

public partial class MainView : UserControl
{
	SoundManager Manager = new SoundManager();
	public MainView()
    {
		InitializeComponent();
		AddSounds();
		AddDevices();
    }

	private void AddSounds()
	{
		foreach (Sound SingleSound in Manager.GetAllSounds())
		{
			SoundAction.Children.Add(new Button()
			{
				Content = SingleSound.Name,
				Command = ReactiveCommand.Create(async () =>
				{
					Manager.PlaySound(SingleSound);
				}),
			});
		}
	}
	private void AddDevices()
	{
		Task<IEnumerable<SoundOutputDevices>> GetOutput = Manager.GetOutputDevices();
		GetOutput.Wait();
		IEnumerable<SoundOutputDevices> Devices = GetOutput.Result;

		foreach (SoundOutputDevices Device in Devices)
		{
			OutPutDevices.Items.Add(Device);
		}

		OutPutDevices.SelectedIndex = 0;
	}

	private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		Task<IEnumerable<SoundOutputDevices>> GetOutput = Manager.GetOutputDevices();
		GetOutput.Wait();
		IEnumerable<SoundOutputDevices> Devices = GetOutput.Result;

		OutPutDevices.Items.Clear();

		foreach (SoundOutputDevices Device in Devices)
		{
			OutPutDevices.Items.Add(Device);
		}

		OutPutDevices.SelectedIndex = 0;
	}

	private void ComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
	{
		if(((ComboBox)sender).ItemCount != 0)
		{
			Manager.ChangeOutputDevice((SoundOutputDevices)((ComboBox)sender).SelectedItem);
		}
	}
}
