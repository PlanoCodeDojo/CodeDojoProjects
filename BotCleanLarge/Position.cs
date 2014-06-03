namespace BotCleanLarge
{
    public class Position
    {
        public Position()
        {
            
        }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }
        //Based on Mattrix
        public int Row { get; set; }
        public int Column { get; set; }

        //For our Algorithm based on bot postion
        //public int Relative_X { get; set; }
        //public int Relative_Y { get; set; }
    }
}