using System;

namespace CardGameArchive
{
    public static class GameTerms
    {
        public enum GameTag
        {
            Patience, // Solitaire
        }

        public enum GameName
        {
            KlondikeDealOne,
            KlondikeDealThree,
            SpiderOneSuit,
            SpiderTwoSuit,
            SpiderFourSuit,
			SpideretteOneSuit,
            SpideretteTwoSuit,
            SpideretteFourSuit,
            Clock,
            Pyramid,
            TriPeaks,
            Freecell,
            Osmosis,
            FlowerGarden,
            Yukon,
            Crossword,
            Golf,
            Frog,
            Maze,
            Memory,

            GoFish,
            President, // Warlords and Scumbags
            Canasta,
            Piquet,
            Hearts,
            Spades,
            FiveHundred,
            Poker,
            Balatro,
        }

		public enum DealDirection
		{
			LeftRight,
			RightLeft
		}
	}

}