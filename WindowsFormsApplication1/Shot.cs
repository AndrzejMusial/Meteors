using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteorGame
{
    public class Shot
    {
        Rectangle boundaries;
        int velocityX;
        int velocityY;
        int trip;
        int shotRange;

        private const int moveInterval = 12;
        private const int width = 3;
        private const int height = 3;

        public Point Location { get; private set; }


        public Shot(Point location, Rectangle boundaries, double angle)
        {
            this.Location = location;
            this.boundaries = boundaries;
            shotRange = boundaries.Height / 2;
            trip = 0;
            velocityX = -(int)(moveInterval * Math.Sin(angle));
            velocityY = (int)(moveInterval * Math.Cos(angle));
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Cyan, Location.X, Location.Y, width, height);
        }

        public bool Move()
        {
            trip += moveInterval;
            if (trip > shotRange)
                return true;
            else
            {
                int X = Location.X - velocityX;
                int Y = Location.Y - velocityY;

                if (X > boundaries.Right)
                    X -= boundaries.Width;
                if (X < boundaries.Left)
                    X += boundaries.Width;

                if (Y > boundaries.Bottom)
                    Y -= boundaries.Height;
                if (Y < boundaries.Top)
                    Y += boundaries.Height;

                Location = new Point(X, Y);

                return false;
            }
        }
    }
}
