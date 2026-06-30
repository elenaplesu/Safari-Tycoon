using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Safari
{

    public enum Difficulty
    {
        Easy, Medium, Hard
    }
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            ConfigureForm();
            LoadBackgroundImage();
            SetupTitleLabel();
            SetupButtons();
        }

        private void ConfigureForm()
        {
            this.Text = "Safari Tycoon"; 
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoadBackgroundImage()
        {
            try
            {
                this.BackgroundImage = Image.FromFile("src/background.jpg"); // perhaps we can use a constant for the path
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Error: background.jpg not found!", "Image Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BackColor = Color.LightGray; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred while loading the background image: {ex.Message}", "Image Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BackColor = Color.LightGray; 
            }
        }

        private void SetupTitleLabel()
        {
            Label titleLabel = new Label();
            titleLabel.Text = "Safari Tycoon"; 
            titleLabel.Font = new Font("Helvetica", 24, FontStyle.Bold); 
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point((this.ClientSize.Width - titleLabel.Width - 125) / 2, (this.ClientSize.Height - titleLabel.Height) / 2 - 100);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.ForeColor = Color.Black; 
            titleLabel.TabStop = false;
            this.Controls.Add(titleLabel);
        }

        private void SetupButtons()
        {
            Button exitButton = CreateStyledButton("Exit"); 
            exitButton.Size = new Size(100, 50); 
                                                 
            exitButton.Location = new Point((this.ClientSize.Width - exitButton.Width) / 2, (this.ClientSize.Height - exitButton.Height) / 2 + 50); 
            exitButton.Click += ExitButton_Click; 
            this.Controls.Add(exitButton);

            Button playButton = CreateStyledButton("Play"); 
            playButton.Size = new Size(100, 50); 
                                                 
            playButton.Location = new Point((this.ClientSize.Width - playButton.Width) / 2, (this.ClientSize.Height - playButton.Height) / 2);
            playButton.Click += PlayButton_Click; 
            this.Controls.Add(playButton);
        }

        // Helper method for creating buttons with common styling
        private Button CreateStyledButton(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1; 
            button.FlatAppearance.BorderColor = Color.DimGray; 
            button.BackColor = Color.Azure; 
            button.TabStop = false; 
            return button;
        }

       
        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            using (var difficultyForm = new DifficultySelectionForm()) 
            {
                if (difficultyForm.ShowDialog(this) == DialogResult.OK)
                {
                    GameWindow gameWindow = new GameWindow(difficultyForm.SelectedDifficulty); 
                    gameWindow.Show();
                    this.Hide();
                    gameWindow.FormClosed += GameWindow_FormClosed;
                }
            }
        }

        private void GameWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is GameWindow gameWindow)
            {
                gameWindow.FormClosed -= GameWindow_FormClosed;
            }
            this.Show();
        }
    }

    public class DifficultySelectionForm : Form
    {
        public Difficulty SelectedDifficulty { get; private set; }

        public DifficultySelectionForm()
        {
            ConfigureForm();
            SetupControls(); 
        }

        private void ConfigureForm()
        {
            this.Text = "Select Difficulty"; 
            this.Size = new Size(150, 225); 
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void SetupControls()
        {
            Label label = new Label
            {
                Text = "Difficulty level:", 
                AutoSize = true,
                Location = new Point(20, 20) 
            };

            Button easyButton = CreateDifficultyButton("Easy", Difficulty.Easy, 20, 50); 
            Button mediumButton = CreateDifficultyButton("Medium", Difficulty.Medium, 20, 90); 
            Button hardButton = CreateDifficultyButton("Hard", Difficulty.Hard, 20, 130); 

            this.Controls.Add(label);
            this.Controls.Add(easyButton);
            this.Controls.Add(mediumButton);
            this.Controls.Add(hardButton);
        }

        
        private Button CreateDifficultyButton(string text, Difficulty difficulty, int x, int y)
        {
            Button button = new Button
            {
                Text = text,
                Tag = difficulty, 
                Size = new Size(100, 30), 
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat
            };

            button.Click += (sender, e) => 
            {
                SelectedDifficulty = difficulty;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            return button;
        }
    }

}