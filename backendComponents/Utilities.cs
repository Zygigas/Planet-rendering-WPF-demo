using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Start_Withouth_numbers_simulation_project.backendComponents
{
    // TODO: Refactor into specialized utility services as the codebase expands to maintain modularity.
    internal static class Utilities
    {
        /// <summary>
        /// generates a file in PNG format and saves it to disk in the current working folder.
        /// </summary>
        /// <param name="filename"></param>
        public static void CreateThumbnail(string filename, WriteableBitmap image)
        {
            if (filename != string.Empty)
            {
                using FileStream stream5 = new(filename + ".png", FileMode.Create);
                PngBitmapEncoder encoder5 = new();
                encoder5.Frames.Add(BitmapFrame.Create(image));
                encoder5.Save(stream5);
            }
        }

        public static WriteableBitmap ReadPNG(string filePath)
        
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("PNG file not found", filePath);

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    var frame = decoder.Frames[0];

                    return new WriteableBitmap(frame);
                }
            }
        

    }
}
