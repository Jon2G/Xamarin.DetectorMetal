using MetalDetector.Services;
using Microcharts;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MetalDetector
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaginaPrincipal : ContentPage
    {
        public MetalDetectorService MetalDetector { get; set; }
        public PaginaPrincipal()
        {
            this.MetalDetector = new MetalDetectorService();
            this.MetalDetector.OnMetalDetected += MetalDetector_OnMetalDetected;
            this.BindingContext = this;
            InitializeComponent();
        }

        private async void Button_Iniciar(object sender, EventArgs e)
        {
            await this.MetalDetector.Start();
        }

        private async void Button_Detener(object sender, EventArgs e)
        {
            await this.MetalDetector.Stop();
        }
        private async void MetalDetector_OnMetalDetected(object sender, EventArgs e)
        {
            await this.MetalDetector.Stop();
            MetalDetectadoPopUp popUp = new MetalDetectadoPopUp(this.MetalDetector);
            var scaleAnimation = new ScaleAnimation
            {
                PositionIn = MoveAnimationOptions.Right,
                PositionOut = MoveAnimationOptions.Left
            };
            popUp.Animation = scaleAnimation;
            await PopupNavigation.Instance.PushAsync(popUp, true);
        }
    }
}