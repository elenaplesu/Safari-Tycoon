using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Safari;
using System.Drawing;

namespace Safari.Tests
{
    [TestClass]
    public class MapTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializeMapWithCorrectDimensions()
        {
            // Arrange
            int width = 10;
            int height = 15;

            // Act
            Map map = new Map(width, height);

            // Assert
            Assert.AreEqual(width, map.Width);
            Assert.AreEqual(height, map.Height);
        }

        [TestMethod]
        public void GetTile_ShouldReturnCorrectTileType()
        {
            // Arrange
            Map map = new Map(10, 10);
            map.SetTile(2, 3, 4); // Set a rock tile

            // Act
            int tileType = map.GetTile(2, 3);

            // Assert
            Assert.AreEqual(4, tileType);
        }

        [TestMethod]
        public void GetTile_ShouldReturnMinusOneForOutOfBounds()
        {
            // Arrange
            Map map = new Map(10, 10);

            // Act
            int tileType = map.GetTile(-1, -1);

            // Assert
            Assert.AreEqual(-1, tileType);
        }

        [TestMethod]
        public void SetTile_ShouldSetTileTypeCorrectly()
        {
            // Arrange
            Map map = new Map(10, 10);

            // Act
            map.SetTile(5, 5, 2); // Set a pond tile

            // Assert
            Assert.AreEqual(2, map.GetTile(5, 5));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetTile_ShouldThrowExceptionForOutOfBounds()
        {
            // Arrange
            Map map = new Map(10, 10);

            // Act
            map.SetTile(-1, -1, 3); // Invalid coordinates
        }

        [TestMethod]
        [Ignore("Implementation pending")]
        public void GetTileColor_ShouldReturnCorrectColor()
        {
            // Arrange
            Map map = new Map(10, 10);

            // Act
            Color color = map.GetTileColor(3); // Grass 

            // Assert
            Assert.AreEqual(Color.Green, color);
        }

        [TestMethod]
        [Ignore("Implementation pending")]
        public void GetTileColor_ShouldReturnBlackForUnknownTileType()
        {
            // Arrange
            Map map = new Map(10, 10);

            // Act
            Color color = map.GetTileColor(-1); // Unknown tile type

            // Assert
            Assert.AreEqual(Color.Black, color);
        }

        [TestMethod]
        public void IsValidFeatureLocation_ShouldReturnFalseForInvalidLocation()
        {
            // Arrange
            Map map = new Map(10, 10);
            map.SetTile(5, 5, 2); // Set a pond tile

            // Act
            bool isValid = map.IsValidFeatureLocation(5, 5);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void GenerateSimplexNoiseMap_ShouldGenerateValidMap()
        {
            // Arrange
            Map map = new Map(10, 10);

            // Act
            map.GenerateSimplexNoiseMap();

            // Assert
            // Check that the map contains at least one river, pond, and rock
            bool hasRiver = false, hasPond = false, hasRock = false;
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    int tile = map.GetTile(x, y);
                    if (tile == 1) hasRiver = true;
                    if (tile == 2) hasPond = true;
                    if (tile == 4) hasRock = true;
                }
            }

            Assert.IsTrue(hasRiver);
            Assert.IsTrue(hasPond);
            Assert.IsTrue(hasRock);
        }
    }
}