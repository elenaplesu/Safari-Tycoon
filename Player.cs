namespace Safari
{
    public class Player
    {
        public int Capital { get; set; }
        public int Visitors { get; set; }

        public Player(int initialCapital)
        {
            Capital = initialCapital;
        }

        public bool CanAfford(int cost)
        {
            return Capital >= cost;
        }

        public void Spend(int amount)
        {
            if (CanAfford(amount))
            {
                Capital -= amount;
            }
        }

        public void EarnCapital(int amount)
        {
            Capital += amount;
        }
    }
}