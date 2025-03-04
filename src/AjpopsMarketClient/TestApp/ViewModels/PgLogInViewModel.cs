using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TestApp.Helpers;
using TestApp.Views;

namespace TestApp.ViewModels;

public partial class PgLogInViewModel : ObservableValidator
{
    public PgLogInViewModel()
    {
        AppVersion = VersionTracking.Default.CurrentVersion;
    }

    [ObservableProperty]
    string? appVersion;

    [ObservableProperty]
    string? email;

    [ObservableProperty]
    string? pwd;

    [ObservableProperty]
    string? textNotifications;

    [RelayCommand]
    async Task GoToRegister()
    {
        if (!string.IsNullOrEmpty(Email) && EmailCheckingHelper.Check(Email))
        {            
            var sendData = new Dictionary<string, object>() { { "emai", Email } };

            await Shell.Current.GoToAsync(nameof(PgRegister), true, sendData);
            return;
        }
        await Shell.Current.GoToAsync(nameof(PgRegister), true);
    }
}
