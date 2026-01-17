namespace FishingCardGame;

public class DepositStack
{
    private Stack<Card> _stack = new Stack<Card>();
    // Méthode pour déposer une carte sur le dessus d'une pile
    public void Deposit(Card c) => _stack.Push(c);
    // On récupère la carte qui est au sommet de la pile sans la retirer
    public Card Top => _stack.Peek();
    // On récupère toutes les cartes sauf celle du dessus
    // On reconstruit la pile en gardant uniquement la carte de dessus
    public List<Card> TakeAllExceptTop()
    {
        var cards = _stack.ToList();
        var top = cards.First();
        cards.RemoveAt(0);
        _stack = new Stack<Card>(new List<Card> { top });
        return cards;
    }
}