using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace BeadandoWinForms
{
    public partial class Form1 : Form
    {
        static System.Windows.Forms.Timer patternRevealTimer = new System.Windows.Forms.Timer(); 
        private string indexer = "0";
        private string color;
        private List<int> pattern = new List<int>();
        private int playerGuessRight = 0;
        private int numberOfErrors = 0;
        private int timeToReveal = 0;
        private int difficulty = 0;
        private int totalBadGuess = 0;
        private const string endGame = "Game Over!";
        private const string playerConceded = "You gave up.\n Want a new game?";
        private const string playerReachedErrorLimit = "Number of bad guesses has been reached. \n Want a new game?";
        private const string playerWon = "You managed to memorize the pattern. Good Job!\n Do you want to go harder?";
        private readonly int[] timeToMemorize = { 15, 20, 25, 23, 21 };
        private readonly int[] numberofPBToReveal = { 5, 6, 7, 9, 12 }; // 10, 18, 25, 32, 41
        private readonly int[] numbOfBadGuessOnDiff = { 4, 4, 3, 3, 2 };
        private DialogResult result;
        private bool easterEggLoop = false;
        public Form1()
        {
            InitializeComponent();
            
        }
        #region GameLogic
        private void GeneratePattern(int numberofPB)
        {
            int index = 0;
            int randNumb;
            Random rnd = new Random();
            do
            {
                randNumb = rnd.Next(1, 82);
                if (!pattern.Contains(randNumb))
                {
                    pattern.Add(randNumb);
                    index++;
                }
            } while (index < numberofPB);
        }

        private void ShowPatternForPlayer()
        {
            patternRevealTimer.Interval = 1000;
            patternRevealTimer.Start();
            timeToReveal = timeToMemorize[difficulty];
            patternRevealTimer.Tick += patternRevealTimer_Tick;
            foreach (int index in pattern)
            {
                indexer = "pb_Hard2_" + index.ToString();
                color = "LawnGreen";
                ChangeColorOfPB(indexer, color);
            }
            
        }
        private void patternRevealTimer_Tick(object sender, EventArgs e)
        {
            lbl_Time.Text = timeToReveal.ToString();
            if (timeToReveal <= 0)
            {
                patternRevealTimer.Stop();
                color = "Silver";
                for (int i = 1; i <= 81; i++)
                {
                    indexer = "pb_Hard2_" + i.ToString();
                    ChangeColorOfPB(indexer, color);
                    lbl_Time.Hide();
                }
            }
            timeToReveal--;
        }
        private void DisablePBClick()
        {
            for (int i = 1; i <= 81; i++)
            {
                Control[] controls = this.Controls.Find("pb_Hard2_" + i.ToString(), true);
                foreach (Control control in controls)
                {
                    if (control.GetType() == typeof(PictureBox))
                    {
                        PictureBox pictureBox = control as PictureBox;
                        pictureBox.Enabled = false;
                        pictureBox.Cursor = System.Windows.Forms.Cursors.Default;
                    }
                }
            }
            foreach (int index in pattern)
            {
                indexer = "pb_Hard2_" + index.ToString();
                color = "LawnGreen";
                ChangeColorOfPB(indexer, color);
            }
        }
        private void newGame() 
        {
            btn_start.Enabled = true;
            btn_concede.Enabled = false;
            lbl_Time.Show();
            playerGuessRight = 0;
            numberOfErrors = 0;
            patternRevealTimer.Tick -= patternRevealTimer_Tick;
            pattern.Clear();
        }

        #endregion

        #region ColorChanger
        private void ChangeColorOfPB(string pb_Index, string color)
        {
            Control[] controls = this.Controls.Find(pb_Index, true);
            if (controls != null && controls.Length > 0)
            {
                foreach (Control control in controls)
                {
                    if (control.GetType() == typeof(PictureBox))
                    {
                        PictureBox pictureBox = control as PictureBox;
                        if (color.Equals("Silver"))
                        {
                            pictureBox.BackColor = Color.FromName(color);
                            pictureBox.Enabled = true;
                            pictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
                        }
                        else if (color.Equals("Green") || color.Equals("Red"))
                        {
                            pictureBox.BackColor = Color.FromName(color);
                            pictureBox.Enabled = false;
                        }
                        else
                        {
                            pictureBox.BackColor = Color.FromName(color);
                        }
                    }
                }
            }
        }
        #endregion

        private void btn_start_Click(object sender, EventArgs e)
        {
            color = "silver";
            for (int i = 1; i <= 81; i++)
            {
                indexer = "pb_Hard2_" + i.ToString();
                ChangeColorOfPB(indexer, color);
            }
            lbl_diff.Text = (difficulty + 1).ToString();
            lbl_Error.Text = "0";
            lbl_Errors.Text = "/"+(numbOfBadGuessOnDiff[difficulty]).ToString();
            lbl_playerFound.Text = "0";
            lbl_patternCount.Text = "/" + (numberofPBToReveal[difficulty]).ToString();
            GeneratePattern(numberofPBToReveal[difficulty]);
            ShowPatternForPlayer();
            btn_start.Enabled = false;
            btn_concede.Enabled = true;
        }

        private void pb_Click(object sender, EventArgs e)
        {
            int tag =Convert.ToInt32(((PictureBox)sender).Tag);
            indexer = "pb_Hard2_" + tag.ToString();
            if (pattern.Contains(tag)) 
            {
                color = "Green";
                playerGuessRight++;
                lbl_playerFound.Text = playerGuessRight.ToString();
            }
            else
            {
                color = "Red";
                numberOfErrors++;
                totalBadGuess++;
            }
            ChangeColorOfPB(indexer,color);
            lbl_Error.Text = numberOfErrors.ToString();
            if (numberOfErrors > numbOfBadGuessOnDiff[difficulty])
            {
                DisablePBClick();
               result = MessageBox.Show(playerReachedErrorLimit, endGame, MessageBoxButtons.YesNo);
                if(result == System.Windows.Forms.DialogResult.Yes) 
                {
                    difficulty = 0;
                    DisablePBClick();
                    newGame();
                }
                else
                {
                    this.Close();
                }
            }

            if (difficulty == timeToMemorize.Length-1 && pattern.Count == playerGuessRight)
            {
                string gameFinished = "WOW. You managed to complete all patterns.\n " +
                    "Total number of bad guesses: " + totalBadGuess;
                MessageBox.Show(gameFinished, endGame);
                this.Close();
                
            }

            if (pattern.Count == playerGuessRight && pattern.Count == numberofPBToReveal[difficulty])
            {
                result = MessageBox.Show(playerWon, endGame, MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    difficulty++;
                    DisablePBClick();
                    newGame();
                }
                else
                {
                    this.Close();
                }
            }
        }


        private void btn_concede_Click(object sender, EventArgs e)
        {
            DisablePBClick();
           result = MessageBox.Show(playerConceded,endGame, MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                difficulty = 0;
                DisablePBClick();
                newGame();
            }
            else { this.Close(); }
        }

        private void pb_easterEgg_Click(object sender, EventArgs e)
        {
            if (easterEggLoop)
            {
                MessageBox.Show("You found a Easter Egg! \n Too bad you stuck in a loop!", "Easter Egg");
                pb_easterEgg_Click(sender, e);
            }
            else 
            {
                MessageBox.Show("You found a Easter Egg!", "Easter Egg");
            }

        }
    }
}
