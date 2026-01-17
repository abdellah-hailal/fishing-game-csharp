namespace FishingCardGame;

public class Player : Person
{
    // La main de joueur (les cartes qu'il possède actuellement)
    public List<Card> Hand { get; private set; } = new List<Card>();
    // La stratégie utilisée par le joueur 
    public IStrategy Strategy { get; set; }
    public Player(string nom, string prenom) : base(nom, prenom) { }
    // Méthode pour ajouter une carte à la main du joueur
    public void AddCard(Card c) => Hand.Add(c);
    // Méthode pour retirer une carte de la main du joueur
    public void RemoveCard(Card c) => Hand.Remove(c);
    public override string ToString() => $"{Prenom} {Nom}";
}
