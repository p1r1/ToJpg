// Sample application that demonstrates a simple shell context menu.
// Ralph Arvesen (www.vertigo.com / www.lostsprings.com)

using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualBasic;

[assembly: CLSCompliant(true)]
namespace _2.jpg_8
{
    static class Program
    {
        public static int kalite = 90;

        // file type to register
        const string FileType = "*";

        // context menu name in the registry
        const string KeyName = "2jpg";

        // context menu text
        const string MenuText = "Resmi JPG'e Cevir";

        [STAThread]
        static void Main(string[] args)
        {
            // process register or unregister commands
            if (!ProcessCommand(args))
            {
                // invoked from shell, process the selected file
                CopyGrayscaleImage(args[0]);
            }
        }
        

        /// <summary>
        /// Process command line actions (register or unregister).
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>True if processed an action in the command line.</returns>
        static bool ProcessCommand(string[] args)
        {
            // register
            if (args.Length == 0 || string.Compare(args[0], "-register", true) == 0)
            {
                // full path to self, %L is placeholder for selected file
                string menuCommand = string.Format(
                    "\"{0}\" \"%L\"", Application.ExecutablePath);

                // register the context menu
                FileShellExtension.Register(Program.FileType,
                    Program.KeyName, Program.MenuText,
                    menuCommand);

                MessageBox.Show(string.Format(
                    "BAŞARILI- --- - BAŞARILI\n\nBU BİR hata MESAJI DEĞİLDİR!!!{0} \n\nMenu basariyla kaydedildi artik programa ihtiyaciniz yok.1 sag tik yeterli",
                    Program.KeyName), Program.KeyName);

                return true;
            }
            

            // unregister		
            if (string.Compare(args[0], "-unregister", true) == 0)
            {
                // unregister the context menu
                FileShellExtension.Unregister(Program.FileType, Program.KeyName);

                MessageBox.Show(string.Format(
                    "The {0} shell extension was unregistered.",
                    Program.KeyName), Program.KeyName);

                return true;
            }

            // command line did not contain an action
            return false;
        }

        static void CopyGrayscaleImage(string filePath)
        {
            try
            {
                // full path to the grayscale copy
                string grayFilePath = Path.Combine(
                    Path.GetDirectoryName(filePath),
                    string.Format("{0} (2jpg){1}",
                    Path.GetFileNameWithoutExtension(filePath),
                    Path.GetExtension(filePath)));

                // using calls Dispose on the objects, important 
                // so the file is not locked when the app terminates
                //quality
                /*
                // kalite al
                kalite = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox("0-100 arasında kalite değeri giriniz.\n Bos birakmayin hata alirsiniz\n Unutmayın kalite arttıkça resim boyutuda artar.",
                                                                              "Kalite",
                                                                              "90",
                                                                              -1, -1));
                if (0 > kalite || kalite > 100)
                {
                    MessageBox.Show("Sayı 0-100 arası olacak.Tekrar deneyin!");
                    System.Environment.Exit(0);
                }
                long kalite2 = Convert.ToInt64(kalite);
                 */
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder , 95L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                //bmp1.Save(@"c:\TestPhotoQualityFifty.jpg", jpgEncoder, myEncoderParameters);
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                /*
                using (Image image = new Bitmap(filePath))
                    image.Save(grayFilePath.Remove(grayFilePath.Length - 4) + ".jpg", jpgEncoder, myEncoderParameters);
                */

                using (Image image = new Bitmap(filePath))
                // Assumes myImage is the PNG you are converting
                using (var b = new Bitmap(image.Width, image.Height)) {
                    b.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                    using (var g = Graphics.FromImage(b)) {
                        g.Clear(Color.White);
                        g.DrawImageUnscaled(image, 0, 0);
                    }

                    // Now save b as a JPEG like you normally would
                    b.Save(grayFilePath.Remove(grayFilePath.Length - 4) + ".jpg", jpgEncoder, myEncoderParameters);
                }
                
               
                    
                
                // success
                /* using (Image image = new Bitmap(filePath))
                     image.Save(grayFilePath.Remove(grayFilePath.Length - 4) + ".jpg", ImageFormat.Jpeg);
                 */
            }
                catch
                {
                    MessageBox.Show("erorcode:101 \n Yanlis birseyler oldu dostum.\n Tekrar deneyin ");
                    return;
                }
            }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        

        }
    }

