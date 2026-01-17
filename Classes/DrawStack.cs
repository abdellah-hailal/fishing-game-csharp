namespace FishingCardGame;

public class DrawStack
{
    private Stack<Card> _stack;
    // Constructeur : crée la pioche à partir d'une collection de cartes
    public DrawStack(IEnumerable<Card> cards) { _stack = new Stack<Card>(cards); }
    // Pour tirer une carte du dessus de la pioche et la retire de la pile
    public Card Draw() => _stack.Pop();
    // Pour donner le nombre de cartes restantes dans la pioche 
    public int Count => _stack.Count;
    // Pour vérifier si la pioche est vide
    public bool IsEmpty => _stack.Count == 0;
}