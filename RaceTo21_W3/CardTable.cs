﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                players[i].Introduce(i+1); // List is 0-indexed but user-friendly player positions would start with 1...
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

        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)
            {
                // For removing the comma: try enumerators to check
                // if this is the last iteration in the foreach loop?
                // what's the right method here?
                Console.Write(player.name + " has: ");
				foreach (string card in player.cards)
                {
                   // while (player.cards.MoveNext())
                    Console.Write("the " + card + ", ");
                }
                Console.Write("=" + player.score + "/21 ");
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

            // ASK IF PLAY AGAIN?
			Console.Write("Do you want to play again? Y/N");
			string response = Console.ReadLine();
			if (response.ToUpper().StartsWith("Y"))
			{
                // Set up a new table!
				CardTable cardTable = new CardTable();
				Game game = new Game(cardTable);
				while (game.nextTask != "GameOver")
				{
					game.DoNextTask();
				}
			}
			else if (response.ToUpper().StartsWith("N"))
			{
                // If not, bye.
                Console.Write("Goodbye!");
			}
			else
			{
				Console.WriteLine("Please answer Y(es) or N(o)!");
			}

			while (Console.ReadKey().Key != ConsoleKey.Enter) { }
		}

		/*
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
            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
        */
	}
}