using AjpopsMarketClient;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TestApp.ViewModels;
using TestApp.Views;

namespace TestApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IAPIClientService, APIClientService>();

		builder.Services.AddTransient<PgLogIn, PgLogInViewModel>();
		builder.Services.AddTransient<PgRegister, PgRegisterViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
