namespace Task3_Ruskii
{
    public class Door
    {
        public int Number { get; }
        public int From { get; set; }
        public int To { get; set; }
        public int Cost { get; set; }

        public Door(int number)
        {
            Number = number;
        }
    }
}