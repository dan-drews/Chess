namespace ChessLibrary
{
    public class Square
    {
        public int Rank { get; set; }
        public Files File { get; set; }

        public Colors Color
        {
            get
            {
                // Even squares when adding rank + file are black
                if (((int)File + Rank) % 2 == 0)
                {
                    return Colors.Black;
                }
                return Colors.White;
            }
        }
    }
}
