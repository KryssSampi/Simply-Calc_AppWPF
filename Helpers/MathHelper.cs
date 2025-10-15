using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simply_Calc_AppWPF.core;

namespace Simply_Calc_AppWPF.Helpers
{

    /// <summary>
    /// Classe utilitaire pour les opérations mathématiques et conversions.
    /// </summary>
    public static class MathHelper
    {
        // ==================== CONVERSIONS DEGRÉS/RADIANS ====================

        /// <summary>
        /// Convertit des degrés en radians.
        /// </summary>
        /// <param name="degrees">Angle en degrés</param>
        /// <returns>Angle en radians</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * CalculatorConstants.DEGREES_TO_RADIANS_FACTOR;
        }

        /// <summary>
        /// Convertit des radians en degrés.
        /// </summary>
        /// <param name="radians">Angle en radians</param>
        /// <returns>Angle en degrés</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * CalculatorConstants.RADIANS_TO_DEGREES_FACTOR;
        }

        /// <summary>
        /// Convertit des degrés en radians ou vice-versa avec option d'arrondi.
        /// </summary>
        /// <param name="value">Valeur à convertir</param>
        /// <param name="reverse">True = radians→degrés, False = degrés→radians</param>
        /// <param name="round">True pour arrondir le résultat</param>
        /// <returns>Valeur convertie</returns>
        public static double ConvertAngle(double value, bool reverse = false, bool round = false)
        {
            double result = reverse
                ? RadiansToDegrees(value)
                : DegreesToRadians(value);

            return round ? Math.Round(result) : result;
        }

        // ==================== VALIDATION ====================

        /// <summary>
        /// Vérifie si une valeur est valide pour arcsin ou arccos (domaine [-1, 1]).
        /// </summary>
        /// <param name="value">Valeur à vérifier</param>
        /// <returns>True si dans le domaine valide</returns>
        public static bool IsValidForArcSin(double value)
        {
            return value >= CalculatorConstants.ARCSIN_ARCCOS_MIN &&
                   value <= CalculatorConstants.ARCSIN_ARCCOS_MAX;
        }

        /// <summary>
        /// Vérifie si une valeur est valide pour arccos (identique à arcsin).
        /// </summary>
        public static bool IsValidForArcCos(double value)
        {
            return IsValidForArcSin(value);
        }

        /// <summary>
        /// Vérifie si un diviseur est sûr (non nul dans la tolérance epsilon).
        /// </summary>
        /// <param name="divisor">Diviseur à vérifier</param>
        /// <returns>True si division sûre</returns>
        public static bool IsSafeDivisor(double divisor)
        {
            return Math.Abs(divisor) >= CalculatorConstants.FLOATING_POINT_EPSILON;
        }

        // ==================== OPÉRATIONS SÉCURISÉES ====================

        /// <summary>
        /// Division sécurisée qui évite la division par zéro.
        /// </summary>
        /// <param name="dividend">Dividende</param>
        /// <param name="divisor">Diviseur</param>
        /// <returns>Résultat ou double.NaN si division par zéro</returns>
        public static double SafeDivide(double dividend, double divisor)
        {
            if (!IsSafeDivisor(divisor))
                return double.NaN;

            return dividend / divisor;
        }

        /// <summary>
        /// Calcul de pourcentage sécurisé.
        /// </summary>
        /// <param name="value">Valeur</param>
        /// <param name="percentage">Pourcentage (ex: 50 pour 50%)</param>
        /// <returns>Résultat du calcul</returns>
        public static double CalculatePercentage(double value, double percentage)
        {
            return value * (percentage / 100.0);
        }

        // ==================== COMPARAISONS FLOTTANTES ====================

        /// <summary>
        /// Compare deux nombres flottants avec tolérance epsilon.
        /// </summary>
        /// <param name="a">Premier nombre</param>
        /// <param name="b">Deuxième nombre</param>
        /// <param name="epsilon">Tolérance (défaut: constante système)</param>
        /// <returns>True si égaux dans la tolérance</returns>
        public static bool AreEqual(double a, double b, double? epsilon = null)
        {
            double tolerance = epsilon ?? CalculatorConstants.FLOATING_POINT_EPSILON;
            return Math.Abs(a - b) < tolerance;
        }

        /// <summary>
        /// Vérifie si un nombre est proche de zéro.
        /// </summary>
        /// <param name="value">Valeur à vérifier</param>
        /// <returns>True si proche de zéro</returns>
        public static bool IsNearZero(double value)
        {
            return Math.Abs(value) < CalculatorConstants.FLOATING_POINT_EPSILON;
        }

        // ==================== ARRONDIS ====================

        /// <summary>
        /// Arrondit un nombre à N décimales.
        /// </summary>
        /// <param name="value">Valeur à arrondir</param>
        /// <param name="decimals">Nombre de décimales</param>
        /// <returns>Valeur arrondie</returns>
        public static double RoundTo(double value, int decimals)
        {
            if (decimals < 0)
                decimals = 0;

            return Math.Round(value, decimals);
        }

        /// <summary>
        /// Arrondit intelligemment selon la magnitude du nombre.
        /// </summary>
        /// <param name="value">Valeur à arrondir</param>
        /// <returns>Valeur arrondie</returns>
        public static double SmartRound(double value)
        {
            double absValue = Math.Abs(value);

            // Très grands nombres : 2 décimales
            if (absValue >= 1000)
                return RoundTo(value, 2);

            // Nombres moyens : 4 décimales
            if (absValue >= 1)
                return RoundTo(value, 4);

            // Petits nombres : 6 décimales
            return RoundTo(value, 6);
        }

        // ==================== DÉTECTION PATTERNS ====================

        /// <summary>
        /// Vérifie si un nombre nécessite la notation scientifique.
        /// </summary>
        /// <param name="value">Valeur à vérifier</param>
        /// <returns>True si notation scientifique recommandée</returns>
        public static bool RequiresScientificNotation(double value)
        {
            return CalculatorConstants.RequiresScientificNotation(value);
        }

        /// <summary>
        /// Vérifie si un résultat est valide (ni NaN ni Infinity).
        /// </summary>
        /// <param name="value">Valeur à vérifier</param>
        /// <returns>True si valide</returns>
        public static bool IsValidResult(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        // ==================== UTILITAIRES ====================

        /// <summary>
        /// Clamp une valeur entre un minimum et un maximum.
        /// </summary>
        /// <param name="value">Valeur à limiter</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <returns>Valeur limitée</returns>
        public static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Interpole linéairement entre deux valeurs.
        /// </summary>
        /// <param name="start">Valeur de départ</param>
        /// <param name="end">Valeur de fin</param>
        /// <param name="t">Facteur d'interpolation (0-1)</param>
        /// <returns>Valeur interpolée</returns>
        public static double Lerp(double start, double end, double t)
        {
            t = Clamp(t, 0, 1);
            return start + (end - start) * t;
        }

        /// <summary>
        /// Calcule la valeur absolue.
        /// </summary>
        public static double Abs(double value) => Math.Abs(value);

        /// <summary>
        /// Retourne le signe d'un nombre (-1, 0, ou 1).
        /// </summary>
        public static int Sign(double value) => Math.Sign(value);

        /// <summary>
        /// Calcule la puissance d'un nombre.
        /// </summary>
        public static double Power(double baseValue, double exponent) => Math.Pow(baseValue, exponent);

        /// <summary>
        /// Calcule la racine carrée.
        /// </summary>
        public static double SquareRoot(double value) => Math.Sqrt(value);
    }
}
