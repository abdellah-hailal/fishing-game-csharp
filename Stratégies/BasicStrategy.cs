namespace FishingCardGame;

public class BasicStrategy : IStrategy
{
    // Stratégie de base : le joueur joue tout simplement la première carte compatible
    public Card ChooseCard(Player player, Card topCard, List<Player> allPlayers, CardColor? forcedColor = null)
    {
        IEnumerable<Card> playable;

        // Si une couleur est imposée par une carte Valet
        if (forcedColor != null)
        {
            
            // le joueur peut jouer soit une carte de la couleur imposée soit une carte Valet
            playable = player.Hand.Where(c => c.Color.Equals(forcedColor) || c.Value.Equals(CardValue.Valet));
        }
        else
        {
            // Sinon la règle normale : le joueur peut jouer une carte de même couleur, de même valeur 
            // ou une carte Valet 
            playable = player.Hand.Where(c =>
                c.Color.Equals(topCard.Color) ||
                c.Value.Equals(topCard.Value) ||
                c.Value.Equals(CardValue.Valet));
        }
        // Renvoie la première carte compatible trouvée, ou null si aucune n'est pas jouable
        return playable.FirstOrDefault();
    }
}
