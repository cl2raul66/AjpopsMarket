using CommunityToolkit.Mvvm.ComponentModel;

namespace TestApp.ViewModels;

[QueryProperty(nameof(Email),"email")]
public partial class PgRegisterViewModel : ObservableValidator
{
    [ObservableProperty]
    string? email;

    [ObservableProperty]
    string? pwd;
}
