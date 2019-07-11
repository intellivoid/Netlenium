using System;
using System.Drawing;

namespace NetleniumServer.Objects
{
    [Serializable]
    public class SizeProperty
    {
        public int Width { get; set; }

        public int Height { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="size"></param>
        public SizeProperty(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }
    }
}
