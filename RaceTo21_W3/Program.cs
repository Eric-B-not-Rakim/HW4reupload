using System;

namespace RaceTo21
{
    class Program
    {
        static public void Main(string[] args)
        {
            CardTable cardTable = new CardTable();
            Game game = new Game(cardTable);
            while (game.nextTask != "GameOver")
            {
                game.DoNextTask();
            }
        }

        // TRACKING TOTAL POINTS:
        // Because this keeps track of the overall structure of the game,
        // I figure I can track total points or high scores for each player here.
        // Otherwise, players will get reset every game, as will their total scores.
    }
}

