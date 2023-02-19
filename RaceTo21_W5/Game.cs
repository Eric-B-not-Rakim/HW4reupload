using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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
        int bustNumber = 0; // This is the number of players busted
        public bool firstGame = true; // is this the first game?
        private bool cheating = false; // lets you cheat for testing purposes if true

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
            //currentTask = Task.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

		public void PlayerShuffle()
		{
            // This is literally just the card shuffling method.
            // But for players.
            // I'm a programming genius.

			Console.WriteLine("Shuffling Players...");

			Random rng = new Random();

			for (int i = 0; i < players.Count; i++)
			{
				Player tmp = players[i];
				int swapindex = rng.Next(players.Count);
				players[i] = players[swapindex];
				players[swapindex] = tmp;
			}
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
                if (firstGame == false)
                {
                    currentTask = Task.IntroducePlayers; // If this isn't our first rodeo, hypothetically we can go to IntroducePlayers
                }
                else
                {
					numberOfPlayers = cardTable.GetNumberOfPlayers();
					currentTask = Task.GetNames;
				}
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
				bustNumber = 0;
				PlayerShuffle();
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
                        int amount = cardTable.HowManyCards();
                        for (int i = 0; i < amount; i++)
                        {
							string card = deck.DealTopCard();
							player.cards.Add(card);
						}
						player.score = ScoreHand(player);
						if (player.score > 21)
						{
							player.status = PlayerStatus.bust;
                            bustNumber++;
						}
						else if (player.score == 21)
						{
							player.status = PlayerStatus.win;
						}
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
                    cardTable.PlayAgain(players);
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
                    // if all but one player has busted:
                    if (bustNumber == (numberOfPlayers - 1))
                    {
						player.status = PlayerStatus.win; // remaining person wins!
						return false;
                    }
                    // otherwise, at least two players are still going!
                    else
                    {
                        return true;
                    }
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
