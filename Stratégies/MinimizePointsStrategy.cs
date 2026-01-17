namespace FishingCardGame;

public class MinimizePointsStrategy : IStrategy
{
    // Stratégie de minimisation : minimiser les points restants en main
    public Card ChooseCard(Player player, Card topCard, List<Player> allPlayers, CardColor? forcedColor = null)
    {
        List<Card> playable;
        
        // Si une couleur est imposée par une carte Valet
        if (forcedColor != null)
        {
            playable = player.Hand
                .Where(c => c.Color.Equals(forcedColor) || c.Value.Equals(CardValue.Valet))
                .ToList();
        }
        else
        {
            // Sinon la règle normale : le joueur peut jouer une carte de même couleur, de même valeur 
            // ou une carte Valet
            playable = player.Hand
                .Where(c => c.Color.Equals(topCard.Color) || c.Value.Equals(topCard.Value) || c.Value.Equals(CardValue.Valet))
                .ToList();
        }

        // Si aucune carte n'est jouable renvoie null
        if (playable.Count == 0)
            return default;

        // Parmi les cartes jouables, on choisit celle qui a le plus de points 
        // cela permet de réduire au maximum le score si le joueur perd 
        return playable.OrderByDescending(c => c.Value.Points).First();
    }
}
