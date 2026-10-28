using Microsoft.Win32;
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

        Planet planetTest;

        public MainWindow()
        {
            MouseDown += MouseDownEventHandler;

            timer.Interval = 32;
            timer.Elapsed += TimerTick;
            
            timer.AutoReset = true;

            InitializeComponent();

            GenerateNoise.NoiseOptions options = new();
            options.seed = 1;

            FastNoiseLite noiseSpace = GenerateNoise.GenerateNoiseSpace(options);
            WriteableBitmap noiseTexture = GenerateNoise.GenerateNoiseTexture(500 * 2, 500, noiseSpace);
            Utilities.CreateThumbnail("test Big Texture", noiseTexture);
            planetTest = new(noiseTexture);

            texture.Source = planetTest.outPutTexture;
            planetTest.Regenerate(scroll);

            // TODO: Refactor to BGRA32 for color support and expand procedural features (biomes, clouds, etc.)
            // 1. Refactor: Migrate texture generation and planetary projection to BGRA32 to support colorization.
            // 2. Optimization: Evaluate performance trade-offs between grayscale buffer conversion and direct color sampling.
            // 3. Feature Expansion: Implement multi-layered procedural generation (e.g., biomes, cloud layers, atmospheric scattering).
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