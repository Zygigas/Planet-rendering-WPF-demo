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

        int outPutStride; 
        int mainStride; 
        byte[] outPutPixels; 
        byte[] MainPixels; 

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
            this.outPutTexture = new WriteableBitmap((int)this.mainTexture.PixelWidth/2,(int)this.mainTexture.PixelHeight, this.mainTexture.DpiX, this.mainTexture.DpiY,PixelFormats.Bgra32,null);

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

            // TODO: Check the texture sampling from the main texture and make sure it is not causing any issues.


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normX = (x - width / 2f) / (width / 2f);
                    float normY = (y - height / 2f) / (height / 2f);

                    float rSquared = normX * normX + normY * normY;

                    int pixelIndex = y * outPutStride + x * 4;

                    if (rSquared <= 1f) // Only process inside the sphere
                    {
                        // Map the 2D screen point to a 3D point on the hemisphere
                        float dz = MathF.Sqrt(1f - rSquared); 
                        float dx = normX;
                        float dy = normY;

                        float u = 0.5f + MathF.Atan2(dz, -dx) / (2f * MathF.PI);
                        float v = 0.5f + MathF.Asin(dy) / MathF.PI;

                        u = scrollFactor - 0.25f + (u - 0.5f); // shift so that sphere center is at viewportCenterU

                        // Normalize range
                        u = (u % 1f + 1f) % 1f;

                        // Convert UV to pixel coordinates
                        int sampleX = (int)(u * mainTexture.PixelWidth) % mainTexture.PixelWidth;
                        int sampleY = (int)(v * mainTexture.PixelHeight) % mainTexture.PixelHeight;

                        int sampleIndex = sampleY * mainStride + sampleX * 4;
                        float value0 = MainPixels[sampleIndex + 0] / 255f;
                        float value1 = MainPixels[sampleIndex + 1] / 255f;
                        float value2 = MainPixels[sampleIndex + 2] / 255f;
                        float value3 = MainPixels[sampleIndex + 3] / 255f;

                        // Smoothing factor
                        float smoothing = Math.Clamp((MathF.Sqrt(rSquared) - 1f) / (0.98f - 1f), 0f, 1f);

                        outPutPixels[pixelIndex + 0] = (byte)(value0 * 255f * smoothing);
                        outPutPixels[pixelIndex + 1] = (byte)(value1 * 255f * smoothing);
                        outPutPixels[pixelIndex + 2] = (byte)(value2 * 255f * smoothing);
                        outPutPixels[pixelIndex + 3] = (byte)(value3 * 255f * smoothing);
                    }
                    else
                    {
                        // Outside the sphere: set to transparent
                        outPutPixels[pixelIndex + 0] = (byte)(0);
                        outPutPixels[pixelIndex + 1] = (byte)(0);
                        outPutPixels[pixelIndex + 2] = (byte)(0);
                        outPutPixels[pixelIndex + 3] = (byte)(0);
                    }
                }
            }
            outPutTexture.WritePixels(new Int32Rect(0, 0, width, height), outPutPixels, outPutStride, 0);
        }


    }
}
