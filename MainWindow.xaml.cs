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

        Timer timer = new Timer();

        //GenerateTexture planetTest = new();

        Planet planetTest;

        public MainWindow()
        {
            MouseDown += MouseDownEventHandler;

            timer.Interval = 32;
            timer.Elapsed += TimerTick;
            
            timer.AutoReset = true;

            InitializeComponent();

            //texture.Source = planetTest.wb;

            //planetTest.TextureGenTest(seed, x, y);
            //planetTest.CreateThumbnail("test.png");

            ///////////////////////////////////////////////

            GenerateNoise.NoiseOptions options = new();
            options.seed = 1;

            FastNoiseLite noiseSpace = GenerateNoise.GenerateNoiseSpace(options);
            WriteableBitmap noiseTexture = GenerateNoise.GenerateNoiseTexture(500 * 2, 500, noiseSpace);
            Utilities.CreateThumbnail("test Big Texture", noiseTexture);
            planetTest = new(noiseTexture);

            texture.Source = planetTest.outPutTexture;
            planetTest.Regenerate(scroll);

            /// TODO: last time you were here you did seamless noise + UV wraps
            /// now: you should convert both the image generation and planet generation to RGB32 so it's easier to work with once we do add colors.
            /// or maybe do a converter? idk if it worth keeping the gray8 implementation for noise... it makes sense but maybe it's not worth the hassle.
            /// 
            /// after that: you should most likely start working on fleshing out the actual planet gen, make some more textures, generate biomes, generate cloud layer, and whatever else you figure out to make it more planet like, features... there is a bunch of documentation on the topic, just browse around.

        }

        private void MouseDownEventHandler(object sender, MouseButtonEventArgs e)
        {
            timer.Start();
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