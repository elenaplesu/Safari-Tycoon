using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Safari
{
    public class GameManager
    {
        private bool isRunning;
        public Map map { get; }
        private bool isPaused;
        private Player player;
        private int numberOfAnimals;
        private int numberOfVisitors;
        private int numberOfRoads;
        private int numberOfJeeps;
        private List<Jeep> availableJeeps = new List<Jeep>();
        private List<Jeep> activeJeeps = new List<Jeep>();
        private List<Poacher> poachers = new List<Poacher>();
        private List<Ranger> rangers = new List<Ranger>();
        private int numberOfPonds;
        private int numberOfBushes;
        private int numberofTrees;
        private int tileSize = 32;

        public Difficulty CurrentDifficulty { get; private set; }

        public GameManager(Map map, Difficulty currentDifficulty)
        {
            player = new Player(3000); // Initialize player with starting capital       100
            this.map = map;
            CurrentDifficulty = currentDifficulty;
        }

        private List<Animal> animals = new List<Animal>();



        public void AddAnimal(Animal animal)
        {
            animals.Add(animal);
            numberOfAnimals++;  // Increment counter when adding
            //UpdateVisitors();
            UIUpdateRequested?.Invoke();
        }

        public void RemoveAnimal(Animal animal)  // Modify to accept specific animal
        {
            if (animals.Remove(animal))
            {
                numberOfAnimals--;
              //  UpdateVisitors();
                UIUpdateRequested?.Invoke();
            }
        }

        public List<Animal> GetAnimals()
        {
            return animals;
        }

        public void AddPoacher(Poacher poacher)
        {
            poachers.Add(poacher);
        }
        public void RemovePoacher(Poacher poacher)
        {
            poachers.Remove(poacher);
        }

        public List<Poacher> GetPoachers()
        {
            return poachers;
        }

        public void UpdatePoachers()
        {
            foreach (var poacher in poachers)
            {
                poacher.TryMove(CheckPoacherMove);
            }
        }

        public void AddRanger(Ranger ranger)
        {
            rangers.Add(ranger);
        }

        public void RemoveRanger(Ranger ranger)
        {
            rangers.Remove(ranger);
        }

        public List<Ranger> GetRangers()
        {
            return rangers;
        }

        public void UpdateRangers() 
        {
            foreach (var ranger in rangers.ToList())
            {
                ranger.MoveToClosestTarget(animals, poachers);
            }
        }

        public void UpdateVisitors()
        {
            // each road adds 4 visitors
            numberOfVisitors = numberOfVisitors + 4;
        }

        public void StartGame()
        {
            isRunning = true;
            isPaused = false;
            //GameLoop();
        }

        public void StopGame()
        {
            isRunning = false;
        }

        public void PauseGame()
        {
            isPaused = true;
        }

        public void UpdateAnimals()
        {
            // Create a copy of the list to avoid modification during enumeration
            var animalsToUpdate = new List<Animal>(animals);

            foreach (var animal in animalsToUpdate)
            {
                // Check if animal still exists in original collection
                if (animals.Contains(animal))
                {
                    animal.UpdatePosition(map);
                }
            }
        }

        public void ResumeGame()
        {
            isPaused = false;
        }

        public int GetCapital()
        {
            return player.Capital;
        }

        public void UpdateCapital(int amount)
        {
            player.Capital += amount;
        }
        public int GetNumberOfAnimals()
        {
            return numberOfAnimals;
        }



        public int GetNumberOfVisitors()
        {
            return numberOfVisitors;
        }



        public void AddVisitors(int count)
        {
            numberOfVisitors += count;
        }
        public int GetNumberOfRoads()
        {
            return numberOfRoads;
        }



        public void AddRoad()
        {
            numberOfRoads++;
        }

        public void AddRoadBlock(int n)
        {
            numberOfRoads += n;
        }

        public void RemoveRoad()
        {
            numberOfRoads--;
        }

        public int GetNumberOfJeeps()
        {
            return numberOfJeeps;
        }




        public void AddJeep()
        {
            availableJeeps.Add(new Jeep());
            numberOfJeeps++;
        }

        public void RemoveJeep()
        {
            if (availableJeeps.Count > 0)
            {
                availableJeeps.RemoveAt(0);

            }
        }

        public bool StartJeepRoute()
        {
            if (availableJeeps.Count == 0) return false;

            var jeep = availableJeeps[0];
            if (jeep.FindPath(map))
            {
                availableJeeps.Remove(jeep);
                activeJeeps.Add(jeep);
                jeep.StartRoute(map);
                UpdateVisitors();
                UIUpdateRequested?.Invoke();
                return true;
            }
            return false;
        }
        public event Action UIUpdateRequested = delegate { };
        // public event Action<int> IncomeEarned = delegate { };

        public void UpdateJeeps()
        {
            foreach (var jeep in activeJeeps.ToList()) // ToList() to avoid modification during iteration
            {
                jeep.UpdatePosition();
                if (!jeep.IsMoving)
                {
                    // Jeep finished its route
                    int income = 50 * GetNumberOfAnimals();
                    player.Capital += income;
                    UIUpdateRequested();
                    activeJeeps.Remove(jeep);
                    availableJeeps.Add(jeep); // Make it available again
                }
            }
        }

        public List<Jeep> GetActiveJeeps() => activeJeeps;

        public Point FindAvailablePositionNearSpecies(Type speciesType)
        {
            // Get all animals of the same species
            var sameSpecies = animals.Where(a => a.GetType() == speciesType).ToList();

            if (sameSpecies.Count == 0)
                return FindAvailablePosition(); // Fallback to random valid position

            // Search around each existing animal of the same species
            foreach (var animal in sameSpecies)
            {
                int animalTileX = (int)(animal.PixelX / tileSize);
                int animalTileY = (int)(animal.PixelY / tileSize);

                // Check adjacent tiles (radius = 2 tiles)
                for (int x = animalTileX - 2; x <= animalTileX + 2; x++)
                {
                    for (int y = animalTileY - 2; y <= animalTileY + 2; y++)
                    {
                        if (x >= 0 && x < map.Width && y >= 0 && y < map.Height)
                        {
                            if (map.GetTile(x, y) == 3 && !IsPositionOccupied(x, y))
                            {
                                return new Point(x, y);
                            }
                        }
                    }
                }
            }

            return FindAvailablePosition(); // Fallback if no nearby space found
        }


        public Point FindAvailablePosition()
        {
            List<Point> availablePositions = new List<Point>();

            for (int y = map.Height - 2; y >= 0; y--)
            {
                for (int x = map.Width - 2; x >= 0; x--)
                {
                    if (map.GetTile(x, y) == 3 && !IsPositionOccupied(x, y))
                    {
                        if (!IsAdjacentToInvalidTile(x, y))
                        {
                            availablePositions.Add(new Point(x, y));

                        }
                    }
                }
            }

            if (availablePositions.Count > 0)
            {
                int randomIndex = new Random().Next(availablePositions.Count);
                return availablePositions[randomIndex];
            }

            return Point.Empty;
        }

        public bool IsPositionOccupied(int x, int y)
        {
            foreach (var animal in animals)
            {
                int animalTileX = (int)(animal.PixelX / tileSize);
                int animalTileY = (int)(animal.PixelY / tileSize);
                if (animalTileX == x && animalTileY == y)
                    return true;
            }
            return false;
        }

        private bool IsAdjacentToInvalidTile(int x, int y)
        {
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx >= 0 && nx < map.Width && ny >= 0 && ny < map.Height)
                {
                    int tile = map.GetTile(nx, ny);
                    // 1 is road and 2 is river
                    if (tile == 1 || tile == 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public int CheckPoacherMove(int x, int y)
        {
            int tileSize = 32;
            // Check if there is animal in the position
            if (IsPositionOccupied(x, y))
            {
                for (int i = animals.Count - 1; i >= 0; i--)
                {
                    var animal = animals[i];
                    int animalTileX = (int)(animal.PixelX / tileSize);
                    int animalTileY = (int)(animal.PixelY / tileSize);
                    if (animalTileX == x && animalTileY == y)
                    {
                        animals.RemoveAt(i);
                        RemoveAnimal(animal);
                    }
                }
                return 0;
            }
            // Check if tile is grass
            if (map.GetTile(x, y) == 3)
            {
                return 1;
            }
            return 2;
        }


        public int GetNumberOfPonds()
        {
            return numberOfPonds;
        }

        public void AddPond()
        {
            numberOfPonds++;
        }

        public void RemovePond()
        {
            numberOfPonds--;
        }

        public int GetNumberOfBushes()
        {
            return numberOfBushes;
        }

        public void AddBush()
        {
            numberOfBushes++;
        }

        public void RemoveBush()
        {
            numberOfBushes--;
        }

        public int GetNumberOfTrees()
        {
            return numberofTrees;
        }

        public void AddTree()
        {
            numberofTrees++;
        }

        public void RemoveTree()
        {
            numberofTrees--;
        }

        public bool HasMinimumVisitors(int minimum) => GetNumberOfVisitors() >= minimum;
        public bool HasMinimumCarnivores(int minimum) => GetAnimals().Count(a => a is Carnivore) >= minimum;
        public bool HasMinimumHerbivores(int minimum) => GetAnimals().Count(a => a is Herbivore) >= minimum;
        public bool HasMinimumCapital(int minimum) => GetCapital() >= minimum;

    }
}