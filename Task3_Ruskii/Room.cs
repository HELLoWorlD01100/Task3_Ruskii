using System.Collections.Generic;

namespace Task3_Ruskii
{
    public class Room
    {
        public int Number { get; }
        public HashSet<Door> InterroomDoors = new();
        public Door Exit { get; set; } = null;

        public Room(int number)
        {
            Number = number;
        }
    }
}