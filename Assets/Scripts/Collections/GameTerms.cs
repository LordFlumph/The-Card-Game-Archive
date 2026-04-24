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
            KlondikeDeal1,
            KlondikeDeal3,
            SpiderOneSuit,
            SpiderTwoSuit,
            Spider4Suit,
            Spiderette,
            Clock,
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
            Pyramid,

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