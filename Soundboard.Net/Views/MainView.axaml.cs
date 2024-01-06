using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using Soundboard.Net.Manager;
using Soundboard.Net.Manager.Model;

namespace Soundboard.Net.Views;

public partial class MainView : UserControl
{
    public MainView()
    {   
		InitializeComponent();
        SoundManager Manager = new SoundManager();
        foreach(Sound SingleSound in Manager.GetAllSounds())
        {
            SoundAction.Children.Add(new Button()
            {
                Content = SingleSound.Name,
                Command = ReactiveCommand.Create(async () =>
                {
					Manager.PlaySound(SingleSound);
                })
            });

		}
        
        
    }
}
