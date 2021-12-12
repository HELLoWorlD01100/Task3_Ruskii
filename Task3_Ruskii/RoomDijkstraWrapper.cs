namespace Task3_Ruskii
{
    public class RoomDijkstraWrapper
    {
        public Room Room { get; }
        public int DijkstraWeight { get; set; } = int.MaxValue;

        public RoomDijkstraWrapper(Room room)
        {
            Room = room;
        }
    }
}