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

        public void Regenerate(float scrollFactor)
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
                    float normX = (x - width / 2f) / (width / 2f);
                    float normY = (y - height / 2f) / (height / 2f);

                    float rSquared = normX * normX + normY * normY;

                    if (rSquared <= 1f) // Only process inside the sphere
                    {
                        // Map the 2D screen point to a 3D point on the hemisphere
                        float dz = MathF.Sqrt(1f - rSquared); // z up
                        float dx = normX;
                        float dy = normY;

                        // Now go from 3D vector (dx, dy, dz) → spherical UV
                        float u = 0.5f + MathF.Atan2(dz, -dx) / (2f * MathF.PI);
                        float v = 0.5f + MathF.Asin(dy) / MathF.PI;

                        u = scrollFactor - 0.25f + (u - 0.5f); // shift so that sphere center is at viewportCenterU

                        // Wrap around to keep it inside 0–1
                        u = (u % 1f + 1f) % 1f;

                        // Convert UV to pixel coordinates
                        int sampleX = (int)(u * mainTexture.PixelWidth) % mainTexture.PixelWidth;
                        int sampleY = (int)(v * mainTexture.PixelHeight) % mainTexture.PixelHeight;

                        float value = MainPixels[sampleY * mainStride + sampleX] / 255f;

                        // Optional: circular falloff (smoothing)
                        float smoothing = Math.Clamp((MathF.Sqrt(rSquared) - 1f) / (0.98f - 1f), 0f, 1f);

                        outPutPixels[y * outPutStride + x] = (byte)(value * 255f * smoothing);
                    }
                    else
                    {
                        // Outside the sphere: set to black or transparent
                        outPutPixels[y * outPutStride + x] = 0;
                    }
                }
            }
            outPutTexture.WritePixels(new Int32Rect(0, 0, width, height), outPutPixels, outPutStride, 0);
        }


    }
}
