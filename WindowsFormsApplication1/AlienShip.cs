using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteorGame
{
    public abstract class AlienShip
    {
        public Random random;
        public Rectangle boundaries;
        public Point[] shape;
        Pen penGreen = new Pen(Brushes.Lime);

        public const int sizeMultiplier = 3;

        public Point Location { get; private set; }
        public virtual Rectangle Area { get; private set; }
        public virtual int Score { get { return 200; } }
        public int SpeedX { get; set; }
        public int SpeedY { get; set; }

        public AlienShip(Rectangle boundaries, Random random)
        {
            this.boundaries = boundaries;
            this.random = random;
            Location = new Point(boundaries.Left, boundaries.Top);
        }

        public void Move()
        {
            int X = Location.X + SpeedX;
            int Y = Location.Y + SpeedY;

            if (X >= boundaries.Right)
                X -= boundaries.Width;
            if (X < boundaries.Left)
                X += boundaries.Width;

            if (Y >= boundaries.Bottom)
                Y -= boundaries.Height;
            if (Y < boundaries.Top)
                Y += boundaries.Height;

            Location = new Point(X, Y);
        }

        public virtual Shot FireShot(Ship ship)
        {
            if (ship.Location.X - Location.X != 0)
            {
                double shotAngle = Math.PI / 2 + Math.Atan2((ship.Location.Y - Location.Y), (ship.Location.X - Location.X)); 
                return new Shot(Location, boundaries, shotAngle);
            }
            return null;
        }

        public void Draw(Graphics g)
        {
            CreateShape();
            g.DrawPolygon(penGreen, shape);
        }

        public abstract void CreateShape();
    }
}
