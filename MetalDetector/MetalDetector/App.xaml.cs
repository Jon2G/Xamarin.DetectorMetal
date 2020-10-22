using Xamarin.Forms;

namespace MetalDetector
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new PaginaPrincipal();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
