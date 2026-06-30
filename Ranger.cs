using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime;


namespace Safari
{
    public class Ranger
    {
        public float PixelX { get; set; }
        public float PixelY { get; set; }
        private readonly Random random = new Random();
        public float Speed { get; protected set; } = 2f;
        private GameManager gameManager { get; set; }
        private String target;
        public const int TileSize = 32;
        private const int MaxX = 30;
        private const int MaxY = 21;


        public Ranger(int startX, int startY, String animal, GameManager manager)
        {
            PixelX = startX * TileSize + TileSize / 2;
            PixelY = startY * TileSize + TileSize / 2;
            target = animal;
            gameManager = manager;
        }

        public void MoveToClosestTarget(List<Animal> animals, List<Poacher> poachers)
        {

            if (target == "Poacher" && poachers != null && poachers.Count > 0)
            {
                Poacher closestPoacher = null;
                float closestPoacherDist = float.MaxValue;

                foreach (var poacher in poachers)
                {
                    float dist = Distance(PixelX, PixelY, poacher.PixelX, poacher.PixelY);
                    if (dist < closestPoacherDist)
                    {
                        closestPoacherDist = dist;
                        closestPoacher = poacher;
                    }
                }

                if (closestPoacher != null && closestPoacherDist <= 10 * TileSize)
                {
                    float dx = closestPoacher.PixelX - PixelX;
                    float dy = closestPoacher.PixelY - PixelY;

                    float length = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (length > 0)
                    {
                        dx /= length;
                        dy /= length;
                    }

                    PixelX += dx * Speed;
                    PixelY += dy * Speed;

                    if (Distance(PixelX, PixelY, closestPoacher.PixelX, closestPoacher.PixelY) <= TileSize * 3f)
                    {
                        gameManager.RemovePoacher(closestPoacher);
                        Die();
                        return;
                    }
                    return;
                }
            }

            CheckForPoachersInRange();

            if (animals == null || animals.Count == 0)
            {
                move();
                return;
            }

            Animal closestAnimal = null;
            float closestDistance = float.MaxValue;

            foreach (var animal in animals)
            {
                if (animal.GetType().Name == target)
                {
                    float dist = Distance(PixelX, PixelY, animal.PixelX, animal.PixelY);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestAnimal = animal;
                    }
                }
            }

            if (closestAnimal != null)
            {
                float dx = closestAnimal.PixelX - PixelX;
                float dy = closestAnimal.PixelY - PixelY;

                float length = (float)Math.Sqrt(dx * dx + dy * dy);
                if (length > 0)
                {
                    dx /= length;
                    dy /= length;
                }

                PixelX += dx * Speed;
                PixelY += dy * Speed;
                if (Distance(PixelX, PixelY, closestAnimal.PixelX, closestAnimal.PixelY) <= 1f)
                {
                    if (target == "Lion")
                    {
                        gameManager.UpdateCapital(+500);
                    } else if (target == "Tiger")
                    {
                        gameManager.UpdateCapital(+600);
                    }
                    closestAnimal.Die();
                    Die();
                }
            } else
            {
                move();
            }
        }

        private void move()
        {
            PixelX += Speed * (random.NextDouble() > 0.5 ? 1 : -1);
            PixelY += Speed * (random.NextDouble() > 0.5 ? 1 : -1);
            PixelX = clamp(PixelX, TileSize / 2, MaxX * TileSize - TileSize / 2);
            PixelY = clamp(PixelY, TileSize / 2, MaxY * TileSize - TileSize / 2);
        }

        private float clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void Die()
        {
            gameManager.RemoveRanger(this);
        }

        private float Distance(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public void CheckForPoachersInRange()
        {
            List<Poacher> currentPoachers = gameManager.GetPoachers();
            const float killRadius = 3 * TileSize;
            List<Poacher> poachersToRemove = new List<Poacher>();

            foreach (var poacher in currentPoachers)
            {
                float currentDistance = Distance(PixelX, PixelY, poacher.PixelX, poacher.PixelY);

                if (currentDistance <= killRadius)
                {
                    poachersToRemove.Add(poacher);
                }
            }
            foreach (var poacher in poachersToRemove)
            {
                gameManager.RemovePoacher(poacher);
            }
        }
    }
}