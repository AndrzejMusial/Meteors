using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteorGame
{
    public class Game
    {
        Rectangle boundaries;
        Random random;
        Ship playerShip;
        AlienShip alienShip;
        List<Meteor> meteors = new List<Meteor>();
        List<Shot> shots = new List<Shot>();
        Shot alienShot;

        const int maxNumberOfPlayerActiveShots = 10;

        int playerShipesLeft;
        int forceshieldActiveCounter;
        int initialNumberOfMeteors;
        bool forceshieldActive;
        bool lastAlienShipBig;
        bool doubleFire;
        bool troubleFire;
        bool shipHitted;
        int wonExtraShips;
        int alienShipsCount;
        int numberOfPlayerActiveShots;

        public event GameOverEventHandler GameIsOver;
        public delegate void GameOverEventHandler(object sender, EventArgs e);

        public int wave { get; private set; }
        public int Score { get; private set;}

        public Game(Rectangle boundaries, Random random)
        {
            this.boundaries = boundaries;
            this.random = random;
            playerShip = new Ship(boundaries);
            ActivateTemporaryForceshield();
            initialNumberOfMeteors = 8;
            numberOfPlayerActiveShots = maxNumberOfPlayerActiveShots;
            CreateMeteors();
            wave = 1;
            forceshieldActive = true;
            playerShipesLeft = 3;
            alienShipsCount = 0;
            wonExtraShips = 0;
            lastAlienShipBig = false;
            doubleFire = false;
            troubleFire = false;
            shipHitted = false;
        }

        private void ActivateTemporaryForceshield()
        {
            forceshieldActive = true;
            forceshieldActiveCounter = 300;
        }

        private void CreateMeteors()
        {
            for (int i = 0; i < initialNumberOfMeteors; i++)
            {
                Meteor meteor = new Meteor(boundaries, random);
                meteors.Add(meteor);
            }
        }

        public void Draw(Graphics g, bool engineWorking, int frame)
        {
            g.FillRectangle(Brushes.Black, boundaries);

            foreach (Meteor meteor in meteors)
                meteor.Draw(g, meteors.Count);

            foreach (Shot shot in shots)
                shot.Draw(g);

            if (alienShip != null)
                alienShip.Draw(g);

            if (alienShot != null)
                alienShot.Draw(g);

            if (playerShip != null)
            {
                playerShip.DrawShip(g);
                playerShip.DrawTrust(g, engineWorking, frame);
            }

            if (forceshieldActive)
                playerShip.DrawForceShield(g, forceshieldActiveCounter);

            if (shipHitted)
            {
                playerShip.DrawExplosion(g, playerShip.Location);
                shipHitted = false;
            }

            playerShip.DrawRemainShipes(g, playerShipesLeft);

            using (Font arial12bold = new Font("Arial", 12, FontStyle.Bold))
            {

                g.DrawString("Score: " + Score + ",  Wave: " + wave + ",  Alien Ships: " + alienShipsCount + " Meteors in Space: " + meteors.Count, arial12bold, Brushes.LightCyan, new PointF(5, 5)); 
                g.DrawString("Q-Exit, F4-Stop, F5-Start, LEFT-Rotate Left, RIGHT-Rotate Right, UP-Trust, SPACE-Fire", arial12bold, Brushes.Red, new PointF(5, boundaries.Bottom - 25));
            }
        }

        public void Go()
        {
            ManagePlayerForceshield();

            MovePlayerShots();
            FireOrMoveAlienShots();
            MoveAllMeteors();

            if (alienShip != null)
                alienShip.Move();
                
            if (!shipHitted)
                playerShip.Move();

            CheckForMeteorsHittedByPlayer();
            if (alienShip != null)
            {
                CheckForAlienHittedByPlayer();
                CheckForAlienPlayerCollision();
            }

            if (alienShot != null)
                CheckForMeteorHittedByAlienShips();

            CheckForPlayerCollision();
            CheckForPlayerHiteedByAlien();

            if (meteors.Count <= 0)
                CreateNextWaveOfMeteors();

            AddPlayerShipsLeftIfConditionsFulfilled();
            ActivateMultipleFire();
            CreateAlienShipIfConditionsFulfilled();
        }

        private void ActivateMultipleFire()
        {
            if (Score > 12000)
            {
                doubleFire = false;
                troubleFire = true;
                numberOfPlayerActiveShots = 3 * maxNumberOfPlayerActiveShots;
            }
            else if (Score > 6000 && Score <= 12000)
            {
                doubleFire = true;
                numberOfPlayerActiveShots = 2 * maxNumberOfPlayerActiveShots;
            }
        }

        private void AddPlayerShipsLeftIfConditionsFulfilled()
        {
            if (Score - wonExtraShips * 4000 >= 4000)
            {
                wonExtraShips++;
                playerShipesLeft++;
            }
        }

        private void CreateAlienShipIfConditionsFulfilled()
        {
            if (alienShip == null && Score - alienShipsCount * 2000 >= 2000)
            {
                if (lastAlienShipBig)
                {
                    alienShip = new AlienShipSmall(boundaries, random);
                    lastAlienShipBig = false;
                }
                else
                {
                    alienShip = new AlienShipBig(boundaries, random);
                    lastAlienShipBig = true;
                }
                alienShipsCount++;
            }
        }

        private void CreateNextWaveOfMeteors()
        {
            wave++;
            ActivateTemporaryForceshield();
            if (wave > 6)
                initialNumberOfMeteors = 12;
            else if (wave > 3 && wave <= 6)
                initialNumberOfMeteors = 10;
            CreateMeteors();
        }

        private void CheckForAlienHittedByPlayer()
        {
            foreach (Shot shot in shots)
                if (alienShip != null && alienShip.Area.Contains(shot.Location))
                {
                    Score += alienShip.Score;
                    alienShip = null;
                    shots.Remove(shot);
                    return;
                }

        }

        private void CheckForPlayerHiteedByAlien()
        {
            if (alienShot != null && playerShip != null && playerShip.Area.Contains(alienShot.Location) && !forceshieldActive)
            {
                shipHitted = true;
                CreateNewPlayerShipOrFinishGame();
            }
        }

        private void CheckForPlayerCollision()
        {
            if (playerShip != null)
            {
                Meteor hittingMeteor;
                if (CheckForPlayerMeteorCollision(out hittingMeteor))
                {
                    if (hittingMeteor.Type != MeteorType.Small)
                    {
                        Meteor firstMeteor;
                        Meteor secondMeteor;
                        hittingMeteor.ReturnTwoSmallerMeteorsAfterHit(boundaries, random, hittingMeteor, random.Next(9), out firstMeteor, out secondMeteor);
                        meteors.Add(firstMeteor);
                        meteors.Add(secondMeteor);
                    }
                    Score += hittingMeteor.Score;
                    meteors.Remove(hittingMeteor);
                    CreateNewPlayerShipOrFinishGame();
                }
            }
        }

        private void CreateNewPlayerShipOrFinishGame()
        {
            if (!forceshieldActive)
            {
                shipHitted = true;
                playerShipesLeft--;
                if (playerShipesLeft >= 0)
                {
                    ActivateTemporaryForceshield();
                    playerShip = new Ship(boundaries);
                }
                else
                    OnGameOver(new EventArgs());
            }
        }

        private void CheckForAlienPlayerCollision()
        {
            if (alienShip != null && playerShip != null && alienShip.Area.Contains(playerShip.Location))
            {
                Score += alienShip.Score;
                shipHitted = true;
                alienShip = null;
                CreateNewPlayerShipOrFinishGame();
            }
        }

        private void CheckForMeteorHittedByAlienShips()
        {
            if (alienShot != null)
            {
                for (int j = meteors.Count - 1; j >= 0; j--)
                {
                    bool hitFound = false;
                    for (int k = meteors[j].HitZones.Count() - 1; k >= 0; k--)
                    {
                        if (meteors[j].HitZones[k].Contains(alienShot.Location))
                        {
                            Meteor hitMeteor = meteors[j];
                            if (hitMeteor.Type != MeteorType.Small)
                            {
                                Meteor firstMeteor;
                                Meteor secondMeteor;
                                hitMeteor.ReturnTwoSmallerMeteorsAfterHit(boundaries, random, hitMeteor, k, out firstMeteor, out secondMeteor);
                                meteors.Add(firstMeteor);
                                meteors.Add(secondMeteor);
                            }
                            meteors.Remove(hitMeteor);
                            alienShot = null;
                            hitFound = true;
                            break;
                        }
                    }
                    if (hitFound)
                        break;
                }
            }
        }

        private void MoveAllMeteors()
        {
            foreach (Meteor meteor in meteors)
                meteor.Move();
        }

        private void MovePlayerShots()
        {
            for (int i = shots.Count - 1; i >= 0; i--)
            {
                if (shots[i].Move())
                    shots.Remove(shots[i]);
            }
        }

        private void ManagePlayerForceshield()
        {
            if (forceshieldActiveCounter > 0)
                forceshieldActiveCounter--;
            else
                forceshieldActive = false;
        }

        private void FireOrMoveAlienShots()
        {
            if (alienShot == null)
            {
                if (alienShip != null)
                    alienShot = alienShip.FireShot(playerShip);
            }
            else
            {
                if (alienShot.Move())
                    alienShot = null;
            }
        }

        private bool CheckForPlayerMeteorCollision(out Meteor hittingMeteor)
        {
            if (playerShip != null)
            {
                for (int i = meteors.Count - 1; i >= 0; i--)
                {
                    for (int j = playerShip.ShipPoints.Count() - 1; j >= 0; j--)
                    {
                        if (meteors[i].Area.Contains(playerShip.ShipPoints[j]))
                        {
                            hittingMeteor = meteors[i];
                            return true;
                        }
                    }
                }
            }
            hittingMeteor = null;
            return false;
            
        }

        private void CheckForMeteorsHittedByPlayer()
        {
            for (int i = shots.Count - 1; i >= 0; i--)
            {
                for (int j = meteors.Count - 1; j >= 0; j--)
                {
                    bool hitFound = false;
                    for (int k = meteors[j].HitZones.Count() - 1; k >= 0; k--)
                    {
                        if (meteors[j].HitZones[k].Contains(shots[i].Location))
                        {
                            Meteor hitMeteor = meteors[j];
                            if (hitMeteor.Type != MeteorType.Small)
                            {
                                Meteor firstMeteor;
                                Meteor secondMeteor;
                                hitMeteor.ReturnTwoSmallerMeteorsAfterHit(boundaries, random, hitMeteor, k, out firstMeteor, out secondMeteor);
                                meteors.Add(firstMeteor);
                                meteors.Add(secondMeteor);
                            }
                            Score += hitMeteor.Score;
                            meteors.Remove(hitMeteor);
                            shots.Remove(shots[i]);
                            hitFound = true;
                            break;
                        }
                    }
                    if(hitFound)
                        break;
                }
            }
        }

        public void FireShot()
        {
            if (playerShip != null)
            {
                if (shots.Count < numberOfPlayerActiveShots)
                {
                    if (doubleFire)
                    {
                        Shot shot = new Shot(playerShip.DoubleCannonRightPosition, boundaries, playerShip.Angle);
                        shots.Add(shot);
                        shot = new Shot(playerShip.DoubleCannonLeftPosition, boundaries, playerShip.Angle);
                        shots.Add(shot);
                    }
                    else if (troubleFire)
                    {
                        Shot shot = new Shot(playerShip.FrontCannonPosition, boundaries, playerShip.Angle);
                        shots.Add(shot);
                        shot = new Shot(playerShip.DoubleCannonRightPosition, boundaries, playerShip.Angle);
                        shots.Add(shot);
                        shot = new Shot(playerShip.DoubleCannonLeftPosition, boundaries, playerShip.Angle);
                        shots.Add(shot);
                    }
                    else
                    {
                        Shot shot = new Shot(playerShip.FrontCannonPosition, boundaries, playerShip.Angle);
                        shots.Add(shot);
                    }
                }
            }
        }

        public void RotateShip(Direction direction)
        {
            playerShip.Rotate(direction);
        }

        public void SpeedUpShip()
        {
            playerShip.SpeedUp();
        }

        protected virtual void OnGameOver(EventArgs e)
        {
            if (GameIsOver != null)
                GameIsOver(this, e);
        }
    }
}
