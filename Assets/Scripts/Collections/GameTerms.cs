using System;

namespace CardGameArchive
{
    public static class GameTerms
    {
        public enum GameTag
        {
            Solitaire,
            Quick,
            Long,
            Luck,
            Skill,
            SinglePlayer,
            Multiplayer,
            Matching,
            Math,
        }

        public enum GameName
        {
            Klondike,
            Spider,
			Spiderette,
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

        public enum GameVariant
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
		}

        public enum DealDirection
		{
			LeftRight,
			RightLeft
		}
	}

}