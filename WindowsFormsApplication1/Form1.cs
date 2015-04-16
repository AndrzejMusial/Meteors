using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeteorGame
{
    public partial class Form1 : Form
    {
        Game game;
        Random random;
        List<Keys> pressedKeys = new List<Keys>();
        bool gameOver;
        bool trust = false;
        int frame = 0;
        Rectangle monitorResolution = Screen.PrimaryScreen.WorkingArea;

        public Form1()
        {
            InitializeComponent();

            this.Size = new Size(monitorResolution.Width, monitorResolution.Height);
            random = new Random();
            game = new Game(this.ClientRectangle, random);
            gameOver = false;
            gameTimer.Enabled = true;
            animationTimer.Enabled = true;
            game.GameIsOver += new Game.GameOverEventHandler(GameOver);

            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.Bounds = Screen.PrimaryScreen.Bounds;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
                Application.Exit();

            if (e.KeyCode == Keys.F4)
                gameTimer.Stop();

            if (e.KeyCode == Keys.F5)
                gameTimer.Start();

            if (gameOver)
                if (e.KeyCode == Keys.S)
                {
                    gameOver = false;
                    game = new Game(this.ClientRectangle, random);
                    gameTimer.Start();
                }
            
            if (e.KeyCode == Keys.Space)
                game.FireShot();

            if (pressedKeys.Contains(e.KeyCode))
                pressedKeys.Remove(e.KeyCode);
            pressedKeys.Add(e.KeyCode);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (pressedKeys.Contains(e.KeyCode))
                pressedKeys.Remove(e.KeyCode);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (gameOver)
            {
                e.Graphics.FillRectangle(Brushes.CadetBlue, this.ClientRectangle);
                using (Font arial48bold = new Font("Arial", 48, FontStyle.Bold))
                using (Font arial24bold = new Font("Arial", 24, FontStyle.Bold))
                {
                    Size stringSize = Size.Ceiling(e.Graphics.MeasureString("GAME OVER", arial48bold));
                    e.Graphics.DrawString(
                        "GAME OVER",
                        arial48bold,
                        Brushes.Yellow,
                        new PointF(
                            this.ClientSize.Width / 2 - stringSize.Width / 2,
                            this.ClientSize.Height / 2 - stringSize.Height / 2));
                    stringSize = Size.Ceiling(e.Graphics.MeasureString("Naciśnij S, bay rozpocząć nową grę lub Q, aby zakończyć", arial24bold));
                    e.Graphics.DrawString(
                        "Naciśnij S, aby rozpocząć nową grę lub Q, aby zakończyć",
                        arial24bold, Brushes.Violet,
                        this.ClientSize.Width - stringSize.Width,
                        this.ClientSize.Height - stringSize.Height);
                }

            }
            else
            {
                game.Draw(e.Graphics, trust, frame);
            }
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            frame++;
            if (frame >= 3)
                frame = 0;
            Refresh();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            game.Go();
            trust = false;
            foreach (Keys key in pressedKeys)
            {
                if (key == Keys.Left)
                {
                    game.RotateShip(Direction.Counterclockwise);
                    return;
                }
                if (key == Keys.Right)
                {
                    game.RotateShip(Direction.Clockwise);
                    return;
                }
                if (key == Keys.Up)
                {
                    game.SpeedUpShip();
                    trust = true;
                    return;
                }

            }
        }

        private void GameOver(object sender, EventArgs e)
        {
            gameTimer.Stop();
            gameOver = true;
            Invalidate();
        }
    }
}
