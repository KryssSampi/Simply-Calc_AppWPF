using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simply_Calc_AppWPF.Models
{
    /// <summary>
 /// Types d'opérateurs supportés par la calculatrice.
 /// </summary>
    public enum OperatorType
    {
        /// <summary>
        /// Opérateur binaire (requiert deux opérandes).
        /// </summary>
        Binary,

        /// <summary>
        /// Opérateur unaire (requiert un seul opérande).
        /// </summary>
        Unary,

        /// <summary>
        /// Fonction scientifique (sin, cos, tan, etc.).
        /// </summary>
        Function
    }

    /// <summary>
    /// Catégories d'opérateurs binaires.
    /// </summary>
    public enum BinaryOperatorCategory
    {
        /// <summary>
        /// Addition (+).
        /// </summary>
        Addition,

        /// <summary>
        /// Soustraction (-).
        /// </summary>
        Subtraction,

        /// <summary>
        /// Multiplication (×, *).
        /// </summary>
        Multiplication,

        /// <summary>
        /// Division (÷, /).
        /// </summary>
        Division,

        /// <summary>
        /// Puissance (^).
        /// </summary>
        Power,

        /// <summary>
        /// Modulo (%).
        /// </summary>
        Modulo
    }

    /// <summary>
    /// Catégories d'opérateurs unaires.
    /// </summary>
    public enum UnaryOperatorCategory
    {
        /// <summary>
        /// Négation (changement de signe).
        /// </summary>
        Negation,

        /// <summary>
        /// Valeur absolue.
        /// </summary>
        Absolute,

        /// <summary>
        /// Racine carrée.
        /// </summary>
        SquareRoot,

        /// <summary>
        /// Inverse (1/x).
        /// </summary>
        Inverse
    }

    /// <summary>
    /// Catégories de fonctions scientifiques.
    /// </summary>
    public enum FunctionCategory
    {
        /// <summary>
        /// Fonction trigonométrique directe (sin, cos, tan).
        /// </summary>
        Trigonometric,

        /// <summary>
        /// Fonction trigonométrique inverse (arcsin, arccos, arctan).
        /// </summary>
        InverseTrigonometric,

        /// <summary>
        /// Fonction logarithmique (log, ln).
        /// </summary>
        Logarithmic,

        /// <summary>
        /// Fonction exponentielle (exp, pow).
        /// </summary>
        Exponential,

        /// <summary>
        /// Constante mathématique (π, e).
        /// </summary>
        Constant
    }

    /// <summary>
    /// Informations détaillées sur un opérateur ou une fonction.
    /// </summary>
    public record OperatorInfo
    {
        /// <summary>
        /// Nom ou symbole de l'opérateur.
        /// </summary>
        public string Symbol { get; init; }

        /// <summary>
        /// Type d'opérateur.
        /// </summary>
        public OperatorType Type { get; init; }

        /// <summary>
        /// Catégorie spécifique (optionnelle).
        /// </summary>
        public object? Category { get; init; }

        /// <summary>
        /// Description textuelle de l'opérateur.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Priorité d'exécution (1 = plus haute).
        /// </summary>
        public int Priority { get; init; }

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public OperatorInfo()
        {
            Symbol = string.Empty;
            Type = OperatorType.Binary;
            Category = null;
            Description = string.Empty;
            Priority = 0;
        }

        /// <summary>
        /// Constructeur complet.
        /// </summary>
        public OperatorInfo(string symbol, OperatorType type, string description, int priority = 0)
        {
            Symbol = symbol;
            Type = type;
            Description = description;
            Priority = priority;
            Category = null;
        }

        /// <summary>
        /// Représentation textuelle.
        /// </summary>
        public override string ToString()
        {
            return $"{Symbol} ({Type}): {Description}";
        }
    }

    /// <summary>
    /// Classe utilitaire pour gérer les informations des opérateurs.
    /// </summary>
    public static class OperatorRegistry
    {
        /// <summary>
        /// Dictionnaire des opérateurs binaires avec leurs informations.
        /// </summary>
        public static readonly Dictionary<string, OperatorInfo> BinaryOperators = new()
        {
            {
                "+",
                new OperatorInfo("+", OperatorType.Binary, "Addition", priority: 1)
                {
                    Category = BinaryOperatorCategory.Addition
                }
            },
            {
                "-",
                new OperatorInfo("-", OperatorType.Binary, "Soustraction", priority: 1)
                {
                    Category = BinaryOperatorCategory.Subtraction
                }
            },
            {
                "×",
                new OperatorInfo("×", OperatorType.Binary, "Multiplication", priority: 2)
                {
                    Category = BinaryOperatorCategory.Multiplication
                }
            },
            {
                "*",
                new OperatorInfo("*", OperatorType.Binary, "Multiplication (alt)", priority: 2)
                {
                    Category = BinaryOperatorCategory.Multiplication
                }
            },
            {
                "÷",
                new OperatorInfo("÷", OperatorType.Binary, "Division", priority: 2)
                {
                    Category = BinaryOperatorCategory.Division
                }
            },
            {
                "/",
                new OperatorInfo("/", OperatorType.Binary, "Division (alt)", priority: 2)
                {
                    Category = BinaryOperatorCategory.Division
                }
            }
        };

        /// <summary>
        /// Dictionnaire des fonctions scientifiques avec leurs informations.
        /// </summary>
        public static readonly Dictionary<string, OperatorInfo> Functions = new()
        {
            {
                "sin",
                new OperatorInfo("sin", OperatorType.Function, "Sinus", priority: 3)
                {
                    Category = FunctionCategory.Trigonometric
                }
            },
            {
                "cos",
                new OperatorInfo("cos", OperatorType.Function, "Cosinus", priority: 3)
                {
                    Category = FunctionCategory.Trigonometric
                }
            },
            {
                "tan",
                new OperatorInfo("tan", OperatorType.Function, "Tangente", priority: 3)
                {
                    Category = FunctionCategory.Trigonometric
                }
            },
            {
                "arcsin",
                new OperatorInfo("arcsin", OperatorType.Function, "Arc Sinus", priority: 3)
                {
                    Category = FunctionCategory.InverseTrigonometric
                }
            },
            {
                "arccos",
                new OperatorInfo("arccos", OperatorType.Function, "Arc Cosinus", priority: 3)
                {
                    Category = FunctionCategory.InverseTrigonometric
                }
            },
            {
                "arctan",
                new OperatorInfo("arctan", OperatorType.Function, "Arc Tangente", priority: 3)
                {
                    Category = FunctionCategory.InverseTrigonometric
                }
            }
        };

        /// <summary>
        /// Obtient les informations d'un opérateur par son symbole.
        /// </summary>
        /// <param name="symbol">Symbole de l'opérateur</param>
        /// <returns>Informations ou null si non trouvé</returns>
        public static OperatorInfo? GetOperatorInfo(string symbol)
        {
            if (BinaryOperators.TryGetValue(symbol, out var opInfo))
                return opInfo;

            if (Functions.TryGetValue(symbol, out var funcInfo))
                return funcInfo;

            return null;
        }

        /// <summary>
        /// Vérifie si un symbole est un opérateur binaire.
        /// </summary>
        public static bool IsBinaryOperator(string symbol)
        {
            return BinaryOperators.ContainsKey(symbol);
        }

        /// <summary>
        /// Vérifie si un symbole est une fonction.
        /// </summary>
        public static bool IsFunction(string symbol)
        {
            return Functions.ContainsKey(symbol);
        }

        /// <summary>
        /// Obtient la priorité d'un opérateur.
        /// </summary>
        /// <param name="symbol">Symbole de l'opérateur</param>
        /// <returns>Priorité (0 si non trouvé)</returns>
        public static int GetPriority(string symbol)
        {
            var info = GetOperatorInfo(symbol);
            return info?.Priority ?? 0;
        }
    }
}

