using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteorGame
{
    public class Meteor
    {
        Random random;
        Pen pen = new Pen(Brushes.Beige);

        private const double sizeMultiplier = 1;

        private const int bigSize = 90;
        private const int bigMaxSpeed = 3;
        private const int bigScore = 10;

        private const int mediumSize = 45;
        private const int mediumMaxSpeed = 4;
        private const int mediumScore = 20;

        private const int smallSize = 15;
        private const int smallMaxSpeed = 5;
        private const int smallScore = 50;

        private Point[] meteorShape;
        private int size;
        private int[] randomCoordinates;

        public Rectangle Area { get { return new Rectangle(new Point(Location.X - size /2, Location.Y - size / 2), new Size(size, size)); } }
        public Rectangle[] HitZones { get; private set; }
        public MeteorType Type {get; private set; }
        public Point Location { get; private set; }
        public int Score { get; private set; }
        public Rectangle Boundaries  {get; private set; }
        public int SpeedX { get; private set; }
        public int SpeedY { get; private set; }

        public Meteor(Rectangle boundaries, Random random)
        {
            this.Boundaries = boundaries;
            this.random = random;
            Type = MeteorType.Big;
            Score = bigScore;
            size = (int)(bigSize * sizeMultiplier);
            Location = new Point(random.Next(boundaries.Left, boundaries.Right), random.Next(boundaries.Top, boundaries.Bottom));
            SpeedX = random.Next(-bigMaxSpeed, bigMaxSpeed);
            SpeedY = random.Next(-bigMaxSpeed, bigMaxSpeed);
            randomCoordinates = GetRandomCoordinates();
            GenerateHitZones();
        }

        public Meteor(Rectangle boundaries, Random random, MeteorType type, Point location, int score, int size, int speedX, int speedY)
        {
            this.Boundaries = boundaries;
            this.random = random;
            this.Type = type;
            this.Location = location;
            this.Score = score;
            this.size = (int)(size * sizeMultiplier);
            this.SpeedX = speedX;
            this.SpeedY = speedY;
            randomCoordinates = GetRandomCoordinates();
            GenerateHitZones();
        }

        private void GenerateHitZones()
        {
            if (Type == MeteorType.Small)
                HitZones = new Rectangle[1] {new Rectangle(Location.X - size / 2, Location.Y - size / 2, size, size) };
            else
            {
                HitZones = new Rectangle[9] 
                {
                    new Rectangle(Location.X - size / 2, Location.Y - size / 2, size / 3, size / 3),
                    new Rectangle(Location.X - size / 6, Location.Y - size / 2, size / 3, size / 3),
                    new Rectangle(Location.X + size / 2, Location.Y - size / 2, size / 3, size / 3),
                    new Rectangle(Location.X - size / 2, Location.Y - size / 6, size / 3, size / 3),
                    new Rectangle(Location.X - size / 6, Location.Y - size / 6, size / 3, size / 3),
                    new Rectangle(Location.X + size / 2, Location.Y - size / 6, size / 3, size / 3),
                    new Rectangle(Location.X - size / 2, Location.Y + size / 6, size / 3, size / 3),
                    new Rectangle(Location.X - size / 6, Location.Y + size / 6, size / 3, size / 3),
                    new Rectangle(Location.X + size / 2, Location.Y + size / 6, size / 3, size / 3)
                };
            }
        }

        private void GenerateMeteorShape()
        {
            meteorShape = new Point[9]
            {
                new Point(Location.X, Location.Y),
                new Point(Location.X + randomCoordinates[0], Location.Y - size / 2),
                new Point(Location.X + randomCoordinates[1], Location.Y + randomCoordinates[2]),
                new Point(Location.X - size /2, Location.Y + randomCoordinates[3]),
                new Point(Location.X + randomCoordinates[4], Location.Y + randomCoordinates[5]),
                new Point(Location.X + randomCoordinates[6], Location.Y + size / 2),
                new Point(Location.X + randomCoordinates[7], Location.Y + randomCoordinates[8]),
                new Point(Location.X + size / 2, Location.Y + randomCoordinates[9]),
                new Point(Location.X + randomCoordinates[10], Location.Y + randomCoordinates[11])
            };
        }

        private int[] GetRandomCoordinates()
        {
            int[] coordinates = new int[12]
            {
                random.Next(-size / 6, size /6 + 1),
                random.Next(-size / 2, -size /6 + 1),
                random.Next(-size / 2, -size /6 + 1),
                random.Next(-size / 6, size /6 + 1),
                random.Next(-size / 2, -size /6 + 1),
                random.Next(size / 6, size /2 + 1),
                random.Next(-size / 6, size /6 + 1),
                random.Next(size / 6, size /2 + 1),
                random.Next(size / 6, size /2 + 1),
                random.Next(-size / 6, size /6 + 1),
                random.Next(size / 6, size /2 + 1),
                random.Next(-size / 2, -size /6 + 1),
            };
            return coordinates;
        }

        public void Draw(Graphics g)
        {
            GenerateMeteorShape();
            g.DrawPolygon(pen, meteorShape);
        }

        public void Move()
        {
            int X = Location.X - SpeedX;
            int Y = Location.Y - SpeedY;

            if (X > Boundaries.Right)
                X -= Boundaries.Width;
            if (X < Boundaries.Left)
                X += Boundaries.Width;

            if (Y > Boundaries.Bottom)
                Y -= Boundaries.Height;
            if (Y < Boundaries.Top)
                Y += Boundaries.Height;

            Location = new Point(X, Y);
            GenerateHitZones();
        }

        public void ReturnTwoSmallerMeteorsAfterHit(Rectangle boundaries, Random random, Meteor crushedMeteor, int hitZone, out Meteor meteor1, out Meteor meteor2)
        {
            MeteorType meteorsType;
            int meteorsSize;
            int maxSpeed;
            int meteorsScore;
            int speedX1;
            int speedY1;
            int speedX2;
            int speedY2;
            Point meteorsLocation = crushedMeteor.Location;
            if (crushedMeteor.Type == MeteorType.Big)
            {
                meteorsType = MeteorType.Medium;
                meteorsSize = mediumSize;
                maxSpeed = mediumMaxSpeed;
                meteorsScore = mediumScore;
            }
            else
            {
                meteorsType = MeteorType.Small;
                meteorsSize = smallSize;
                maxSpeed = smallMaxSpeed;
                meteorsScore = smallScore;
            }
            SetupCreatedMeteorsSpeedsDependingOnHitPoint(random, crushedMeteor, hitZone, maxSpeed, out speedX1, out speedY1, out speedX2, out speedY2);

            meteor1 = new Meteor(boundaries, random, meteorsType, meteorsLocation, meteorsScore, meteorsSize, speedX1, speedY1);
            meteor2 = new Meteor(boundaries, random, meteorsType, meteorsLocation, meteorsScore, meteorsSize, speedX2, speedY2);
        }

        private static void SetupCreatedMeteorsSpeedsDependingOnHitPoint(Random random, Meteor crushedMeteor, int hitZone, int maxSpeed, 
            out int speedX1, out int speedY1, out int speedX2, out int speedY2)
        {
            switch (hitZone)
            {
                case 0:
                    speedX1 = crushedMeteor.SpeedX + random.Next(maxSpeed + 1);
                    speedY1 = crushedMeteor.SpeedY;
                    speedX2 = crushedMeteor.SpeedX;
                    speedY2 = crushedMeteor.SpeedY + random.Next(maxSpeed + 1);
                    break;
                case 1:
                    speedX1 = crushedMeteor.SpeedX - random.Next(maxSpeed + 1);
                    speedY1 = crushedMeteor.SpeedY;
                    speedX2 = crushedMeteor.SpeedX + random.Next(maxSpeed + 1);
                    speedY2 = crushedMeteor.SpeedY;
                    break;
                case 2:
                    speedX1 = crushedMeteor.SpeedX - random.Next(maxSpeed + 1);
                    speedY1 = crushedMeteor.SpeedY;
                    speedX2 = crushedMeteor.SpeedX;
                    speedY2 = crushedMeteor.SpeedY + random.Next(maxSpeed + 1);
                    break;
                case 3:
                    speedX1 = crushedMeteor.SpeedX;
                    speedY1 = crushedMeteor.SpeedY - random.Next(maxSpeed + 1);
                    speedX2 = crushedMeteor.SpeedX;
                    speedY2 = crushedMeteor.SpeedY + random.Next(maxSpeed + 1);
                    break;
                case 4:
                    speedX1 = crushedMeteor.SpeedX - random.Next(maxSpeed + 1);
                    speedY1 = crushedMeteor.SpeedY - random.Next(maxSpeed + 1);
                    speedX2 = crushedMeteor.SpeedX + random.Next(maxSpeed + 1);
                    speedY2 = crushedMeteor.SpeedY + random.Next(maxSpeed + 1);
                    break;
                case 5:
                    speedX1 = crushedMeteor.SpeedX;
                    speedY1 = crushedMeteor.SpeedY - random.Next(maxSpeed + 1);
                    speedX2 = crushedMeteor.SpeedX;
                    speedY2 = crushedMeteor.SpeedY + random.Next(maxSpeed + 1);
                    break;
                case 6:
                    speedX1 = crushedMeteor.SpeedX + random.Next(maxSpeed + 1);
                    speedY1 = crushedMeteor.SpeedY;
                    speedX2 = crushedMeteor.SpeedX;
                    speedY2 = crushedMeteor.SpeedY - random.Next(maxSpeed + 1);
                    break;
                case 7:
                    speedX1 = crushedMeteor.SpeedX - random.Next(maxSpeed + 1);
                    speedY1 = crushedMeteor.SpeedY;
                    speedX2 = crushedMeteor.SpeedX + random.Next(maxSpeed + 1);
                    speedY2 = crushedMeteor.SpeedY;
                    break;
                default:
                    speedX1 = crushedMeteor.SpeedX - random.Next(maxSpeed + 1);
                    speedY1 = crushedMeteor.SpeedY;
                    speedX2 = crushedMeteor.SpeedX;
                    speedY2 = crushedMeteor.SpeedY - random.Next(maxSpeed + 1);
                    break;
            }
        }

        
    }
}
