using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Safari.Tests
{
    [TestClass]
    public class JeepTests
    {
        private Map map;

        [TestInitialize]
        public void TestInitialize()
        {
            map = new Map(10, 10);

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    map.SetTile(x, y, 0); // Default to grass
                }
            }

            map.SetTile(1, 1, 6); // Top door
            map.SetTile(1, 8, 6); // Bottom door
            for (int y = 2; y < 8; y++)
            {
                map.SetTile(1, y, 5); // Road
            }
        }

        [TestMethod]
        public void TestStartRoute_ValidPath()
        {
            var jeep = new Jeep();

            jeep.StartRoute(map);

            Assert.IsTrue(jeep.IsMoving, "Jeep should start moving when a valid path exists.");
            Assert.AreEqual(1 * 32 + 16, jeep.PixelX, "Jeep should start at the first tile of the path.");
            Assert.AreEqual(1 * 32 + 16, jeep.PixelY, "Jeep should start at the first tile of the path.");
        }

        [TestMethod]
        public void TestStartRoute_NoPath()
        {
            // Arrange
            var jeep = new Jeep();

            map.SetTile(1, 8, 0); // Remove bottom door

            // Act
            jeep.StartRoute(map);

            // Assert
            Assert.IsFalse(jeep.IsMoving, "Jeep should not start moving when no valid path exists.");
        }

        [TestMethod]
        public void TestUpdatePosition_CompletesRoute()
        {
            var jeep = new Jeep();
            jeep.StartRoute(map);

            while (jeep.IsMoving)
            {
                jeep.UpdatePosition();
            }
            Assert.IsFalse(jeep.IsMoving, "Jeep should stop moving after completing the route.");
        }

        [TestMethod]
        public void TestHasPath_ValidPath()
        {
            bool hasPath = Jeep.HasPath(map);

            Assert.IsTrue(hasPath, "Jeep should detect a valid path on the map.");
        }

        [TestMethod]
        public void TestHasPath_NoPath()
        {
            map.SetTile(1, 8, 0); // Remove bottom door

            bool hasPath = Jeep.HasPath(map);

            Assert.IsFalse(hasPath, "Jeep should not detect a path when no valid path exists.");
        }

        [TestMethod]
        public void TestFindPath_ValidPath()
        {
            var jeep = new Jeep();

            // Act
            bool foundPath = jeep.FindPath(map);

            // Assert
            Assert.IsTrue(foundPath, "Jeep should find a valid path on the map.");
        }

        [TestMethod]
        public void TestFindPath_NoPath()
        {
            // Arrange
            var jeep = new Jeep();

            // Modify the map to remove the bottom door
            map.SetTile(1, 8, 0); // Remove bottom door

            // Act
            bool foundPath = jeep.FindPath(map);

            // Assert
            Assert.IsFalse(foundPath, "Jeep should not find a path when no valid path exists.");
        }
    }
}
