using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simply_Calc_AppWPF.core
{

    /// <summary>
    /// Moteur de calcul principal de la calculatrice.
    /// Gère l'exécution des opérations arithmétiques et des fonctions scientifiques.
    /// </summary>
    public class CalculatorEngine
    {
        private readonly Dictionary<string, Func<double, double, double>> _binaryOperators;
        private readonly Dictionary<string, Func<double, double?, double>> _functions;

        /// <summary>
        /// Initialise le moteur de calcul avec les opérateurs et fonctions.
        /// </summary>
        public CalculatorEngine()
        {
            _binaryOperators = InitializeBinaryOperators();
            _functions = InitializeFunctions();
        }

        // ==================== OPÉRATIONS BINAIRES ====================

        /// <summary>
        /// Calcule le résultat d'une opération binaire.
        /// </summary>
        /// <param name="firstValue">Premier opérande</param>
        /// <param name="secondValue">Second opérande</param>
        /// <param name="operatorSymbol">Symbole de l'opérateur (+, -, ×, ÷)</param>
        /// <returns>Résultat du calcul</returns>
        /// <exception cref="ArgumentException">Si l'opérateur est inconnu</exception>
        /// <exception cref="DivideByZeroException">Si division par zéro</exception>
        public double Calculate(double firstValue, double secondValue, string operatorSymbol)
        {
            if (!_binaryOperators.ContainsKey(operatorSymbol))
                throw new ArgumentException($"Opérateur inconnu: {operatorSymbol}", nameof(operatorSymbol));

            // Validation spécifique pour la division
            if (operatorSymbol == CalculatorConstants.OPERATOR_DIVISION &&
                Math.Abs(secondValue) < CalculatorConstants.FLOATING_POINT_EPSILON)
            {
                throw new DivideByZeroException(CalculatorConstants.ERROR_DIVISION_BY_ZERO);
            }

            return _binaryOperators[operatorSymbol](firstValue, secondValue);
        }

        /// <summary>
        /// Initialise le dictionnaire des opérateurs binaires.
        /// </summary>
        private Dictionary<string, Func<double, double, double>> InitializeBinaryOperators()
        {
            return new Dictionary<string, Func<double, double, double>>
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
        /// Récupère la fonction d'opération pour un symbole donné.
        /// </summary>
        /// <param name="operatorSymbol">Symbole de l'opérateur</param>
        /// <returns>Fonction d'opération ou null si non trouvée</returns>
        public Func<double, double, double>? GetBinaryOperator(string operatorSymbol)
        {
            return _binaryOperators.TryGetValue(operatorSymbol, out var op) ? op : null;
        }

        // ==================== FONCTIONS SCIENTIFIQUES ====================

        /// <summary>
        /// Applique une fonction scientifique sur une ou deux valeurs.
        /// </summary>
        /// <param name="functionName">Nom de la fonction (sin, cos, tan, etc.)</param>
        /// <param name="value">Valeur principale (en radians pour trigo)</param>
        /// <param name="multiplier">Multiplicateur optionnel</param>
        /// <returns>Résultat du calcul</returns>
        /// <exception cref="ArgumentException">Si la fonction est inconnue</exception>
        /// <exception cref="ArgumentOutOfRangeException">Si valeur hors domaine</exception>
        public double ApplyFunction(string functionName, double value, double? multiplier = null)
        {
            if (!_functions.ContainsKey(functionName))
                throw new ArgumentException($"Fonction inconnue: {functionName}", nameof(functionName));

            // Validation du domaine pour fonctions arc
            if (IsInverseTrigonometric(functionName))
            {
                if (!CalculatorConstants.IsValidForArcFunction(value))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        $"{functionName} requiert une valeur entre -1 et 1"
                    );
                }
            }

            return _functions[functionName](value, multiplier);
        }

        /// <summary>
        /// Initialise le dictionnaire des fonctions scientifiques.
        /// </summary>
        private Dictionary<string, Func<double, double?, double>> InitializeFunctions()
        {
            return new Dictionary<string, Func<double, double?, double>>
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

        /// <summary>
        /// Récupère la fonction pour un nom donné.
        /// </summary>
        /// <param name="functionName">Nom de la fonction</param>
        /// <returns>Fonction ou null si non trouvée</returns>
        public Func<double, double?, double>? GetFunction(string functionName)
        {
            return _functions.TryGetValue(functionName, out var func) ? func : null;
        }

        /// <summary>
        /// Obtient tous les noms de fonctions disponibles.
        /// </summary>
        /// <returns>Collection des noms de fonctions</returns>
        public IEnumerable<string> GetAvailableFunctions()
        {
            return _functions.Keys;
        }

        /// <summary>
        /// Obtient tous les symboles d'opérateurs disponibles.
        /// </summary>
        /// <returns>Collection des symboles d'opérateurs</returns>
        public IEnumerable<string> GetAvailableOperators()
        {
            return _binaryOperators.Keys;
        }

        // ==================== VALIDATION ====================

        /// <summary>
        /// Valide si une opération est possible sans erreur.
        /// </summary>
        /// <param name="operatorSymbol">Symbole de l'opérateur</param>
        /// <param name="firstValue">Premier opérande</param>
        /// <param name="secondValue">Second opérande</param>
        /// <returns>True si l'opération est valide</returns>
        public bool ValidateOperation(string operatorSymbol, double firstValue, double secondValue)
        {
            // Vérifier que l'opérateur existe
            if (!_binaryOperators.ContainsKey(operatorSymbol))
                return false;

            // Vérifier division par zéro
            if (operatorSymbol == CalculatorConstants.OPERATOR_DIVISION &&
                Math.Abs(secondValue) < CalculatorConstants.FLOATING_POINT_EPSILON)
                return false;

            // Vérifier que les valeurs sont valides
            if (double.IsNaN(firstValue) || double.IsNaN(secondValue))
                return false;

            if (double.IsInfinity(firstValue) || double.IsInfinity(secondValue))
                return false;

            return true;
        }

        /// <summary>
        /// Valide si une fonction peut être appliquée sur une valeur.
        /// </summary>
        /// <param name="functionName">Nom de la fonction</param>
        /// <param name="value">Valeur à tester</param>
        /// <returns>True si la fonction est applicable</returns>
        public bool ValidateFunction(string functionName, double value)
        {
            // Vérifier que la fonction existe
            if (!_functions.ContainsKey(functionName))
                return false;

            // Vérifier que la valeur est valide
            if (double.IsNaN(value))
                return false;

            // Validation domaine pour fonctions inverses
            if (IsInverseTrigonometric(functionName))
            {
                return CalculatorConstants.IsValidForArcFunction(value);
            }

            return true;
        }

        /// <summary>
        /// Vérifie si une fonction est une fonction trigonométrique inverse.
        /// </summary>
        /// <param name="functionName">Nom de la fonction</param>
        /// <returns>True si c'est une fonction arc*</returns>
        public bool IsInverseTrigonometric(string functionName)
        {
            return functionName == CalculatorConstants.FUNCTION_ARCSIN ||
                   functionName == CalculatorConstants.FUNCTION_ARCCOS ||
                   functionName == CalculatorConstants.FUNCTION_ARCTAN;
        }

        // ==================== UTILITAIRES ====================

        /// <summary>
        /// Vérifie si un résultat est valide (ni NaN ni Infinity).
        /// </summary>
        /// <param name="result">Résultat à vérifier</param>
        /// <returns>True si le résultat est valide</returns>
        public static bool IsValidResult(double result)
        {
            return !double.IsNaN(result) && !double.IsInfinity(result);
        }

        /// <summary>
        /// Arrondit un résultat selon la précision configurée.
        /// </summary>
        /// <param name="value">Valeur à arrondir</param>
        /// <param name="decimals">Nombre de décimales (défaut: 10)</param>
        /// <returns>Valeur arrondie</returns>
        public static double RoundResult(double value, int decimals = 10)
        {
            if (!IsValidResult(value))
                return value;

            return Math.Round(value, decimals);
        }
    }
}
