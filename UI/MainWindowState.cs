using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simply_Calc_AppWPF.core;
using Simply_Calc_AppWPF.Models;
using Simply_Calc_AppWPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace Simply_Calc_AppWPF
{
    /// <summary>
    /// Partial class contenant toutes les variables d'état de la calculatrice.
    /// </summary>
    public partial class MainWindow : Window
    {
        // ==================== ÉTAT PRINCIPAL ====================

        /// <summary>
        /// État complet de la calculatrice (encapsule toutes les variables).
        /// </summary>
        private CalculatorState _state = new();

        // ==================== SERVICES ====================

        /// <summary>
        /// Moteur de calcul principal.
        /// </summary>
        private CalculatorEngine _engine = new();

        /// <summary>
        /// Évaluateur d'expressions mathématiques.
        /// </summary>
        private ExpressionEvaluator _evaluator = new();

        /// <summary>
        /// Registre des fonctions scientifiques.
        /// </summary>
        private FunctionRegistry _functionRegistry = new();

        /// <summary>
        /// Gestionnaire d'erreurs.
        /// </summary>
        private ErrorHandler _errorHandler = new();

        /// <summary>
        /// Historique des opérations.
        /// </summary>
        private OperationHistory _history = new();

        // ==================== DICTIONNAIRES ====================

        /// <summary>
        /// Dictionnaire des opérateurs binaires (+, -, ×, ÷).
        /// </summary>
        private Dictionary<string, Func<double, double, double>> _operations = new();

        /// <summary>
        /// Dictionnaire des fonctions scientifiques (sin, cos, tan, etc.).
        /// </summary>
        private Dictionary<string, Func<double, double?, double>> _functions = new();

        // ==================== TAILLES DE POLICE PAR DÉFAUT ====================

        /// <summary>
        /// Taille de police par défaut du champ resultatTexte.
        /// </summary>
        private double _defaultResultatFontSize = new();

        /// <summary>
        /// Taille de police par défaut du bloc ResultatBlock.
        /// </summary>
        private double _defaultResultBlockFontSize = new();

        /// <summary>
        /// Taille de police par défaut du champ operationTexte.
        /// </summary>
        private double _defaultOperationFontSize = new();

        // ==================== INITIALISATION ====================

        /// <summary>
        /// Initialise l'état de la calculatrice et tous les services.
        /// </summary>
        private void InitializeState()
        {
            // Créer l'état
            _state = new CalculatorState();

            // Initialiser les services
            _engine = new CalculatorEngine();
            _evaluator = new ExpressionEvaluator();
            _functionRegistry = new FunctionRegistry();
            _errorHandler = new ErrorHandler(maxHistorySize: 50);
            _history = new OperationHistory(maxCapacity: 100);

            // Initialiser les dictionnaires (seront remplis plus tard)
            _operations = new Dictionary<string, Func<double, double, double>>();
            _functions = new Dictionary<string, Func<double, double?, double>>();
        }

        /// <summary>
        /// Initialise le dictionnaire des opérateurs binaires.
        /// </summary>
        private void InitializeOperators()
        {
            _operations = new Dictionary<string, Func<double, double, double>>
            {
                { CalculatorConstants.OPERATOR_ADDITION, (a, b) => a + b },
                { CalculatorConstants.OPERATOR_SUBTRACTION, (a, b) => a - b },
                { CalculatorConstants.OPERATOR_MULTIPLICATION, (a, b) => a * b },
                { CalculatorConstants.OPERATOR_MULTIPLICATION_ALT, (a, b) => a * b },
                { CalculatorConstants.OPERATOR_DIVISION, (a, b) => a / b },
                { CalculatorConstants.OPERATOR_DIVISION_ALT, (a, b) => a / b }
            };
        }

        /// <summary>
        /// Initialise le dictionnaire des fonctions scientifiques.
        /// </summary>
        private void InitializeFunctions()
        {
            _functions = new Dictionary<string, Func<double, double?, double>>
            {
                // Fonctions trigonométriques directes
                {
                    CalculatorConstants.FUNCTION_SIN,
                    (angle, multiplier) => multiplier.HasValue
                        ? multiplier.Value * Math.Sin(angle)
                        : Math.Sin(angle)
                },
                {
                    CalculatorConstants.FUNCTION_COS,
                    (angle, multiplier) => multiplier.HasValue
                        ? multiplier.Value * Math.Cos(angle)
                        : Math.Cos(angle)
                },
                {
                    CalculatorConstants.FUNCTION_TAN,
                    (angle, multiplier) => multiplier.HasValue
                        ? multiplier.Value * Math.Tan(angle)
                        : Math.Tan(angle)
                },

                // Fonctions trigonométriques inverses
                {
                    CalculatorConstants.FUNCTION_ARCSIN,
                    (value, multiplier) =>
                    {
                        if (value < -1 || value > 1)
                            return double.NaN;

                        double result = Math.Asin(value);
                        return multiplier.HasValue ? multiplier.Value * result : result;
                    }
                },
                {
                    CalculatorConstants.FUNCTION_ARCCOS,
                    (value, multiplier) =>
                    {
                        if (value < -1 || value > 1)
                            return double.NaN;

                        double result = Math.Acos(value);
                        return multiplier.HasValue ? multiplier.Value * result : result;
                    }
                },
                {
                    CalculatorConstants.FUNCTION_ARCTAN,
                    (value, multiplier) =>
                    {
                        double result = Math.Atan(value);
                        return multiplier.HasValue ? multiplier.Value * result : result;
                    }
                }
            };
        }
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            string digit = btn.Content?.ToString() ?? "";

            // Fermeture parenthèse si nécessaire
            if (_state.IsParenthesisJustClosed)
            {
                OperatorButton_Click(CalculatorConstants.OPERATOR_MULTIPLICATION);
                _state.IsParenthesisJustClosed = false;
            }

            // Gestion constantes
            if (_state.IsConstantUsed)
            {
                OperatorButton_Click(CalculatorConstants.OPERATOR_MULTIPLICATION);
                _state.IsConstantUsed = false;
            }

            // Vérification longueur max
            if (_state.InputBuffer.Length >= CalculatorConstants.MAX_INPUT_LENGTH)
                return;

            // Reset si égal vient d'être fait
            if (_state.IsEqualDone)
            {
                ResetCalculatorExcept();
                _state.IsEqualDone = false;
            }

            // Ajout du chiffre
            _state.InputBuffer += digit;
            resultatTexte.Text = _state.InputBuffer;
        }

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier qu'il n'y a pas déjà un point
            if (_state.InputBuffer.Contains(".") || _state.InputBuffer.Contains(","))
                return;

            // Si buffer vide, ajouter "0."
            if (string.IsNullOrEmpty(_state.InputBuffer))
            {
                _state.InputBuffer = "0.";
            }
            else
            {
                _state.InputBuffer += ".";
            }

            resultatTexte.Text = _state.InputBuffer;
        }
    }

}

