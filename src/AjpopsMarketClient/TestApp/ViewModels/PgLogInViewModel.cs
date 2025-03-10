using AjpopsMarketClient;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TestApp.Helpers;
using TestApp.Views;

namespace TestApp.ViewModels;

public partial class PgLogInViewModel : ObservableValidator
{
    readonly IAPIClientService aPIClientServ;

    public PgLogInViewModel(IAPIClientService aPIClientService)
    {
        AppVersion = VersionTracking.Default.CurrentVersion;
        aPIClientServ = aPIClientService;
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
    async Task SendSampleMail()
    {
        var result = await aPIClientServ.SendEmail();
        await Shell.Current.DisplayAlert("Notificacion", result, "Cerrar");
    }

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
