using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Gpio;
using GIS = GHIElectronics.UWP.Shields;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Raspi.Utility
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //init board 
        private GIS.FEZUtility utility;
        private DispatcherTimer timer;
      
        //init pins
        GIS.FEZUtility.AnalogPin PotensioPin=GIS.FEZUtility.AnalogPin.A0;
        GIS.FEZUtility.AnalogPin SoundPin = GIS.FEZUtility.AnalogPin.A1;
        GIS.FEZUtility.DigitalPin PIRPin = GIS.FEZUtility.DigitalPin.V01;
        GIS.FEZUtility.DigitalPin LedPin = GIS.FEZUtility.DigitalPin.V00;
        GIS.FEZUtility.PwmPin DimLedPin = GIS.FEZUtility.PwmPin.P1;
        GIS.FEZUtility.PwmPin ServoPin = GIS.FEZUtility.PwmPin.P0;
        //additional vars
        bool PirState = false;
        bool next;

        private async void Setup()
        {
            this.utility = await GIS.FEZUtility.CreateAsync();
            this.utility.SetDigitalDriveMode(LedPin, GpioPinDriveMode.Output);
            this.utility.SetDigitalDriveMode(PIRPin, GpioPinDriveMode.Input);

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
            this.timer.Tick += this.OnTick;
            this.timer.Start();
        }

        private void OnTick(object sender, object e)
        {
            //read analog
            this.TxtSound.Text = this.utility.ReadAnalog(SoundPin).ToString("N2");
            var Geser = this.utility.ReadAnalog(PotensioPin);
            this.TxtPotensio.Text = Geser.ToString("N2");
            //write digital
            this.TxtLed.Text = this.next.ToString();
            this.utility.WriteDigital(LedPin, this.next);
            //write pwm
            this.utility.SetPwmDutyCycle(ServoPin, Geser);
            this.utility.SetLedState(GIS.FEZUtility.Led.Led1, this.next);
            TxtServo.Text = Geser.ToString("N2");
            //dim led
            this.utility.SetPwmDutyCycle(DimLedPin, Geser);
            this.TxtDimLed.Text = Geser.ToString("N2");
            this.next = !this.next;
            //read pir sensor
            var Pir = this.utility.ReadDigital(PIRPin);
            if (Pir)
            {
                if (PirState == false)
                {
                    TxtPir.Text ="Ada gerakan.";
                    PirState = true;
                }
            }
            else
            {
                if (PirState)
                {
                    TxtPir.Text="Gerakan berhenti.";
                    PirState = false;
                }
            }

        }
        public MainPage()
        {
            this.InitializeComponent();
            this.Setup();
        }
    }
}
