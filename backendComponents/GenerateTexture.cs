using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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


        const int height = 2000;
        const int width = 2000;

        public WriteableBitmap wb = new(height, width, 96, 96, PixelFormats.Bgra32, null);

        private FastNoiseLite GenerateNoise(int seed) 
        {
            //this probably could be made into alpha config class/file/struct
            FastNoiseLite noise = new();
            noise.SetSeed(seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(0.002f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(5);
            noise.SetFractalLacunarity(2.0f);
            noise.SetFractalGain(0.5f);
            return noise;
        }

        public void TextureGenTest(int tick)
        {
            
            

            //probably alpha better way to do it, but need to test for speed before optimizing needlesly.
            int stride = wb.BackBufferStride;
            byte[] pixels = new byte[height * stride];

            FastNoiseLite noise1 = GenerateNoise(1);
            FastNoiseLite noise2 = GenerateNoise(2 );
            FastNoiseLite noise3 = GenerateNoise(3 );
            


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sampleX = (int)(x);
                    int sampleY = (int)(y);
                    
                    
                    float redVal = noise1.GetNoise(sampleX, sampleY, tick) % noise2.GetNoise(sampleX, sampleY, tick);
                    float greenVal = noise3.GetNoise(sampleX, sampleY, tick) % noise2.GetNoise(sampleX, sampleY, tick);
                    float blueVal = noise1.GetNoise(sampleX, sampleY, tick) % noise3.GetNoise(sampleX, sampleY, tick);

                    redVal = (redVal + 1) / 2.0f; // Normalize to 0 - 1
                    greenVal = (greenVal + 1) / 2.0f; // Normalize to 0 - 1
                    blueVal = (blueVal + 1) / 2.0f; // Normalize to 0 - 1

                    // Optional: Apply alpha circular falloff for spherical shading
                    double dx = x - width / 2.0;
                    double dy = y - height / 2.0;
                    double dist = Math.Sqrt(dx * dx + dy * dy) / (width / 2.0);
                    dist = Math.Clamp(dist, 0, 1);
                    double fade = Math.Clamp((dist - 0.96) / (1 - 0.96), 0, 1);
                    float falloff = (float)(1 - dist * dist); // smoother than linear

                    float redBrightness = redVal * falloff;
                    float greenBrightness = greenVal * falloff;
                    float blueBrightness = blueVal * falloff;

                    byte red = (byte)(redBrightness * 255);
                    byte green = (byte)(greenBrightness * 255);
                    byte blue = (byte)(blueBrightness * 255);
                    byte alpha = (byte)((1f - fade) * 255);

                        
                    

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
