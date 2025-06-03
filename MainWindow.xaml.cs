using stars_without_numbers___simulacrum.backendComponents;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Start_Withouth_numbers_simulation_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int tick = 0;
        GenerateTexture planetTest = new();
        public MainWindow()
        {
            InitializeComponent();

            texture.Source = planetTest.wb;

            this.MouseDown += MouseDownEvent;

            planetTest.TextureGenTest(tick);
            planetTest.CreateThumbnail("test.png");

        }

        private void MouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            tickUp();
        }

        void tickUp()
        {
            tick++;
            planetTest.TextureGenTest(tick);
            planetTest.CreateThumbnail("test.png");
        }

    }
}