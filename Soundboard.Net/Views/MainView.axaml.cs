using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using Soundboard.Net.Manager;
using Soundboard.Net.Manager.Model;
using System.Windows.Input;

namespace Soundboard.Net.Views;

public partial class MainView : UserControl
{
	SoundManager Manager = new SoundManager();
	public MainView()
    {
		InitializeComponent();
		AddSounds();
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
}
