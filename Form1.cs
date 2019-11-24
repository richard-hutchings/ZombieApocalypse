using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zombieShooter
{
    public partial class Form1 : Form
    {
        bool goup; //player movements
        bool godown;
        bool goleft;
        bool goright;
        string facing = "up"; // this string is used to guide the bullets
        double playerHealth = 100;
        int speed = 10;
        int ammo = 10;
        int zombieSpeed = 1;
        int score = 0; // score count
        bool gameOver = false; // check if game over
        Random rnd = new Random(); //an instance of a random class, used to create a random no.

        public Form1()
        {
            InitializeComponent();
        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            if (gameOver) return;
            // key left
            if(e.KeyCode == Keys.Left)
            {
                goleft = true;
                facing = "left";
                player.Image = Properties.Resources.left;
            }
            //key right
            if (e.KeyCode == Keys.Right)
            {
                goright = true;
                facing = "right";
                player.Image = Properties.Resources.right;
            }
            //key up
            if (e.KeyCode == Keys.Up)
            {
                goup = true;
                facing = "up";
                player.Image = Properties.Resources.up;
            }
            //key down
            if (e.KeyCode == Keys.Down)  
            {
                godown = true;
                facing = "down";
                player.Image = Properties.Resources.down;
            }


        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            if (gameOver) return;

            if(e.KeyCode == Keys.Left)
            {
                goleft = false;
            }

            if(e.KeyCode == Keys.Right)
            {
                goright = false;
            }

            if (e.KeyCode == Keys.Up)
            {
                goup = false;
            }

            if(e.KeyCode == Keys.Down)
            {
                godown = false;
            }
            // Spacebar used in keyUp function to reduce multiple ammo rounds.
            if(e.KeyCode == Keys.Space && ammo > 0)
            {
                ammo--; //lose a bullet
                shoot(facing);//shooting direction

                if(ammo < 1)
                {
                    DropAmmo(); //invokes drop ammo function
                }
            }

        }

        private void gameEngine(object sender, EventArgs e)
        {
            if (playerHealth > 1)
            {
                //assigns the progress bar to health
                progressBar1.Value = Convert.ToInt32(playerHealth);
            }
            else
            {
                //game over
                player.Image = Properties.Resources.dead;  //change player image to dead
                timer1.Stop();
                gameOver = true;
            }

            label1.Text = "     Ammo: " + ammo; //displays ammo count
            label2.Text = "Kills:  " + score;  //displays kill score

            if (playerHealth < 20)
            {
                //changes progress bar to red
                progressBar1.ForeColor = System.Drawing.Color.Red; 
            }
            //player go left
            if(goleft && player.Left > 0)
            {
                player.Left -= speed;
            }
            //player go right
            if (goright && player.Left + player.Width < 930)
            {
                player.Left += speed;
            }
            //player go up
            if (goup && player.Top > 60)
            {
                player.Top -= speed;
            }
            //player go down
            if (godown && player.Top + player.Height < 700)
            {
                player.Top += speed;
            }

            foreach (Control x in this.Controls)
            {

                //deals with the ammo
                if (x is PictureBox && x.Tag == "ammo")
                {
                    if (((PictureBox)x).Bounds.IntersectsWith(player.Bounds))
                    {
                        this.Controls.Remove(((PictureBox)x)); //removes ammo pictureBox

                        ((PictureBox)x).Dispose();
                        ammo += 5;  //increases ammo by 5
                    }
                }

                //deals with the bullet
                if(x is PictureBox && x.Tag == "bullet")
                {
                    //if the bullet hits the 4 sides of the game
                    if (((PictureBox)x).Left < 1 || ((PictureBox)x).Left > 930 || ((PictureBox)x).Top < 10
                        || ((PictureBox)x).Top > 700)
                    {
                        //removes bullet
                        this.Controls.Remove(((PictureBox)x));
                        ((PictureBox)x).Dispose();
                    }
                }

                //deals with the Zombies
                if(x is PictureBox && x.Tag == "zombie")
                {
                    if (((PictureBox)x).Bounds.IntersectsWith(player.Bounds))
                    {
                        playerHealth -= 1;  // takes a health point
                    }

                    if (((PictureBox)x).Left > player.Left)
                    {
                        //zombie left
                        ((PictureBox)x).Left -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zleft;
                    }

                    if (((PictureBox)x).Top > player.Top)
                    {
                        //Zombie top
                        ((PictureBox)x).Top -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zup;
                    }

                    if (((PictureBox)x).Left < player.Left)
                    {
                        //Zombie right
                        ((PictureBox)x).Left += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zright;
                    }

                    if (((PictureBox)x).Top < player.Top)
                    {
                        //zombie bottom
                        ((PictureBox)x).Top += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zdown;
                    }
                }

                foreach (Control j in this.Controls)
                {
                    if (( j is PictureBox && j.Tag == "bullet" ) && ( x is PictureBox && x.Tag == "zombie" ))
                    {
                         if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;
                            this.Controls.Remove(j); //removes bullet
                            j.Dispose();
                            this.Controls.Remove(x); // removes zombie
                            x.Dispose();
                            makeZombies(); // make another Zombie
                        }
                    }
                }
            }
        }

        private void DropAmmo()
        {
            //creates ammo 
            PictureBox ammo = new PictureBox(); //creates a new instance of picture box
            ammo.Image = Properties.Resources.ammo_Image; //sets ammo image
            ammo.SizeMode = PictureBoxSizeMode.AutoSize; 
            ammo.Left = rnd.Next(10, 890); // sets location to random lefft
            ammo.Top = rnd.Next(50, 600); //sets location to random top
            ammo.Tag = "ammo"; // sets ammo tag
            this.Controls.Add(ammo); // adds ammo picturebox to screen
            ammo.BringToFront(); 
            player.BringToFront();

        }

        private void shoot(string direct)
        {
            //creates bullets
            bullet shoot = new bullet(); //Creates new bullet instance
            shoot.direction = direct; //assigns direction to bullet
            shoot.bulletLeft = player.Left + (player.Width / 2); // places bullet to left half of player
            shoot.bulletTop = player.Top + (player.Height / 2); //places bullet to top half of player
            shoot.mkBullet(this); // runs mkbullet function from bullet class
        }

        private void makeZombies()
        {
            //makes zombies
            PictureBox zombie = new PictureBox();  //creates a new zombie instance
            zombie.Tag = "zombie"; //asigns the tag
            zombie.Image = Properties.Resources.zdown; //default zombie image
            zombie.Left = rnd.Next(0, 900);
            zombie.Top = rnd.Next(0, 800);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;
            this.Controls.Add(zombie); // adds zombie pictureBox to screen
            player.BringToFront();

        }

        private void player_Click(object sender, EventArgs e)
        {

        }
    }
}
