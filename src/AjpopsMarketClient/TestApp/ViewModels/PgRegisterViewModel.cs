using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TestApp.ViewModels;

[QueryProperty(nameof(Email),"email")]
public partial class PgRegisterViewModel : ObservableValidator
{
    [ObservableProperty]
    string? email;

    [ObservableProperty]
    string? pwd;

    [RelayCommand]
    async Task GoToBack()
    {
        await Shell.Current.GoToAsync("..", true);
    }
}
