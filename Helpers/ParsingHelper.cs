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
    /// Classe utilitaire pour le parsing et la validation des entrées.
    /// </summary>
    public static class ParsingHelper
    {
        // ==================== PARSING NOMBRES ====================

        /// <summary>
        /// Parse un double avec gestion complète des formats (virgule/point).
        /// </summary>
        /// <param name="input">Chaîne à parser</param>
        /// <param name="result">Résultat du parsing</param>
        /// <returns>True si parsing réussi</returns>
        public static bool TryParseDouble(string input, out double result)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Normalisation : virgule → point
            string normalized = input.Replace(',', '.');

            // Tentative de parsing avec InvariantCulture
            return double.TryParse(
                normalized,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out result
            );
        }

        /// <summary>
        /// Parse un int avec validation.
        /// </summary>
        public static bool TryParseInt(string input, out int result)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            return int.TryParse(input.Trim(), out result);
        }

        /// <summary>
        /// Parse un float avec validation.
        /// </summary>
        public static bool TryParseFloat(string input, out float result)
        {
            result = 0f;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            string normalized = input.Replace(',', '.');

            return float.TryParse(
                normalized,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out result
            );
        }

        // ==================== EXTRACTION DE TOKENS ====================

        /// <summary>
        /// Extrait le dernier token numérique d'une chaîne.
        /// </summary>
        /// <param name="text">Texte source</param>
        /// <returns>Dernier nombre trouvé ou string.Empty</returns>
        public static string GetLastNumericToken(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            int end = text.Length;
            int start = end - 1;

            // Remonter jusqu'à un caractère non-numérique
            while (start >= 0 && IsNumericChar(text[start]))
            {
                start--;
            }

            // Extraire le token
            if (start + 1 < end)
            {
                return text.Substring(start + 1);
            }

            return string.Empty;
        }

        /// <summary>
        /// Extrait un nombre à partir d'une position donnée.
        /// </summary>
        /// <param name="text">Texte source</param>
        /// <param name="startIndex">Index de départ</param>
        /// <returns>Tuple (nombre, nouvel index)</returns>
        public static (double value, int newIndex) ExtractNumber(string text, int startIndex)
        {
            if (startIndex >= text.Length)
                return (0, startIndex);

            int index = startIndex;
            bool hasDecimal = false;
            var buffer = new System.Text.StringBuilder();

            // Gérer le signe négatif
            if (index < text.Length && text[index] == '-')
            {
                buffer.Append('-');
                index++;
            }

            // Extraire les chiffres et le point décimal
            while (index < text.Length)
            {
                char c = text[index];

                if (char.IsDigit(c))
                {
                    buffer.Append(c);
                    index++;
                }
                else if ((c == '.' || c == ',') && !hasDecimal)
                {
                    buffer.Append('.');
                    hasDecimal = true;
                    index++;
                }
                else
                {
                    break;
                }
            }

            if (TryParseDouble(buffer.ToString(), out double result))
            {
                return (result, index);
            }

            return (0, startIndex);
        }

        // ==================== VALIDATION CARACTÈRES ====================

        /// <summary>
        /// Vérifie si un caractère est un opérateur arithmétique.
        /// </summary>
        /// <param name="c">Caractère à vérifier</param>
        /// <returns>True si opérateur</returns>
        public static bool IsOperator(char c)
        {
            return c is '+' or '-' or '×' or '÷' or '*' or '/';
        }

        /// <summary>
        /// Vérifie si un caractère fait partie d'un nombre.
        /// </summary>
        private static bool IsNumericChar(char c)
        {
            return char.IsDigit(c) || c == '.' || c == ',';
        }

        /// <summary>
        /// Vérifie si une chaîne est un opérateur valide.
        /// </summary>
        public static bool IsOperatorString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return str is "+" or "-" or "×" or "÷" or "*" or "/";
        }

        /// <summary>
        /// Vérifie si une chaîne est une fonction scientifique.
        /// </summary>
        public static bool IsFunction(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return str is "sin" or "cos" or "tan" or "arcsin" or "arccos" or "arctan";
        }

        /// <summary>
        /// Vérifie si une chaîne est une constante.
        /// </summary>
        public static bool IsConstant(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return str is "π" or "e" or "pi";
        }

        // ==================== VALIDATION EXPRESSIONS ====================

        /// <summary>
        /// Valide qu'une expression ne contient que des caractères autorisés.
        /// </summary>
        /// <param name="expression">Expression à valider</param>
        /// <returns>True si valide</returns>
        public static bool ValidateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return false;

            // Caractères autorisés : chiffres, opérateurs, parenthèses, point, constantes
            const string allowedChars = "0123456789+-×÷*/.(),πe ";

            foreach (char c in expression)
            {
                if (!allowedChars.Contains(c) && !char.IsLetter(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Vérifie l'équilibre des parenthèses dans une expression.
        /// </summary>
        /// <param name="expression">Expression à vérifier</param>
        /// <returns>True si parenthèses équilibrées</returns>
        public static bool AreParenthesesBalanced(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return true;

            int count = 0;

            foreach (char c in expression)
            {
                if (c == '(')
                    count++;
                else if (c == ')')
                    count--;

                // Si count devient négatif, il y a une ')' avant '('
                if (count < 0)
                    return false;
            }

            // À la fin, count doit être 0
            return count == 0;
        }

        // ==================== RECHERCHE ET SURLIGNAGE ====================

        /// <summary>
        /// Trouve toutes les occurrences d'un terme dans un texte.
        /// </summary>
        /// <param name="text">Texte à analyser</param>
        /// <param name="searchTerm">Terme recherché</param>
        /// <returns>Liste des indices de début d'occurrence</returns>
        public static List<int> FindOccurrences(string text, string searchTerm)
        {
            var positions = new List<int>();

            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(searchTerm))
                return positions;

            string lowerText = text.ToLower();
            string lowerSearch = searchTerm.ToLower();

            int index = lowerText.IndexOf(lowerSearch, StringComparison.Ordinal);
            while (index != -1)
            {
                positions.Add(index);
                index = lowerText.IndexOf(lowerSearch, index + searchTerm.Length, StringComparison.Ordinal);
            }

            return positions;
        }

        /// <summary>
        /// Vérifie si un texte contient un terme de recherche (insensible casse).
        /// </summary>
        public static bool Contains(string text, string searchTerm)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(searchTerm))
                return false;

            return text.ToLower().Contains(searchTerm.ToLower());
        }

        // ==================== NETTOYAGE ET NORMALISATION ====================

        /// <summary>
        /// Nettoie une chaîne pour le parsing (supprime espaces, normalise).
        /// </summary>
        /// <param name="input">Chaîne à nettoyer</param>
        /// <returns>Chaîne nettoyée</returns>
        public static string CleanInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Supprimer espaces
            string cleaned = input.Replace(" ", "");

            // Normaliser virgule en point
            cleaned = cleaned.Replace(',', '.');

            return cleaned;
        }

        /// <summary>
        /// Supprime les zéros trailing d'un nombre décimal.
        /// </summary>
        /// <param name="number">Nombre en string</param>
        /// <returns>Nombre sans trailing zeros</returns>
        public static string RemoveTrailingZeros(string number)
        {
            if (string.IsNullOrEmpty(number) || !number.Contains('.'))
                return number;

            return number.TrimEnd('0').TrimEnd('.');
        }

        // ==================== VALIDATION LONGUEUR ====================

        /// <summary>
        /// Vérifie si une entrée respecte la longueur maximale autorisée.
        /// </summary>
        /// <param name="input">Entrée à vérifier</param>
        /// <param name="maxLength">Longueur maximale (défaut: constante)</param>
        /// <returns>True si longueur acceptable</returns>
        public static bool IsWithinMaxLength(string input, int? maxLength = null)
        {
            if (string.IsNullOrEmpty(input))
                return true;

            int max = maxLength ?? CalculatorConstants.MAX_INPUT_LENGTH;
            return input.Length <= max;
        }

        /// <summary>
        /// Tronque une chaîne à une longueur maximale.
        /// </summary>
        public static string Truncate(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
                return input;

            return input.Substring(0, maxLength);
        }
    }
}
