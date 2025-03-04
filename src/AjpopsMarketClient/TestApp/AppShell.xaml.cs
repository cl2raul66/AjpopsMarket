using TestApp.Views;

namespace TestApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(PgRegister), typeof(PgRegister));
    }
}
