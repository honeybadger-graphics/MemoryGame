using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BeadandoWinForms
{
    public partial class Form1 : Form
    {
        static System.Windows.Forms.Timer patternRevealTimer = new System.Windows.Forms.Timer();
        private string pb_name = "0";  // Variable for PictureBox name  
        private string color;          // Name of a color
        private List<int> Pattern = new List<int>(); // List to store PB for player to find
        private int playerGuessRight = 0; // Counter for found PBs
        private int numberOfErrors = 0; // Counter for errors
        private int timeToReveal = 0; // Counter for revealed time
        private int difficulty = 0; // Difficulty counter
        private int totalBadGuess = 0; // Player's total number of bad guesses
        private const string endGame = "Game Over!";
        private const string playerConceded = "You gave up.\n Want a new game?";
        private const string playerReachedErrorLimit = "Number of bad guesses has been reached. \n Want a new game?";
        private const string playerWon = "You managed to memorize the pattern. Good Job!\n Do you want to go harder?";
        private readonly int[] timeToMemorize = { 15, 20, 25, 23, 22 }; // Time to be revealed for the difficulty
        private readonly int[] numberofPBToReveal = { 5, 6, 7, 9, 12 }; // Numbers to generate at certain difficulty
        private readonly int[] numbOfBadGuessOnDiff = { 4, 4, 3, 3, 2 }; // Number of errors at certain difficulty
        private DialogResult result;

        public Form1()
        {
            InitializeComponent();

        }
        #region GameLogic
        /// <summary>
        /// Generates a random pattern for the player to be memorized.
        /// </summary>
        /// <param name="numberofPB">How many things the player find. </param>
        private void GeneratePattern(int numberofPB)
        {
            int index = 0;
            int randNumb;
            Random rnd = new Random();
            do
            {
                randNumb = rnd.Next(1, 82);
                if (!Pattern.Contains(randNumb))
                {
                    Pattern.Add(randNumb);
                    index++;
                }
            } while (index < numberofPB);
        }

        /// <summary>
        /// Shows the pattern for the player.
        /// </summary>
        private void ShowPatternForPlayer()
        {
            
            patternRevealTimer.Interval = 1000;
            patternRevealTimer.Start();
            timeToReveal = timeToMemorize[difficulty];
            patternRevealTimer.Tick += PatternRevealTimer_Tick;
            lbl_Time.Show();
            foreach (int index in Pattern)
            {
                pb_name = "pb_Hard2_" + index.ToString();
                color = "LawnGreen";
                ChangeColorOfPB(pb_name, color);
            }

        }
        /// <summary>
        /// Decreases the time for the player to memorize the pattern.
        /// If the time reaches zero it hides the pattern and enables the PBs.
        /// </summary>
        private void PatternRevealTimer_Tick(object sender, EventArgs e)
        {
            if (timeToReveal <= 0)
            {
                patternRevealTimer.Stop();
                color = "Silver";
                for (int i = 1; i <= 81; i++)
                {
                    pb_name = "pb_Hard2_" + i.ToString();
                    ChangeColorOfPB(pb_name, color);
                }
                pnl_game_hard_9x9.Enabled = true;
                lbl_Time.Hide();
            }
            lbl_Time.Text = timeToReveal.ToString();
            timeToReveal--;
        }
        /// <summary>
        /// Shows the last pattern to the player. Prevents unwandted clicks.
        /// </summary>
        private void DisablePBClick()
        {
            foreach (int index in Pattern)
            {
                pb_name = "pb_Hard2_" + index.ToString();
                color = "LawnGreen";
                ChangeColorOfPB(pb_name, color);
            }
            pnl_game_hard_9x9.Enabled = false;
            
        }
        /// <summary>
        /// Set up for a new game or higher difficulty.
        /// </summary>
        private void NewGame()
        {
            btn_start.Enabled = true;
            btn_concede.Enabled = false;
            color = "silver";
            for(int i = 1; i<=81; i++)
            {
                pb_name = "pb_Hard2_" + i.ToString();
                ChangeColorOfPB(pb_name, color);
            }
        }

        #endregion

        #region ColorChanger
        /// <summary>
        /// If a box is clicked it changes its color. If in game it changes according to the pattern. 
        /// If its correct then green if not then red then changes the right counters.
        /// </summary>
        /// <param name="pb_Index"> Name of the box that should change. </param>
        /// <param name="color"> Background color of the box to be changed into. </param>
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

        /// <summary>
        ///  Resets certain counters and the pattern then starts the game.
        /// </summary>
        private void BTN_start_Click(object sender, EventArgs e)
        {
            Pattern.Clear();
            playerGuessRight = 0;
            numberOfErrors = 0;
            timeToReveal = 0;
            patternRevealTimer.Tick -= PatternRevealTimer_Tick;
            lbl_diff.Text = (difficulty + 1).ToString();
            lbl_Error.Text = "0";
            lbl_Errors.Text = "/" + (numbOfBadGuessOnDiff[difficulty]).ToString();
            lbl_playerFound.Text = "0";
            lbl_patternCount.Text = "/" + (numberofPBToReveal[difficulty]).ToString();
            GeneratePattern(numberofPBToReveal[difficulty]);
            ShowPatternForPlayer();
            btn_start.Enabled = false;
            btn_concede.Enabled = true;
        }

        /// <summary>
        /// Things to do if a certain PB was clicked. Manages Win/Lose condition.
        /// </summary>
        private void PB_Click(object sender, EventArgs e)
        { 
            int tag = Convert.ToInt32(((PictureBox)sender).Tag);
            pb_name = "pb_Hard2_" + tag.ToString();
            if (Pattern.Contains(tag))  //If its right
            {
                color = "Green";
                playerGuessRight++;
                lbl_playerFound.Text = playerGuessRight.ToString();
            }
            else    //If its wrong
            {
                color = "Red";
                numberOfErrors++;
                totalBadGuess++;
            }
            ChangeColorOfPB(pb_name, color);
            lbl_Error.Text = numberOfErrors.ToString();
            if (numberOfErrors > numbOfBadGuessOnDiff[difficulty]) // Number of errors met
            {
                DisablePBClick();
                result = MessageBox.Show(playerReachedErrorLimit, endGame, MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    WriteToFile("Failed", totalBadGuess, difficulty);
                    totalBadGuess = 0;
                    difficulty = 0;
                    NewGame();
                    
                }
                else
                {
                    WriteToFile("Failed", totalBadGuess, difficulty);
                    this.Close();
                }
            }

            if (numberofPBToReveal[difficulty] == playerGuessRight && difficulty != numberofPBToReveal.Length - 1) // Difficulty completed and player won.
            {
                result = MessageBox.Show(playerWon, endGame, MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    difficulty++;
                    DisablePBClick();
                    NewGame();
                }
                else
                {
                    WriteToFile("Level Completed", totalBadGuess, difficulty);
                    this.Close();
                }
            }

            if (numberofPBToReveal[difficulty] == playerGuessRight) //Player completed the game
            {
                string gameFinished = "WOW. You managed to complete all patterns.\n " +
                    "Total number of bad guesses: " + totalBadGuess;
                MessageBox.Show(gameFinished, endGame);
                WriteToFile("Game Completed", totalBadGuess, difficulty);
                this.Close();

            }
        }


        /// <summary>
        /// Player Concedes the game.
        /// </summary>
        private void BTN_concede_Click(object sender, EventArgs e)
        {
            DisablePBClick();
            lbl_Time.Hide();
            WriteToFile("Conceded", totalBadGuess, difficulty);
            result = MessageBox.Show(playerConceded, endGame, MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                difficulty = 0;
                totalBadGuess = 0;
                NewGame();
            }
            else { this.Close(); }
        }

        /// <summary>
        /// Easter Egg (randomly selected things to chose from)!
        /// </summary>
        private void PB_easterEgg_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            double randomEasterEgg = rnd.NextDouble();
            MessageBox.Show("You found a Easter Egg!", "Easter Egg");
            if (randomEasterEgg <= 0.33) 
            {
                System.Diagnostics.Process.Start("https://this-person-does-not-exist.com/en");
            }
            else if (0.33<randomEasterEgg && randomEasterEgg <= 0.66)
            {
                System.Diagnostics.Process.Start("https://hackertyper.net/");
            }
            else
            {
                System.Diagnostics.Process.Start("https://youtu.be/dQw4w9WgXcQ");
            }
        }
        /// <summary>
        /// Writes Game logs to file, can be used to track played games and other things.
        /// </summary>
        /// <param name="cond"> Condition of how the game ended (win, lose, give up, completed).</param> 
        /// <param name="errors"> Number of total errors when the game ended</param>
        /// <param name="diff"> Difficulty of the game when ended.</param>
        private void WriteToFile(string cond, int errors, int diff) 
        {
            
            string date = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "GameLog.txt"), true))
            {
                outputFile.WriteLine("Date: {0} \t Difficulty: {1} \t Condition: {2} \t TotalErrors: {3}", date, diff+1, cond, errors);
            }
        
        }
    }
}
