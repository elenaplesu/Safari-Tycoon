using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Safari.Tests
{
    [TestClass]
    public class GameManagerTests
    {
        private GameManager gameManager;
        private Map map;

        [TestInitialize]
        public void TestInitialize()
        {
            map = new Map(20, 20);
            gameManager = new GameManager(map, Difficulty.Medium);
        }

        [TestMethod]
        public void TestAddAnimal()
        {
            // Arrange
            var lion = new Lion(gameManager, map);
            int initialCount = gameManager.GetNumberOfAnimals();

            // Act
            gameManager.AddAnimal(lion);

            // Assert
            Assert.AreEqual(initialCount + 1, gameManager.GetNumberOfAnimals());
            Assert.IsTrue(gameManager.GetAnimals().Contains(lion));
        }

        [TestMethod]
        public void TestRemoveAnimal()
        {
            // Arrange
            var lion = new Lion(gameManager, map);
            gameManager.AddAnimal(lion);
            int initialCount = gameManager.GetNumberOfAnimals();

            // Act
            gameManager.RemoveAnimal(lion);

            // Assert
            Assert.AreEqual(initialCount - 1, gameManager.GetNumberOfAnimals());
            Assert.IsFalse(gameManager.GetAnimals().Contains(lion));
        }

        [TestMethod]
        public void TestUpdateVisitors()
        {
            // Arrange
            var lion1 = new Lion(gameManager, map);
            var lion2 = new Lion(gameManager, map);
            gameManager.AddAnimal(lion1);
            gameManager.AddAnimal(lion2);

            // Act
            int expectedVisitors = (int)(gameManager.GetNumberOfAnimals() * 1.5);

            // Assert
            Assert.AreEqual(expectedVisitors, gameManager.GetNumberOfVisitors());
        }

        [TestMethod]
        public void TestUpdateCapital()
        {
            // Arrange
            int initialCapital = gameManager.GetCapital();
            int amount = 500;

            // Act
            gameManager.UpdateCapital(amount);

            // Assert
            Assert.AreEqual(initialCapital + amount, gameManager.GetCapital());
        }

        [TestMethod]
        public void TestFindAvailablePosition()
        {
            // Act
            Point position = gameManager.FindAvailablePosition();

            // Assert
            Assert.AreNotEqual(Point.Empty, position);
            Assert.AreEqual(3, map.GetTile(position.X, position.Y)); // 3 is grass
        }

        [TestMethod]
        public void TestIsPositionOccupied()
        {
            // Arrange
            var tiger = new Tiger(gameManager, map);
            tiger.SetPosition(32, 32); // Set to position (1,1) in tile coordinates
            gameManager.AddAnimal(tiger);

            // Act & Assert
            Assert.IsTrue(gameManager.IsPositionOccupied(1, 1));
            Assert.IsFalse(gameManager.IsPositionOccupied(2, 2));
        }

        [TestMethod]
        public void TestAddAndGetRanger()
        {
            // Arrange
            var ranger = new Ranger(5, 5, "Lion", gameManager);

            // Act
            gameManager.AddRanger(ranger);

            // Assert
            Assert.IsTrue(gameManager.GetRangers().Contains(ranger));
        }

        [TestMethod]
        public void TestAddAndGetPoacher()
        {
            // Arrange
            var poacher = new Poacher(2, 2);

            // Act
            gameManager.AddPoacher(poacher);

            // Assert
            Assert.IsTrue(gameManager.GetPoachers().Contains(poacher));
        }

        [TestMethod]
        public void TestAddJeep()
        {
            // Arrange
            int initialCount = gameManager.GetNumberOfJeeps();

            // Act
            gameManager.AddJeep();

            // Assert
            Assert.AreEqual(initialCount + 1, gameManager.GetNumberOfJeeps());
        }

        [TestMethod]
        public void TestHasMinimumVisitors()
        {
            // Arrange
            gameManager.AddVisitors(100);

            // Act & Assert
            Assert.IsTrue(gameManager.HasMinimumVisitors(50));
            Assert.IsFalse(gameManager.HasMinimumVisitors(200));
        }

        [TestMethod]
        public void TestHasMinimumCapital()
        {
            // Arrange - Start with 1,000,000 capital from constructor

            // Act & Assert
            Assert.IsTrue(gameManager.HasMinimumCapital(500000));
            Assert.IsTrue(gameManager.HasMinimumCapital(1000000));
            Assert.IsFalse(gameManager.HasMinimumCapital(1500000));
        }
    }
}
