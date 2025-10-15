using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simply_Calc_AppWPF.core;

namespace Simply_Calc_AppWPF.Services
{

    /// <summary>
    /// Informations sur une fonction enregistrée.
    /// </summary>
    public record FunctionInfo
    {
        /// <summary>
        /// Nom de la fonction (ex: "sin", "cos").
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Description de la fonction.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Catégorie de la fonction.
        /// </summary>
        public string Category { get; init; }

        /// <summary>
        /// Nombre de paramètres requis.
        /// </summary>
        public int ParameterCount { get; init; }

        /// <summary>
        /// Domaine de validité (description textuelle).
        /// </summary>
        public string Domain { get; init; }

        /// <summary>
        /// Image de la fonction (description textuelle).
        /// </summary>
        public string Range { get; init; }

        public FunctionInfo(string name, string description, string category, int paramCount = 1)
        {
            Name = name;
            Description = description;
            Category = category;
            ParameterCount = paramCount;
            Domain = "ℝ";
            Range = "ℝ";
        }

        public override string ToString() => $"{Name}: {Description}";
    }

    /// <summary>
    /// Registre centralisé de toutes les fonctions scientifiques disponibles.
    /// Permet l'enregistrement, la recherche et l'exécution de fonctions.
    /// </summary>
    public class FunctionRegistry
    {
        private readonly Dictionary<string, Func<double, double?, double>> _functions;
        private readonly Dictionary<string, FunctionInfo> _functionsInfo;

        public FunctionRegistry()
        {
            _functions = new Dictionary<string, Func<double, double?, double>>();
            _functionsInfo = new Dictionary<string, FunctionInfo>();

            InitializeDefaultFunctions();
        }

        // ==================== INITIALISATION ====================

        /// <summary>
        /// Initialise toutes les fonctions par défaut de la calculatrice.
        /// </summary>
        private void InitializeDefaultFunctions()
        {
            InitializeTrigonometric();
            InitializeInverseTrigonometric();
            InitializeConstants();
        }

        /// <summary>
        /// Initialise les fonctions trigonométriques (sin, cos, tan).
        /// </summary>
        private void InitializeTrigonometric()
        {
            // Sinus
            RegisterFunction(
                CalculatorConstants.FUNCTION_SIN,
                (angle, multiplier) => multiplier.HasValue
                    ? multiplier.Value * Math.Sin(angle)
                    : Math.Sin(angle),
                new FunctionInfo(
                    CalculatorConstants.FUNCTION_SIN,
                    "Calcule le sinus d'un angle",
                    "Trigonométrique"
                )
                {
                    Domain = "ℝ (en radians)",
                    Range = "[-1, 1]"
                }
            );

            // Cosinus
            RegisterFunction(
                CalculatorConstants.FUNCTION_COS,
                (angle, multiplier) => multiplier.HasValue
                    ? multiplier.Value * Math.Cos(angle)
                    : Math.Cos(angle),
                new FunctionInfo(
                    CalculatorConstants.FUNCTION_COS,
                    "Calcule le cosinus d'un angle",
                    "Trigonométrique"
                )
                {
                    Domain = "ℝ (en radians)",
                    Range = "[-1, 1]"
                }
            );

            // Tangente
            RegisterFunction(
                CalculatorConstants.FUNCTION_TAN,
                (angle, multiplier) => multiplier.HasValue
                    ? multiplier.Value * Math.Tan(angle)
                    : Math.Tan(angle),
                new FunctionInfo(
                    CalculatorConstants.FUNCTION_TAN,
                    "Calcule la tangente d'un angle",
                    "Trigonométrique"
                )
                {
                    Domain = "ℝ \\ {π/2 + kπ} (en radians)",
                    Range = "ℝ"
                }
            );
        }

        /// <summary>
        /// Initialise les fonctions trigonométriques inverses (arcsin, arccos, arctan).
        /// </summary>
        private void InitializeInverseTrigonometric()
        {
            // Arc Sinus
            RegisterFunction(
                CalculatorConstants.FUNCTION_ARCSIN,
                (value, multiplier) =>
                {
                    if (value < -1 || value > 1)
                        return double.NaN;

                    double result = Math.Asin(value);
                    return multiplier.HasValue ? multiplier.Value * result : result;
                },
                new FunctionInfo(
                    CalculatorConstants.FUNCTION_ARCSIN,
                    "Calcule l'arc sinus (inverse du sinus)",
                    "Trigonométrique Inverse"
                )
                {
                    Domain = "[-1, 1]",
                    Range = "[-π/2, π/2] (en radians)"
                }
            );

            // Arc Cosinus
            RegisterFunction(
                CalculatorConstants.FUNCTION_ARCCOS,
                (value, multiplier) =>
                {
                    if (value < -1 || value > 1)
                        return double.NaN;

                    double result = Math.Acos(value);
                    return multiplier.HasValue ? multiplier.Value * result : result;
                },
                new FunctionInfo(
                    CalculatorConstants.FUNCTION_ARCCOS,
                    "Calcule l'arc cosinus (inverse du cosinus)",
                    "Trigonométrique Inverse"
                )
                {
                    Domain = "[-1, 1]",
                    Range = "[0, π] (en radians)"
                }
            );

            // Arc Tangente
            RegisterFunction(
                CalculatorConstants.FUNCTION_ARCTAN,
                (value, multiplier) =>
                {
                    double result = Math.Atan(value);
                    return multiplier.HasValue ? multiplier.Value * result : result;
                },
                new FunctionInfo(
                    CalculatorConstants.FUNCTION_ARCTAN,
                    "Calcule l'arc tangente (inverse de la tangente)",
                    "Trigonométrique Inverse"
                )
                {
                    Domain = "ℝ",
                    Range = "]-π/2, π/2[ (en radians)"
                }
            );
        }

        /// <summary>
        /// Initialise les constantes mathématiques.
        /// </summary>
        private void InitializeConstants()
        {
            // Pi (π)
            RegisterFunction(
                "π",
                (multiplier, _) => multiplier * CalculatorConstants.PI,
                new FunctionInfo("π", "Constante Pi (3.14159...)", "Constante", 0)
                {
                    Domain = "N/A",
                    Range = "≈ 3.14159"
                }
            );

            // Euler (e)
            RegisterFunction(
                "e",
                (multiplier, _) => multiplier * CalculatorConstants.E,
                new FunctionInfo("e", "Constante d'Euler (2.71828...)", "Constante", 0)
                {
                    Domain = "N/A",
                    Range = "≈ 2.71828"
                }
            );
        }

        // ==================== ENREGISTREMENT ====================

        /// <summary>
        /// Enregistre une nouvelle fonction dans le registre.
        /// </summary>
        /// <param name="name">Nom unique de la fonction</param>
        /// <param name="function">Implémentation de la fonction</param>
        /// <param name="info">Informations descriptives</param>
        public void RegisterFunction(string name, Func<double, double?, double> function, FunctionInfo info)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (function == null)
                throw new ArgumentNullException(nameof(function));

            if (info == null)
                throw new ArgumentNullException(nameof(info));

            _functions[name] = function;
            _functionsInfo[name] = info;
        }

        /// <summary>
        /// Désenregistre une fonction du registre.
        /// </summary>
        /// <param name="name">Nom de la fonction à supprimer</param>
        /// <returns>True si suppression réussie</returns>
        public bool UnregisterFunction(string name)
        {
            bool removedFunc = _functions.Remove(name);
            bool removedInfo = _functionsInfo.Remove(name);
            return removedFunc && removedInfo;
        }

        // ==================== RÉCUPÉRATION ====================

        /// <summary>
        /// Récupère une fonction par son nom.
        /// </summary>
        /// <param name="name">Nom de la fonction</param>
        /// <returns>Fonction ou null si non trouvée</returns>
        public Func<double, double?, double>? GetFunction(string name)
        {
            return _functions.TryGetValue(name, out var func) ? func : null;
        }

        /// <summary>
        /// Récupère les informations d'une fonction.
        /// </summary>
        /// <param name="name">Nom de la fonction</param>
        /// <returns>Informations ou null si non trouvée</returns>
        public FunctionInfo? GetFunctionInfo(string name)
        {
            return _functionsInfo.TryGetValue(name, out var info) ? info : null;
        }

        /// <summary>
        /// Obtient toutes les fonctions disponibles.
        /// </summary>
        /// <returns>Dictionnaire des fonctions</returns>
        public IReadOnlyDictionary<string, Func<double, double?, double>> GetAllFunctions()
        {
            return _functions;
        }

        /// <summary>
        /// Obtient toutes les informations de fonctions.
        /// </summary>
        /// <returns>Collection des informations</returns>
        public IEnumerable<FunctionInfo> GetAllFunctionInfos()
        {
            return _functionsInfo.Values;
        }

        // ==================== RECHERCHE ====================

        /// <summary>
        /// Recherche des fonctions par catégorie.
        /// </summary>
        /// <param name="category">Catégorie recherchée</param>
        /// <returns>Liste des fonctions correspondantes</returns>
        public IEnumerable<FunctionInfo> SearchByCategory(string category)
        {
            return _functionsInfo.Values
                .Where(info => info.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Recherche des fonctions par mot-clé dans le nom ou la description.
        /// </summary>
        /// <param name="keyword">Mot-clé recherché</param>
        /// <returns>Liste des fonctions correspondantes</returns>
        public IEnumerable<FunctionInfo> SearchByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<FunctionInfo>();

            return _functionsInfo.Values.Where(info =>
                info.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                info.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Obtient toutes les catégories disponibles.
        /// </summary>
        /// <returns>Liste unique des catégories</returns>
        public IEnumerable<string> GetAllCategories()
        {
            return _functionsInfo.Values
                .Select(info => info.Category)
                .Distinct()
                .OrderBy(cat => cat);
        }

        // ==================== VALIDATION ====================

        /// <summary>
        /// Vérifie si une fonction existe dans le registre.
        /// </summary>
        /// <param name="name">Nom de la fonction</param>
        /// <returns>True si la fonction existe</returns>
        public bool FunctionExists(string name)
        {
            return _functions.ContainsKey(name);
        }

        /// <summary>
        /// Valide si une valeur est dans le domaine d'une fonction.
        /// </summary>
        /// <param name="functionName">Nom de la fonction</param>
        /// <param name="value">Valeur à tester</param>
        /// <returns>True si valide</returns>
        public bool IsValueInDomain(string functionName, double value)
        {
            if (!FunctionExists(functionName))
                return false;

            // Validation spécifique pour fonctions arc
            if (functionName is CalculatorConstants.FUNCTION_ARCSIN or CalculatorConstants.FUNCTION_ARCCOS)
            {
                return value >= CalculatorConstants.ARCSIN_ARCCOS_MIN &&
                       value <= CalculatorConstants.ARCSIN_ARCCOS_MAX;
            }

            // Pas de contrainte pour les autres fonctions
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        // ==================== EXÉCUTION ====================

        /// <summary>
        /// Exécute une fonction sur une valeur donnée.
        /// </summary>
        /// <param name="functionName">Nom de la fonction</param>
        /// <param name="value">Valeur d'entrée</param>
        /// <param name="multiplier">Multiplicateur optionnel</param>
        /// <returns>Résultat du calcul</returns>
        /// <exception cref="ArgumentException">Si fonction inconnue</exception>
        public double Execute(string functionName, double value, double? multiplier = null)
        {
            if (!FunctionExists(functionName))
                throw new ArgumentException($"Fonction inconnue: {functionName}", nameof(functionName));

            var function = _functions[functionName];
            return function(value, multiplier);
        }

        /// <summary>
        /// Tente d'exécuter une fonction avec gestion d'erreurs.
        /// </summary>
        /// <param name="functionName">Nom de la fonction</param>
        /// <param name="value">Valeur d'entrée</param>
        /// <param name="result">Résultat si succès</param>
        /// <param name="multiplier">Multiplicateur optionnel</param>
        /// <returns>True si exécution réussie</returns>
        public bool TryExecute(string functionName, double value, out double result, double? multiplier = null)
        {
            result = 0;

            try
            {
                if (!FunctionExists(functionName))
                    return false;

                if (!IsValueInDomain(functionName, value))
                    return false;

                result = Execute(functionName, value, multiplier);
                return CalculatorEngine.IsValidResult(result);
            }
            catch
            {
                return false;
            }
        }

        // ==================== UTILITAIRES ====================

        /// <summary>
        /// Nombre total de fonctions enregistrées.
        /// </summary>
        public int Count => _functions.Count;

        /// <summary>
        /// Génère un rapport textuel de toutes les fonctions disponibles.
        /// </summary>
        /// <returns>Rapport formaté</returns>
        public string GenerateReport()
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("=== FONCTIONS DISPONIBLES ===\n");

            var categories = GetAllCategories();

            foreach (var category in categories)
            {
                report.AppendLine($"--- {category} ---");
                var functions = SearchByCategory(category);

                foreach (var func in functions)
                {
                    report.AppendLine($"  • {func.Name}: {func.Description}");
                    report.AppendLine($"    Domaine: {func.Domain}");
                    report.AppendLine($"    Image: {func.Range}");
                    report.AppendLine();
                }
            }

            return report.ToString();
        }

        /// <summary>
        /// Représentation textuelle du registre.
        /// </summary>
        public override string ToString()
        {
            return $"FunctionRegistry: {Count} fonctions enregistrées";
        }
    }
}
