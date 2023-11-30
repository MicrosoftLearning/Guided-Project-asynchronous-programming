using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using UI.ImageRendering;
using LangtonsAnt;
using Json;
using UI;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string logFileName = "log.txt";
        string logPath;
        ILogger logger;
        GameBuffer buffer;
        DispatcherTimer gameTimer;
        PlayUIMode playUiState;
        EditUIMode editUiState;

        string rule = "LR";
        const int imagesPerSecond = 25;
        const int nGenerations = 1000;
        const int generationsPerStep = 1;

        public MainWindow()
        {
            InitializeComponent();
            logPath = Path.Combine(Directory.GetCurrentDirectory(), logFileName);
            logger = new GameLogger(logPath);
            buffer = CreateGameBuffer(null, nGenerations, rule);
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(1000 / imagesPerSecond);
            gameTimer.Tick += (sender, args) =>
            {
                try
                {
                    if (buffer.MoveNext(generationsPerStep))
                        UpdateGameView(buffer.Current!);
                    else
                    {
                        MessageBox.Show("Game Over. We no longer have any ants.");
                        PlayUIState = PlayUIMode.Stopped;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occured calculating game state.");
                }
            };
            txtEditRule.Text = rule;
            Rule = rule;
            PlayUIState = PlayUIMode.Stopped;
        }

        private IGame CreateGame(string initialRule = "LR")
        {
            IGame newGame = null;
            newGame = new Game(128, null);
            newGame.Ants = new List<IAnt>(new IAnt[] {
              new GeneralizedAnt(i: newGame.Size / 2 + 1, j: newGame.Size / 2 + 1, direction: 0) { Rule = initialRule }
            });
            return newGame;
        }

        private GameBuffer CreateGameBuffer(IGame? initialState = null, int nGenerations = 100, string initialRule = "LR")
        {
            if (initialState == null)
            {
                initialState = CreateGame(initialRule);
            }
            return new GameBuffer(initialState, nGenerations);
        }

        #region Properties

        PlayUIMode PlayUIState
        {
            get { return playUiState; }
            set
            {
                switch (value)
                {
                    case PlayUIMode.Playing:
                        gameTimer.Start();
                        btnPlay.Visibility = Visibility.Collapsed;
                        btnPause.Visibility = Visibility.Visible;
                        break;
                    case PlayUIMode.Paused:
                        gameTimer.Stop();
                        btnPause.Visibility = Visibility.Collapsed;
                        btnPlay.Visibility = Visibility.Visible;
                        break;
                    case PlayUIMode.Stopped:
                        gameTimer.Stop();
                        buffer = CreateGameBuffer(null, nGenerations, rule);
                        UpdateGameView(buffer.Current!);
                        btnPlay.Visibility = Visibility.Visible;
                        btnPause.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
                playUiState = value;
                logger.LogInformation($"Game playing state has changed, new status: {value}");
            }
        }

        /// <summary>
        /// This property switches UI between "Normal" or "Play" state and "Edit" states
        /// </summary>
        EditUIMode EditUIState
        {
            get { return editUiState; }
            set
            {
                switch (value)
                {
                    case EditUIMode.EditingColors:
                        PlayUIState = PlayUIMode.Paused;
                        SetColorsEditMode(true);
                        break;
                    case EditUIMode.EditingAnt:
                        PlayUIState = PlayUIMode.Paused;
                        SetAntEditMode(true);
                        break;
                    case EditUIMode.EditingRule:
                        PlayUIState = PlayUIMode.Paused;
                        SetRuleEditMode(true);
                        break;
                    case EditUIMode.NotEditing:
                    default:
                        SetRuleEditMode(false);
                        SetAntEditMode(false);
                        SetColorsEditMode(false);

                        if (editUiState != EditUIMode.NotEditing)
                        {
                            buffer.FlushBuffer();
                        }
                        break;
                }
                editUiState = value;
            }
        }

        /// <summary>
        /// Switch UI into "rule edit" mode
        /// </summary>
        /// <param name="value">is on</param>
        void SetRuleEditMode(bool value)
        {
            // Disable everything else
            SetButtonsNonEditMode(!value);

            if (value)
            {
                pnlRuleText.Visibility = Visibility.Collapsed;
                btnEditRuleStart.Visibility = Visibility.Collapsed;
                txtEditRule.Visibility = Visibility.Visible;
                btnEditRuleApply.Visibility = Visibility.Visible;
                btnEditRuleCancel.Visibility = Visibility.Visible;
            }
            else
            {
                pnlRuleText.Visibility = Visibility.Visible;
                btnEditRuleStart.Visibility = Visibility.Visible;
                txtEditRule.Visibility = Visibility.Collapsed;
                btnEditRuleApply.Visibility = Visibility.Collapsed;
                btnEditRuleCancel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Enable or disable buttons not related to editing
        /// </summary>
        /// <param name="value">Enable buttons</param>
        void SetButtonsNonEditMode(bool value)
        {
            // Play - Stop - Pause
            btnPlay.IsEnabled = value;
            btnStop.IsEnabled = value;
            btnPause.IsEnabled = value;

            // Previous and next buttons
            btnStepBackward.IsEnabled = value;
            btnStepForward.IsEnabled = value;

            // Edit ants and colors
            btnEditAnt.IsEnabled = value;
            btnEditCellColor.IsEnabled = value;
            btnEditRuleStart.IsEnabled = value;

            // Save and load
            btnSave.IsEnabled = value;
            btnLoad.IsEnabled = value;
        }

        /// <summary>
        /// Switch UI into Ant edit mode
        /// </summary>
        /// <param name="value">is on</param>
        void SetAntEditMode(bool value)
        {
            SetButtonsAntOrColorsEditMode(value);
            lblEditingAnts.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Switch UI into Cell Colors edit mode
        /// </summary>
        /// <param name="value">is on</param>
        void SetColorsEditMode(bool value)
        {
            SetButtonsAntOrColorsEditMode(value);
            lblEditingColors.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Switch UI into Ant or cell colors edit mode
        /// </summary>
        /// <param name="value">is on</param>
        void SetButtonsAntOrColorsEditMode(bool value)
        {
            SetButtonsNonEditMode(!value);

            if (value)
            {
                btnEditAnt.Visibility = Visibility.Collapsed;
                btnEditCellColor.Visibility = Visibility.Collapsed;
                btnBackToGame.Visibility = Visibility.Visible;
            }
            else
            {
                btnEditAnt.Visibility = Visibility.Visible;
                btnEditCellColor.Visibility = Visibility.Visible;
                btnBackToGame.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Set current ants rule string updating UI in the process
        /// </summary>
        public string Rule
        {
            get
            {
                return rule;
            }
            set
            {
                if (buffer.Current == null)
                    throw new InvalidOperationException("Cannot set ants rule when current game state is null");

                if (!IsRuleValid(value))
                    throw new InvalidOperationException("The rule can only consist from L and R characters and be from 2 to 14 in length. Example: LLRR");

                foreach (Ant a in buffer.Current.Ants)
                {
                    ((GeneralizedAnt)a).Rule = value;
                }
                buffer.FlushBuffer();

                SetRuleText(value);
                rule = value;
                logger.LogInformation($"Rule for ant has changed, new rule string: {value}");
            }
        }

        private bool IsRuleValid(string proposedRule)
        {
            return Regex.IsMatch(proposedRule, "^[L|R]{2,14}$");
        }

        private void SetRuleText(string rule)
        {
            List<TextBlock> tbs = CreateColoredRuleControls(rule);
            pnlRuleText.Children.Clear();
            foreach (TextBlock tb in tbs)
            {
                pnlRuleText.Children.Add(tb);
            }
        }

        public int MaxColor
        {
            get { return (rule?.Length ?? 0) - 1; }
        }

        #endregion

        #region UI

        private void UpdateGameView(IGame gameState)
        {
            try
            {
                ImageSource source;
                source = GameImageRenderer.GetGenerationImageSourceX2(gameState);
                imgGame.Source = source;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Wasn't able to render image for a game state.");
            }
            lblGenerationN.Text = gameState.GenerationN.ToString();
        }

        private List<TextBlock> CreateColoredRuleControls(string rule)
        {
            BrushConverter bc = new BrushConverter();
            return rule
                // Calculate colors for rule letters.
                .Select((c, index) => new { Char = c, Color = ColorBytes.ColorSequence[index] })
                .Select(cc => new TextBlock()
                {
                    Text = cc.Char.ToString(),
                    // Invert black text on very dark backgound.
                    Foreground = (cc.Color[0] + cc.Color[1] + cc.Color[2] < 128 * 2.5) ? Brushes.White : Brushes.Black,
                    // cc.Color is BGR
                    Background = new SolidColorBrush(Color.FromRgb(cc.Color[2], cc.Color[1], cc.Color[0])),
                    FontWeight = FontWeights.Bold
                }).ToList();
        }
        #endregion

        #region Event Handlers

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            PlayUIState = PlayUIMode.Playing;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            PlayUIState = PlayUIMode.Paused;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            PlayUIState = PlayUIMode.Stopped;
        }

        private void btnStepBackward_Click(object sender, RoutedEventArgs e)
        {
            PlayUIState = PlayUIMode.Paused;

            if (!buffer.MovePrevious())
                MessageBox.Show($"Cannot move back. We only store a limited number of previous generations of the game.");
            UpdateGameView(buffer.Current!);
        }

        private void btnStepForward_Click(object sender, RoutedEventArgs e)
        {
            PlayUIState = PlayUIMode.Paused;

            if (!buffer.MoveNext())
                MessageBox.Show("Game Over. We no longer have any ants.");
            UpdateGameView(buffer.Current!);
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (buffer.Current == null)
                throw new InvalidOperationException("Cannot save the game when current game state is null");

            PlayUIState = PlayUIMode.Paused;

            var saveFileDialog = new SaveFileDialog() { Filter = "JSON Document(*.json)|*.json" };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string jsonString = GameJSONSerializer.ToJson(buffer.Current);
                    await File.WriteAllTextAsync(saveFileDialog.FileName, jsonString);
                }
                catch (JSONSerializationException jsex)
                {
                    MessageBox.Show(jsex.Message);
                    return;
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Could not save JSON file to disk. {ex}");
                    return;
                }
                logger.LogInformation($"Game saved to {saveFileDialog.SafeFileName}");
            }
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            PlayUIState = PlayUIMode.Paused;

            string fileName;
            var openFileDialog = new OpenFileDialog() { Filter = "JSON Document(*.json)|*.json" };
            if (openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;
                try
                {
                    string json = await File.ReadAllTextAsync(fileName);
                    IGame loadedGame = GameJSONSerializer.FromJson(json);
                    buffer = CreateGameBuffer(loadedGame, nGenerations, rule);

                    // Update the rule in te UI              
                    string? commonRule = CalculateCommonRule(buffer.Current.Ants);
                    if (commonRule != null)
                    {
                        txtEditRule.Text = commonRule;
                        this.Rule = commonRule;
                    }
                    else
                    {
                        SetRuleText("VARIOUS");
                    }
                    UpdateGameView(buffer.Current);
                }
                catch (JSONSerializationException jsex)
                {
                    MessageBox.Show(jsex.Message);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Could not load JSON file from disk. {ex}");
                }
                logger.LogInformation($"Loaded game from {openFileDialog.SafeFileName}");
            }
        }

        private void btnEditRuleStart_Click(object sender, RoutedEventArgs e)
        {
            EditUIState = EditUIMode.EditingRule;
        }

        private void btnEditRuleApply_Click(object sender, RoutedEventArgs e)
        {
            string newText = (txtEditRule.Text ?? "").ToUpper();

            try
            {
                Rule = newText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            EditUIState = EditUIMode.NotEditing;
        }

        private void btnEditRuleCancel_Click(object sender, RoutedEventArgs e)
        {
            EditUIState = EditUIMode.NotEditing;
        }

        private void btnEditAnt_Click(object sender, RoutedEventArgs e)
        {
            EditUIState = EditUIMode.EditingAnt;
        }

        private void btnEditCellColor_Click(object sender, RoutedEventArgs e)
        {
            EditUIState = EditUIMode.EditingColors;
        }

        private void btnBackToGame_Click(object sender, RoutedEventArgs e)
        {
            EditUIState = EditUIMode.NotEditing;
        }

        /// <summary>
        /// If all the ants have the same rule, return the rule string, else return null
        /// </summary>
        /// <param name="ants">GeneralizedAnt list</param>
        /// <returns></returns>
        private string? CalculateCommonRule(IList<IAnt> ants)
        {
            string? commonRule = null;
            foreach (IAnt a in ants)
            {
                string rule = ((GeneralizedAnt)a).Rule;
                if (commonRule == null)
                    commonRule = rule;
                else if (rule != commonRule)
                    return null;
            }
            return commonRule;
        }

        private void imgGame_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (buffer.Current == null)
                throw new InvalidOperationException("Cannot edit game field when current game state is null");

            var img = (Image)sender;

            var ratio = (double)img.RenderSize.Width / buffer.Current.Size;
            Point clickPoint = e.GetPosition(img);
            var gameI = (int)(clickPoint.Y / ratio);
            var gameJ = (int)(clickPoint.X / ratio);

            if (EditUIState == EditUIMode.EditingAnt)
            {
                IAnt? ant = buffer.Current.Ants.FirstOrDefault(a => (gameI == a.I) && (gameJ == a.J));
                if (ant != null)
                {
                    if (ant.Direction == AntDirection.Left)
                    {
                        // Remove the ant
                        buffer.Current.Ants.Remove(ant);
                    }
                    else
                    {
                        // Turn the ant
                        ant.RotateCW();
                    }
                }
                else
                {
                    // Add ant
                    ant = new GeneralizedAnt(i: gameI, j: gameJ, direction: AntDirection.Up) { Rule = this.Rule };
                    buffer.Current.Ants.Add(ant);
                }
            }

            if (EditUIState == EditUIMode.EditingColors)
            {
                var newColorIndex = (byte)((buffer.Current.Field[gameI, gameJ] + 1) % (MaxColor + 1));
                buffer.Current.Field[gameI, gameJ] = newColorIndex;
            }
            UpdateGameView(buffer.Current);
        }
        #endregion
    }
}