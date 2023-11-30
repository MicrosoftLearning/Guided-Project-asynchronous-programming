using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UI.ImageRendering;

namespace LangtonsAnt
{
    class GameImageRenderer
    {
        public static BitmapSource GetGenerationImageSourceX2(IGame g)
        {
            // Define parameters used to create the BitmapSource.
            PixelFormat pf = PixelFormats.Bgr24;
            int width = g.Size * 2;
            int height = g.Size * 2;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];

            // Initialize the image with data.
            byte[] pxl_color;
            for (int i = 0; i < g.Size; i++)
            {
                for (int j = 0; j < g.Size; j++)
                {
                    // Any ants there?
                    IAnt? ant = g.Ants.FirstOrDefault(a => (i == a.I) && (j == a.J));
                    if (ant != null)
                    {
                        // Draw one of the ants
                        var headColor = ColorBytes.Red;
                        var tailColor = ColorBytes.Blue;
                        switch (ant.Direction)
                        {
                            case AntDirection.Up:
                                Draw4in2XScaled(rawImage, rawStride, i, j, headColor, tailColor, headColor, tailColor);
                                break;
                            case AntDirection.Right:
                                Draw4in2XScaled(rawImage, rawStride, i, j, tailColor, tailColor, headColor, headColor);
                                break;
                            case AntDirection.Down:
                                Draw4in2XScaled(rawImage, rawStride, i, j, tailColor, headColor, tailColor, headColor);
                                break;
                            case AntDirection.Left:
                                Draw4in2XScaled(rawImage, rawStride, i, j, headColor, headColor, tailColor, tailColor);
                                break;
                        }
                    }
                    else
                    {
                        pxl_color = ColorBytes.ColorSequence[g.Field[i, j]];
                        Draw4in2XScaled(rawImage, rawStride, i, j, pxl_color, pxl_color, pxl_color, pxl_color);
                    }

                }
            }
            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);
            return bitmap;
        }

        private static void Draw4in2XScaled(byte[] rawImage, int rawStride,
            int i, int j,
            byte[] colorUpperLeft, byte[] colorLowerLeft, byte[] colorUpperRight, byte[] colorLowerRight)
        {
            // 4 pixels for 1
            Buffer.BlockCopy(colorUpperLeft, 0, rawImage, 2 * i * rawStride + 2 * j * 3, 3);
            Buffer.BlockCopy(colorLowerLeft, 0, rawImage, (2 * i + 1) * rawStride + 2 * j * 3, 3);
            Buffer.BlockCopy(colorUpperRight, 0, rawImage, 2 * i * rawStride + (2 * j + 1) * 3, 3);
            Buffer.BlockCopy(colorLowerRight, 0, rawImage, (2 * i + 1) * rawStride + (2 * j + 1) * 3, 3);
        }
    }
}
