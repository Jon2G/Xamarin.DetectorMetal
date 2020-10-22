using MetalDetector.Services;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Pages;
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
    public partial class MetalDetectadoPopUp : PopupPage
    {
        public MetalDetectorService MetalDetector { get; set; }
        public MetalDetectadoPopUp(MetalDetectorService MetalDetector)
        {
            this.MetalDetector = MetalDetector;
            this.BindingContext = this;
            InitializeComponent();
        }

        public async void Ok(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.RemovePageAsync(this, true);
        }
    }
}