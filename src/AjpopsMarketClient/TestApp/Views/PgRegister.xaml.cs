using TestApp.ViewModels;

namespace TestApp.Views;

public partial class PgRegister : ContentPage
{
	public PgRegister(PgRegisterViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}