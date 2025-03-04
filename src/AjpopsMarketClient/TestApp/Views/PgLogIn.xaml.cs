using TestApp.ViewModels;

namespace TestApp.Views;

public partial class PgLogIn : ContentPage
{
	public PgLogIn(PgLogInViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}