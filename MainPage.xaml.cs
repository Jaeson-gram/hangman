using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.CompilerServices;

namespace HangMan;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    #region UI Properties

    public string Spotlight
    {
        get => spotlight;

        set
        {
            spotlight = value;

            OnPropertyChanged();
        }
    }

    public List<char> Alphabets
    {
        get => alphabets;

        set
        {
            alphabets = value;

            OnPropertyChanged();
        }
    }


    public string Message
    {
        get => message;

        set
        {
            message = value;

            OnPropertyChanged();
        }
    }

    public string GameStatus
    {
        get => gameStatus;

        set
        {
            gameStatus = value;

            OnPropertyChanged();
        }
    }

    public string CurrentImage
    {
        get => currentImage; 
        
        set
        {
            currentImage = value;

            OnParentChanged();
        }
    }
    #endregion

    #region Fields
    List<string> wordsToGuess = new()
    {
        "Jesus",
        "HolySpirit",
        "Daniel",
        "David",
        "Bible",
        "Elohim",
        "IAM",
    };

    string answer = "";

    private string spotlight;

    //characters that the user will input while guessing
    List<char> guessed = new();
    private List<char> alphabets = new();
    private string message;
    int mistakes = 0;
    int maxMistakes = 6;
    private string gameStatus;
    private string currentImage = "img0.jpg";

    #endregion


    public MainPage()
    {
        InitializeComponent();

        Alphabets.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

        //
        BindingContext = this;

        PickWord();

        CalculateWord(answer, guessed);
    }


    #region Game Engine

    //method that picks a random word from the list of words. The user will be asked to guess what this word is
    private void PickWord()
    {
        answer = wordsToGuess[new Random().Next(0, wordsToGuess.Count)];

        Debug.WriteLine(answer);
    }

    private void CalculateWord(string answer, List<char> guessed)
    {
        //If the user guessed an alphabet that is available in the word we want them to guess, then return the alphabet, otherwise just return _, which is literallly the same character there..
        var temp = answer.Select(x => (guessed.IndexOf(x) >= 0 ? x : '_')).ToArray();

        //pass the answer to the spotlight variable.. separated by a space
        Spotlight = string.Join(' ', temp);
    }

     private void HandleGuess(char alphabet)
     {
        /* if it has not already been clicked.. */
        if (guessed.IndexOf(alphabet) == -1)
        {
            guessed.Add(alphabet);
        }

        /*  if it's part of the letter we want them to guess.. */
        if (answer.IndexOf(alphabet) >= 0)
        {
            CalculateWord(answer, guessed);

            CheckIfGameWon();
        }

        /* if it's not a letter in the word we want to guess.. */
        else if (answer.IndexOf(alphabet) == -1)
        {
            mistakes++;

            UpdateStatus();

            CheckIfGameLost();

            CurrentImage = $"img{mistakes}.jpg";    /* change the image to the next lost step */
        }
     }

    /* method to check if the game is lost already */
    private void CheckIfGameLost()
    {
        if (mistakes == maxMistakes)
        {
            Message = "You Lost!";

            DisableButtons();
        }
    }

    /* method to disable all the buttons.. we'll use this method when the user wins or loses */
    private void DisableButtons()
    {
        foreach (var child in alphabetsContainer.Children)
        {
            var btn = child as Button;

            if (btn is not null)
            {
                btn.IsEnabled = false;
            }
        }
    }

    /* method to enable all the buttons.. we'll use this method when the user resets the game*/
    private void EnableButtons()
    {
        foreach (var child in alphabetsContainer.Children)
        {
            var btn = child as Button;

            if (btn is not null)
            {
                btn.IsEnabled = true;
            }
        }
    }

    /* method to check if the game is won already */
    private void CheckIfGameWon()
    {
        if (Spotlight.Replace(" ", "") == answer)
        {
            Message = "You Win!";

            DisableButtons();
        }
    }

    /* method to update the status of the game (let the user know how many tries the they have left) */

    private void UpdateStatus()
    {
        GameStatus = $"Errors: {mistakes} of {maxMistakes}";
    }

    #endregion

    /* event handler for when any button is clicked. That button is disabled and its value is sent to the HandleGuess method above to check if it's a part of the letters of the word to be guessed */

    private void Button_Clicked(object sender, EventArgs e)
    {
        var btn = sender as Button;

        if (btn is not null)
        {
            var alphabet = btn.Text;

            btn.IsEnabled = false;

            HandleGuess(alphabet[0]);
        }
    }

    
    /* Event handler to reset the game when the 'Reset' button is clicked */
    private void Reset_Button_Clicked(object sender, EventArgs e)
    {
        mistakes = 0;
        guessed = new List<char>();
        CurrentImage = "img0.jpg";
        PickWord();
        CalculateWord(answer, guessed);
        Message = "";
        UpdateStatus();
        EnableButtons();
    }
}

