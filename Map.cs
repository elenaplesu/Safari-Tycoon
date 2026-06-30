using System;
using System.Drawing;

namespace Safari
{

    public enum TileType
    {
        Unknown = -1, 
        River = 1,
        Pond = 2,
        Grass = 3,
        Rock = 4,
        Road = 5,
        Door = 6,
        Bush = 7,
        Tree = 8,
    }


    /// <summary>
    /// Generates 2D Simplex Noise.
    /// Based on the reference implementation by Ken Perlin.
    /// </summary>
    public class SimplexNoise
    {
        private const int GradientSizeTable = 256;
        private readonly byte[] _perm;
        private readonly float[] _gradients;
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the SimplexNoise class with a given seed.
        /// Sets up the permutation table and gradient vectors.
        /// </summary>
        /// <param name="seed">The seed for the random number generator.</param>
        public SimplexNoise(int seed)
        {
            _random = new Random(seed);
            _perm = new byte[GradientSizeTable * 2];
            _gradients = new float[GradientSizeTable * 3];

            for (int i = 0; i < GradientSizeTable; i++)
            {
                _perm[i] = (byte)i;

                // Generate random normalized gradient vectors for each hash value.
                // These vectors point in random directions on a sphere.
                float z = (float)(_random.NextDouble() * 2 - 1);
                float r = (float)Math.Sqrt(1f - z * z);
                float theta = (float)(_random.NextDouble() * 2 * Math.PI);
                _gradients[i * 3] = r * (float)Math.Cos(theta);
                _gradients[i * 3 + 1] = r * (float)Math.Sin(theta);
                _gradients[i * 3 + 2] = z; // Z component is unused in 2D noise, but kept for potential 3D extension
            }

            // Shuffle the permutation table using the Fisher-Yates algorithm.
            for (int i = 0; i < GradientSizeTable; i++)
            {
                int j = _random.Next(GradientSizeTable);
                byte temp = _perm[i];
                _perm[i] = _perm[j];
                _perm[j] = temp;
            }

            // Duplicate the permutation table to avoid index out of bounds issues
            // when calculating the hash for coordinates outside the [0, 255] range.
            for (int i = 0; i < GradientSizeTable; i++)
            {
                _perm[GradientSizeTable + i] = _perm[i];
            }
        }

        /// <summary>
        /// Calculates the 2D Simplex noise value for the given coordinates (x, y).
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The noise value, typically in the range [-1, 1].</returns>
        public float Noise2D(float x, float y)
        {
            float n0, n1, n2; // Noise contributions from the three corners

            // Skew the input space to determine which simplex cell we're in.
            // The skew factor is based on the dimension (2D).
            const float F2 = 0.366025403f; // F2 = 0.5 * (sqrt(3.0) - 1.0)
            const float G2 = 0.211324865f; // G2 = (3.0 - Math.sqrt(3.0)) / 6.0

            float s = (x + y) * F2;
            float xs = x + s;
            float ys = y + s;

            // Determine the floor of the skewed coordinates to find the origin corner (i, j) of the simplex cell.
            int i = FastFloor(xs);
            int j = FastFloor(ys);

            // Unskew the cell origin back to (x,y) space.
            // This gives the coordinates of the bottom-left corner of the square containing the simplex.
            float t = (i + j) * G2;
            float X0 = i - t;
            float Y0 = j - t;

            // Calculate the distance from the unskewed origin to the input point (x, y).
            float x0 = x - X0;
            float y0 = y - Y0;

            // For 2D, the simplex is an equilateral triangle. We need to determine
            // the other two corners based on whether (x0, y0) is in the lower or upper triangle.
            int i1, j1; // Offsets for the second corner in (i,j) skewed space
            if (x0 > y0)
            {
                // Lower triangle, order: (0,0)->(1,0)->(1,1)
                i1 = 1; j1 = 0;
            }
            else
            {
                // Upper triangle, order: (0,0)->(0,1)->(1,1)
                i1 = 0; j1 = 1;
            }

            // Calculate the coordinates of the other two corners in (x,y) unskewed space.
            // The offsets (+ G2) are due to the unskewing transformation.
            float x1 = x0 - i1 + G2;
            float y1 = y0 - j1 + G2;
            float x2 = x0 - 1.0f + 2.0f * G2; // The third corner is always at (1,1) in the skewed space
            float y2 = y0 - 1.0f + 2.0f * G2;

            // Calculate the contribution from the three corners.
            // Contribution is zero if the point is outside the circular support centered at the corner.
            // The falloff function is (0.5 - r^2)^4, where r^2 is the squared distance from the corner.
            // The noise value at each corner is the dot product of the distance vector to the corner
            // and the pseudorandom gradient vector associated with that corner's integer coordinates.

            float t0 = 0.5f - x0 * x0 - y0 * y0; // Squared distance from point to corner (i,j)
            if (t0 < 0.0f) n0 = 0.0f;
            else
            {
                t0 *= t0;
                // Calculate dot product with gradient at (i,j) and apply falloff.
                n0 = t0 * t0 * Grad(GetGradient(i, j), x0, y0);
            }

            float t1 = 0.5f - x1 * x1 - y1 * y1; // Squared distance from point to corner (i+i1, j+j1)
            if (t1 < 0.0f) n1 = 0.0f;
            else
            {
                t1 *= t1;
                // Calculate dot product with gradient at (i+i1, j+j1) and apply falloff.
                n1 = t1 * t1 * Grad(GetGradient(i + i1, j + j1), x1, y1);
            }

            float t2 = 0.5f - x2 * x2 - y2 * y2; // Squared distance from point to corner (i+1, j+1)
            if (t2 < 0.0f) n2 = 0.0f;
            else
            {
                t2 *= t2;
                // Calculate dot product with gradient at (i+1, j+1) and apply falloff.
                n2 = t2 * t2 * Grad(GetGradient(i + 1, j + 1), x2, y2);
            }

            // Sum the contributions from each corner and scale the result
            // to typically fall within the range of [-1, 1].
            return 70.0f * (n0 + n1 + n2);
        }

        /// <summary>
        /// Computes the floor of a float value, handling negative numbers correctly.
        /// This is a performance optimization over Math.Floor and explicit casting for negative numbers.
        /// </summary>
        private int FastFloor(float x)
        {
            // If x is positive, standard casting works. If negative, casting truncates towards zero,
            // so we need to subtract 1 unless x is exactly an integer.
            return x > 0 ? (int)x : (int)x - 1;
        }

        /// <summary>
        /// Gets the pseudorandom gradient index for a given pair of integer coordinates (i, j).
        /// Uses the permutation table to combine the coordinates into a hash,
        /// which is then used as an index into the gradient table.
        /// </summary>
        private int GetGradient(int i, int j)
        {
            // Mask with 0xFF to wrap coordinates around the table size (256).
            // This ensures that coordinates outside [0, 255] are handled correctly
            // by leveraging the duplicated part of the permutation table.
            return _perm[(i + _perm[j & 0xFF]) & 0xFF] & 0xFF;
        }

        /// <summary>
        /// Calculates the dot product of the pseudorandom gradient vector (determined by hash)
        /// and the distance vector from the corner to the evaluation point (x, y).
        /// The 'hash' value is used to select one of 16 predefined gradient vectors.
        /// </summary>
        private float Grad(int hash, float x, float y)
        {
            // The hash value (0-15) determines which of the 16 gradient vectors to use.
            // These vectors are permutations of (+-1, +-1), (+-1, 0), (0, +-1), (+-1, +-1) etc.
            // and are effectively mapped to dot products efficiently using bitwise operations on the hash.
            int h = hash & 15;
            float u = h < 8 ? x : y; // Component depends on hash
            // V component logic is slightly more complex based on specific hash values.
            float v = h < 4 ? y : h == 12 || h == 14 ? x : 0;

            // Based on bits in hash, apply signs (+ or -) to u and v.
            // h & 1 determines sign of u, h & 2 determines sign of v.
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
    }

    /// <summary>
    /// Represents the game map, holding tile data and providing methods for generation.
    /// </summary>
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        private int[][] tiles;
        public Random random;

        /// <summary>
        /// Defines standard colors for different tile types.
        /// </summary>
        public static class TileColors
        {
            public static readonly Color Pond = Color.LightBlue;
            public static readonly Color Grass = Color.Green;
            public static readonly Color Rock = Color.Gray;
            public static readonly Color Road = Color.Red;
            public static readonly Color Door = Color.Brown;
            public static readonly Color River = Color.Blue; 
        }

        /// <summary>
        /// Initializes a new instance of the Map class with specified dimensions.
        /// Generates the map content upon creation.
        /// </summary>
        /// <param name="width">The width of the map.</param>
        /// <param name="height">The height of the map.</param>
        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            tiles = new int[width][];
            random = new Random();

            for (int i = 0; i < width; i++)
            {
                tiles[i] = new int[height];
            }

            GenerateSimplexNoiseMap();
        }

        /// <summary>
        /// Generates the map content using Simplex noise to distribute features like ponds and rocks.
        /// Also adds a random river and fixed entry/exit points.
        /// </summary>
        public void GenerateSimplexNoiseMap()
        {
            SimplexNoise noise = new SimplexNoise(random.Next());

            // Scale factor for noise. Controls the 'zoom' level of the noise pattern.
            // A larger scale value results in more smaller features.
            float scale = 0.2f;

            // Initialize the map with grass tiles as the base terrain.
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    tiles[x][y] = 3; 
                }
            }

            // Define the target percentage of ponds and rocks.
            int desiredPonds = (int)(Width * Height * 0.02);
            int desiredRocks = (int)(Width * Height * 0.02);
            int pondsCreated = 0;
            int rocksCreated = 0;

            // Store potential feature locations along with their noise values.
            // This allows sorting based on noise for placing features in areas of high/low noise.
            var potentialFeatureLocations = new System.Collections.Generic.List<Tuple<int, int, float>>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // Get noise value between -1 and 1 for each tile position.
                    float noiseValue = noise.Noise2D(x * scale, y * scale);

                    potentialFeatureLocations.Add(new Tuple<int, int, float>(x, y, noiseValue));
                }
            }

            // Sort locations by noise value in descending order to place rocks at highest peaks.
            potentialFeatureLocations.Sort((a, b) => b.Item3.CompareTo(a.Item3));

            // Iterate through sorted locations and place rocks until the desired count is reached,
            // checking the IsValidFeatureLocation method to prevent adjacent rocks.
            int i = 0;
            while (rocksCreated < desiredRocks && i < potentialFeatureLocations.Count)
            {
                int x = potentialFeatureLocations[i].Item1;
                int y = potentialFeatureLocations[i].Item2;

                if (IsValidFeatureLocation(x, y))
                {
                    tiles[x][y] = 4; // Rock 
                    rocksCreated++;
                }

                i++;
            }

            // Sort locations by noise value in ascending order to place ponds at lowest valleys.
            potentialFeatureLocations.Sort((a, b) => a.Item3.CompareTo(b.Item3));

            // Iterate through sorted locations and place ponds until the desired count is reached,
            // checking the IsValidFeatureLocation method to prevent adjacent ponds or rocks.
            i = 0;
            while (pondsCreated < desiredPonds && i < potentialFeatureLocations.Count)
            {
                int x = potentialFeatureLocations[i].Item1;
                int y = potentialFeatureLocations[i].Item2;

                if (IsValidFeatureLocation(x, y))
                {
                    tiles[x][y] = 2; // Pond 
                    pondsCreated++;
                }

                i++;
            }

            // Generate a simple river.
            // Starts at a random point and moves in a generally fixed direction with small random changes.
            int riverLength = Width; 
            int startX = random.Next(Width);
            int startY = random.Next(Height);
            int direction = random.Next(4); // 0: up, 1: right, 2: down, 3: left

            for (int r = 0; r < riverLength; r++)
            {
                if (startX >= 0 && startX < Width && startY >= 0 && startY < Height)
                {
                    tiles[startX][startY] = 1; // River 
                }

                // Move to the next tile based on current direction, clamping to map bounds.
                switch (direction)
                {
                    case 0: 
                        startY = (startY > 0) ? startY - 1 : startY;
                        break;
                    case 1: 
                        startX = (startX < Width - 1) ? startX + 1 : startX;
                        break;
                    case 2: 
                        startY = (startY < Height - 1) ? startY + 1 : startY;
                        break;
                    case 3: 
                        startX = (startX > 0) ? startX - 1 : startX;
                        break;
                }

                // Randomly change direction to create bends in the river.
                if (random.Next(100) < 20) // here is 20 percent chance
                {
                    direction = random.Next(4);
                }
            }

            // Add fixed entry and exit points at the center of the top and bottom rows.
            int entryX = Width / 2; 
            int entryY = 0; 
            tiles[entryX][entryY] = 6; // Door 

            int exitX = Width / 2; 
            int exitY = Height - 1;
            tiles[exitX][exitY] = 6; // Door 
        }

        /// <summary>
        /// Checks if a given location is suitable for placing a feature (pond or rock)
        /// by checking if it is currently grass and has no adjacent ponds or rocks.
        /// </summary>
        /// <param name="x">The x-coordinate to check.</param>
        /// <param name="y">The y-coordinate to check.</param>
        /// <returns>True if the location is valid for placing a feature, false otherwise.</returns>
        public bool IsValidFeatureLocation(int x, int y)
        {
            // Location is only valid if it's currently a grass tile.
            if (tiles[x][y] != 3) 
                return false;

            // Check all 8 adjacent tiles (including diagonals).
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // current tile

                    int nx = x + dx;
                    int ny = y + dy;

                    // Check if the adjacent tile is within map bounds.
                    if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                    {
                        // If any adjacent tile is a pond or rock, this location is not valid
                        if (tiles[nx][ny] == 2 || tiles[nx][ny] == 4)
                            return false;
                    }
                }
            }

            return true; 
        }

        /// <summary>
        /// Generates map content using simple random placement of features.
        /// This method is an alternative to the Simplex noise generation.
        /// </summary>
        private void GenerateRandomMap()
        {
            // Initialize with grass.
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    tiles[x][y] = 3; // Grass
                }
            }

            int riverLength = Width;
            int startX = random.Next(Width);
            int startY = random.Next(Height);
            int direction = random.Next(4);

            for (int i = 0; i < riverLength; i++)
            {
                if (startX >= 0 && startX < Width && startY >= 0 && startY < Height)
                {
                    tiles[startX][startY] = 1; // River
                }

                switch (direction)
                {
                    case 0: startY = (startY > 0) ? startY - 1 : startY; break;
                    case 1: startX = (startX < Width - 1) ? startX + 1 : startX; break;
                    case 2: startY = (startY < Height - 1) ? startY + 1 : startY; break;
                    case 3: startX = (startX > 0) ? startX - 1 : startX; break;
                }

                if (random.Next(100) < 20)
                {
                    direction = random.Next(4);
                }
            }

            // Randomly place ponds and rocks on grass tiles.
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (tiles[x][y] == 3) 
                    {
                        int value = random.Next(100);
                        if (value < 2) 
                        {
                            tiles[x][y] = 2; // Pond
                        }
                        else if (value < 4) 
                        {
                            tiles[x][y] = 4; // Rock
                        }
                    
                    }
                }
            }

            //Fixed entry and exit points.
            int entryX = Width / 2;
            int entryY = 0;
            tiles[entryX][entryY] = 6; // Door

            int exitX = Width / 2;
            int exitY = Height - 1;
            tiles[exitX][exitY] = 6; // Door
        }

        /// <summary>
        /// Gets the tile type at the specified coordinates.
        /// Returns -1 if the coordinates are out of bounds.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The integer representing the tile type, or -1 if out of bounds.</returns>
        public int GetTile(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return tiles[x][y];
            }
            return -1; // Indicate out of bounds
        }

        /// <summary>
        /// Sets the tile type at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="value">The integer value representing the new tile type.</param>
        public void SetTile(int x, int y, int value)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                tiles[x][y] = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(x), "Coordinates are out of bounds.");
            }
        }

        /// <summary>
        /// Gets the corresponding System.Drawing.Color for a given tile type integer value.
        /// </summary>
        /// <param name="tileType">The integer value representing the tile type.</param>
        /// <returns>The Color associated with the tile type.</returns>
        public Color GetTileColor(int tileType)
        {
            switch (tileType)
            {
                case 1: return TileColors.River; // Added River mapping based on usage in Generate methods
                case 2: return TileColors.Pond;
                case 3: return TileColors.Grass;
                case 4: return TileColors.Rock;
                case 5: return TileColors.Road;
                case 6: return TileColors.Door;
                default: return Color.Black; // Default for unknown tile types
            }
        }
    }
}