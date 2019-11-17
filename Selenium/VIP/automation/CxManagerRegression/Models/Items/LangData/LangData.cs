namespace Models.Items.LangData
{
    public class LangData
    {
        public string Title { get; set; }
        public Engine Engine { get; set; }
        public Price Price { get; set; }
        public object[] IndividualOptions { get; set; }
        public object[] SerialData { get; set; }
        public object[] TechnicalData { get; set; }
    }

    public class Engine
    {
    }

    public class Price
    {
        public object[] Dfo { get; set; }
        public Leasing Leasing { get; set; }
    }

    public class Leasing
    {
    }
}
