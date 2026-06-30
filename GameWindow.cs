using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Safari
{
    public partial class GameWindow : Form
    {
        private Map map;
        public readonly GameManager gameManager;
        private Label capitalLabel;
        private Label animalsLabel;
        private Label visitorsLabel;
        private Label difficultyLabel;
        private Label timeLabel;
        private Label roadsLabel;
        private Label jeepsLabel;
        private Label pondLabel;
        private Label BushLabel;
        private Label TreeLabel;
        private bool isPlacingRoad = false;
        private bool isRemovingRoad = false;
        private Image grassImage;
        private Image waterImage;
        private Image pondImage;
        private Image rockImage;
        private Image roadImage;
        private Image doorImage;
        private Image tigerImage;
        private Image lionImage;
        private Image giraffeImage;
        private Image rhinocerosImage;
        private Image jeepImage;
        private Dictionary<Type, Image> animalImages = new Dictionary<Type, Image>();
        private Timer gameTimer;
        private Image poacherImage;
        private Image rangerImage;
        private Timer poacherTimer;
        private Timer rangerTimer;
        private Image bushImage;
        private Image treeImage;
        private readonly Random _random = new Random();

        


        // Viewport tracking
        private Rectangle viewport = new Rectangle(0, 0, 20, 15); // Shows 20x15 tiles at a time
        private Point viewOffset = Point.Empty; // Current scroll position
        private bool showMiniMap = true;
        private float miniMapScale = 1f; // Scale of minimap compared to main map

        public Difficulty CurrentDifficulty { get; private set; }


        //easy difficulty timer 
        private Timer weekTimer;
        private int elapsedWeeks = 0;

        //medium difficulty timer
        private Timer dayTimer;
        private int elapsedDays = 0;

        //ahrd difficulty timer 
        private Timer hourTimer;
        private int elapsedHours = 0;


        //winning conditions 
        private int consecutiveGoodWeeks = 0;
        private int consecutiveGoodDays = 0;
        private int consecutiveGoodHours = 0;


        public GameWindow(Difficulty difficulty)
        {
            try
            {
                grassImage = Image.FromFile("src/grass.jpg"); // Looks in executable directory
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Grass image not found. Expected path: " +
                               Path.GetFullPath("grass.jpg") + "\nError: " + ex.Message);
                grassImage = new Bitmap(16, 16);
            }

            try
            {
                waterImage = Image.FromFile("src/water.jpg");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Water image not found. Expected path: " +
                               Path.GetFullPath("water.jpg") + "\nError: " + ex.Message);
                waterImage = new Bitmap(16, 16);
            }

            try
            {
                pondImage = Image.FromFile("src/pond-nobg.png");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Pond image not found. Expected path: " +
                               Path.GetFullPath("pond-nobg.png") + "\nError: " + ex.Message);
                pondImage = new Bitmap(16, 16);
            }

            try
            {
                rockImage = Image.FromFile("src/rock-nobg.png");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Rock image not found. Expected path: " +
                               Path.GetFullPath("rock-nobg.png") + "\nError: " + ex.Message);
                rockImage = new Bitmap(16, 16);
            }

            try
            {
                roadImage = Image.FromFile("src/road.jpg");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Road image not found. Expected path: " +
                               Path.GetFullPath("road.jpg") + "\nError: " + ex.Message);
                roadImage = new Bitmap(16, 16);
            }

            try
            {
                doorImage = Image.FromFile("src/door.jpg");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Door image not found. Expected path: " +
                               Path.GetFullPath("src/door.jpg") + "\nError: " + ex.Message);
                doorImage = new Bitmap(16, 16);
            }

            try
            {
                tigerImage = Image.FromFile("src/tiger.png");
                animalImages.Add(typeof(Tiger), tigerImage);

                lionImage = Image.FromFile("src/lion.png");
                animalImages.Add(typeof(Lion), lionImage);

                giraffeImage = Image.FromFile("src/giraffe.png");
                animalImages.Add(typeof(Giraffe), giraffeImage);

                rhinocerosImage = Image.FromFile("src/rhino.png");
                animalImages.Add(typeof(Rhinoceros), rhinocerosImage);

            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Animal image not found. Expected path: " + "\nError: " + ex.Message);
                tigerImage = new Bitmap(16, 16);
                lionImage = new Bitmap(16, 16);
                giraffeImage = new Bitmap(16, 16);
                rhinocerosImage = new Bitmap(16, 16);
            }

            try
            {
                jeepImage = Image.FromFile("src/jeep-nobg.png");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Jeep image not found. Expected path: " +
                               Path.GetFullPath("src/jeep-nobg.png") + "\nError: " + ex.Message);
                jeepImage = new Bitmap(16, 16);
            }

            try
            {
                poacherImage = Image.FromFile("src/poacher.png");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Poacher image not found. Expected path: " +
                               Path.GetFullPath("poacher.png") + "\nError: " + ex.Message);
                poacherImage = new Bitmap(16, 16);
            }

            try
            {
                rangerImage = Image.FromFile("src/ranger.png");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Ranger image not found. Expected path: " +
                               Path.GetFullPath("ranger.png") + "\nError: " + ex.Message);
                poacherImage = new Bitmap(16, 16);
            }

            try
            {
                bushImage = Image.FromFile("src/bush-nobg.png");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Bush image not found. Expected path: " +
                               Path.GetFullPath("bush-nobg.png") + "\nError: " + ex.Message);
                bushImage = new Bitmap(16, 16);
            }

            try
            {
                treeImage = Image.FromFile("src/tree-nobg.png");
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Tree image not found. Expected path: " +
                               Path.GetFullPath("tree-nobg.png") + "\nError: " + ex.Message);
                treeImage = new Bitmap(16, 16);
            }

            InitializeComponent();
            map = new Map(40, 40);

            int tileSize = 32;
            int uiPanelHeight = 80; // Top panel height
            int bottomPanelHeight = 40; // Bottom panel height
            viewport = new Rectangle(0, 0, 30, 16);

            // Initialize the GameManager
            CurrentDifficulty = difficulty;
            gameManager = new GameManager(map, difficulty);
            // Subscribe to events AFTER!!!!!!! GameManager is initialized
            gameManager.UIUpdateRequested += UpdateUI;
            // gameManager.IncomeEarned += ShowIncomeNotification;
            this.Text = "Safari Tycoon - Play";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ClientSize = new Size(960, 696); // Set window size  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! testing with another ui panel
            this.MaximizeBox = false;
            // Enable double buffering
            this.DoubleBuffered = true;

            // Create a UI panel at the top
            Panel uiPanel = new Panel();
            uiPanel.Size = new Size(this.ClientSize.Width, 80); 
            uiPanel.BackColor = Color.LightGray;
            this.Controls.Add(uiPanel);

            // Create a UI panel at the bottom
            Panel uiPanelBottom = new Panel();
            uiPanelBottom.Size = new Size(this.ClientSize.Width, 40);
            uiPanelBottom.BackColor = Color.LightGray;
            uiPanelBottom.Location = new Point(0, this.ClientSize.Height - uiPanelBottom.Height);
            this.Controls.Add(uiPanelBottom);


            // Add a label to display capital
            capitalLabel = new Label();
            capitalLabel.Text = $"Capital: ${gameManager.GetCapital()}";
            capitalLabel.AutoSize = true;
            capitalLabel.Location = new Point(10, 15);
            capitalLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            uiPanel.Controls.Add(capitalLabel);


            // Add a label to display number of animals
            animalsLabel = new Label();
            animalsLabel.Text = $"Animals: {gameManager.GetNumberOfAnimals()}";
            animalsLabel.AutoSize = true;
            animalsLabel.Location = new Point(160, 15);
            animalsLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            uiPanel.Controls.Add(animalsLabel);

            // Add a label to display number of visitors
            visitorsLabel = new Label();
            visitorsLabel.Text = $"Visitors: {gameManager.GetNumberOfVisitors()}";
            visitorsLabel.AutoSize = true;
            visitorsLabel.Location = new Point(310, 15);
            visitorsLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            uiPanel.Controls.Add(visitorsLabel);

            difficultyLabel = new Label();
            difficultyLabel.Text = $"Difficulty: {CurrentDifficulty}";
            difficultyLabel.AutoSize = true;
            difficultyLabel.Location = new Point(460, 15);
            difficultyLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            uiPanel.Controls.Add(difficultyLabel);

            //timeLabel = new Label();
            //timeLabel.Text = $"Time: {gameManager.GetTime()}";

            timeLabel = new Label();
            timeLabel.AutoSize = true;
            timeLabel.Location = new Point(610, 15);
            timeLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            uiPanel.Controls.Add(timeLabel);

            


            if(difficulty == Difficulty.Easy)
            {
                weekTimer = new System.Windows.Forms.Timer();
                weekTimer.Interval = 6000; // 
                weekTimer.Tick += WeekTimer_Tick;
                weekTimer.Start();

            }
            else if(difficulty == Difficulty.Medium)
            {
                dayTimer = new System.Windows.Forms.Timer();
                dayTimer.Interval = 3000; 
                dayTimer.Tick += DayTimer_Tick;
                dayTimer.Start();

            }
            else if(difficulty == Difficulty.Hard)
            {
                hourTimer = new System.Windows.Forms.Timer();
                hourTimer.Interval = 1500;
                hourTimer.Tick += HourTimer_Tick;
                hourTimer.Start();
            }






                // Add a label to display number of roads
                roadsLabel = new Label();
            roadsLabel.Text = $"Roads: {gameManager.GetNumberOfRoads()}";
            roadsLabel.AutoSize = true;
            roadsLabel.Location = new Point(10, 10);
            roadsLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            uiPanelBottom.Controls.Add(roadsLabel);

            // Add a label to display number of jeeps
            jeepsLabel = new Label();
            jeepsLabel.Text = $"Jeeps: {gameManager.GetNumberOfJeeps()}";
            jeepsLabel.AutoSize = true;
            jeepsLabel.Location = new Point(110, 10);
            jeepsLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            uiPanelBottom.Controls.Add(jeepsLabel);

            // Add a label to display number of ponds
            pondLabel = new Label();
            pondLabel.Text = $"Ponds: {gameManager.GetNumberOfPonds()}";
            pondLabel.AutoSize = true;
            pondLabel.Location = new Point(210, 10);
            pondLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            uiPanelBottom.Controls.Add(pondLabel);

            // Add a label to display number of Bushes
            BushLabel = new Label();
            BushLabel.Text = $"Bushes: {gameManager.GetNumberOfBushes()}";
            BushLabel.AutoSize = true;
            BushLabel.Location = new Point(310, 10);
            BushLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            uiPanelBottom.Controls.Add(BushLabel);

            // Add a label to display number of trees
            TreeLabel = new Label();
            TreeLabel.Text = $"Trees: {gameManager.GetNumberOfTrees()}";
            TreeLabel.AutoSize = true;
            TreeLabel.Location = new Point(410, 10);
            TreeLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            uiPanelBottom.Controls.Add(TreeLabel);

            // Add an Exit button
            Button exitButton = new Button();
            exitButton.Text = "Exit";
            exitButton.Size = new Size(100, 30);
            exitButton.Location = new Point(850, 45); // Moved down to fit in the new panel height
            exitButton.Click += (sender, e) => this.Close();
            uiPanel.Controls.Add(exitButton);

            // Add a Market button
            Button marketButton = new Button();
            marketButton.Text = "Market";
            marketButton.Size = new Size(100, 30);
            marketButton.Location = new Point(750, 45); // Moved down to fit in the new panel height
            marketButton.Click += (sender, e) => OpenMarket();
            uiPanel.Controls.Add(marketButton);

            // Add a Place Road button
            Button placeRoadButton = new Button();
            placeRoadButton.Text = "Place Road";
            placeRoadButton.Size = new Size(100, 30);
            placeRoadButton.Location = new Point(10, 45); // Moved down to fit in the new panel height
            placeRoadButton.Click += (sender, e) => StartPlacingRoad();
            uiPanel.Controls.Add(placeRoadButton);

            // Add a Stop Placing button
            Button stopPlacingButton = new Button();
            stopPlacingButton.Text = "Stop Placing";
            stopPlacingButton.Size = new Size(100, 30);
            stopPlacingButton.Location = new Point(110, 45); // Moved down to fit in the new panel height
            stopPlacingButton.Click += (sender, e) => StopPlacingRoad();
            uiPanel.Controls.Add(stopPlacingButton);

            // Add a Remove Road button
            Button removeRoadButton = new Button();
            removeRoadButton.Text = "Remove Road";
            removeRoadButton.Size = new Size(100, 30);
            removeRoadButton.Location = new Point(210, 45); // Moved down to fit in the new panel height
            removeRoadButton.Click += (sender, e) => StartRemovingRoad();
            uiPanel.Controls.Add(removeRoadButton);

            // Add a Place Pond button
            Button placePondButton = new Button();
            placePondButton.Text = "Place Pond";
            placePondButton.Size = new Size(100, 30);
            placePondButton.Location = new Point(440, 45); // Moved down to fit in the new panel height
            placePondButton.Click += (sender, e) => StartPlacingPond();
            uiPanel.Controls.Add(placePondButton);

            // Add a Place Bush button
            Button placeBushButton = new Button();
            placeBushButton.Text = "Place Bush";
            placeBushButton.Size = new Size(100, 30);
            placeBushButton.Location = new Point(540, 45); // Moved down to fit in the new panel height
            placeBushButton.Click += (sender, e) => StartPlacingBush();
            uiPanel.Controls.Add(placeBushButton);

            // Add a Place tree button
            Button placeTreeButton = new Button();
            placeTreeButton.Text = "Place Tree";
            placeTreeButton.Size = new Size(100, 30);
            placeTreeButton.Location = new Point(640, 45); // Moved down to fit in the new panel height
            placeTreeButton.Click += (sender, e) => StartPlacingTree();
            uiPanel.Controls.Add(placeTreeButton);

            // Add an Travel button
            Button travelButton = new Button();
            travelButton.Text = "Travel";
            travelButton.Size = new Size(100, 30);
            travelButton.Location = new Point(330, 45); // Moved down to fit in the new panel height
            travelButton.Click += (sender, e) =>
            {
                if (gameManager.GetNumberOfJeeps() < 1)
                {
                    MessageBox.Show("You don't have any jeep to travel.");
                }
                else if (!gameManager.StartJeepRoute())
                {
                    MessageBox.Show("There is no valid path between doors or all jeeps are traveling. ");
                }
                else
                {
                    UpdateUI();
                    MessageBox.Show("Jeep has started its tour!");
                }
            };
            uiPanel.Controls.Add(travelButton);

            // Add mouse click event handler for placing and removing roads
            this.MouseClick += new MouseEventHandler(GameWindow_MouseClick);
            this.MouseClick += new MouseEventHandler(PondPlacement_MouseClick);
            this.MouseClick += new MouseEventHandler(BushPlacement_MouseClick);
            this.MouseClick += new MouseEventHandler(TreePlacement_MouseClick);

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            poacherTimer = new System.Windows.Forms.Timer();
            SetRandomPoacherInterval();
            poacherTimer.Tick += PoacherTimer_Tick;
            poacherTimer.Start();

            rangerTimer = new System.Windows.Forms.Timer();
            rangerTimer.Interval = 30000;
            rangerTimer.Tick += RangerTimer_Tick;
            rangerTimer.Start();
        }
        


        



        private bool isPlacingPond = false;
        private void StartPlacingPond()
        {
            if (gameManager.GetNumberOfPonds() > 0)
            {
                isPlacingPond = true;
                isPlacingRoad = false;
                isRemovingRoad = false;

                isPlacingTree = false;
                isPlacingBush = false;

            }
            else
            {
                MessageBox.Show("You don't have any ponds to place. Buy more from the Market.");
            }
        }

        private void PondPlacement_MouseClick(object sender, MouseEventArgs e)
        {
            int tileSize = 32;
            int uiPanelHeight = 80;
            int x = (e.X + viewOffset.X) / tileSize;
            int y = (e.Y - uiPanelHeight + viewOffset.Y) / tileSize;

            if (x >= 0 && x < map.Width && y >= 0 && y < map.Height)
            {
                if (isPlacingPond && map.GetTile(x, y) == 3) // Grass tile
                {
                    if (gameManager.GetNumberOfPonds() > 0)
                    {
                        map.SetTile(x, y, 2); // Pond
                        gameManager.RemovePond();
                        UpdateUI();
                        Invalidate();
                        isPlacingPond = false;
                    }
                    else
                    {
                        MessageBox.Show("No ponds to place!");
                        isPlacingPond = false;
                    }
                }
            }
        }


        private bool isPlacingTree = false;
        private void StartPlacingTree()
        {
            if (gameManager.GetNumberOfTrees() > 0)
            {
                isPlacingTree = true;
                isPlacingBush = false;
                isPlacingPond = false;
                isPlacingRoad = false;
                isRemovingRoad = false;
            }
            else
            {
                MessageBox.Show("You don't have any trees to place. Buy more from the Market.");
            }
        }

        private void TreePlacement_MouseClick(object sender, MouseEventArgs e)
        {
            int tileSize = 32;
            int uiPanelHeight = 80;
            int x = (e.X + viewOffset.X) / tileSize;
            int y = (e.Y - uiPanelHeight + viewOffset.Y) / tileSize;

            if (x >= 0 && x < map.Width && y >= 0 && y < map.Height)
            {
                if (isPlacingTree)
                {
                    if (map.GetTile(x, y) == 3)
                        if (gameManager.GetNumberOfTrees() > 0)
                        {
                            map.SetTile(x, y, 8);
                            gameManager.RemoveTree();
                            UpdateUI();
                            Invalidate();
                            Update();
                            isPlacingTree = false; // Stop placing after one click
                        }
                        else
                        {
                            isPlacingTree = false;
                            MessageBox.Show("No trees to place!");
                        }
                }
            }
            Invalidate();
        }


        private bool isPlacingBush = false;
        private void StartPlacingBush()
        {
            if (gameManager.GetNumberOfBushes() > 0)
            {
                isPlacingBush = true;
                isPlacingPond = false;
                isPlacingRoad = false;
                isRemovingRoad = false;

                isPlacingTree = false;
            }
            else
            {
                MessageBox.Show("You don't have any bushes to place. Buy more from the Market.");
            }
        }

        private void BushPlacement_MouseClick(object sender, MouseEventArgs e)
        {
            int tileSize = 32;
            int uiPanelHeight = 80; 
            
            int x = (e.X + viewOffset.X) / tileSize;
            int y = (e.Y - uiPanelHeight + viewOffset.Y) / tileSize;
            if (x >= 0 && x < map.Width && y >= 0 && y < map.Height)
            {
                if (isPlacingBush)
                {
                    if (map.GetTile(x, y) == 3)
                        if (gameManager.GetNumberOfBushes() > 0)
                        {
                            map.SetTile(x, y, 7);
                            gameManager.RemoveBush();
                            UpdateUI();
                            Invalidate();
                            Update();
                            isPlacingBush = false; // Stop placing after one click
                        }
                        else
                        {
                            isPlacingBush = false;
                            MessageBox.Show("No bushes to place!");
                        }
                }
            }
            Invalidate();
        }



        private void SetRandomPoacherInterval()
        {
            int seconds = _random.Next(10, 40 + 1);
            poacherTimer.Interval = seconds * 1000;
        }

        private void PoacherTimer_Tick(object sender, EventArgs e)
        {
            poacherTimer.Stop();
            Point pos = gameManager.FindAvailablePosition();
            if (pos == Point.Empty)
            {
                MessageBox.Show("No available space for a new poacher!");
                return;
            }
            else
            {
                var newPoacher = new Poacher(pos.X, pos.Y);

                if (newPoacher == null)
                {
                    MessageBox.Show($"Could not create a new poacher!");
                    return;
                }
                gameManager.AddPoacher(newPoacher);
                Invalidate();
            }
            SetRandomPoacherInterval(); // Get new random interval
            poacherTimer.Start();
        }


        private void GameTimer_Tick(object sender, EventArgs e)
        {
            gameManager.UpdateAnimals();
            gameManager.UpdateJeeps();
            gameManager.UpdatePoachers();
            gameManager.UpdateRangers();

            if(gameManager.GetNumberOfAnimals() <= 0 && gameManager.GetCapital() <= 0)
            {
                gameTimer.Stop();
                
                if(weekTimer != null) weekTimer.Stop();
                else if (dayTimer != null) dayTimer.Stop();
                else if (hourTimer != null) hourTimer.Stop();

                DialogResult result = MessageBox.Show("Game Over!", "Game Over", MessageBoxButtons.OK);

                if(result == DialogResult.OK) 
                {
                    this.Close();
                }
            }


            Invalidate();
        }

        private void RangerTimer_Tick(object sender, EventArgs e)
        {
            foreach (var ranger in gameManager.GetRangers())
            {
                gameManager.UpdateCapital(-100);
            }
            Invalidate();
        }

        public void RestartRangerTimer()
        {
            rangerTimer.Stop();
            rangerTimer.Start();
        }

        // Start placing a road
        private void StartPlacingRoad()
        {
            if (gameManager.GetNumberOfRoads() > 0)
            {
                isPlacingRoad = true;
                isRemovingRoad = false;
                // MessageBox.Show("Road placement mode activated. Click on grass tiles to place roads.");
            }
            else
            {
                MessageBox.Show("You don't have any roads to place. Buy more from the Market.");
            }
        }


        private void StopPlacingRoad()
        {
            isPlacingRoad = false;
            //  MessageBox.Show("Road placement stopped.");
        }

        // Start removing a road
        private void StartRemovingRoad()
        {
            isRemovingRoad = !isRemovingRoad; // Toggle removal mode
            isPlacingRoad = false;


        }
        private void GameWindow_MouseClick(object sender, MouseEventArgs e)
        {
            int tileSize = 32;
            int uiPanelHeight = 80;

            // Calculate tile coordinates accounting for viewport offset and UI panel
            int x = (e.X + viewOffset.X) / tileSize;
            int y = (e.Y - uiPanelHeight + viewOffset.Y) / tileSize;

            // Ensure coordinates are within map bounds
            if (x >= 0 && x < map.Width && y >= 0 && y < map.Height)
            {
                if (isPlacingRoad)
                {
                    if (map.GetTile(x, y) == 3) // Check if it's grass
                    {
                        if (gameManager.GetNumberOfRoads() > 0)
                        {
                            map.SetTile(x, y, 5); // Place road
                            gameManager.RemoveRoad();
                            UpdateUI();
                            Invalidate();
                        }
                        else
                        {
                            isPlacingRoad = false;
                            MessageBox.Show("No roads to place!");
                        }
                    }
                }
                else if (isRemovingRoad)
                {
                    if (map.GetTile(x, y) == 5) // Check if it's road
                    {
                        map.SetTile(x, y, 3); // Revert to grass
                        gameManager.AddRoad();
                        UpdateUI();
                        Invalidate();
                    }
                }
            }
        }

        private void DrawMiniMap(Graphics g)
        {
            if (!showMiniMap || map == null) return;

            int tileSize = 32;
            int uiPanelHeight = 80;
            int bottomPanelHeight = 40;

            // Calculate desired minimap size (about 1/6th of screen width)
            int desiredWidth = this.ClientSize.Width / 6;
            int desiredHeight = this.ClientSize.Height / 6;

            // Calculate scale to fit the map proportionally
            miniMapScale = Math.Min(
                desiredWidth / (float)(map.Width * tileSize),
                desiredHeight / (float)(map.Height * tileSize)
            );

            // Ensure minimap isn't too small
            miniMapScale = Math.Max(miniMapScale, 0.05f); // Minimum scale of 5%

            int miniMapWidth = (int)(map.Width * tileSize * miniMapScale);
            int miniMapHeight = (int)(map.Height * tileSize * miniMapScale);

            // Position in bottom right with 10px margin, above the bottom panel
            int miniMapX = this.ClientSize.Width - miniMapWidth - 10;
            int miniMapY = this.ClientSize.Height - miniMapHeight - bottomPanelHeight - 10;

            // Draw semi-transparent background
            using (var bgBrush = new SolidBrush(Color.FromArgb(200, Color.DarkGray)))
            {
                g.FillRectangle(bgBrush, miniMapX, miniMapY, miniMapWidth, miniMapHeight);
            }

            // Draw all tiles
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    int drawX = miniMapX + (int)(x * tileSize * miniMapScale);
                    int drawY = miniMapY + (int)(y * tileSize * miniMapScale);
                    int drawSize = (int)(tileSize * miniMapScale);

                    // Ensure at least 1 pixel is drawn
                    drawSize = Math.Max(1, drawSize);

                    using (var brush = new SolidBrush(GetTileColor(map.GetTile(x, y))))
                    {
                        g.FillRectangle(brush, drawX, drawY, drawSize, drawSize);
                    }
                }
            }

            // Draw viewport rectangle
            int viewX = miniMapX + (int)(viewOffset.X * miniMapScale);
            int viewY = miniMapY + (int)(viewOffset.Y * miniMapScale);
            int viewWidth = (int)(viewport.Width * tileSize * miniMapScale);
            int viewHeight = (int)(viewport.Height * tileSize * miniMapScale);

            // Ensure rectangle is visible
            using (var pen = new Pen(Color.Red, 2f))
            {
                g.DrawRectangle(pen, viewX, viewY, viewWidth, viewHeight);
            }
        }

        private void OpenMarket()
        {
            MarketWindow marketWindow = new MarketWindow(gameManager, UpdateUI, this);
            marketWindow.ShowDialog(); // Show the Market window as a modal dialog
        }

        public void UpdateUI()
        {
            capitalLabel.Text = $"Capital: ${gameManager.GetCapital()}";
            animalsLabel.Text = $"Animals: {gameManager.GetNumberOfAnimals()}";
            visitorsLabel.Text = $"Visitors: {gameManager.GetNumberOfVisitors()}";
            roadsLabel.Text = $"Roads: {gameManager.GetNumberOfRoads()}";
            jeepsLabel.Text = $"Jeeps: {gameManager.GetNumberOfJeeps()}";
            pondLabel.Text = $"Ponds: {gameManager.GetNumberOfPonds()}";
            BushLabel.Text = $"Bushes: {gameManager.GetNumberOfBushes()}";
            TreeLabel.Text = $"Trees: {gameManager.GetNumberOfTrees()}";


            //timeLabel.Text = $"Weeks passed: {elapsedWeeks}";
            UpdateTimeLabel();
        }

        private float Distance(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int tileSize = 32;
            int uiPanelHeight = 80;

            const int tilesToShowX = 30;
            const int tilesToShowY = 18;

            // Calculate visible area based on viewport
            int startX = viewOffset.X / tileSize;
            int startY = viewOffset.Y / tileSize;
           int endX = Math.Min(map.Width, startX + tilesToShowX);
             int endY = Math.Min(map.Height, startY + tilesToShowY);

            // Draw only the visible portion of the map
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    // Calculate screen position accounting for view offset
                    int drawX = x * tileSize - viewOffset.X;
                    int drawY = y * tileSize - viewOffset.Y + uiPanelHeight;

                    // Only draw if within visible bounds
                    if (drawX >= -tileSize && drawX < this.ClientSize.Width &&
                        drawY >= uiPanelHeight - tileSize && drawY < this.ClientSize.Height)
                    {

                        int tileType = map.GetTile(x, y);
                        if (tileType == 3) // Grass tile
                        {
                            e.Graphics.DrawImage(grassImage, drawX, drawY, tileSize, tileSize);
                        }
                        else if (tileType == 1) // River tile
                        {
                            e.Graphics.DrawImage(waterImage, drawX, drawY, tileSize, tileSize);
                        }
                        else if (tileType == 2) // Pond tile
                        {
                            e.Graphics.DrawImage(pondImage, drawX, drawY, tileSize, tileSize);
                        }
                        else if (tileType == 4) // Rock tile
                        {
                            e.Graphics.DrawImage(rockImage, drawX, drawY, tileSize, tileSize);
                        }
                        else if (tileType == 5) // Road tile
                        {
                            e.Graphics.DrawImage(roadImage, drawX, drawY, tileSize, tileSize);
                        }
                        else if (tileType == 6) // Door tile
                        {
                            e.Graphics.DrawImage(doorImage, drawX, drawY, tileSize, tileSize);
                        }
                        else if (tileType == 7) // Bush tile
                        {
                            e.Graphics.DrawImage(bushImage, drawX, drawY, tileSize, tileSize);
                        }
                        else if (tileType == 8) // Tree tile
                        {
                            e.Graphics.DrawImage(treeImage, drawX, drawY, tileSize, tileSize);
                        }
                        else
                        {
                            Color tileColor = GetTileColor(tileType);
                            using (Brush brush = new SolidBrush(tileColor))
                            {
                                e.Graphics.FillRectangle(brush, drawX, drawY, tileSize, tileSize);
                            }
                        }
                    }
                }
            }

            // Draw animals in viewport
            foreach (var animal in gameManager.GetAnimals())
            {
                int animalX = (int)animal.PixelX;
                int animalY = (int)animal.PixelY;

                // Check if animal is in viewport
                if (animalX >= viewOffset.X && animalX < viewOffset.X + viewport.Width * tileSize &&
                    animalY >= viewOffset.Y && animalY < viewOffset.Y + viewport.Height * tileSize)
                {
                    int drawX = animalX - viewOffset.X - (tileSize / 2);
                    int drawY = animalY - viewOffset.Y - (tileSize / 2) + uiPanelHeight;

                    if (animalImages.TryGetValue(animal.GetType(), out var image))
                    {
                        e.Graphics.DrawImage(image, drawX, drawY, tileSize, tileSize);
                    }
                }
            }

            // Draw jeeps in viewport
            foreach (var jeep in gameManager.GetActiveJeeps())
            {
                int jeepX = (int)jeep.PixelX;
                int jeepY = (int)jeep.PixelY;

                if (jeepX >= viewOffset.X && jeepX < viewOffset.X + viewport.Width * tileSize &&
                    jeepY >= viewOffset.Y && jeepY < viewOffset.Y + viewport.Height * tileSize)
                {
                    int drawX = jeepX - viewOffset.X - (tileSize / 2);
                    int drawY = jeepY - viewOffset.Y - (tileSize / 2) + uiPanelHeight;
                    e.Graphics.DrawImage(jeepImage, drawX, drawY, tileSize, tileSize);
                }
            }
            foreach (var poacher in gameManager.GetPoachers())
            {
                int poacherX = (int)poacher.PixelX;
                int poacherY = (int)poacher.PixelY;

                if (poacherX >= viewOffset.X && poacherX < viewOffset.X + viewport.Width * tileSize &&
                    poacherY >= viewOffset.Y && poacherY < viewOffset.Y + viewport.Height * tileSize)
                {
                    int drawX = poacherX - viewOffset.X - (tileSize / 2);
                    int drawY = poacherY - viewOffset.Y - (tileSize / 2) + uiPanelHeight;


                    float opacity = 0.3f; 
                    float detectionRange = tileSize * 5; 

                    foreach (var ranger in gameManager.GetRangers())
                    {
                        float distance = Distance(poacher.PixelX, poacher.PixelY,
                                                ranger.PixelX, ranger.PixelY);

                        if (distance < detectionRange)
                        {
                            opacity = 1f;
                        }
                    }

                    using (var attributes = new ImageAttributes())
                    {
                        attributes.SetColorMatrix(new ColorMatrix { Matrix33 = opacity });

                        e.Graphics.DrawImage(
                            poacherImage,
                            new Rectangle(drawX, drawY, tileSize, tileSize),
                            0, 0, poacherImage.Width, poacherImage.Height,
                            GraphicsUnit.Pixel,
                            attributes);
                    }
                }
            }


            foreach (var ranger in gameManager.GetRangers())
            {
                int rangerX = (int)ranger.PixelX;
                int rangerY = (int)ranger.PixelY;

                if (rangerX >= viewOffset.X && rangerX < viewOffset.X + viewport.Width * tileSize &&
                    rangerY >= viewOffset.Y && rangerY < viewOffset.Y + viewport.Height * tileSize)
                {
                    int drawX = rangerX - viewOffset.X - (tileSize / 2);
                    int drawY = rangerY - viewOffset.Y - (tileSize / 2) + uiPanelHeight;
                    e.Graphics.DrawImage(rangerImage, drawX, drawY, tileSize, tileSize);
                }
            }

            // Draw minimap if enabled
            if (showMiniMap)
            {
                DrawMiniMap(e.Graphics);
            }
        }

        private Color GetTileColor(int tileType)
        {
            return map.GetTileColor(tileType);
        }



        // difficulty timers

        


        private void UpdateTimeLabel()
        {
            if (CurrentDifficulty == Difficulty.Easy)
            {
                timeLabel.Text = $"Weeks passed: {elapsedWeeks}";
            }
            else if (CurrentDifficulty == Difficulty.Medium)
            {
                timeLabel.Text = $"Days passed: {elapsedDays}";
            }
            else if(CurrentDifficulty == Difficulty.Hard)
            {
                timeLabel.Text = $"Hours passed: {elapsedHours}";
            }
        }
        private void DayTimer_Tick(object sender, EventArgs e)
        {
            elapsedDays++;
            UpdateTimeLabel();

            if (CheckWinningConditions())
            {
                gameTimer.Stop();

                if (weekTimer != null) weekTimer.Stop();
                else if (dayTimer != null) dayTimer.Stop();
                else if (hourTimer != null) hourTimer.Stop();

                DialogResult result = MessageBox.Show("Game Won!", "Game Won", MessageBoxButtons.OK);

                if (result == DialogResult.OK)
                {
                    this.Close();
                }
            }
        }

        private void WeekTimer_Tick(object sender, EventArgs e)
        {
            elapsedWeeks++;
            UpdateTimeLabel();

            if (CheckWinningConditions())
            {
                gameTimer.Stop();

                if (weekTimer != null) weekTimer.Stop();
                else if (dayTimer != null) dayTimer.Stop();
                else if (hourTimer != null) hourTimer.Stop();

                DialogResult result = MessageBox.Show("Game Won!", "Game Won", MessageBoxButtons.OK);

                if (result == DialogResult.OK)
                {
                    this.Close();
                }
            }
        }

        private void HourTimer_Tick(object sender, EventArgs e)
        {
            elapsedHours++;
            UpdateTimeLabel();

            if (CheckWinningConditions())
            {
                gameTimer.Stop();

                if (weekTimer != null) weekTimer.Stop();
                else if (dayTimer != null) dayTimer.Stop();
                else if (hourTimer != null) hourTimer.Stop();

                DialogResult result = MessageBox.Show("Game Won!", "Game Won", MessageBoxButtons.OK);

                if (result == DialogResult.OK)
                {
                    this.Close();
                }
            }
        }
        protected override void OnResize(EventArgs e)
{
    base.OnResize(e);
    
    // Recalculate viewport size when window is resized
    int tileSize = 32;
    int uiPanelHeight = 80;
    int bottomPanelHeight = 40;
    
    int availableWidth = this.ClientSize.Width;
    int availableHeight = this.ClientSize.Height - uiPanelHeight - bottomPanelHeight;
    
    viewport.Width = availableWidth / tileSize;
    viewport.Height = availableHeight / tileSize; 
            
            
            // Ensure viewport stays within map bounds
            viewOffset.X = Math.Min(viewOffset.X, 40 * tileSize - viewport.Width * tileSize);
            viewOffset.Y = Math.Min(viewOffset.Y, 40 * tileSize - viewport.Height * tileSize);
            viewOffset.X = Math.Max(0, viewOffset.X);
            viewOffset.Y = Math.Max(0, viewOffset.Y);



            Invalidate();
}
        

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int tileSize = 32;
            int scrollSpeed = tileSize; // Scroll by one full tile at a time

            // Calculate maximum allowed offsets
            int maxOffsetX = Math.Max(0, map.Width * tileSize - viewport.Width * tileSize);
            int maxOffsetY = Math.Max(0, map.Height * tileSize - viewport.Height * tileSize);

            switch (keyData)
            {
                case Keys.Left:
                    viewOffset.X = Math.Max(0, viewOffset.X - scrollSpeed);
                    Invalidate();
                    return true;

                case Keys.Right:
                    viewOffset.X = Math.Min(maxOffsetX, viewOffset.X + scrollSpeed);
                    Invalidate();
                    return true;

                case Keys.Up:
                    viewOffset.Y = Math.Max(0, viewOffset.Y - scrollSpeed);
                    Invalidate();
                    return true;

                case Keys.Down:
                    viewOffset.Y = Math.Min(maxOffsetY, viewOffset.Y + scrollSpeed);
                    Invalidate();
                    return true;

                case Keys.M:
                    showMiniMap = !showMiniMap;
                    Invalidate();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }




        //for checking winning conditions 
        private bool CheckWinningConditions()
        {
            bool currentConditionsMet = gameManager.HasMinimumVisitors(40) &&
                                      gameManager.HasMinimumCarnivores(5) &&
                                      gameManager.HasMinimumHerbivores(5) &&
                                      gameManager.HasMinimumCapital(5000);

            if (currentConditionsMet)
            {
                switch (CurrentDifficulty)
                {
                    case Difficulty.Easy:
                        consecutiveGoodWeeks++;  
                        if (consecutiveGoodWeeks >= 60) return true;
                        break;

                    case Difficulty.Medium:
                        consecutiveGoodDays++;    
                        if (consecutiveGoodDays >= 60) return true;
                        break;

                    case Difficulty.Hard:
                        consecutiveGoodHours++;   
                        if (consecutiveGoodHours >= 60) return true;
                        break;
                }
            }
            else
            {
                consecutiveGoodWeeks = 0;
                consecutiveGoodDays = 0;
                consecutiveGoodHours = 0;
            }

            return false;
        }

    }
}