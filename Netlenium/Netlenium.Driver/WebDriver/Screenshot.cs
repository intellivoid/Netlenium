using System;
#if NETCOREAPP2_0 || NETSTANDARD2_0
#else
using System.Drawing;
using System.Drawing.Imaging;
#endif
using System.IO;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// File format for saving screenshots.
    /// </summary>
    public enum ScreenshotImageFormat
    {
        /// <summary>
        /// W3C Portable Network Graphics image format.
        /// </summary>
        Png,

        /// <summary>
        /// Joint Photgraphic Experts Group image format.
        /// </summary>
        Jpeg,

        /// <summary>
        /// Graphics Interchange Format image format.
        /// </summary>
        Gif,

        /// <summary>
        /// Tagged Image File Format image format.
        /// </summary>
        Tiff,

        /// <summary>
        /// Bitmap image format.
        /// </summary>
        Bmp
    }

    /// <summary>
    /// Represents an image of the page currently loaded in the browser.
    /// </summary>
    [Serializable]
    public class Screenshot
    {
        private string base64Encoded = string.Empty;
        private byte[] byteArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="Screenshot"/> class.
        /// </summary>
        /// <param name="base64EncodedScreenshot">The image of the page as a Base64-encoded string.</param>
        public Screenshot(string base64EncodedScreenshot)
        {
            base64Encoded = base64EncodedScreenshot;
            byteArray = Convert.FromBase64String(base64Encoded);
        }

        /// <summary>
        /// Gets the value of the screenshot image as a Base64-encoded string.
        /// </summary>
        public string AsBase64EncodedString
        {
            get { return base64Encoded; }
        }

        /// <summary>
        /// Gets the value of the screenshot image as an array of bytes.
        /// </summary>
        public byte[] AsByteArray
        {
            get { return byteArray; }
        }

        /// <summary>
        /// Saves the screenshot to a Portable Network Graphics (PNG) file, overwriting the
        /// file if it already exists.
        /// </summary>
        /// <param name="fileName">The full path and file name to save the screenshot to.</param>
        public void SaveAsFile(string fileName)
        {
            SaveAsFile(fileName, ScreenshotImageFormat.Png);
        }

        /// <summary>
        /// Saves the screenshot to a file, overwriting the file if it already exists.
        /// </summary>
        /// <param name="fileName">The full path and file name to save the screenshot to.</param>
        /// <param name="format">A <see cref="ScreenshotImageFormat"/> value indicating the format
        /// to save the image to.</param>
        public void SaveAsFile(string fileName, ScreenshotImageFormat format)
        {
#if NETCOREAPP2_0 || NETSTANDARD2_0
            if (format != ScreenshotImageFormat.Png)
            {
                throw new WebDriverException(".NET Core does not support image manipulation, so only Portable Network Graphics (PNG) format is supported");
            }
#endif

            using (var imageStream = new MemoryStream(byteArray))
            {
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                { 
#if NETCOREAPP2_0 || NETSTANDARD2_0
                    imageStream.WriteTo(fileStream);
#else
                    using (var screenshotImage = Image.FromStream(imageStream))
                    {
                        screenshotImage.Save(fileStream, ConvertScreenshotImageFormat(format));
                    }
#endif
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="string">String</see> that represents the current <see cref="object">Object</see>.
        /// </summary>
        /// <returns>A <see cref="string">String</see> that represents the current <see cref="object">Object</see>.</returns>
        public override string ToString()
        {
            return base64Encoded;
        }

#if NETCOREAPP2_0 || NETSTANDARD2_0
#else
        private static ImageFormat ConvertScreenshotImageFormat(ScreenshotImageFormat format)
        {
            var returnedFormat = ImageFormat.Png;
            switch (format)
            {
                case ScreenshotImageFormat.Jpeg:
                    returnedFormat = ImageFormat.Jpeg;
                    break;

                case ScreenshotImageFormat.Gif:
                    returnedFormat = ImageFormat.Gif;
                    break;

                case ScreenshotImageFormat.Bmp:
                    returnedFormat = ImageFormat.Bmp;
                    break;

                case ScreenshotImageFormat.Tiff:
                    returnedFormat = ImageFormat.Tiff;
                    break;
            }

            return returnedFormat;
        }
#endif
    }
}
