using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SitUpChairSensor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int _maxTimeOnChair = 20; // seconds
        private int _minDistance = 30; // cm
        private int _minLack = 5; 

        private UltrasonicDistanceSensor _sensor;
        private int timeOnChair = 0;
        private int lackCount = 0; 

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;

            _sensor = new UltrasonicDistanceSensor
                (UltrasonicDistanceSensor.AvailableGpioPin.GpioPin_23, UltrasonicDistanceSensor.AvailableGpioPin.GpioPin_24);

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += SensorScanning;
            timer.Start();
        }

        private void SensorScanning(object sender, object e)
        {
            var distance = _sensor.GetDistance();
            Debug.WriteLine($"Distance {distance} s");
            Debug.WriteLine($"LackCount {lackCount}");

            // There is somebody
            if (distance <= _minDistance)
            {
                lackCount = 0;
                timeOnChair++;
                if (timeOnChair == _maxTimeOnChair)
                {
                    Debug.WriteLine("Il est temps de faire de l'exercice !");
                    Task.Run(SendToBot);
                    timeOnChair = 0;
                }
            }
            else
            {
                if(lackCount == _minLack){
                    timeOnChair = 0;
                    lackCount = 0; 
                }
                else
                {
                    lackCount++;
                }    
            }

            Debug.WriteLine($"Temps sur la chaine {timeOnChair} s");
        }

        public async Task SendToBot()
        {
            var client = new DirectLineClient("X2pU-lf1tlc.cwA.mws.HG93u-P2-Cd6EuxHQZowPEOIJKc2SYcTGnVlwK4_arE");
            var conversation = (await client.Conversations.StartConversationWithHttpMessagesAsync()).Body;

            var activity = new Activity
            {
                From = new ChannelAccount("Jean"),
                Text = "move please",
                Type = ActivityTypes.Message
            };

            await client.Conversations.PostActivityAsync(conversation.ConversationId, activity);
        }
    }
}
