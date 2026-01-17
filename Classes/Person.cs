namespace FishingCardGame;

public class Person
{
    // pour avoir ID unique de chaque personne
    public Guid Id { get; } = Guid.NewGuid();
    public string Nom { get; }
    public string Prenom { get; }

    public Person(string nom, string prenom)
    {
        Nom = nom; 
        Prenom = prenom;
    }
}