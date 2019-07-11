using System;

namespace NetleniumServer.Objects
{
    [Serializable]
    public class Size
    {
        public int Width { get; set; }

        public int Height { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="size"></param>
        public Size(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }
    }
}
