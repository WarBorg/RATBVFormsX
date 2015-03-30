using Microsoft.Phone.Controls;
using Xamarin.Forms;

namespace RATBVFormsX.WinPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            // TODO: Refactor: I don't like so much having this static field...
            var navigationPage = MvxFormsPhoneViewPresenter.NavigationPage;
            Content = navigationPage.ConvertPageToUIElement(this);
        }
    }
}
