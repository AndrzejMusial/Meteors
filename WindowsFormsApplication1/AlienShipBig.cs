using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteorGame
{
    public class AlienShipBig : AlienShip
    {
        private const int maxSpeed = 5;
        
        public override Rectangle Area { get { return new Rectangle(Location.X - 5 * sizeMultiplier, Location.Y - 3 * sizeMultiplier, 10 * sizeMultiplier, 6* sizeMultiplier); } }

        public AlienShipBig(Rectangle boundaries, Random random) :
            base (boundaries, random)
        {
            SpeedX = random.Next(maxSpeed / 2, maxSpeed + 1);
            SpeedY = random.Next(maxSpeed / 2, maxSpeed + 1);
        }

        public override Shot FireShot(Ship ship)
        {
            double shotAngle = random.Next(-180, 180) * Math.PI / 180;
            return new Shot(Location, boundaries, shotAngle);;
        }

        public override void CreateShape()
        {
            shape = new Point[10]
            {
                new Point(Location.X - 5 * sizeMultiplier, Location.Y),
                new Point(Location.X + 5 * sizeMultiplier, Location.Y),
                new Point(Location.X + 2 * sizeMultiplier, Location.Y - 3 * sizeMultiplier),
                new Point(Location.X - 2 * sizeMultiplier, Location.Y - 3 * sizeMultiplier),
                new Point(Location.X - 5 * sizeMultiplier, Location.Y),
                new Point(Location.X - 5 * sizeMultiplier, Location.Y - 1 * sizeMultiplier),
                new Point(Location.X - 2 * sizeMultiplier, Location.Y - 3 * sizeMultiplier),
                new Point(Location.X + 2 * sizeMultiplier, Location.Y - 3 * sizeMultiplier),
                new Point(Location.X + 5 * sizeMultiplier, Location.Y - 1 * sizeMultiplier),
                new Point(Location.X + 5 * sizeMultiplier, Location.Y)
            };
        }

    }
}
