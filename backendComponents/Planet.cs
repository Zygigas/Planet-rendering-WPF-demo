using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Start_Withouth_numbers_simulation_project.backendComponents
{
    internal class Planet
    {
        WriteableBitmap mainTexture;
        public WriteableBitmap outPutTexture;

        int outPutStride; //outPutTexture.BackBufferStride
        int mainStride; // mainTexture.BackBufferStride
        byte[] outPutPixels; // [height * outPutStride];
        byte[] MainPixels; // pixel array representation of the mainTexture.

        /// <summary>
        /// Makes a new planet, based off of the provided main texture.
        /// </summary>
        /// <param name="mainTexture"> Should be twice as big in width then height, in order to properly represent 2 sides of a planet</param>
        public Planet(WriteableBitmap mainTexture) 
        {
            if (mainTexture.Height*2 != mainTexture.Width )
            {
                throw new Exception("mainTexture should have twice the width of height. This is to make representing the other side of the planet easier.");
            }
            this.mainTexture = mainTexture;
            this.outPutTexture = new WriteableBitmap((int)this.mainTexture.PixelWidth/2,(int)this.mainTexture.PixelHeight, this.mainTexture.DpiX, this.mainTexture.DpiY,PixelFormats.Gray8,null);

            this.outPutStride = outPutTexture.BackBufferStride;
            this.outPutPixels = new byte[this.outPutStride * outPutTexture.PixelHeight];

            this.mainStride = mainTexture.BackBufferStride;
            this.MainPixels = new byte[mainTexture.BackBufferStride * mainTexture.PixelHeight];

            mainTexture.CopyPixels(MainPixels, mainTexture.BackBufferStride, 0);

        }

        public void Regenerate(int xOffset, int yOffset)
        {
            int height = outPutTexture.PixelHeight;
            int width = outPutTexture.PixelWidth;

            // wrap around if the offset exceedes the main image size to avoid out of bound error and sampling erronous data (if this was C or C++ anyway...)
            //xOffset = xOffset >= mainTexture.PixelWidth ? xOffset % mainTexture.PixelWidth : xOffset;
            //yOffset = yOffset >= mainTexture.PixelWidth ? yOffset % mainTexture.PixelWidth : yOffset;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float deltaY = y - height / 2;
                    float deltaX = x - width / 2;

                    int sampleX = (x + xOffset) % mainTexture.PixelWidth;
                    int sampleY = (y + yOffset) % mainTexture.PixelHeight;

                    float value = MainPixels[sampleY * mainStride + sampleX] / 255f;

                    //Apply alpha circular falloff for spherical shading
                    float distanceFromOrigin = MathF.Sqrt(MathF.Pow(deltaX, 2) + MathF.Pow(deltaY, 2));
                    float maxDiameter = MathF.Max(width, height) / 2;

                    float smoothing = Math.Clamp((distanceFromOrigin / maxDiameter - 1f) / (0.98f - 1f), 0f, 1f);

                    outPutPixels[y * outPutStride + x] = (byte)(value * 255 * smoothing);
                }
            }
            outPutTexture.WritePixels(new Int32Rect(0, 0, width, height), outPutPixels, outPutStride, 0);
        }


    }
}
