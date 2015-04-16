using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteorGame
{
    public class Ship
    {
        private const int rotationSpeed = 5;
        private const double maxSpeed = 8;
        private const double speedUpRate = 0.5;
        private const int sizeMultiplier = 2;

        int rotationAngle;
        Pen penWhite = new Pen(Brushes.White);
        Pen penYellow = new Pen(Brushes.Yellow);
        Pen penOrange = new Pen(Brushes.Orange);
        Pen penRed = new Pen(Brushes.Red);
        Pen penMagenta = new Pen(Brushes.Magenta);
        Point[] shipPoints;
        Point[] smallTrustPoints;
        Point[] bigTrustPoints;
        Point[] rotatedShipPoints;
        Point[] rotatedSmallTrustPoints;
        Point[] rotatedBigTrustPoints;
        double speedX;
        double speedY;
        int velocityX;
        int velocityY;
        Rectangle boundaries;

        public Point[] ShipPoints { get { return shipPoints; } }
        public Point FrontCannonPosition { get { return rotatedShipPoints[1]; } }
        public Point DoubleCannonRightPosition { get { return rotatedShipPoints[2]; } }
        public Point DoubleCannonLeftPosition { get { return rotatedShipPoints[4]; } }
        public double Angle { get; private set; }
        public Point Location { get; private set; }
        public Rectangle Area { get { return new Rectangle(Location.X - 4 * sizeMultiplier, Location.Y - 4 * sizeMultiplier, 8 * sizeMultiplier, 8 * sizeMultiplier );} }
        public int RotationAngle
        { 
            get { return rotationAngle; }
            set
            {
                if (value > 180)
                    rotationAngle = value - 360;
                else if (value <= -180)
                    rotationAngle = value + 360;
                else
                    rotationAngle = value;
                Angle = (rotationAngle * Math.PI) / 180;
            }
        }

        public Ship(Rectangle boundaries)
        {
            this.boundaries = boundaries;
            velocityX = 0;
            velocityY = 0;
            RotationAngle = 0;
            Location = new Point((boundaries.Left + boundaries.Right) / 2, (boundaries.Top + boundaries.Bottom) / 2);
            shipPoints = CreateShipShape(Location);
            CreateTrustShapes();
            CalculateRotatedShipAndTrust();
        }

        private Point[] CreateShipShape(Point location)
        {
            Point[] shapePoints = new Point[6]
            {
                location,
                new Point(location.X, location.Y - 6 * sizeMultiplier),
                new Point(location.X - 4 * sizeMultiplier, location.Y + 4 * sizeMultiplier),
                new Point(location.X, location.Y + 2 * sizeMultiplier),
                new Point(location.X + 4 * sizeMultiplier, location.Y + 4 * sizeMultiplier),
                new Point(location.X, location.Y - 6 * sizeMultiplier),
            };
            return shapePoints;
        }

        private void CreateTrustShapes()
        {
            smallTrustPoints = new Point[4]
            {
                new Point(Location.X, Location.Y + 2 * sizeMultiplier),
                new Point(Location.X - 1 * sizeMultiplier, Location.Y + 3 * sizeMultiplier),
                new Point(Location.X, Location.Y + 7 * sizeMultiplier),
                new Point(Location.X + 1 * sizeMultiplier, Location.Y + 3 * sizeMultiplier),
            };

            bigTrustPoints = new Point[4]
            {
                new Point(Location.X, Location.Y + 2 * sizeMultiplier),
                new Point(Location.X - 2 * sizeMultiplier, Location.Y + 5 * sizeMultiplier),
                new Point(Location.X, Location.Y + 11 * sizeMultiplier),
                new Point(Location.X + 2 * sizeMultiplier, Location.Y + 5 * sizeMultiplier),
            };
        }

        public void CalculateRotatedShipAndTrust()
        {
            rotatedShipPoints = CalculateRotated(shipPoints);
            rotatedBigTrustPoints = CalculateRotated(bigTrustPoints);
            rotatedSmallTrustPoints = CalculateRotated(smallTrustPoints);
        }

        private Point[] CalculateRotated(Point[] shape)
        {
            Point[] rotatedShapePoints = new Point[shape.Count()];
            for (int i = 0; i < shape.Count(); i++)
            {
                int rX = shape[i].X - Location.X;
                int rY = shape[i].Y - Location.Y;
                int deltaX = (int)(rX * Math.Cos(Angle) - rY * Math.Sin(Angle));
                int deltaY = (int)(rY * Math.Cos(Angle) + rX * Math.Sin(Angle));
                rotatedShapePoints[i] = new Point(Location.X + deltaX, Location.Y + deltaY);
            }
            return rotatedShapePoints;
        }

        public void SpeedUp()
        {
            double sX = speedX - speedUpRate * Math.Sin(Angle);
            double sY = speedY + speedUpRate * Math.Cos(Angle);
           
            double speed = Math.Sqrt(sX * sX + sY * sY);
            if (speed < maxSpeed)
            { 
                speedX = sX;
                speedY = sY;
                velocityX = (int)speedX;
                velocityY = (int)speedY;
                if (velocityX == 0)
                    velocityX = 1;
                if (velocityY == 0)
                    velocityY = 1;
            }
        }

        public void Move()
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
            shipPoints = CreateShipShape(Location);
            CreateTrustShapes();
            CalculateRotatedShipAndTrust();
        }

        public void Rotate(Direction direction)
        {
            if (direction == Direction.Clockwise)
                RotationAngle += rotationSpeed;
            else
                RotationAngle -= rotationSpeed;
            CalculateRotatedShipAndTrust();
        }

        public void DrawShip(Graphics g)
        {
            g.DrawPolygon(penYellow, rotatedShipPoints);
        }

        public void DrawTrust(Graphics g, bool engineWorking, int frame)
        {
            if (engineWorking)
            {
                if (frame == 2)
                    g.FillPolygon(Brushes.Red, rotatedSmallTrustPoints);
                else
                    g.FillPolygon(Brushes.Orange, rotatedBigTrustPoints);
            }
        }

        public void DrawForceShield(Graphics g, int counter)
        {
            if (counter < 50)    
                g.DrawEllipse(penWhite, Location.X - 7 * sizeMultiplier, Location.Y - 7 * sizeMultiplier, 14 * sizeMultiplier, 14 * sizeMultiplier);
            else
                g.DrawEllipse(penMagenta, Location.X - 7 * sizeMultiplier, Location.Y - 7 * sizeMultiplier, 14 * sizeMultiplier, 14 * sizeMultiplier);
        }

        public void DrawRemainShipes(Graphics g, int shipesLeft)
        {
            if (shipesLeft > 0)
            {
                for (int i = 1; i <= shipesLeft; i++)
                {
                    Point[] remainShipIcon = CreateShipShape(new Point(boundaries.Right - i * 8 * sizeMultiplier - 5, boundaries.Top + 6 * sizeMultiplier + 5));
                    g.DrawPolygon(penMagenta, remainShipIcon);
                }
            }
        }

        public void DrawExplosion(Graphics g, Point location)
        {
            Pen pen;
            for (int i = 0; i <= 20; i++)
            {
                if (i <= 6)
                    pen = penYellow;
                else if (i > 13)
                    pen = penRed;
                else
                    pen = penOrange;
                g.DrawEllipse(pen, location.X - i * sizeMultiplier, location.Y - i * sizeMultiplier, 2 * i * sizeMultiplier, 2 * i * sizeMultiplier);
            }
        }
    }
}
