namespace FishingCardGame;

public class CardPair
{
    public List<Card> Cards { get; private set; }

    public CardPair()
    {
        Cards = new List<Card>();

        // Construire le paquet de 52 cartes
        foreach (var color in CardColor.All)
        {
            foreach (var value in CardValue.All)
            {
                Cards.Add(new Card(value, color));
            }
        }

        // Vérification automatique
        ValidateDeck();
    }

    // Mélanger le paquet
    public void Shuffle()
    {
        Random rnd = new Random();
        Cards = Cards.OrderBy(c => rnd.Next()).ToList();
    }

    // Méthode pour vérifier la validité du paquet
    private void ValidateDeck()
    {
        int total = Cards.Count;
        int distinctCount = Cards
            .Select(c => $"{c.Value}-{c.Color}")
            .Distinct()
            .Count();

        if (total != 52 || distinctCount != 52)
        {
            throw new InvalidOperationException("Erreur : le paquet n’a pas exactement 52 cartes uniques !");
        }
    }
}

