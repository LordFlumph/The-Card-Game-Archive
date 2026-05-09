using System;

namespace CardGameArchive
{
    public static class GameTerms
    {
        public enum GameCategory
        {
            Patience, // Solitaire
        }

        public enum GameName
        {
            // Patience
            KlondikeDealOne,
            KlondikeDealThree,
            SpiderOneSuit,
            SpiderTwoSuit,
            SpiderFourSuit,
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

            // Other
            GoFish,
            President, // Warlords and Scumbags
            Canasta,
            Piquet,
            Hearts,
            Spades,
            FiveHundred,
            Poker,
        }
    }

}