using System;
using System.Collections.Generic;
using System.Drawing;

namespace Safari
{
    /// <summary>
    /// Represents a base class for all animals in the game.
    /// Provides common functionality such as movement, aging, and reproduction.
    /// </summary>
    public abstract class Animal
    {
		//for testing
        protected GameManager gameManager { get; }

        protected double multiplier;

        protected Map map { get; }

        public float PixelX { get; protected set; }

        public float PixelY { get; protected set; }

        public float Speed { get; protected set; } = 2f;

        protected const int TileSize = 32;
        protected Random random = new Random();

        public AnimalState CurrentState { get; protected set; } = AnimalState.Wandering;

        protected DateTime? restStartTime;
        protected int restDuration = 30000; // 30 seconds in milliseconds
        protected Point? targetWaterTile;
        protected List<Point> visitedWaterSources = new List<Point>();
        protected Point? currentWaterSource;
        protected const int MaxRememberedSources = 3;

        public enum AgeStage { Young, Adult, Old }
        public AgeStage CurrentAge { get; private set; } = AgeStage.Young;

        protected DateTime BirthTime { get; } = DateTime.Now;
        protected TimeSpan YoungDuration ;
        protected TimeSpan AdultDuration ;
        protected TimeSpan OldDuration;

        protected bool CanReproduce => CurrentAge == AgeStage.Adult;
        protected DateTime? LastReproductionTime;
        protected TimeSpan ReproductionCooldown = TimeSpan.FromSeconds(15);

        public DateTime LastFedTime { get; set; } = DateTime.Now;
        protected TimeSpan TimeUntilStarve { get; set; } = TimeSpan.FromSeconds(40);
        public bool IsStarving => (DateTime.Now - LastFedTime) > TimeUntilStarve;
        public enum AnimalState
        {
            Wandering,
            MovingToWater,
            Resting,
            Returning
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animal"/> class.
        /// </summary>
        /// <param name="gameManager">The game manager instance.</param>
        /// <param name="map">The map instance.</param>
        public Animal(GameManager gameManager, Map map)
        {
            System.Diagnostics.Debug.Assert(gameManager != null, "GameManager cannot be null");
            System.Diagnostics.Debug.Assert(map != null, "Map cannot be null");

            if (gameManager == null || map == null)
                throw new InvalidOperationException("Critical dependencies not initialized");

            this.gameManager = gameManager;
            this.map = map;
            if (gameManager.CurrentDifficulty==Difficulty.Easy)
            this.multiplier = 2;
            else if (gameManager.CurrentDifficulty == Difficulty.Medium)
                this.multiplier = 0.75;
            if (gameManager.CurrentDifficulty == Difficulty.Hard)
                this.multiplier = 1;

         YoungDuration = TimeSpan.FromSeconds(30*multiplier);
         AdultDuration = TimeSpan.FromSeconds(30*multiplier);
        OldDuration = TimeSpan.FromSeconds(30 * multiplier);
    }

        /// <summary>
        /// Sets the position of the animal in pixels.
        /// </summary>
        /// <param name="pixelX">The X-coordinate in pixels.</param>
        /// <param name="pixelY">The Y-coordinate in pixels.</param>
        public void SetPosition(float pixelX, float pixelY)
        {
            this.PixelX = pixelX;
            this.PixelY = pixelY;
        }

        /// <summary>
        /// Updates the position and state of the animal.
        /// </summary>
        /// <param name="map">The map instance.</param>
        public virtual void UpdatePosition(Map map)
        {
            if (IsStarving)
            {
                Die();
                return;
            }
            UpdateAge();

            if (CurrentAge == AgeStage.Old && (DateTime.Now - BirthTime) >= YoungDuration + AdultDuration + OldDuration)
            {
                Die();
                return;
            }

            switch (CurrentState)
            {
                case AnimalState.Wandering:
                    Wander(gameManager.map);
                    break;
                case AnimalState.MovingToWater:
                    MoveToWater(gameManager.map);
                    break;
                case AnimalState.Resting:
                    Rest();
                    break;
                case AnimalState.Returning:
                    ReturnFromWater();
                    break;
            }

            if (CanReproduce && random.NextDouble() < 0.005) // 0.5% chance per update
            {
                TryReproduce();
            }
        }

        /// <summary>
        /// Updates the age stage of the animal based on its lifespan.
        /// </summary>
        private void UpdateAge()
        {
            TimeSpan age = DateTime.Now - BirthTime;
            if (age < YoungDuration)
                CurrentAge = AgeStage.Young;
            else if (age < YoungDuration + AdultDuration)
                CurrentAge = AgeStage.Adult;
            else
                CurrentAge = AgeStage.Old;
        }

        /// <summary>
        /// Attempts to reproduce if conditions are met.
        /// </summary>
        private void TryReproduce()
        {
            if (LastReproductionTime.HasValue &&
                (DateTime.Now - LastReproductionTime.Value) < ReproductionCooldown)
                return;

            if (!HasSameSpeciesNearby())
                return;

            Point? spawnPos = FindNearbyEmptyTile();
            if (spawnPos.HasValue)
            {
                Animal offspring = CreateOffspring();
                offspring.SetPosition(spawnPos.Value.X * TileSize + TileSize / 2f,
                                      spawnPos.Value.Y * TileSize + TileSize / 2f);
                gameManager.AddAnimal(offspring);
                LastReproductionTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Checks if there are animals of the same species nearby.
        /// </summary>
        /// <param name="radius">The radius to check for nearby animals.</param>
        /// <returns><c>true</c> if a same-species animal is nearby; otherwise, <c>false</c>.</returns>
        protected bool HasSameSpeciesNearby(int radius = 3)
        {
            int currentTileX = (int)(PixelX / TileSize);
            int currentTileY = (int)(PixelY / TileSize);

            for (int x = currentTileX - radius; x <= currentTileX + radius; x++)
            {
                for (int y = currentTileY - radius; y <= currentTileY + radius; y++)
                {
                    if (x >= 0 && x < gameManager.map.Width &&
                        y >= 0 && y < gameManager.map.Height)
                    {
                        foreach (var otherAnimal in gameManager.GetAnimals())
                        {
                            int otherTileX = (int)(otherAnimal.PixelX / TileSize);
                            int otherTileY = (int)(otherAnimal.PixelY / TileSize);

                            if (otherTileX == x && otherTileY == y &&
                                otherAnimal.GetType() == this.GetType() &&
                                otherAnimal != this)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        public virtual void Feed()
        {
            LastFedTime = DateTime.Now;
        }

        /// <summary>
        /// Finds a nearby empty tile for spawning offspring.
        /// </summary>
        /// <returns>The position of the empty tile, or <c>null</c> if none is found.</returns>
        private Point? FindNearbyEmptyTile()
        {
            int currentTileX = (int)(PixelX / TileSize);
            int currentTileY = (int)(PixelY / TileSize);

            for (int radius = 1; radius <= 2; radius++)
            {
                for (int x = currentTileX - radius; x <= currentTileX + radius; x++)
                {
                    for (int y = currentTileY - radius; y <= currentTileY + radius; y++)
                    {
                        if (x >= 0 && x < gameManager.map.Width && y >= 0 && y < gameManager.map.Height &&
                            gameManager.map.GetTile(x, y) == 3 && !gameManager.IsPositionOccupied(x, y))
                        {
                            return new Point(x, y);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Creates an offspring of the current animal.
        /// </summary>
        /// <returns>The offspring animal.</returns>
        protected abstract Animal CreateOffspring();

        /// <summary>
        /// Handles the death of the animal.
        /// </summary>
        public void Die()
        {
            gameManager.RemoveAnimal(this);
        }

        protected float clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        /// <summary>
        /// Handles the wandering behavior of the animal.
        /// </summary>
        /// <param name="map">The map instance.</param>
        protected virtual void Wander(Map map)
        {
            PixelX += Speed * (random.NextDouble() > 0.5 ? 1 : -1);
            PixelY += Speed * (random.NextDouble() > 0.5 ? 1 : -1);

            PixelX = clamp(PixelX, 0, map.Width * TileSize);
            PixelY = clamp(PixelY, 0, map.Height * TileSize);

            if (random.Next(100) < 5)
            {
                targetWaterTile = FindNearestWater(gameManager.map);
                if (targetWaterTile.HasValue)
                {
                    CurrentState = AnimalState.MovingToWater;
                }
            }
        }

        /// <summary>
        /// Handles the movement of the animal toward water.
        /// </summary>
        /// <param name="map">The map instance.</param>
        protected virtual void MoveToWater(Map map)
        {
            targetWaterTile = FindNearestWater(gameManager.map);

            if (!targetWaterTile.HasValue)
            {
                CurrentState = AnimalState.Wandering;
                return;
            }

            currentWaterSource = targetWaterTile.Value;

            float targetX = targetWaterTile.Value.X * TileSize + TileSize / 2;
            float targetY = targetWaterTile.Value.Y * TileSize + TileSize / 2;

            float dx = targetX - PixelX;
            float dy = targetY - PixelY;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0)
            {
                dx /= distance;
                dy /= distance;
            }

            PixelX += dx * Speed;
            PixelY += dy * Speed;

            int currentTileX = (int)(PixelX / TileSize);
            int currentTileY = (int)(PixelY / TileSize);

            if (map.GetTile(currentTileX, currentTileY) == 1 || map.GetTile(currentTileX, currentTileY) == 2)
            {
                CurrentState = AnimalState.Resting;
                restStartTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Handles the resting behavior of the animal.
        /// </summary>
        protected virtual void Rest()
        {
            if (!restStartTime.HasValue)
            {
                CurrentState = AnimalState.Wandering;
                return;
            }

            if ((DateTime.Now - restStartTime.Value).TotalMilliseconds >= restDuration)
            {
                if (currentWaterSource.HasValue)
                {
                    visitedWaterSources.Add(currentWaterSource.Value);
                    if (visitedWaterSources.Count > MaxRememberedSources)
                    {
                        visitedWaterSources.RemoveAt(0);
                    }
                }

                CurrentState = AnimalState.Returning;
            }
        }

        /// <summary>
        /// Handles the behavior of the animal returning from water.
        /// </summary>
        protected virtual void ReturnFromWater()
        {
            if (!currentWaterSource.HasValue)
            {
                CurrentState = AnimalState.Wandering;
                return;
            }

            float waterCenterX = currentWaterSource.Value.X * TileSize + TileSize / 2;
            float waterCenterY = currentWaterSource.Value.Y * TileSize + TileSize / 2;

            float dx = PixelX - waterCenterX;
            float dy = PixelY - waterCenterY;

            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            if (distance > 0)
            {
                dx /= distance;
                dy /= distance;
            }

            PixelX += dx * Speed;
            PixelY += dy * Speed;

            float newDistance = DistanceTo(currentWaterSource.Value);
            if (newDistance > TileSize * 3)
            {
                currentWaterSource = null;
                CurrentState = AnimalState.Wandering;
            }
        }

        /// <summary>
        /// Calculates the distance to a specific tile.
        /// </summary>
        /// <param name="tile">The tile to calculate the distance to.</param>
        /// <returns>The distance to the tile.</returns>
        private float DistanceTo(Point tile)
        {
            float tileCenterX = tile.X * TileSize + TileSize / 2;
            float tileCenterY = tile.Y * TileSize + TileSize / 2;
            float dx = PixelX - tileCenterX;
            float dy = PixelY - tileCenterY;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Finds the nearest water tile.
        /// </summary>
        /// <param name="map">The map instance.</param>
        /// <param name="excludeVisited">Whether to exclude recently visited water sources.</param>
        /// <returns>The nearest water tile, or <c>null</c> if none is found.</returns>
        protected Point? FindNearestWater(Map map, bool excludeVisited = true)
        {
            int currentTileX = (int)(PixelX / TileSize);
            int currentTileY = (int)(PixelY / TileSize);

            for (int radius = 1; radius < 10; radius++)
            {
                for (int x = currentTileX - radius; x <= currentTileX + radius; x++)
                {
                    for (int y = currentTileY - radius; y <= currentTileY + radius; y++)
                    {
                        if (x >= 0 && x < map.Width && y >= 0 && y < map.Height)
                        {
                            int tile = map.GetTile(x, y);
                            var point = new Point(x, y);

                            if ((tile == 1 || tile == 2) &&
                                (!excludeVisited || !visitedWaterSources.Contains(point)))
                            {
                                return point;
                            }
                        }
                    }
                }
            }
            if (excludeVisited && visitedWaterSources.Count > 0)
            {
                visitedWaterSources.Clear();
                return FindNearestWater(gameManager.map, false);
            }

            return null;
        }

        public abstract void Eat();

    }

    public abstract class Herbivore : Animal
    {
        protected Herbivore(GameManager gameManager, Map map) : base(gameManager, gameManager.map)
        {
        }

        public override void Eat(){}

    }

    public abstract class Carnivore : Animal
    {
        protected Carnivore(GameManager gameManager, Map map) : base(gameManager, gameManager.map)
        {
        }

        public override void Eat(){}
    }

    public class Giraffe : Herbivore
    {
        public Giraffe(GameManager gameManager, Map map)
            : base(gameManager, gameManager.map)
        {
            Speed = 1.5f;
        }

        protected override Animal CreateOffspring()
        {
            return new Giraffe(gameManager, gameManager.map);
        }
    }

    public class Rhinoceros : Herbivore
    {
        public Rhinoceros(GameManager gameManager, Map map)
            : base(gameManager, gameManager.map)
        {
            Speed = 1f;
        }

        protected override Animal CreateOffspring()
        {
            return new Rhinoceros(gameManager, gameManager.map);
        }
    }

    public class Lion : Carnivore
    {
        public Lion(GameManager gameManager, Map map) : base(gameManager, gameManager.map)
        {
            Speed = 2.5f;
        }

        protected override Animal CreateOffspring()
        {
            return new Lion(gameManager, gameManager.map);
        }
    }

    public class Tiger : Carnivore
    {
        public Tiger(GameManager gameManager, Map map) : base(gameManager, gameManager.map)
        {
            Speed = 3f;
        }

        protected override Animal CreateOffspring()
        {
            return new Tiger(gameManager, gameManager.map);
        }
    }
}
