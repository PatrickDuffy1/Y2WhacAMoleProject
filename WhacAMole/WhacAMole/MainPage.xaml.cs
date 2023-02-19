using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WhacAMole
{
    public partial class MainPage : ContentPage
    {

        Random rnd;
        const int maxMoles = 3;
        const int totalHighScores = 5;
        int _countDown;
        int milliseconds = 0;
        int currentGridSize = 4; // 4x4 grid or 5x5 grid
        int currentMole = 0;
        int timeLimit = 30; // amount of time until game ends in seconds
        int moleAppearTime = 2000; // Initial time between moles apperaing in milliseconds
        int moleAppear = 0; // mole appears when moleAppear is greater than moleAppearTime
        int[] molesX = new int[maxMoles]; // list of all mole x positions
        int[] molesY = new int[maxMoles]; // list of all mole y positions
        int currentScore = 0;
        int[] highScore4x4 = new int[totalHighScores]; // list of high scores on the 4x4 grid
        int[] highScore5x5 = new int[totalHighScores]; // list of high scores on the 4x4 grid
        bool gameIsActive = false;

        ImageButton[] moles = new ImageButton[maxMoles]; // array of moles
        public MainPage()
        {
            InitializeComponent();
            rnd = new Random();
            _countDown = timeLimit;
            SetUpMyTimers();
        }

        // Set up moles at start of game
        private void SetMoles()
        {
            for (int i = 0; i < maxMoles; i++)
            {
                moles[i] = new ImageButton();

                moles[i].Source = ImageSource.FromFile("mole2.png");
                moles[i].WidthRequest = 80;
                //moles[i].HeightRequest = 50;
                moles[i].HorizontalOptions = LayoutOptions.Center;
                moles[i].VerticalOptions = LayoutOptions.Center;
                moles[i].Clicked += ImgBtnMole_Clicked;

                // Sets mole[i] as child of 4x4 grid or 5x5 grid
                if (currentGridSize == 4)
                {
                    moles[i].HeightRequest = 60;
                    GridMoles4.Children.Add(moles[i]);
                    DisplayGridSize.Text = "4 x 4";
                }
                else if (currentGridSize == 5)
                {
                    moles[i].HeightRequest = 50;
                    GridMoles5.Children.Add(moles[i]);
                    DisplayGridSize.Text = "5 x 5";
                }

                moles[i].IsVisible = false;
                GameStatus.IsVisible = true;

                currentScore = 0;
                LblCountDown.Text = timeLimit.ToString();
                LblScore.Text = currentScore.ToString();

                gameIsActive = true;
            }
        }

        private void SetUpMyTimers()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(100),
                                () => {
                                    Device.BeginInvokeOnMainThread(
                                        () => { TimerFunctions(); });
                                    return true;
                                });
        }

        private void TimerFunctions()
        {
            if (gameIsActive == true)
            {
                if (_countDown <= 0)
                {
                    // Hides moles if the time is less than or equal to 0

                    HideMoles();
                    checkIfHighScore();
                    gameIsActive = false;
                }
                else
                {
                    milliseconds += 100;
                    moleAppear += 100;

                    // mole appears when moleAppear is greater than moleAppearTime
                    if (moleAppear > moleAppearTime)
                    {
                        MoveTheMole(currentMole);
                        currentMole++;
                        moleAppear = 0;

                        if (currentMole >= maxMoles)
                        {
                            currentMole = 0;
                        }
                    }

                    // Every second sets timer
                    if (milliseconds % 1000 == 0)
                    {
                        _countDown--;
                        LblCountDown.Text = _countDown.ToString();

                        // Every second reduces time between moles appearing. Minimum time between moles appearing is 300ms
                        if (moleAppearTime > 300)
                        {
                            moleAppearTime -= 100;
                        }
                    }
                }
            }

        }

        private void HideMoles()
        {
            for (int i = 0; i < maxMoles; i++)
            {
                moles[i].IsVisible = false;
            }
        }

        private void BtnSwitch_Clicked(object sender, EventArgs e)
        {
            switch (BtnSwitch.Text)
            {
                case "4x4":
                    reset4x4();
                    break;

                case "5x5":
                    reset5x5();
                    break;
            }
        }

        // Displays high scores for 3x3 and 5x5 grid
        private void setHighScores()
        {
            HighScores4x4Display.Text = "";
            HighScores5x5Display.Text = "";

            for (int i = 0; i < totalHighScores; i++)
            {
                HighScores4x4Display.Text += (i + 1) + ".  " + highScore4x4[i] + "\n";
                HighScores5x5Display.Text += (i + 1) + ".  " + highScore5x5[i] + "\n";
            }
        }

        // Checks if most recent game was quicker than other highscores and updates array highscores if true
        private void checkIfHighScore()
        {
            bool newHighScore = false;

            for (int i = 0; i < totalHighScores && newHighScore == false; i++)
            {
                if (currentGridSize == 4 && currentScore > highScore4x4[i])
                {
                    for (int j = totalHighScores - 1; j > i; j--)
                    {
                        highScore4x4[j] = highScore4x4[j - 1];
                    }

                    highScore4x4[i] = currentScore;
                    newHighScore = true;
                }
                else if (currentGridSize == 5 && currentScore > highScore5x5[i])
                {
                    for (int j = totalHighScores - 1; j > i; j--)
                    {
                        highScore5x5[j] = highScore5x5[j - 1];
                    }

                    highScore5x5[i] = currentScore;
                    newHighScore = true;
                }
            }
        }

        private void reset4x4()
        {
            currentGridSize = 4;
            ResetTheGrid();
            SetMoles();
            GridMoles5.IsVisible = false;
            GridMoles4.IsVisible = true;
            GridMoles5.IsEnabled = false;
            GridMoles4.IsEnabled = true;
            BtnSwitch.Text = "5x5";
        }

        private void reset5x5()
        {
            currentGridSize = 5;
            ResetTheGrid();
            SetMoles();
            GridMoles4.IsVisible = false;
            GridMoles5.IsVisible = true;
            GridMoles4.IsEnabled = false;
            GridMoles5.IsEnabled = true;
            BtnSwitch.Text = "4x4";
        }

        private void ResetTheGrid()
        {
            LblScore.Text = "0";
            _countDown = timeLimit;
            milliseconds = 0;
            moleAppear = 0;
            moleAppearTime = 2000;

            if (BtnRestart.Text == "Restart")
            {
                HideMoles();
            }
        }

        private void ImgBtnMole_Clicked(object sender, EventArgs e)
        {
            if (gameIsActive == true)
            {
                currentScore += 10;
                LblScore.Text = currentScore.ToString();

                ImageButton imageButton = (ImageButton)sender;
                imageButton.IsVisible = false;
            }
        }

        private void BtnTest_Clicked(object sender, EventArgs e)
        {
            MoveTheMole(currentMole);
        }

        private void MoveTheMole(int currentMole)
        {
            int r = 0, c = 0;
            bool isOccupied = false;

            // Keeps looping until the moles new position is not occupied
            do
            {
                r = rnd.Next(0, currentGridSize);
                c = rnd.Next(0, currentGridSize);

                isOccupied = false;

                // Checks if the selected position is occupied
                for (int i = 0; i < maxMoles; i++)
                {
                    if (molesX[i] == r && molesY[i] == c)
                    {
                        isOccupied = true;
                    }
                }

            } while (isOccupied);

            moles[currentMole].SetValue(Grid.RowProperty, r);
            moles[currentMole].SetValue(Grid.ColumnProperty, c);

            molesX[currentMole] = r;
            molesY[currentMole] = c;

            moles[currentMole].IsVisible = true;
            moles[currentMole].IsEnabled = true;
        }

        private void BtnHighScores_Clicked(object sender, EventArgs e)
        {
            if (BtnHighScores.Text == "High Scores")
            {
                setHighScores();
                HighscoreDisplay.IsVisible = true;
                HighscoreDisplay.IsEnabled = true;
                GridMoles5.IsVisible = false;
                GridMoles4.IsVisible = false;
                GridMoles5.IsEnabled = false;
                GridMoles4.IsEnabled = false;
                GameStatus.IsVisible = false;
                BtnSwitch.IsVisible = false;
                BtnRestart.IsVisible = false;
                BtnHighScores.Text = "Hide High Scores";
            }
            else if (BtnHighScores.Text == "Hide High Scores")
            {
                reset4x4();
                HighscoreDisplay.IsVisible = false;
                HighscoreDisplay.IsEnabled = false;
                BtnSwitch.IsVisible = true;
                BtnRestart.IsVisible = true;
                BtnHighScores.Text = "High Scores";
            }
        }

        private void BtnRestart_Clicked(object sender, EventArgs e)
        {
            switch (currentGridSize)
            {
                case 4:
                    reset4x4();
                    break;

                case 5:
                    reset5x5();
                    break;
            }

            BtnRestart.Text = "Restart";
        }
    }
}
