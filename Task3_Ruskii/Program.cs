using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Task3_Ruskii
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region ReadAndParseCountsInfo
            var lines = File.ReadLines(Constants.InFile).ToArray();
            var firstLine = lines[0].Split(' ');
            var roomsCount = int.Parse(firstLine[0]);
            var doorsCount = int.Parse(firstLine[1]);
            var originalRoomNumber = int.Parse(firstLine[2]);
            var numberOfChips = int.Parse(firstLine[3]);
            #endregion
            #region ReadAndParseRooms
            var doors = new Dictionary<int, Door>();
            for (var roomNumber = 1; roomNumber <= roomsCount; roomNumber++)
            {
                var currentRoomInfo = lines[roomNumber];
                var doorNumbers = currentRoomInfo[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                foreach (var doorNumber in doorNumbers)
                {
                    if (doors.ContainsKey(doorNumber))
                    {
                        doors[doorNumber].To = roomNumber;
                    }
                    else
                    {
                        doors[doorNumber] = new Door(doorNumber) { From = roomNumber};
                    }
                }
            }

            var costInfo = lines[(roomsCount + 1)..].SelectMany(x => x.Split(' ')).ToArray();
            for (var doorNumber = 1; doorNumber < doorsCount; doorNumber++)
            {
                doors[doorNumber].Cost = int.Parse(costInfo[doorNumber - 1]);
            }

            var rooms = new Dictionary<int, Room>();
            for (var doorNumber = 1; doorNumber <= doorsCount; doorNumber++)
            {
                var currentDoor = doors[doorNumber];
                var from = currentDoor.From;
                var to = currentDoor.To;
                if (!rooms.ContainsKey(from))
                    rooms[from] = new Room(from);
                if (to != 0 && !rooms.ContainsKey(to))
                    rooms[to] = new Room(to);

                if (to == 0)
                {
                    rooms[from].Exit = currentDoor;
                }
                else
                {
                    rooms[from].InterroomDoors.Add(currentDoor);
                }

                if (to != 0)
                {
                    rooms[to].InterroomDoors.Add(new Door(currentDoor.Number)
                        {From = to, To = from, Cost = currentDoor.Cost});
                }
            }

            var roomDijkstraWrappers = rooms
                .ToDictionary(
                    x => x.Key, 
                    x => new RoomDijkstraWrapper(x.Value));
            #endregion

            var previous = new Dictionary<int, int>();
            var visited = new HashSet<int>();
            var currentRoomWrapper = roomDijkstraWrappers[originalRoomNumber];
            currentRoomWrapper.DijkstraWeight = 0;
            while (visited.Count != roomDijkstraWrappers.Count && currentRoomWrapper.DijkstraWeight != int.MaxValue)
            {
               visited.Add(currentRoomWrapper.Room.Number);

                foreach (var door in currentRoomWrapper.Room.InterroomDoors)
                {
                    if (door.To == 0)
                        continue;

                    var nextRoomWrapper = roomDijkstraWrappers[door.To];
                    if (currentRoomWrapper.DijkstraWeight + door.Cost < nextRoomWrapper.DijkstraWeight)
                    {
                        nextRoomWrapper.DijkstraWeight = currentRoomWrapper.DijkstraWeight + door.Cost;
                        previous[nextRoomWrapper.Room.Number] = currentRoomWrapper.Room.Number;
                    }
                }

                if (visited.Count == roomDijkstraWrappers.Count)
                    break;

                currentRoomWrapper = roomDijkstraWrappers
                    .Where(x => !visited.Contains(x.Key))
                    .Aggregate((x, y) => x.Value.DijkstraWeight < y.Value.DijkstraWeight ? x : y).Value;
            }

            var nearestExit = roomDijkstraWrappers.Values
                .Where(x => x.Room.Exit != null)
                .Aggregate((x, y) =>
                    x.DijkstraWeight + x.Room.Exit.Cost < y.DijkstraWeight + y.Room.Exit.Cost ? x : y);

            var nearestExitCost = nearestExit.DijkstraWeight + nearestExit.Room.Exit.Cost;
            var sb = new StringBuilder();
            if (nearestExitCost <= numberOfChips)
            {
                sb.AppendLine("Y");
                sb.AppendLine(nearestExitCost.ToString());
                var path = new Stack<int>();
                if (nearestExit.Room.Number == originalRoomNumber)
                {
                    path.Push(originalRoomNumber);
                }
                else
                {
                    
                    var currentRoomNumber = nearestExit.Room.Number;
                    while (currentRoomNumber != originalRoomNumber && previous.ContainsKey(currentRoomNumber))
                    {
                        path.Push(currentRoomNumber);
                        currentRoomNumber = previous[currentRoomNumber];
                    }

                    path.Push(currentRoomNumber);
                }

                sb.AppendLine(string.Join(' ', path));
            }
            else
            {
                sb.AppendLine("N");
            }
            
            File.WriteAllText(Constants.OutFile, sb.ToString());
            
        }
    }
}