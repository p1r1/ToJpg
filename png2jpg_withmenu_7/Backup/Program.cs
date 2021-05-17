// Sample application that demonstrates a simple shell context menu.
// Ralph Arvesen (www.vertigo.com / www.lostsprings.com)

using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

[assembly: CLSCompliant(true)]
namespace SimpleContextMenu
{
	static class Program
	{
		// file type to register
		const string FileType = "jpegfile";

		// context menu name in the registry
		const string KeyName = "Simple Context Menu";

		// context menu text
		const string MenuText = "Copy to Grayscale";

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
					"The {0} shell extension was registered.",
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

		/// <summary>
		/// Make a grayscale copy of the image.
		/// </summary>
		/// <param name="filePath">Full path to the image to copy.</param>
		static void CopyGrayscaleImage(string filePath)
		{
			try
			{
				// full path to the grayscale copy
				string grayFilePath = Path.Combine(
					Path.GetDirectoryName(filePath),
					string.Format("{0} (grayscale){1}",
					Path.GetFileNameWithoutExtension(filePath),
					Path.GetExtension(filePath)));

				// using calls Dispose on the objects, important 
				// so the file is not locked when the app terminates
				using (Image image = new Bitmap(filePath))
				using (Bitmap grayImage = new Bitmap(image.Width, image.Height))
				using (Graphics g = Graphics.FromImage(grayImage))
				{
					// setup grayscale matrix
					ImageAttributes attr = new ImageAttributes();
					attr.SetColorMatrix(new ColorMatrix(new float[][]{   
						new float[]{0.3086F,0.3086F,0.3086F,0,0},
						new float[]{0.6094F,0.6094F,0.6094F,0,0},
						new float[]{0.082F,0.082F,0.082F,0,0},
						new float[]{0,0,0,1,0,0},
						new float[]{0,0,0,0,1,0},
						new float[]{0,0,0,0,0,1}}));

					// create the grayscale image
					g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
						0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attr);

					// save to the file system
					grayImage.Save(grayFilePath, ImageFormat.Jpeg);

					// success
					MessageBox.Show(string.Format("Copied grayscale image {0}", grayFilePath), Program.KeyName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("An error occurred: {0}", ex.Message), Program.KeyName);
				return;
			}
		}
	}
}
