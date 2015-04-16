using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteorGame
{
    public class AlienShipSmall : AlienShip
    {
        private const int maxSpeed = 8;

        public override Rectangle Area { get { return new Rectangle(Location.X - 4 * sizeMultiplier, Location.Y - 3 * sizeMultiplier, 8 * sizeMultiplier, 5 * sizeMultiplier); } }
        public override int Score { get { return 500; } }

        public AlienShipSmall(Rectangle boundaries, Random random) :
            base (boundaries, random)
        {
            SpeedX = random.Next(maxSpeed / 2, maxSpeed + 1);
            SpeedY = random.Next(maxSpeed / 2, maxSpeed + 1);
        }

        public override void CreateShape()
        {
            shape = new Point[11]
            {
                Location,
                new Point(Location.X, Location.Y + 2 * sizeMultiplier),
                new Point(Location.X - 2 * sizeMultiplier, Location.Y + 2 * sizeMultiplier),
                new Point(Location.X - 5 * sizeMultiplier, Location.Y),
                new Point(Location.X - 2 * sizeMultiplier, Location.Y - 2 * sizeMultiplier),
                new Point(Location.X - 1 * sizeMultiplier, Location.Y - 3 * sizeMultiplier),
                new Point(Location.X + 1 * sizeMultiplier, Location.Y - 3 * sizeMultiplier),
                new Point(Location.X + 2 * sizeMultiplier, Location.Y - 2 * sizeMultiplier),
                new Point(Location.X + 5 * sizeMultiplier, Location.Y),
                new Point(Location.X + 2 * sizeMultiplier, Location.Y + 2 * sizeMultiplier),
                new Point(Location.X, Location.Y + 2 * sizeMultiplier)
            };
        }

    }
}
