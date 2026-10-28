using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Start_Withouth_numbers_simulation_project.backendComponents
{
    internal static class GenerateNoise
    {
        /// <summary>
        /// structure that includes all relevant options for noise generation with default values (to ease the constructor arguments amount)
        /// </summary>
        public struct NoiseOptions()
        {
            public int seed = (int)DateTime.Now.Ticks;
            public FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.OpenSimplex2;
            public float frequency = 1.5f;
            public FastNoiseLite.FractalType fractalType = FastNoiseLite.FractalType.FBm;
            public int fractalOctaves = 6;
            public float fractalLacunarity = 3f;
            public float fractalGain = -0.4f;
            public float FractalWeightedStrength = -0.3f;
        }

        /// <summary>
        /// Generate a Noise space accessible using .getNoise.
        /// </summary>
        /// <param name="options">struct containing all the generator settings relevant to make the noise needed</param>
        /// <returns></returns>
        public static FastNoiseLite GenerateNoiseSpace(NoiseOptions options)
        {
            // Fast noise does not support more then 3 dimensional noise, for better smoothing, we should implement proper DI and replace fastnoise with a difrent library.
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

        /// <summary>
        /// generates a bitmap image that is a noise texture generated from specific noise. The Image is black and white, 1 being the highest noise value at specific pixel coordinate, and 0 the oposite.
        /// </summary>
        /// <param name="width">the width in px of the texture image</param>
        /// <param name="height">the height in px of the texture image</param>
        /// <param name="noiseSpace">the noise space to use for texture generation</param>
        /// <returns></returns>
        public static WriteableBitmap GenerateNoiseTexture(
            int width,
            int height,
            FastNoiseLite noiseSpace
        )
        {
            WriteableBitmap outPutTexture = new(width, height, 96, 96, PixelFormats.Bgra32, null);

            int stride = outPutTexture.BackBufferStride;
            byte[] pixels = new byte[height * stride];


            //TODO: Thic code might requier cleanup
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Current solution produces wrapping near the poles, which is not a proper mapping in technical terms, but might look better visually
                    // TODO: experiment with the warping to find a satisfactory balance
                    const float avoidWarpStart = 0.3f; 
                    const float avoidWarpEnd = 0.7f;

                    const float noiseScale = 2f; // noiseScale of the 3D circle - scales frequency

                    // torus transform - Mirroring artifacts and warping
                    // we use this method due to currently relying on 3d noise instead of 4d noise, which would be easier
                    float xNorm = (float)x / width;
                    float yNorm = ((float)y / height) * (avoidWarpEnd-avoidWarpStart) + avoidWarpStart;

                    
                    float xAngle = xNorm  * 2 * MathF.PI ;
                    float yAngle = yNorm * MathF.PI ;

                    float ySin = MathF.Sin(yNorm+MathF.PI);

                    float nx = noiseScale * MathF.Sin(xAngle)*ySin;
                    float ny = noiseScale * MathF.Cos(xAngle) * ySin;
                    float nz = noiseScale * MathF.Cos(yAngle);

                    float noiseValue = noiseSpace.GetNoise(nx,ny,nz);

                    //normalize the noise values from -1 to 1 range into bitmap 0 to 1 range
                    noiseValue = (noiseValue + 1f) / 2f;
                    int pixelIndex = y * stride + x * 4;
                    // since noise is a float between 0 and 1, we can use it as precentage of max value (pure white)
                    pixels[pixelIndex + 0] = (byte)(noiseValue * byte.MaxValue);
                    pixels[pixelIndex + 1] = (byte)(noiseValue * byte.MaxValue);
                    pixels[pixelIndex + 2] = (byte)(noiseValue * byte.MaxValue);
                    pixels[pixelIndex + 3] = (byte)(byte.MaxValue);
                }
            }

            outPutTexture.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            return outPutTexture;
        }
    }
}
