﻿using System;
using System.Drawing;

namespace NetleniumServer.Objects
{
    [Serializable]
    public class Location
    {
        public int X { get; set; }

        public int Y { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="point"></param>
        public Location(Point point)
        {
            X = point.X;
            Y = point.Y;
        }
    }
}