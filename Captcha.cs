using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCaptcha
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GenerateRandomCode(5));

            foreach (Char element in randomNumbers(5))
            {
                Console.Write(element);
            }

            Console.WriteLine();

            Console.WriteLine(new String(randomNumbers(5).ToArray()));

            GenerateCaptcha(new String(randomNumbers(5).ToArray()), @"C:\Users\Lucas\Desktop\captcha.png");

            Console.ReadLine();
        }
        static string GenerateRandomCode(int length)
        {
            string charPool = "ABCDEFGOPQRSTUVWXY1234567890ZabcdefghijklmHIJKLMNnopqrstuvwxyz";
            StringBuilder rs = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                rs.Append(charPool[(int)(random.NextDouble() * charPool.Length)]);
            }
            return rs.ToString();
        }

        static List<Char> randomNumbers(int totalNumbers)
        {
            Random rand = new Random();
            List<Char> outputList = new List<Char>();

            for (int i = 0; i < totalNumbers; i++)
            {
                // resultado é 1 letra maiuscula, 2 letra minuscula senão numero
                var letterNumber = rand.Next(0, 3);

                if (letterNumber == 1)
                {
                    //(ASCII 65-90 = A-Z)
                    outputList.Add((Char)rand.Next(65, 90));
                }
                else if (letterNumber == 2)
                {
                    //(ASCII 97-122 = a-z)
                    outputList.Add((Char)rand.Next(97, 122));
                }
                else
                {
                    //(ASCII 48-50 = 0-9)
                    outputList.Add((Char)rand.Next(48, 57));
                }
            }
            return outputList;
        }

        static void GenerateCaptcha(string text, string path)
        {
            int iHeight = 80;
            int iWidth = 190;
            Random oRandom = new Random();

            int[] aBackgroundNoiseColor = new int[] { 150, 150, 150 };
            int[] aTextColor = new int[] { 0, 0, 0 };
            int[] aFontEmSizes = new int[] { 15, 20, 25, 30, 35 };

            string[] aFontNames = new string[]
            {
             "Comic Sans MS",
             "Arial",
             "Times New Roman",
             "Georgia",
             "Verdana",
             "Geneva"
            };

            FontStyle[] aFontStyles = new FontStyle[]
            {  
             FontStyle.Bold,
             FontStyle.Italic,
             FontStyle.Regular,
             FontStyle.Strikeout,
             FontStyle.Underline
            };

            HatchStyle[] aHatchStyles = new HatchStyle[]
            {
             HatchStyle.BackwardDiagonal, HatchStyle.Cross, 
	            HatchStyle.DashedDownwardDiagonal, HatchStyle.DashedHorizontal,
             HatchStyle.DashedUpwardDiagonal, HatchStyle.DashedVertical, 
	            HatchStyle.DiagonalBrick, HatchStyle.DiagonalCross,
             HatchStyle.Divot, HatchStyle.DottedDiamond, HatchStyle.DottedGrid, 
	            HatchStyle.ForwardDiagonal, HatchStyle.Horizontal,
             HatchStyle.HorizontalBrick, HatchStyle.LargeCheckerBoard, 
	            HatchStyle.LargeConfetti, HatchStyle.LargeGrid,
             HatchStyle.LightDownwardDiagonal, HatchStyle.LightHorizontal, 
	            HatchStyle.LightUpwardDiagonal, HatchStyle.LightVertical,
             HatchStyle.Max, HatchStyle.Min, HatchStyle.NarrowHorizontal, 
	            HatchStyle.NarrowVertical, HatchStyle.OutlinedDiamond,
             HatchStyle.Plaid, HatchStyle.Shingle, HatchStyle.SmallCheckerBoard, 
	            HatchStyle.SmallConfetti, HatchStyle.SmallGrid,
             HatchStyle.SolidDiamond, HatchStyle.Sphere, HatchStyle.Trellis, 
	            HatchStyle.Vertical, HatchStyle.Wave, HatchStyle.Weave,
             HatchStyle.WideDownwardDiagonal, HatchStyle.WideUpwardDiagonal, HatchStyle.ZigZag
            };

            string sCaptchaText = text;

            //Creates an output Bitmap
            Bitmap oOutputBitmap = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
            Graphics oGraphics = Graphics.FromImage(oOutputBitmap);
            oGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            //Create a Drawing area
            RectangleF oRectangleF = new RectangleF(0, 0, iWidth, iHeight);
            Brush oBrush = default(Brush);

            //Draw background (Lighter colors RGB 100 to 255)
            oBrush = new HatchBrush(
                aHatchStyles[oRandom.Next(aHatchStyles.Length - 1)],
                Color.FromArgb((oRandom.Next(100, 255)),
                (oRandom.Next(100, 255)),
                (oRandom.Next(100, 255))),
                Color.White);

            oGraphics.FillRectangle(oBrush, oRectangleF);

            System.Drawing.Drawing2D.Matrix oMatrix = new System.Drawing.Drawing2D.Matrix();
            int i = 0;
            for (i = 0; i <= sCaptchaText.Length - 1; i++)
            {
                oMatrix.Reset();
                int iChars = sCaptchaText.Length;
                int x = iWidth / (iChars + 1) * i;
                int y = iHeight / 2;

                //Rotate text Random
                oMatrix.RotateAt(oRandom.Next(-40, 40), new PointF(x, y));
                oGraphics.Transform = oMatrix;

                //Draw the letters with Random Font Type, Size and Color
                oGraphics.DrawString
                (
                    //Text
                    sCaptchaText.Substring(i, 1),

                    //Random Font Name and Style
                    new Font(
                        aFontNames[oRandom.Next(aFontNames.Length - 1)],
                        aFontEmSizes[oRandom.Next(aFontEmSizes.Length - 1)],
                        aFontStyles[oRandom.Next(aFontStyles.Length - 1)]),

                        //Random Color (Darker colors RGB 0 to 100)
                        new SolidBrush(
                            Color.FromArgb(oRandom.Next(0, 100),
                            oRandom.Next(0, 100), 
                            oRandom.Next(0, 100))),                   
                        x,
                        oRandom.Next(10, 40)
                );
                oGraphics.ResetTransform();
            }

            MemoryStream oMemoryStream = new MemoryStream();
            oOutputBitmap.Save(oMemoryStream, System.Drawing.Imaging.ImageFormat.Png);
            byte[] oBytes = oMemoryStream.GetBuffer();

            oOutputBitmap.Dispose();
            oMemoryStream.Close();

            ByteArrayToFile(path, oBytes);
        }

        static bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }
    }


}
