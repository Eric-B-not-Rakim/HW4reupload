using System;
using System.Collections.Generic;
using System.Linq; // currently only needed if we use alternate shuffle method

namespace RaceTo21
{
    public class Deck
    {
        List<Card> cards = new List<Card>();
		Dictionary<Card, string> cardPics = new Dictionary<Card, string>();

		public Deck()
        {
            Console.WriteLine("*********** Building deck...");
            string[] suits = { "Spades", "Hearts", "Clubs", "Diamonds" };

			for (int cardVal = 1; cardVal <= 13; cardVal++)
            {
                foreach (string cardSuit in suits)
                {
                    string cardName;
                    string cardLongName;
                    switch (cardVal)
                    {
                        case 1:
                            cardName = "A";
                            cardLongName = "Ace";
                            break;
                        case 11:
                            cardName = "J";
                            cardLongName = "Jack";
                            break;
                        case 12:
                            cardName = "Q";
                            cardLongName = "Queen";
                            break;
                        case 13:
                            cardName = "K";
                            cardLongName = "King";
                            break;
                        default:
                            cardName = cardVal.ToString();
                            cardLongName = cardName;
                            break;
                    }
                    Card c = new Card(cardName + cardSuit.First<char>(), cardLongName + " of " + cardSuit);
                    cards.Add(c);

                    // PICTURES:
                    if (cardName.Length > 1 && cardName == cardLongName)
                        // ^^^ Check how many digits in the cardName: if it's 2-9, we need an extra step:
                    { // This adds a zero in front of the 2-9 value, so we get the right pic.
                        string picName = "0" + cardName;
                        picName.ToLower();
                        cardSuit.ToLower();
						cardPics[c] = "card_" + cardSuit + "_" + picName;
                        // This should return "card_diamonds_02", for example
					}
					else
                    {
                        string picName = cardName;
						picName.ToLower();
						cardSuit.ToLower();
						cardPics[c] = "card_" + cardSuit + "_" + picName;
                        // and this should return "card_diamonds_10", for example
					}
				}
			}
        }



		public void Shuffle()
        {
            Console.WriteLine("Shuffling Cards...");

            Random rng = new Random();

            // one-line method that uses Linq:
            // cards = cards.OrderBy(a => rng.Next()).ToList();

            // multi-line method that uses Array notation on a list!
            // (this should be easier to understand)
            for (int i=0; i<cards.Count; i++)
            {
                Card tmp = cards[i];
                int swapindex = rng.Next(cards.Count);
                cards[i] = cards[swapindex];
                cards[swapindex] = tmp;
            }
        }

        /* Maybe we can make a variation on this that's more useful,
         * but at the moment it's just really to confirm that our 
         * shuffling method(s) worked! And normally we want our card 
         * table to do all of the displaying, don't we?!
         */

        public void ShowAllCards()
        {
            for (int i=0; i<cards.Count; i++)
            {
                Console.Write(i+":"+cards[i].displayName); // a list property can look like an Array!
                if (i < cards.Count -1)
                {
                    Console.Write(" ");
                } else
                {
                    Console.WriteLine("");
                }
            }
        }

        public string DealTopCard()
        {
            string card = cards[cards.Count - 1].displayName;
            cards.RemoveAt(cards.Count - 1);
            // Console.WriteLine("I'm giving you " + card);
            return card;
        }
    }
}

