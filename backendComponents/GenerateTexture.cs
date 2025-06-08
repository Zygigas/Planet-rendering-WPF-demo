using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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

        const int height = 500;
        const int width = 500;

        public WriteableBitmap wb = new(height, width, 96, 96, PixelFormats.Bgra32, null);

        struct NoiseOptions()
        {
            public int seed = 0;
            public FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.OpenSimplex2;
            public float frequency = 0.01f;
            public FastNoiseLite.FractalType fractalType = FastNoiseLite.FractalType.FBm;
            public int fractalOctaves = 5;
            public float fractalLacunarity = 2f;
            public float fractalGain = -0.5f;
            public float FractalWeightedStrength = -0.1f;
        }

        private FastNoiseLite GenerateNoise(NoiseOptions options)
        {
            FastNoiseLite noise = new();
            noise.SetSeed(options.seed);
            noise.SetNoiseType(options.noiseType);
            noise.SetFrequency(options.frequency);
            noise.SetFractalType(options.fractalType);
            noise.SetFractalOctaves(options.fractalOctaves);
            noise.SetFractalLacunarity(options.fractalLacunarity);
            noise.SetFractalGain(options.fractalGain);
            noise.SetFractalWeightedStrength(options.FractalWeightedStrength);
            return noise;
        }

        public void TextureGenTest(int seed,int xOffset,int yOffset)
        {
            //probably alpha better way to do it, but need to test for speed before optimizing needlesly.
            int stride = wb.BackBufferStride;
            byte[] pixels = new byte[height * stride];

            NoiseOptions noiseOptions = new NoiseOptions();
            noiseOptions.seed = seed;

            FastNoiseLite noise1 = GenerateNoise(noiseOptions);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                { 
                    //cords normalized to the circle origin.
                    float deltaY = y - height / 2;
                    float deltaX = x - width / 2;

                    //Apply alpha circular falloff for spherical shading
                    float distanceFromOrigin = MathF.Sqrt(MathF.Pow(deltaX, 2) + MathF.Pow(deltaY, 2));
                    float maxDiameter = MathF.Max(width, height)/ 2;

                    float value = noise1.GetNoise((deltaX + xOffset), (deltaY + yOffset));


                    // output is from -1 to 1, we want to normalize it to 0 to 1
                    value = (value + 1f) / 2f;

                    //smoothing blurr
                    float smoothing = Math.Clamp((distanceFromOrigin / maxDiameter - 1f) / (0.98f - 1f), 0f, 1f);
                    //TODO: figure out some better antialiasing method maybe...

                    byte red = (byte)(value * 155);
                    byte green = (byte)(value * 255);
                    byte blue = (byte)(value * 200);
                    byte alpha = distanceFromOrigin <= maxDiameter ? (byte)(255*smoothing) : (byte)0;

                    int index = y * stride + x * 4;
                    pixels[index + 0] = blue;
                    pixels[index + 1] = green;
                    pixels[index + 2] = red;
                    pixels[index + 3] = alpha;
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
