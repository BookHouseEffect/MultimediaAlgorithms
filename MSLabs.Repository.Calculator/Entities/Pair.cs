namespace MSLabs.Repository.Calculator.Entities
{
    public class Pair<TKey, TValue>
    {
        public Pair()
        {
        }

        public Pair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        public TKey Key { get; set; }

        public TValue Value { get; set; }
    }
}
