using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Schema;

namespace RaceTo21
{
    public class CardTable
    {
		public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i + 1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }

        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();
            while (response.Length < 1)
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            return response;
        }

        public bool OfferACard(Player player)
        {
            while (true)
            {
                Console.Write(player.name + ", do you want a card? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        public int HowManyCards()
        {
			Console.WriteLine("How many cards? (1-3)");
			string r = Console.ReadLine();
			int.TryParse(r, out int amount);
            return amount;
		}

        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)
            {
                Console.Write(player.name + " has: ");
                //var lastCard = player.cards.Last();
                foreach (string card in player.cards)
                {
                    if (card == player.cards.Last()) // Checks if this is the last card in the hand
                    {
                        if (card == player.cards.First())
                        {
                            Console.Write("the " + card);
                        }
                        else
                        {
							Console.Write("and the " + card);
						}
					}
                    else
                    {
                        Console.Write("the " + card + ", "); // otherwise, continues the sentence
                    }
                }
                Console.Write(" = " + player.score + "/21 ");
                if (player.status != PlayerStatus.active)
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }

        public void AnnounceWinner(Player player)
        {
            if (player != null)
            {
                Console.WriteLine(player.name + " wins!");
            }
            else
            {
                Console.WriteLine("Everyone busted!");
            }
        }

        public void PlayAgain(List<Player> players)
        {
            int totalPlayers = players.Count;
            foreach (Player player in players)
            {
                Console.Write(player.name + ", do you want to play again? Y/N");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    player.score = 0;
                    player.cards.Clear();
                    Console.WriteLine("Great! See you soon!");
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    // If not, bye.
                    totalPlayers--;
                    Console.WriteLine("Thank you for playing, " + player.name + "!");
                    players.Remove(player);
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }

            if (totalPlayers < 2)
            {
                Console.WriteLine("Not enough players!");
				Console.WriteLine("Press <Enter> to exit... ");
				while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            }
            else
            {
                // Set up a new table!
                CardTable cardTable = new CardTable();
                Game game = new Game(cardTable);
                game.firstGame = false;
                game.DoNextTask();
            }
        }
    }
}