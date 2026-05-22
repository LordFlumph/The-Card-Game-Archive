namespace CardGameArchive.Solitaire.Freecell
{
    using CardGameArchive.Solitaire.Klondike;

    public class FreecellGameRules : KlondikeGameRules
    {
        protected override bool IsWasteMoveValid(Card card, ZoneParent destination, Card parentCard, bool simulation = false) => false;

		protected override bool IsCellMoveValid(Card card, ZoneParent destination, Card parentCard = null, bool simulation = false) => destination.CardCount == 0;
	}
}