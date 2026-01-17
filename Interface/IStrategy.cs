namespace FishingCardGame;

public interface IStrategy
{
    Card ChooseCard(Player player, Card topCard, List<Player> allPlayers, CardColor? forcedColor = null);
}

