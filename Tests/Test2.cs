using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Safari.Tests
{
    [TestClass]
    public class AnimalTests
    {
        private GameManager gameManager;
        private Map map;

        [TestInitialize]
        public void TestInitialize()
        {
            map = new Map(10, 10);
            gameManager = new GameManager(map, Difficulty.Medium);
        }

        [TestMethod]
        public void TestSetPosition()
        {
            // Arrange
            var giraffe = new Giraffe(gameManager, map);

            // Act
            giraffe.SetPosition(100, 200);

            // Assert
            Assert.AreEqual(100, giraffe.PixelX);
            Assert.AreEqual(200, giraffe.PixelY);
        }

        [TestMethod]
        public void TestUpdateAge()
        {
            // Arrange
            var giraffe = new Giraffe(gameManager, map);

            // Act
            System.Threading.Thread.Sleep(1000); // Simulate time passing
            giraffe.UpdatePosition(map);

            // Assert
            Assert.IsTrue(giraffe.CurrentAge == Animal.AgeStage.Young || giraffe.CurrentAge == Animal.AgeStage.Adult);
        }

        //[TestMethod]
        //public void TestIsStarving()
        //{
        //    // Arrange
        //    var lion = new Lion(gameManager, map);

        //    // Act
        //    System.Threading.Thread.Sleep(5000); // Simulate time passing
        //    bool isStarving = lion.IsStarving;

        //    // Assert
        //    Assert.IsTrue(isStarving);
        //}

        [TestMethod]
        public void TestDie()
        {
            // Arrange
            var tiger = new Tiger(gameManager, map);
            gameManager.AddAnimal(tiger);

            // Act
            tiger.Die();

            // Assert
            Assert.IsFalse(gameManager.GetAnimals().Contains(tiger));
        }


        //[TestMethod]
        //public void TestRestingState()
        //{
        //    // Arrange
        //    var lion = new Lion(gameManager, map);
        //    map.SetTile(1, 1, 1); // Set a water tile
        //    lion.SetPosition(32, 32);

        //    // Act
        //    lion.UpdatePosition(map);

        //    // Assert
        //    Assert.AreEqual(Animal.AnimalState.Resting, lion.CurrentState);
        //}
    }
}
