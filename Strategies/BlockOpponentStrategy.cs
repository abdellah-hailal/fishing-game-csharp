namespace FishingCardGame;

public class BlockOpponentStrategy : IStrategy
{
    // Stratégie de blocage : bloquer un joueur cible en évitant de lui laisser jouer facilement 
    
    private Player _target; // joueur à bloquer
    public BlockOpponentStrategy(Player target) { _target = target; }

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

        // On analyse la main de joueur cible
        // On cherche la couleur la plus fréquente dans sa main
        var targetCommonColor = _target.Hand
            .GroupBy(c => c.Color)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key;

        // Si possible, on joue une carte qui n'est pas de cette couleur dominante
        // pour réduire les chances que le joueur cible puisse jouer
        var blockingCard = playable.FirstOrDefault(c => !c.Color.Equals(targetCommonColor));
        // Si on a trouvé la carte qui bloque la couleur de joueur cible, on le joue
        if (!blockingCard.Equals(default(Card)))
            return blockingCard;
        // Sinon on joue la première carte jouable
        return playable.First();
    }
}
