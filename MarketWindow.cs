using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Safari
{
    public partial class MarketWindow : Form
    {
        private GameManager gameManager;
        private Action updateCapitalDisplay;
        private Button buyGiraffeButton;
        private Button buyRhinocerosButton;
        private Button buyLionButton;
        private Button buyTigerButton;
        private Button hireRangerButton;
        private Button killLionButton;
        private Button killTigerButton;
        private Button sellAnimalButton;
        private Button sellGiraffeButton;
        private Button sellRhinocerosButton;
        private Button sellLionButton;
        private Button sellTigerButton;
        private Button feedAnimalsButton;
        private Button feedHerbivoresButton;
        private Button feedCarnivoresButton;


        private GameWindow gameWindow;



        public MarketWindow(GameManager gameManager, Action updateCapitalDisplay, GameWindow gameWindow)
        {
            this.gameManager = gameManager;
            this.updateCapitalDisplay = updateCapitalDisplay;
            this.gameWindow = gameWindow;
            InitializeComponent();
            this.Text = "Market";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(650, 650); // Increased height to fit the animal buttons

            // Add a label for the market
            Label marketLabel = new Label();
            marketLabel.Text = "Welcome to the Market!";
            marketLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            marketLabel.AutoSize = true;
            marketLabel.Location = new Point(50, 20);
            this.Controls.Add(marketLabel);

            // Add a button to buy animals
            Button buyAnimalButton = new Button();
            buyAnimalButton.Text = "Buy Animal";
            buyAnimalButton.Size = new Size(120, 50);
            buyAnimalButton.Location = new Point(50, 80);
            buyAnimalButton.Click += (sender, e) => ShowAnimalButtons();
            this.Controls.Add(buyAnimalButton);

            // Add a button to hire ranger
            Button hireRangerButton = new Button();
            hireRangerButton.Text = "Hire Ranger ($100 - salary)";
            hireRangerButton.Size = new Size(120, 50);
            hireRangerButton.Location = new Point(50, 140);
            hireRangerButton.Click += (sender, e) => ShowHuntButtons();
            this.Controls.Add(hireRangerButton);


            // Add a button to buy jeeps
            Button buyJeepButton = new Button();
            buyJeepButton.Text = "Buy Jeep ($300)";
            buyJeepButton.Size = new Size(120, 50);
            buyJeepButton.Location = new Point(50, 200);
            buyJeepButton.Click += (sender, e) => BuyJeep();
            this.Controls.Add(buyJeepButton);

            // Add a button to buy pond
            Button buyPondButton = new Button();
            buyPondButton.Text = "Buy Pond ($100)";
            buyPondButton.Size = new Size(120, 50);
            buyPondButton.Location = new Point(50, 260);
            buyPondButton.Click += (sender, e) => BuyPond();
            this.Controls.Add(buyPondButton);

            // button to buy pond
            Button buyBushButton = new Button();
            buyBushButton.Text = "Buy Bush ($100)";
            buyBushButton.Size = new Size(120, 50);
            buyBushButton.Location = new Point(50, 320);
            buyBushButton.Click += (sender, e) => BuyBush();
            this.Controls.Add(buyBushButton);

            // button to buy Tree
            Button buyTreeButton = new Button();
            buyTreeButton.Text = "Buy Tree ($100)";
            buyTreeButton.Size = new Size(120, 50);
            buyTreeButton.Location = new Point(50, 380);
            buyTreeButton.Click += (sender, e) => BuyTree();
            this.Controls.Add(buyTreeButton);

            // button to buy roads
            Button buyRoadButton = new Button();
            buyRoadButton.Text = "Buy Road (5 for $30)";
            buyRoadButton.Size = new Size(120, 50);
            buyRoadButton.Location = new Point(50, 440);
            buyRoadButton.Click += (sender, e) => BuyRoad();
            this.Controls.Add(buyRoadButton);

            //button to sell animals
            sellAnimalButton = new Button();
            sellAnimalButton.Text = "Sell Animal";
            sellAnimalButton.Size = new Size(120, 50);
            sellAnimalButton.Location = new Point(50, 500);
            sellAnimalButton.Click += (sender, e) => ShowSellAnimalButtons();
            this.Controls.Add(sellAnimalButton);

            // Initialize sell animal buttons (hidden by default)
            sellGiraffeButton = new Button();
            sellGiraffeButton.Text = "Giraffe ($150)";
            sellGiraffeButton.Size = new Size(120, 50);
            sellGiraffeButton.Location = new Point(300, 80);
            sellGiraffeButton.Click += (sender, e) => SellAnimal("Giraffe");
            sellGiraffeButton.Visible = false;
            this.Controls.Add(sellGiraffeButton);

            sellRhinocerosButton = new Button();
            sellRhinocerosButton.Text = "Rhinoceros ($150)";
            sellRhinocerosButton.Size = new Size(120, 50);
            sellRhinocerosButton.Location = new Point(300, 140);
            sellRhinocerosButton.Click += (sender, e) => SellAnimal("Rhinoceros");
            sellRhinocerosButton.Visible = false;
            this.Controls.Add(sellRhinocerosButton);

            sellLionButton = new Button();
            sellLionButton.Text = "Lion ($150)";
            sellLionButton.Size = new Size(120, 50);
            sellLionButton.Location = new Point(300, 200);
            sellLionButton.Click += (sender, e) => SellAnimal("Lion");
            sellLionButton.Visible = false;
            this.Controls.Add(sellLionButton);

            sellTigerButton = new Button();
            sellTigerButton.Text = "Tiger ($150)";
            sellTigerButton.Size = new Size(120, 50);
            sellTigerButton.Location = new Point(300, 260);
            sellTigerButton.Click += (sender, e) => SellAnimal("Tiger");
            sellTigerButton.Visible = false;
            this.Controls.Add(sellTigerButton);

            // Initialize animal buttons (hidden by default)
            buyGiraffeButton = new Button();
            buyGiraffeButton.Text = "Giraffe ($200)";
            buyGiraffeButton.Size = new Size(120, 50);
            buyGiraffeButton.Location = new Point(200, 80);
            buyGiraffeButton.Click += (sender, e) => BuyAnimal("Giraffe", 200);
            buyGiraffeButton.Visible = false;
            this.Controls.Add(buyGiraffeButton);

            buyRhinocerosButton = new Button();
            buyRhinocerosButton.Text = "Rhinoceros ($200)";
            buyRhinocerosButton.Size = new Size(120, 50);
            buyRhinocerosButton.Location = new Point(200, 140);
            buyRhinocerosButton.Click += (sender, e) => BuyAnimal("Rhinoceros", 200);
            buyRhinocerosButton.Visible = false;
            this.Controls.Add(buyRhinocerosButton);

            buyLionButton = new Button();
            buyLionButton.Text = "Lion ($300)";
            buyLionButton.Size = new Size(120, 50);
            buyLionButton.Location = new Point(200, 200);
            buyLionButton.Click += (sender, e) => BuyAnimal("Lion", 300);
            buyLionButton.Visible = false;
            this.Controls.Add(buyLionButton);

            buyTigerButton = new Button();
            buyTigerButton.Text = "Tiger ($300)";
            buyTigerButton.Size = new Size(120, 50);
            buyTigerButton.Location = new Point(200, 260);
            buyTigerButton.Click += (sender, e) => BuyAnimal("Tiger", 300);
            buyTigerButton.Visible = false;
            this.Controls.Add(buyTigerButton);

            killLionButton = new Button();
            killLionButton.Text = "Lion";
            killLionButton.Size = new Size(120, 50);
            killLionButton.Location = new Point(200, 80);
            killLionButton.Click += (sender, e) => HuntAnimal("Lion");
            killLionButton.Visible = false;
            this.Controls.Add(killLionButton);

            killTigerButton = new Button();
            killTigerButton.Text = "Tiger";
            killTigerButton.Size = new Size(120, 50);
            killTigerButton.Location = new Point(200, 140);
            killTigerButton.Click += (sender, e) => HuntAnimal("Tiger");
            killTigerButton.Visible = false;
            this.Controls.Add(killTigerButton);

            feedAnimalsButton = new Button();
            feedAnimalsButton.Text = "Feed Animals";
            feedAnimalsButton.Size = new Size(120, 50);
            feedAnimalsButton.Location = new Point(50, 560);
            feedAnimalsButton.Click += (sender, e) => ShowFeedButtons();
            this.Controls.Add(feedAnimalsButton);

            feedHerbivoresButton = new Button();
            feedHerbivoresButton.Size = new Size(120, 50);
            feedHerbivoresButton.Location = new Point(200, 80);
            feedHerbivoresButton.Click += (sender, e) => FeedHerbivores();
            feedHerbivoresButton.Visible = false;
            this.Controls.Add(feedHerbivoresButton);

            feedCarnivoresButton = new Button();
            feedCarnivoresButton.Size = new Size(120, 50);
            feedCarnivoresButton.Location = new Point(200, 140);
            feedCarnivoresButton.Click += (sender, e) => FeedCarnivores();
            feedCarnivoresButton.Visible = false;
            this.Controls.Add(feedCarnivoresButton);
        }

        private void ShowFeedButtons()
        {
            HideAnimalButtons();
            HideHuntButtons();
            HideSellButtons();

            // Calculate number of animals in each category
            int herbivoreCount = gameManager.GetAnimals().Count(a => a is Herbivore);
            int carnivoreCount = gameManager.GetAnimals().Count(a => a is Carnivore);

            // Update button texts with prices
            feedHerbivoresButton.Text = $"Feed Herbivores\n({herbivoreCount} × $25 = ${herbivoreCount * 25})";
            feedCarnivoresButton.Text = $"Feed Carnivores\n({carnivoreCount} × $25 = ${carnivoreCount * 25})";

            feedHerbivoresButton.Visible = herbivoreCount > 0;
            feedCarnivoresButton.Visible = carnivoreCount > 0;

            if (!feedHerbivoresButton.Visible && !feedCarnivoresButton.Visible)
            {
                MessageBox.Show("No animals available to feed!");
            }
        }

        private void HideFeedButtons()
        {
            feedHerbivoresButton.Visible = false;
            feedCarnivoresButton.Visible = false;
        }

        private void HideAnimalButtons()
        {
            buyGiraffeButton.Visible = false;
            buyRhinocerosButton.Visible = false;
            buyLionButton.Visible = false;
            buyTigerButton.Visible = false;
            HideSellButtons();
            HideFeedButtons();
        }

        private void HideHuntButtons()
        {
            killLionButton.Visible = false;
            killTigerButton.Visible = false;
            HideSellButtons();
            HideFeedButtons();
        }

        private void HideSellButtons()
        {
            sellGiraffeButton.Visible = false;
            sellRhinocerosButton.Visible = false;
            sellLionButton.Visible = false;
            sellTigerButton.Visible = false;
            HideFeedButtons();
        }
        private void FeedHerbivores()
        {
            var herbivores = gameManager.GetAnimals().Where(a => a is Herbivore).ToList();
            int totalCost = herbivores.Count * 25;

            if (gameManager.GetCapital() < totalCost)
            {
                MessageBox.Show($"Not enough capital! Need ${totalCost} to feed {herbivores.Count} herbivores.");
                return;
            }

            foreach (var animal in herbivores)
            {
                animal.Feed();
            }

            gameManager.UpdateCapital(-totalCost);
            updateCapitalDisplay();
            MessageBox.Show($"Fed {herbivores.Count} herbivores for ${totalCost}!");
            HideFeedButtons();
        }

        private void FeedCarnivores()
        {
            var carnivores = gameManager.GetAnimals().Where(a => a is Carnivore).ToList();
            int totalCost = carnivores.Count * 25;

            if (gameManager.GetCapital() < totalCost)
            {
                MessageBox.Show($"Not enough capital! Need ${totalCost} to feed {carnivores.Count} carnivores.");
                return;
            }

            foreach (var animal in carnivores)
            {
                animal.Feed();
            }

            gameManager.UpdateCapital(-totalCost);
            updateCapitalDisplay();
            MessageBox.Show($"Fed {carnivores.Count} carnivores for ${totalCost}!");
            HideFeedButtons();
        }
        // Show animal buttons when "Buy Animal" is clicked
        private void ShowAnimalButtons()
        {
            HideHuntButtons();
            buyGiraffeButton.Visible = true;
            buyRhinocerosButton.Visible = true;
            buyLionButton.Visible = true;
            buyTigerButton.Visible = true;
        }

        private void ShowHuntButtons()
        {
            HideAnimalButtons();
            killLionButton.Visible = true;
            killTigerButton.Visible = true;
        }



        private void ShowSellAnimalButtons()
        {
            HideAnimalButtons();
            HideHuntButtons();

            // Only show buttons for animal types that have adults available
            sellGiraffeButton.Visible = HasAdultAnimal(typeof(Giraffe));
            sellRhinocerosButton.Visible = HasAdultAnimal(typeof(Rhinoceros));
            sellLionButton.Visible = HasAdultAnimal(typeof(Lion));
            sellTigerButton.Visible = HasAdultAnimal(typeof(Tiger));

            if (!sellGiraffeButton.Visible && !sellRhinocerosButton.Visible &&
                !sellLionButton.Visible && !sellTigerButton.Visible)
            {
                MessageBox.Show("No adult animals available to sell!");
            }
        }

        private bool HasAdultAnimal(Type animalType)
        {
            foreach (var animal in gameManager.GetAnimals())
            {
                if (animal.GetType() == animalType && animal.CurrentAge == Animal.AgeStage.Adult)
                {
                    return true;
                }
            }
            return false;
        }

        private void SellAnimal(string animalType)
        {
            Type speciesType = null;
            switch (animalType)
            {
                case "Giraffe": speciesType = typeof(Giraffe); break;
                case "Rhinoceros": speciesType = typeof(Rhinoceros); break;
                case "Lion": speciesType = typeof(Lion); break;
                case "Tiger": speciesType = typeof(Tiger); break;
            }

            // Find the first adult animal of this type
            Animal animalToSell = null;
            foreach (var animal in gameManager.GetAnimals())
            {
                if (animal.GetType() == speciesType && animal.CurrentAge == Animal.AgeStage.Adult)
                {
                    animalToSell = animal;
                    break;
                }
            }

            if (animalToSell != null)
            {
                gameManager.RemoveAnimal(animalToSell);
                gameManager.UpdateCapital(150);
                updateCapitalDisplay();
                gameWindow.Invalidate();
                MessageBox.Show($"{animalType} sold for $150!");
            }
            else
            {
                MessageBox.Show($"No adult {animalType} available to sell!");
            }

            // Hide the sell buttons after selling
            sellGiraffeButton.Visible = false;
            sellRhinocerosButton.Visible = false;
            sellLionButton.Visible = false;
            sellTigerButton.Visible = false;
        }



        // Buy Animal functionality
        private void BuyAnimal(string animalType, int cost)
        {
            Type speciesType = null;
            switch (animalType)
            {
                case "Giraffe": speciesType = typeof(Giraffe); break;
                case "Rhinoceros": speciesType = typeof(Rhinoceros); break;
                case "Lion": speciesType = typeof(Lion); break;
                case "Tiger": speciesType = typeof(Tiger); break;
            }
            if (gameManager.GetCapital() >= cost)
            {
                Point position = gameManager.FindAvailablePositionNearSpecies(speciesType);
                if (position == Point.Empty)
                {
                    MessageBox.Show("No available space for the animal!");
                    return;
                }

                Animal newAnimal = CreateAnimal(animalType, position);
                if (newAnimal == null)
                {
                    MessageBox.Show($"Could not create {animalType}!");
                    return;
                }

                gameManager.UpdateCapital(-cost);
                gameManager.AddAnimal(newAnimal);
                updateCapitalDisplay();
                gameWindow.Invalidate();
                MessageBox.Show($"{animalType} purchased and placed at ({position.X}, {position.Y})!");
            }
            else
            {
                MessageBox.Show("Not enough capital!");
            }
        }

        private Animal CreateAnimal(string animalType, Point position)
        {
            int TileSize = 32;
            float pixelX = position.X * TileSize + TileSize / 2f;
            float pixelY = position.Y * TileSize + TileSize / 2f;

            Animal animal = null;
            switch (animalType)
            {
                case "Tiger":
                    animal = new Tiger(gameManager, gameManager.map);
                    break;
                case "Lion":
                    animal = new Lion(gameManager, gameManager.map);
                    break;
                case "Giraffe":
                    animal = new Giraffe(gameManager, gameManager.map);
                    break;
                case "Rhinoceros":
                    animal = new Rhinoceros(gameManager, gameManager.map);
                    break;
            }

            if (animal != null)
            {
                animal.SetPosition(pixelX, pixelY);
            }

            return animal;
        }



        // Get the cost of an animal
        private int GetAnimalCost(string animalType)
        {
            switch (animalType)
            {
                case "Giraffe": return 200;
                case "Rhinoceros": return 200;
                case "Lion": return 300;
                case "Tiger": return 300;
                default: return 0;
            }
        }


        private void HuntAnimal(string animalType)
        {
            Point pos = gameManager.FindAvailablePosition();
            if (pos == Point.Empty)
            {
                MessageBox.Show("No available space for a new ranger!");
                return;
            }
            else
            {
                var newRanger = new Ranger(pos.X, pos.Y, animalType, gameManager);
                if (newRanger == null)
                {
                    MessageBox.Show($"Could not create a new ranger!");
                    return;
                }
                gameManager.AddRanger(newRanger);
                MessageBox.Show($"Ranger hired at {pos.X}, {pos.Y}");
                gameWindow.Invalidate();
            }
        }

        // Buy Road functionality
        private void BuyRoad()
        {

            // $50 for 5 roads 

            if (gameManager.GetCapital() >= 30)
            {
                gameManager.UpdateCapital(-30);
                gameManager.AddRoadBlock(5);
                updateCapitalDisplay();
                MessageBox.Show($"Purchased {5} roads for ${30}!");
            }
            else
            {
                MessageBox.Show($"Not enough capital! Need ${30} to buy 5 roads.");
            }

        }

        // Buy Jeep functionality
        private void BuyJeep()
        {
            if (gameManager.GetCapital() >= 300)
            {
                gameManager.UpdateCapital(-300);
                gameManager.AddJeep();
                updateCapitalDisplay();
                MessageBox.Show("Jeep purchased!");
            }
            else
            {
                MessageBox.Show("Not enough capital!");
            }
        }

        private void BuyPond()
        {
            if (gameManager.GetCapital() >= 100)
            {
                gameManager.UpdateCapital(-100);
                gameManager.AddPond();
                updateCapitalDisplay();
                MessageBox.Show("Pond purchased!");
            }
            else
            {
                MessageBox.Show("Not enough capital!");
            }
        }

        private void BuyBush()
        {
            if (gameManager.GetCapital() >= 100)
            {
                gameManager.UpdateCapital(-100);
                gameManager.AddBush();
                updateCapitalDisplay();
                MessageBox.Show("Bush purchased!");
            }
            else
            {
                MessageBox.Show("Not enough capital!");
            }
        }

        private void BuyTree()
        {
            if (gameManager.GetCapital() >= 100)
            {
                gameManager.UpdateCapital(-100);
                gameManager.AddTree();
                updateCapitalDisplay();
                MessageBox.Show("Tree purchased!");
            }
            else
            {
                MessageBox.Show("Not enough capital!");
            }
        }

    }
}
