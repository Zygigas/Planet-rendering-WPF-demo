using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace stars_without_numbers___simulacrum.backendComponents
{
    internal class GenerateTexture
    {
        //for now, rawdogging the dependecy, should be replaced with dependecy inversion later on.
        readonly static FastNoiseLite noise = new();


        const int height = 100;
        const int width = 100;

        readonly WriteableBitmap wb = new(height, width, 96, 96, PixelFormats.Bgra32, null);

        public void TextureGenTest()
        {
            //this probably could be made into a config class/file/struct
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(0.02f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(5);
            noise.SetFractalLacunarity(2.0f);
            noise.SetFractalGain(0.5f);

            //probably a better way to do it, but need to test for speed before optimizing needlesly.
            int stride = wb.BackBufferStride;
            byte[] pixels = new byte[height * stride];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Generate noise value from -1 to 1
                    float val = noise.GetNoise(x, y);
                    val = (val + 1) / 2.0f; // Normalize to 0 - 1

                    // Optional: Apply a circular falloff for spherical shading
                    double dx = x - width / 2.0;
                    double dy = y - height / 2.0;
                    double dist = Math.Sqrt(dx * dx + dy * dy) / (width / 2.0);
                    dist = Math.Clamp(dist, 0, 1);
                    float falloff = (float)(1 - dist * dist); // smoother than linear

                    float brightness = val * falloff;

                    byte r = (byte)(brightness * 200);
                    byte g = (byte)(brightness * 180);
                    byte b = (byte)(brightness * 160);
                    byte a = (dist < 1.0 ? (byte)255 : (byte)0); // circle mask alpha

                    int index = y * stride + x * 4;
                    pixels[index + 0] = b;
                    pixels[index + 1] = g;
                    pixels[index + 2] = r;
                    pixels[index + 3] = a;
                }
            }
            wb.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);


        }
        public void CreateThumbnail(string filename)
        {
            if (filename != string.Empty)
            {
                using FileStream stream5 = new(filename, FileMode.Create);
                PngBitmapEncoder encoder5 = new();
                encoder5.Frames.Add(BitmapFrame.Create(wb));
                encoder5.Save(stream5);
            }
        }

    }
}
