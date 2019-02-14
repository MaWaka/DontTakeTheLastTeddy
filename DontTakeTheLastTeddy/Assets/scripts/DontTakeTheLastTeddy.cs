using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Game manager
/// </summary>
public class DontTakeTheLastTeddy : MonoBehaviour
{
    Board board;
    Player player1;
    Player player2;

    //multiple games support
    int numGames = 10;
    Timer showScoreTimer;


    // events invoked by class
    TakeTurn takeTurnEvent = new TakeTurn();
    GameOver gameOverEvent = new GameOver();
    GameStarting gameStartingEvent = new GameStarting();

    /// <summary>
    /// Awake is called before Start
    /// </summary>
    void Awake()
    {
        // retrieve board and player references
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
        player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();

        // register as invoker and listener
        EventManager.AddTakeTurnInvoker(this);
        EventManager.AddGameOverInvoker(this);
        EventManager.AddGameStartingInvoker(this);
        EventManager.AddTurnOverListener(HandleTurnOverEvent);
    }

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start()
	{
        showScoreTimer = gameObject.AddComponent<Timer>();
        showScoreTimer.Duration = 1;

        //play the game a predetermined number of times
        int gameCounter = 0;
        bool playerOneGoesFirst = true;
        while (gameCounter < numGames)
        {
            gameStartingEvent.Invoke();
            if (playerOneGoesFirst)
            {
                StartGame(PlayerName.Player1, Difficulty.Easy, Difficulty.Hard);
            }
            else
            {
                StartGame(PlayerName.Player2, Difficulty.Easy, Difficulty.Hard);
            }

            //alternate which player goes first for each game
            playerOneGoesFirst = !playerOneGoesFirst;
            
            //pause to show score
            showScoreTimer.Run();
           

            gameCounter++;
        } 
        
    }

    /// <summary>
    /// Adds the given listener for the TakeTurn event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddTakeTurnListener(UnityAction<PlayerName, Configuration> listener)
    {
        takeTurnEvent.AddListener(listener);
    }

    /// <summary>
    /// Adds the given listener for the GameStarting event
    /// </summary>
    public void AddGameStartingListener(UnityAction listener)
    {
        gameStartingEvent.AddListener(listener);
    }

    /// <summary>
    /// Adds the given listener for the GameOver event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddGameOverListener(UnityAction<PlayerName> listener)
    {
        gameOverEvent.AddListener(listener);
    }

    /// <summary>
    /// Starts a game with the given player taking the
    /// first turn
    /// </summary>
    /// <param name="firstPlayer">player taking first turn</param>
    /// <param name="player1Difficulty">difficulty for player 1</param>
    /// <param name="player2Difficulty">difficulty for player 2</param>
    void StartGame(PlayerName firstPlayer, Difficulty player1Difficulty,
        Difficulty player2Difficulty)
    {
        // set player difficulties
        player1.Difficulty = player1Difficulty;
        player2.Difficulty = player2Difficulty;

        // create new board
        board.CreateNewBoard();
        takeTurnEvent.Invoke(firstPlayer,
            board.Configuration);
    }

    /// <summary>
    /// Handles the TurnOver event by having the 
    /// other player take their turn
    /// </summary>
    /// <param name="player">who finished their turn</param>
    /// <param name="newConfiguration">the new board configuration</param>
    void HandleTurnOverEvent(PlayerName player, 
        Configuration newConfiguration)
    {
        board.Configuration = newConfiguration;

        // check for game over
        if (newConfiguration.Empty)
        {
            // fire event with winner
            if (player == PlayerName.Player1)
            {
                gameOverEvent.Invoke(PlayerName.Player2);
            }
            else
            {
                gameOverEvent.Invoke(PlayerName.Player1);
            }
        }
        else
        {
            // game not over, so give other player a turn
            if (player == PlayerName.Player1)
            {
                takeTurnEvent.Invoke(PlayerName.Player2,
                    newConfiguration);
            }
            else
            {
                takeTurnEvent.Invoke(PlayerName.Player1,
                    newConfiguration);
            }
        }
    }
}
