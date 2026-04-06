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
            NONE,

            // Patience
            Klondike,
            Spider,
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
            Poker,
        }
    }

}