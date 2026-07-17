using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            Blackjack
        }

        public enum GameVariant
        {
            NONE = -1,
			KlondikeDealOne,
			KlondikeDealThree,

			SpiderOneSuit,
			SpiderTwoSuit,
			SpiderFourSuit,

			SpideretteOneSuit,
			SpideretteTwoSuit,
			SpideretteFourSuit,

            Clock,
            Watch,
            
            PyramidTraditional,
            PyramidRelaxed,
		}

        public enum DealDirection
		{
			LeftRight,
			RightLeft
		}

		[System.Serializable]
		public struct TagInfo
		{
            public GameTag Tag;
			public string DisplayName;

            public TagInfo(GameTag tag, string displayName = "")
			{
				Tag = tag;

                if (displayName == "")
                    DisplayName = Tag.ToString();
                else
				    DisplayName = displayName;
			}
		}

        static readonly HashSet<TagInfo> AllTags = new()
        {
            new TagInfo(GameTag.Solitaire), new TagInfo(GameTag.Quick), new TagInfo(GameTag.Long),
            new TagInfo(GameTag.Luck), new TagInfo(GameTag.Skill), new TagInfo(GameTag.Quick),
            new TagInfo(GameTag.Long), new TagInfo(GameTag.Luck), new TagInfo(GameTag.Skill),
            new TagInfo(GameTag.SinglePlayer), new TagInfo(GameTag.Multiplayer), new TagInfo(GameTag.Matching),
            new TagInfo(GameTag.Math)
        };

        public static TagInfo GetTagInfo(GameTag tag) => AllTags.First(o => o.Tag == tag);
	}	
}