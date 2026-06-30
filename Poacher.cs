using System;

namespace Safari
{
    public class Poacher
    {
        public float PixelX { get; set; }
        public float PixelY { get; set; }
        private Direction currentDirection;
        private readonly Random random = new Random();
        public float Speed { get; protected set; } = 2f;
        public const int TileSize = 32;
        private const int MaxX = 30;
        private const int MaxY = 21;

        public enum Direction { Up, Down, Left, Right }

        public Poacher(int startX, int startY)
        {
            PixelX = startX * TileSize + TileSize / 2;
            PixelY = startY * TileSize + TileSize / 2;
            currentDirection = GetRandomDirection();
        }

        private float clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public void TryMove(Func<int, int, int> CheckPoacherMove)
        {
            float newPixelX = PixelX;
            float newPixelY = PixelY;

            switch (currentDirection)
            {
                case Direction.Up:
                    newPixelY -= Speed;
                    newPixelY = clamp(newPixelY, TileSize / 2, MaxY * TileSize - TileSize / 2);
                    break;

                case Direction.Down:
                    newPixelY += Speed;
                    newPixelY = clamp(newPixelY, TileSize / 2, MaxY * TileSize - TileSize / 2);
                    break;

                case Direction.Left:
                    newPixelX -= Speed;
                    newPixelX = clamp(newPixelX, TileSize / 2, MaxX * TileSize - TileSize / 2);
                    break;

                case Direction.Right:
                    newPixelX += Speed;
                    newPixelX = clamp(newPixelX, TileSize / 2, MaxX * TileSize - TileSize / 2);
                    break;
            }

            int tileX = (int)(newPixelX / TileSize);
            int tileY = (int)(newPixelY / TileSize);

            int moveStatus = CheckPoacherMove(tileX, tileY);

            if (moveStatus == 0 || moveStatus == 1)
            {
                PixelX = newPixelX;
                PixelY = newPixelY;
            }
            else
            {
                ChangeDirection();
            }
            if (random.Next(5) == 0)
                ChangeDirection();
        }

        private void ChangeDirection() => currentDirection = GetRandomDirection();
        private Direction GetRandomDirection() => (Direction)random.Next(0, 4);
    }
}