using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Simply_Calc_AppWPF.core;

namespace Simply_Calc_AppWPF
{

    /// <summary>
    /// Fenêtre principale de la calculatrice scientifique WPF.
    /// Coordonne les partial classes et gère le cycle de vie de l'application.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructeur principal - Initialise tous les composants.
        /// </summary>
        public MainWindow()
        {
            // Initialisation XAML
            InitializeComponent();

            // Initialisation de l'état
            InitializeState();

            // Initialisation des opérateurs et fonctions
            InitializeOperators();
            InitializeFunctions();

            // Sauvegarde des tailles de police par défaut
            SaveDefaultFontSizes();

            // Abonnement aux événements
            SubscribeToEvents();

            // Configuration du titre de la fenêtre
            this.Title = $"{CalculatorConstants.APP_NAME} - v{CalculatorConstants.APP_VERSION}";

            // Événement de mise à jour de layout
            this.LayoutUpdated += MainWindow_LayoutUpdated;

            // Événement clavier global
            this.KeyDown += MainWindow_KeyDown;
        }

        /// <summary>
        /// Sauvegarde les tailles de police par défaut pour reset ultérieur.
        /// </summary>
        private void SaveDefaultFontSizes()
        {
            _defaultResultatFontSize = resultatTexte.FontSize;
            _defaultResultBlockFontSize = ResultatBlock.FontSize;
            _defaultOperationFontSize = operationTexte.FontSize;
        }

        /// <summary>
        /// Abonne tous les événements nécessaires.
        /// </summary>
        private void SubscribeToEvents()
        {
            // Événements de changement de texte
            resultatTexte.TextChanged += resultatTexte_TextChanged;
            operationTexte.TextChanged += OperationTexte_TextChanged;
            ResultatBlock.TextChanged += ResultatBlock_TextChanged;

            // Événements des boutons numériques
            zeroBouton.Click += NumberButton_Click;
            unBouton.Click += NumberButton_Click;
            deuxBouton.Click += NumberButton_Click;
            troisBouton.Click += NumberButton_Click;
            quatreBouton.Click += NumberButton_Click;
            cinqBouton.Click += NumberButton_Click;
            sixBouton.Click += NumberButton_Click;
            septBouton.Click += NumberButton_Click;
            huitBouton.Click += NumberButton_Click;
            neufBouton.Click += NumberButton_Click;
            pointBouton.Click += DecimalButton_Click;

            // Événements des opérateurs
            additionBouton.Click += (s, e) => OperatorButton_Click(CalculatorConstants.OPERATOR_ADDITION);
            soustractionBouton.Click += (s, e) => OperatorButton_Click(CalculatorConstants.OPERATOR_SUBTRACTION);
            multiplicationBouton.Click += (s, e) => OperatorButton_Click(CalculatorConstants.OPERATOR_MULTIPLICATION);
            divisionBouton.Click += (s, e) => OperatorButton_Click(CalculatorConstants.OPERATOR_DIVISION);

            // Événements des fonctions trigonométriques
            sinBouton.Click += FuncButton_Click;
            cosBouton.Click += FuncButton_Click;
            tanBouton.Click += FuncButton_Click;
            arcSinBouton.Click += FuncButton_Click;
            arcCosBouton.Click += FuncButton_Click;
            arcTanBouton.Click += FuncButton_Click;

            // Événements des constantes
            piBouton.Click += Pi_Click;
            expBouton.Click += Exp_Click;

            // Événements des touches spéciales
            egalBouton.Click += Equals_Click;
            effacerDernierBoutton.Click += DeleteLast_Click;
            effacerEntreeBouton.Click += ClearEntry_Click;
            effacerToutBouton.Click += ClearAll_Click;
            changerSigneBouton.Click += ChangeSign_Click;

            // Événement commun à tous les boutons (pour désactivation focus)
            foreach (var button in new[] {
                zeroBouton, unBouton, deuxBouton, troisBouton, quatreBouton, cinqBouton,
                sixBouton, septBouton, huitBouton, neufBouton, pointBouton,
                additionBouton, soustractionBouton, multiplicationBouton, divisionBouton,
                sinBouton, cosBouton, tanBouton, arcSinBouton, arcCosBouton, arcTanBouton,
                piBouton, expBouton, egalBouton, effacerDernierBoutton,
                effacerEntreeBouton, effacerToutBouton, changerSigneBouton
            })
            {
                button.Click += CommonButtonClick;
            }
        }

        /// <summary>
        /// Gestionnaire d'événement commun à tous les boutons.
        /// Désactive le focus et gère les états d'erreur.
        /// </summary>
        private void CommonButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Focusable = false;
            }

            // Si erreur affichée, nettoyer avant nouvelle saisie
            if (_state.IsErrorState)
            {
                ClearErrorStyles();
                _state.InputBuffer = string.Empty;
                resultatTexte.Text = "0";
                operationTexte.Text = string.Empty;
            }

            // Si opération vient d'être faite, préparer nouvelle saisie
            if (_state.IsOperationJustDone)
            {
                _state.InputBuffer = string.Empty;
                _state.IsOperationJustDone = false;
            }
        }

        /// <summary>
        /// Événement de fermeture de la fenêtre.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Sauvegarder l'historique si configuré
            if (_history != null && _history.Count > 0)
            {
                try
                {
                    string logPath = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "CalculatriceWPF",
                        "history.csv"
                    );

                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath));
                    _history.ExportToCsv(logPath);
                }
                catch
                {
                    // Ignorer erreurs de sauvegarde
                }
            }
        }
    }
}