using Microsoft.Win32;
using stars_without_numbers___simulacrum.backendComponents;
using Start_Withouth_numbers_simulation_project.backendComponents;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timer = System.Timers.Timer;

namespace Start_Withouth_numbers_simulation_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int seed = (int)DateTime.Now.Ticks;
        float scroll = 0f;
        float scrollFactor = 0.001f;

        //GenerateTexture planetTest = new();

        Planet planetTest;

        public MainWindow()
        {
            MouseDown += MouseDownEventHandler;


            Timer timer = new Timer();

            timer.Interval = 16;
            timer.Elapsed += TimerTick;
            timer.Start();
            timer.AutoReset = true;

            InitializeComponent();

            //texture.Source = planetTest.wb;

            //planetTest.TextureGenTest(seed, x, y);
            //planetTest.CreateThumbnail("test.png");

            ///////////////////////////////////////////////

            GenerateNoise.NoiseOptions options = new();

            FastNoiseLite noiseSpace = GenerateNoise.GenerateNoiseSpace(options);
            WriteableBitmap noiseTexture = GenerateNoise.GenerateNoiseTexture(500 * 2, 500, noiseSpace);
            Utilities.CreateThumbnail("test Big Texture", noiseTexture);
            planetTest = new(noiseTexture);

            texture.Source = planetTest.outPutTexture;
            planetTest.Regenerate(scroll);

        }

        private void MouseDownEventHandler(object sender, MouseButtonEventArgs e)
        {
            scrollFactor += scrollFactor;
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            scroll += scrollFactor;
            if (scroll > 1f)
            {
                scroll = scroll % 1f;
            }
            this.Dispatcher.Invoke(() => planetTest.Regenerate(scroll));
        }

    }
}