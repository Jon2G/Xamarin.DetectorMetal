using Acr.UserDialogs;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MetalDetector.Services
{
    public class MetalDetectorService : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, args);
        }
        #endregion
        public double MetalPower { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public List<ChartEntry> History { get; set; }
        public Chart Chart { get; set; }
        public event EventHandler OnMetalDetected;
        /// <summary>
        /// El sensor geo-magnetico ó de rotación de vectores en similar al gyroscopio pero usa un magnetómetro en su lugar
        /// Es menos sensible que el girscpio pero consume menos energia
        ///Detecta la orientación relativa del celular respecto a el campo magnetico de la tierra y lo mide en microteslas (µ).
        /// </summary>
        public MetalDetectorService()
        {
            this.MetalPower =
            this.X =
            this.Y =
            this.Z = 0;
            this.History = new List<ChartEntry>();
        }
        public async Task Start()
        {
            await Task.Yield();
            try
            {
                Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
                if (!Magnetometer.IsMonitoring)
                {
                    Magnetometer.Start(SensorSpeed.Fastest);
                    Acr.UserDialogs.UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig("Leectura iniciada!")
                    {
                        Position = ToastPosition.Top,
                        BackgroundColor = Color.DodgerBlue,
                        MessageTextColor = Color.Black,
                        Duration = TimeSpan.FromSeconds(5)
                    });
                }
            }
            catch (FeatureNotSupportedException)
            {
                await UserDialogs.Instance.AlertAsync("Este celular no cuenta con Magenetometro...", "Imposible continuar", "Ok");
            }
        }
        public async Task Stop()
        {
            await Task.Yield();
            UserDialogs.Instance.Toast(new Acr.UserDialogs.ToastConfig("Leectura finalizada!")
            {
                Position = ToastPosition.Top,
                BackgroundColor = Color.DodgerBlue,
                MessageTextColor = Color.Black,
                Duration = TimeSpan.FromSeconds(5)
            });
            Magnetometer.Stop();
            Magnetometer.ReadingChanged -= Magnetometer_ReadingChanged;
            this.Chart = new BarChart() { Entries = this.History, MinValue = 0, MaxValue = 60, AnimationDuration = TimeSpan.FromMilliseconds(200) };
            OnPropertyChanged(nameof(Chart));
            await this.Chart.AnimateAsync(true);
        }
        private async void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            System.Numerics.Vector3 vector = e.Reading.MagneticField;
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
            this.MetalPower = Math.Round(Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2) + Math.Pow(this.Z, 2)));
            OnPropertyChanged(nameof(MetalPower));
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
            OnPropertyChanged(nameof(Z));
            if (this.History.Count > 150)
            {
                this.History.RemoveRange(0, 100);
            }
            Magnetometer.Stop();
            if (this.MetalPower > 60)
            {
                Vibration.Vibrate(100);
                OnMetalDetected?.Invoke(this, EventArgs.Empty);
            }
            this.History.Add(new Microcharts.ChartEntry((float)this.MetalPower) { Color = SKColor.Parse(Color.MidnightBlue.ToHex()) });
            this.Chart = new BarChart() { Entries = this.History, MinValue = 0, MaxValue = 60, AnimationDuration = TimeSpan.FromMilliseconds(200) };
            OnPropertyChanged(nameof(Chart));
            await this.Chart.AnimateAsync(false);
            //  await this.Chart.AnimateAsync(true);
            Magnetometer.Start(SensorSpeed.Fastest);
        }
    }
}
