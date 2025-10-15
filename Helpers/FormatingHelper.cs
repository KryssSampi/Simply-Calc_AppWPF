using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simply_Calc_AppWPF.core;

namespace Simply_Calc_AppWPF.Helpers
{
    /// <summary>
    /// Classe utilitaire pour le formatage et l'affichage des résultats.
    /// </summary>
    public static class FormattingHelper
    {
        // ==================== FORMATAGE RÉSULTATS ====================

        /// <summary>
        /// Formate un résultat selon les contraintes d'affichage.
        /// Gère automatiquement notation scientifique, troncature, etc.
        /// </summary>
        /// <param name="value">Valeur à formater</param>
        /// <param name="maxLength">Longueur max (défaut: constante)</param>
        /// <returns>Chaîne formatée</returns>
        public static string FormatResult(double value, int? maxLength = null)
        {
            int max = maxLength ?? CalculatorConstants.MAX_DISPLAY_LENGTH;

            // Gestion cas invalides
            if (double.IsNaN(value))
                return CalculatorConstants.ERROR_MATH;

            if (double.IsInfinity(value))
                return CalculatorConstants.ERROR_INVALID_RESULT;

            // Notation scientifique si nécessaire
            if (RequiresScientificNotation(value))
            {
                return FormatScientific(value);
            }

            // Formatage décimal standard
            string result = value.ToString(CalculatorConstants.DECIMAL_FORMAT, CultureInfo.InvariantCulture);

            // Troncature si trop long
            if (result.Length > max)
            {
                result = TruncateNumber(value, max);
            }

            return result;
        }

        /// <summary>
        /// Formate un résultat de manière stricte (pour affichage final).
        /// </summary>
        /// <param name="value">Valeur à formater</param>
        /// <param name="maxLength">Longueur maximale</param>
        /// <returns>Chaîne formatée</returns>
        public static string FormatResultStrict(double value, int maxLength = 13)
        {
            // Gestion cas invalides
            if (double.IsNaN(value) || double.IsInfinity(value))
                return "Erreur";

            // Partie entière
            string integerPart = Math.Truncate(value).ToString();
            string text;

            // Notation scientifique si nécessaire
            if (RequiresScientificNotation(value))
            {
                text = $"({FormatScientific(value)}) ";
            }
            else
            {
                // Affichage normal avec 6 décimales max
                text = value.ToString(CalculatorConstants.DECIMAL_FORMAT);
            }

            // Troncature intelligente
            if (text.Length > maxLength && integerPart.Length < maxLength)
            {
                int decimalPlaces = maxLength - integerPart.Length - 1;
                decimalPlaces = Math.Max(0, decimalPlaces);
                string format = "0." + new string('#', decimalPlaces);
                text = value.ToString(format);
            }

            return text;
        }

        // ==================== NOTATION SCIENTIFIQUE ====================

        /// <summary>
        /// Vérifie si notation scientifique est requise.
        /// </summary>
        private static bool RequiresScientificNotation(double value)
        {
            return CalculatorConstants.RequiresScientificNotation(value);
        }

        /// <summary>
        /// Formate un nombre en notation scientifique.
        /// </summary>
        /// <param name="value">Valeur à formater</param>
        /// <returns>Chaîne en notation scientifique (ex: 1.23E+6)</returns>
        public static string FormatScientific(double value)
        {
            return value.ToString(CalculatorConstants.SCIENTIFIC_FORMAT, CultureInfo.InvariantCulture);
        }

        // ==================== TRONCATURE ====================

        /// <summary>
        /// Tronque un nombre pour tenir dans une longueur maximale.
        /// </summary>
        /// <param name="value">Valeur à tronquer</param>
        /// <param name="maxLength">Longueur maximale</param>
        /// <returns>Chaîne tronquée</returns>
        public static string TruncateNumber(double value, int maxLength)
        {
            string integerPart = Math.Truncate(value).ToString();

            // Si la partie entière dépasse déjà la limite
            if (integerPart.Length >= maxLength)
            {
                return FormatScientific(value);
            }

            // Calculer nombre de décimales possibles
            int availableDecimals = maxLength - integerPart.Length - 1; // -1 pour le point
            availableDecimals = Math.Max(0, availableDecimals);

            // Créer format dynamique
            string format = "0." + new string('#', availableDecimals);
            string result = value.ToString(format, CultureInfo.InvariantCulture);

            // Si toujours trop long, forcer troncature
            if (result.Length > maxLength)
            {
                result = result.Substring(0, maxLength);
            }

            return result;
        }

        /// <summary>
        /// Tronque une chaîne décimale en gardant le maximum de précision.
        /// </summary>
        public static string TruncateDecimal(double value, int maxDecimals)
        {
            string format = "0." + new string('#', maxDecimals);
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        // ==================== NETTOYAGE ====================

        /// <summary>
        /// Supprime les zéros trailing d'une chaîne numérique.
        /// </summary>
        /// <param name="numberString">Chaîne à nettoyer</param>
        /// <returns>Chaîne sans trailing zeros</returns>
        public static string RemoveTrailingZeros(string numberString)
        {
            if (string.IsNullOrEmpty(numberString) || !numberString.Contains('.'))
                return numberString;

            return numberString.TrimEnd('0').TrimEnd('.');
        }

        /// <summary>
        /// Nettoie un résultat pour affichage (gère "-0", etc.).
        /// </summary>
        /// <param name="result">Résultat à nettoyer</param>
        /// <returns>Résultat nettoyé</returns>
        public static string CleanResult(string result)
        {
            if (string.IsNullOrEmpty(result))
                return "0";

            // Remplacer "-0" par "0"
            if (result == "-0" || result == "-0.0")
                return "0";

            // Supprimer trailing zeros si décimal
            if (result.Contains('.'))
            {
                result = RemoveTrailingZeros(result);
            }

            return result;
        }

        // ==================== FORMATAGE SPÉCIAL ====================

        /// <summary>
        /// Formate un nombre avec séparateurs de milliers.
        /// </summary>
        /// <param name="value">Valeur à formater</param>
        /// <returns>Chaîne avec séparateurs</returns>
        public static string FormatWithThousandsSeparator(double value)
        {
            return value.ToString("N2", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Formate un nombre en devise.
        /// </summary>
        /// <param name="value">Valeur à formater</param>
        /// <returns>Chaîne formatée en devise</returns>
        public static string FormatAsCurrency(double value)
        {
            return value.ToString("C", CultureInfo.CurrentCulture);
        }
    }
}
