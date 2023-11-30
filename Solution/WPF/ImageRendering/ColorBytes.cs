using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace UI.ImageRendering
{
    static class ColorBytes
    {
        const int RedMask = 0xFF0000;
        const int GreenMask = 0x00FF00;
        const int BlueMask = 0x0000FF;

        static byte[] ColorToBgrArray(Color c)
        {
            return new byte[] { c.B, c.G, c.R };
        }

        static byte[] CCToBgrArray(CustomColors cc)
        {
            return HexToBgrArray((int)cc);
        }

        static byte[] HexToBgrArray(int hexValue)
        {
            return new byte[] { (byte)(hexValue & BlueMask), (byte)((hexValue & GreenMask) >> 8), (byte)((hexValue & RedMask) >> 16) };
        }

        static CustomColors[] CustomColorsArray = new[]
{
            CustomColors.Black,
            CustomColors.White,

            CustomColors.Green,
            CustomColors.Red,
            CustomColors.Blue,

            CustomColors.Yellow,
            CustomColors.Pink,
            CustomColors.LightBlue,

            CustomColors.DarkGreen,
            CustomColors.LightRed,
            CustomColors.Violet,

            CustomColors.Orange,
            CustomColors.Magenta,
            CustomColors.Cian
        };

        public static byte[][] ColorSequence = new byte[][] {
            CCToBgrArray(CustomColorsArray[0]),
            CCToBgrArray(CustomColorsArray[1]),

            CCToBgrArray(CustomColorsArray[2]),
            CCToBgrArray(CustomColorsArray[3]),
            CCToBgrArray(CustomColorsArray[4]),

            CCToBgrArray(CustomColorsArray[5]),
            CCToBgrArray(CustomColorsArray[6]),
            CCToBgrArray(CustomColorsArray[7]),

            CCToBgrArray(CustomColorsArray[8]),
            CCToBgrArray(CustomColorsArray[9]),
            CCToBgrArray(CustomColorsArray[10]),

            CCToBgrArray(CustomColorsArray[11]),
            CCToBgrArray(CustomColorsArray[12]),
            CCToBgrArray(CustomColorsArray[13])
        };

        public static byte[] Red = ColorToBgrArray(Colors.Red);
        public static byte[] Black = ColorToBgrArray(Colors.Black);
        public static byte[] White = ColorToBgrArray(Colors.White);
        public static byte[] Green = ColorToBgrArray(Colors.Green);
        public static byte[] Blue = ColorToBgrArray(Colors.Blue);
        public static byte[] Yellow = ColorToBgrArray(Colors.Yellow);
        public static byte[] Gray = ColorToBgrArray(Colors.Gray);
    }
}
