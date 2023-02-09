using System;
using System.Collections.Generic;
using System.Linq;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
		CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public string nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
            nextTask = "GetNumberOfPlayers";
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

		/* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         */
		public enum Task
		{
			GetNumberOfPlayers,
			GetNames,
			IntroducePlayers,
			PlayerTurn,
			CheckForEnd,
			GameOver
		}
		public Task currentTask = Task.GetNumberOfPlayers;


		public void DoNextTask()
		{
			Console.WriteLine("================================"); // this line should be elsewhere right?
			if (currentTask == Task.GetNumberOfPlayers)
			{
				numberOfPlayers = cardTable.GetNumberOfPlayers();
				currentTask = Task.GetNames;
			}
			else if (currentTask == Task.GetNames)
			{
				for (var count = 1; count <= numberOfPlayers; count++)
				{
					var name = cardTable.GetPlayerName(count);
					AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
				}
				currentTask = Task.IntroducePlayers;
			}
			else if (currentTask == Task.IntroducePlayers)
			{
				cardTable.ShowPlayers(players);
				currentTask = Task.PlayerTurn;
			}
			else if (currentTask == Task.PlayerTurn)
			{
				cardTable.ShowHands(players);
				Player player = players[currentPlayer];
				if (player.status == PlayerStatus.active)
				{
					if (cardTable.OfferACard(player))
					{
						string card = deck.DealTopCard();
						player.cards.Add(card);
						player.score = ScoreHand(player);
						if (player.score > 21)
						{
							player.status = PlayerStatus.bust;
						}
						else if (player.score == 21)
						{
							player.status = PlayerStatus.win;
						}
                        /* UGH
                        else if ((players.Count) == PlayerStatus.bust)
                        {
                            player.status = PlayerStatus.win;
                        }
                        */
					}
					else
					{
						player.status = PlayerStatus.stay;
					}
				}
				cardTable.ShowHand(player);
				currentTask = Task.CheckForEnd;
			}
			else if (currentTask == Task.CheckForEnd)
			{
				if (!CheckActivePlayers())
				{
					Player winner = DoFinalScoring();
					cardTable.AnnounceWinner(winner);
					currentTask = Task.GameOver;
				}
				else
				{
					currentPlayer++;
					if (currentPlayer > players.Count - 1)
					{
						currentPlayer = 0; // back to the first player...
					}
					currentTask = Task.PlayerTurn;
				}
			}
			else // we shouldn't get here...
			{
				Console.WriteLine("I'm sorry, I don't know what to do now!");
				currentTask = Task.GameOver;
			}
		}

		public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                foreach (string card in player.cards)
                {
                    //string faceValue = card.Remove(card.Length - 1);
                    char i = card.First<char>();
                    string faceValue = Convert.ToString(i);
					switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
            }
            return score;
        }

        public bool CheckActivePlayers()
        {
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.win)
                {
                    return false;
                }
                else if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }
            }
            return false; // everyone has stayed or busted, or someone won!
        }

        public Player DoFinalScoring()
        {
            int highScore = 0;
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay) // still could win...
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                // if busted don't bother checking!
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }
            return null; // everyone must have busted because nobody won!
        }
    }
}
