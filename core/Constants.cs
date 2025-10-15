using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simply_Calc_AppWPF.core
{
    /// <summary>
    /// Constantes globales de l'application calculatrice.
    /// Centralise tous les magic numbers et chaînes de configuration.
    /// </summary>
    public static class CalculatorConstants
    {
        // ==================== LIMITES D'AFFICHAGE ====================

        /// <summary>
        /// Longueur maximale de la saisie utilisateur (inputBuffer).
        /// </summary>
        public const int MAX_INPUT_LENGTH = 13;

        /// <summary>
        /// Longueur maximale du résultat affiché.
        /// Au-delà, passage en notation scientifique ou troncature.
        /// </summary>
        public const int MAX_DISPLAY_LENGTH = 15;

        /// <summary>
        /// Longueur maximale de la zone d'opérations (historique).
        /// </summary>
        public const int MAX_OPERATION_LENGTH = 30;

        /// <summary>
        /// Nombre de décimales maximum affichées.
        /// </summary>
        public const int MAX_DECIMAL_PLACES = 6;

        // ==================== SEUILS NUMÉRIQUES ====================

        /// <summary>
        /// Seuil supérieur pour déclencher la notation scientifique.
        /// Valeurs >= 1 000 000 → format 1.23E+6
        /// </summary>
        public const double SCIENTIFIC_NOTATION_THRESHOLD_HIGH = 1e6;

        /// <summary>
        /// Seuil inférieur pour déclencher la notation scientifique.
        /// Valeurs <= 0.0000001 → format 1.23E-7
        /// </summary>
        public const double SCIENTIFIC_NOTATION_THRESHOLD_LOW = 1e-7;

        /// <summary>
        /// Epsilon pour comparaisons flottantes (tolérance d'égalité).
        /// </summary>
        public const double FLOATING_POINT_EPSILON = 1e-10;

        // ==================== TAILLES DE POLICE PAR DÉFAUT ====================

        /// <summary>
        /// Taille de police par défaut pour le champ de résultat (zone de saisie).
        /// </summary>
        public const double DEFAULT_RESULT_FONT_SIZE = 24.0;

        /// <summary>
        /// Taille de police par défaut pour le bloc de résultat final (affichage calculé).
        /// </summary>
        public const double DEFAULT_RESULT_BLOCK_FONT_SIZE = 18.0;

        /// <summary>
        /// Taille de police par défaut pour la zone d'opérations (historique).
        /// </summary>
        public const double DEFAULT_OPERATION_FONT_SIZE = 20.0;

        /// <summary>
        /// Taille de police réduite pour longs textes dans les opérations.
        /// </summary>
        public const double REDUCED_OPERATION_FONT_SIZE = 15.0;

        /// <summary>
        /// Seuil de caractères pour déclencher la réduction de taille (opérations).
        /// </summary>
        public const int OPERATION_TEXT_LENGTH_THRESHOLD = 14;

        /// <summary>
        /// Seuil de caractères pour déclencher une réduction supplémentaire.
        /// </summary>
        public const int OPERATION_TEXT_LENGTH_THRESHOLD_SEVERE = 45;

        // ==================== CONSTANTES MATHÉMATIQUES ====================

        /// <summary>
        /// Constante Pi (π) - Rapport circonférence/diamètre.
        /// </summary>
        public const double PI = Math.PI;

        /// <summary>
        /// Constante d'Euler (e) - Base logarithme naturel.
        /// </summary>
        public const double E = Math.E;

        /// <summary>
        /// Symbole Unicode pour Pi.
        /// </summary>
        public const string PI_SYMBOL = "π";

        /// <summary>
        /// Symbole Unicode pour Euler.
        /// </summary>
        public const string E_SYMBOL = "e";

        /// <summary>
        /// Nombre d'heures standard de travail (pour calculs RH).
        /// Utilisé dans le contexte des heures supplémentaires.
        /// </summary>
        public const int STANDARD_WORK_HOURS = 40;

        // ==================== FORMATS D'AFFICHAGE ====================

        /// <summary>
        /// Format de notation scientifique : 1.23E+6
        /// </summary>
        public const string SCIENTIFIC_FORMAT = "0.##E+0";

        /// <summary>
        /// Format décimal standard avec 6 décimales max : 123.456789
        /// </summary>
        public const string DECIMAL_FORMAT = "0.######";

        /// <summary>
        /// Format décimal strict sans trailing zeros : 123.45
        /// </summary>
        public const string DECIMAL_FORMAT_STRICT = "0.##########";

        // ==================== MESSAGES D'ERREUR ====================

        /// <summary>
        /// Message d'erreur pour division par zéro.
        /// </summary>
        public const string ERROR_DIVISION_BY_ZERO = "Impossible de diviser par 0";

        /// <summary>
        /// Message d'erreur générique pour erreurs de syntaxe.
        /// </summary>
        public const string ERROR_SYNTAX = "Syntax error";

        /// <summary>
        /// Message d'erreur générique pour erreurs mathématiques.
        /// </summary>
        public const string ERROR_MATH = "Math error";

        /// <summary>
        /// Message d'erreur pour valeurs hors domaine (ex: arcsin(2)).
        /// </summary>
        public const string ERROR_DOMAIN = "Valeur hors domaine";

        /// <summary>
        /// Message d'erreur pour résultats invalides (NaN, Infinity).
        /// </summary>
        public const string ERROR_INVALID_RESULT = "Résultat invalide";

        /// <summary>
        /// Message générique pour erreur inconnue.
        /// </summary>
        public const string ERROR_UNKNOWN = "Erreur inconnue";

        // ==================== TAILLES DE POLICE POUR ERREURS ====================

        /// <summary>
        /// Taille de police pour erreur de syntaxe.
        /// </summary>
        public const int ERROR_FONT_SIZE_SYNTAX = 16;

        /// <summary>
        /// Taille de police pour erreur mathématique générique.
        /// </summary>
        public const int ERROR_FONT_SIZE_MATH = 18;

        /// <summary>
        /// Taille de police pour erreur de division par zéro.
        /// </summary>
        public const int ERROR_FONT_SIZE_DIVISION = 15;

        /// <summary>
        /// Taille de police par défaut pour erreurs non spécifiées.
        /// </summary>
        public const int ERROR_FONT_SIZE_DEFAULT = 16;

        // ==================== SYMBOLES D'OPÉRATEURS ====================

        /// <summary>
        /// Symbole d'addition.
        /// </summary>
        public const string OPERATOR_ADDITION = "+";

        /// <summary>
        /// Symbole de soustraction.
        /// </summary>
        public const string OPERATOR_SUBTRACTION = "-";

        /// <summary>
        /// Symbole de multiplication (Unicode).
        /// </summary>
        public const string OPERATOR_MULTIPLICATION = "×";

        /// <summary>
        /// Symbole de division (Unicode).
        /// </summary>
        public const string OPERATOR_DIVISION = "÷";

        /// <summary>
        /// Symbole de multiplication alternatif (ASCII).
        /// </summary>
        public const string OPERATOR_MULTIPLICATION_ALT = "*";

        /// <summary>
        /// Symbole de division alternatif (ASCII).
        /// </summary>
        public const string OPERATOR_DIVISION_ALT = "/";

        // ==================== NOMS DE FONCTIONS ====================

        /// <summary>
        /// Fonction sinus.
        /// </summary>
        public const string FUNCTION_SIN = "sin";

        /// <summary>
        /// Fonction cosinus.
        /// </summary>
        public const string FUNCTION_COS = "cos";

        /// <summary>
        /// Fonction tangente.
        /// </summary>
        public const string FUNCTION_TAN = "tan";

        /// <summary>
        /// Fonction arc sinus (inverse).
        /// </summary>
        public const string FUNCTION_ARCSIN = "arcsin";

        /// <summary>
        /// Fonction arc cosinus (inverse).
        /// </summary>
        public const string FUNCTION_ARCCOS = "arcos";

        /// <summary>
        /// Fonction arc tangente (inverse).
        /// </summary>
        public const string FUNCTION_ARCTAN = "arctan";

        // ==================== LIMITES DE VALIDATION ====================

        /// <summary>
        /// Valeur minimale pour arc sinus et arc cosinus.
        /// </summary>
        public const double ARCSIN_ARCCOS_MIN = -1.0;

        /// <summary>
        /// Valeur maximale pour arc sinus et arc cosinus.
        /// </summary>
        public const double ARCSIN_ARCCOS_MAX = 1.0;

        /// <summary>
        /// Facteur de conversion degrés → radians (π/180).
        /// </summary>
        public const double DEGREES_TO_RADIANS_FACTOR = Math.PI / 180.0;

        /// <summary>
        /// Facteur de conversion radians → degrés (180/π).
        /// </summary>
        public const double RADIANS_TO_DEGREES_FACTOR = 180.0 / Math.PI;

        // ==================== CARACTÈRES SPÉCIAUX ====================

        /// <summary>
        /// Caractère de parenthèse ouvrante.
        /// </summary>
        public const char CHAR_OPEN_PARENTHESIS = '(';

        /// <summary>
        /// Caractère de parenthèse fermante.
        /// </summary>
        public const char CHAR_CLOSE_PARENTHESIS = ')';

        /// <summary>
        /// Caractère de point décimal.
        /// </summary>
        public const char CHAR_DECIMAL_POINT = '.';

        /// <summary>
        /// Caractère de virgule décimale (format français).
        /// </summary>
        public const char CHAR_DECIMAL_COMMA = ',';

        /// <summary>
        /// Caractère de signe moins (négatif).
        /// </summary>
        public const char CHAR_MINUS = '-';

        // ==================== VALEURS PAR DÉFAUT ====================

        /// <summary>
        /// Valeur par défaut affichée au démarrage.
        /// </summary>
        public const string DEFAULT_DISPLAY_VALUE = "0";

        /// <summary>
        /// Chaîne vide pour réinitialisation.
        /// </summary>
        public const string EMPTY_STRING = "";

        /// <summary>
        /// Valeur par défaut pour multiplicateur (constantes).
        /// </summary>
        public const double DEFAULT_MULTIPLIER = 1.0;

        // ==================== CONFIGURATION UI ====================

        /// <summary>
        /// Nombre de colonnes max pour affichage multi-colonnes.
        /// </summary>
        public const int MAX_DISPLAY_COLUMNS = 3;

        /// <summary>
        /// Nombre de lignes par section dans affichages tabulaires.
        /// </summary>
        public const int ROWS_PER_SECTION = 6;

        /// <summary>
        /// Nombre d'items par ligne dans affichage heures supplémentaires.
        /// </summary>
        public const int OVERTIME_ITEMS_PER_ROW = 18;

        // ==================== MÉTADONNÉES APPLICATION ====================

        /// <summary>
        /// Version de l'application.
        /// </summary>
        public const string APP_VERSION = "1.0.0";

        /// <summary>
        /// Nom de l'application.
        /// </summary>
        public const string APP_NAME = "Calculatrice Scientifique WPF";

        /// <summary>
        /// Copyright de l'application.
        /// </summary>
        public const string APP_COPYRIGHT = "© 2024 Votre Nom. Tous droits réservés.";

        // ==================== MÉTHODES UTILITAIRES ====================

        /// <summary>
        /// Détermine la taille de police pour un message d'erreur donné.
        /// </summary>
        /// <param name="errorMessage">Le message d'erreur</param>
        /// <returns>Taille de police appropriée</returns>
        public static int GetErrorFontSize(string errorMessage)
        {
            return errorMessage switch
            {
                ERROR_SYNTAX => ERROR_FONT_SIZE_SYNTAX,
                ERROR_MATH => ERROR_FONT_SIZE_MATH,
                ERROR_DIVISION_BY_ZERO => ERROR_FONT_SIZE_DIVISION,
                _ => ERROR_FONT_SIZE_DEFAULT
            };
        }

        /// <summary>
        /// Vérifie si une valeur nécessite la notation scientifique.
        /// </summary>
        /// <param name="value">Valeur à vérifier</param>
        /// <returns>True si notation scientifique requise</returns>
        public static bool RequiresScientificNotation(double value)
        {
            double absValue = Math.Abs(value);
            return absValue >= SCIENTIFIC_NOTATION_THRESHOLD_HIGH ||
                   (absValue > 0 && absValue < SCIENTIFIC_NOTATION_THRESHOLD_LOW);
        }

        /// <summary>
        /// Vérifie si une valeur est dans le domaine valide pour arcsin/arccos.
        /// </summary>
        /// <param name="value">Valeur à vérifier</param>
        /// <returns>True si valide</returns>
        public static bool IsValidForArcFunction(double value)
        {
            return value >= ARCSIN_ARCCOS_MIN && value <= ARCSIN_ARCCOS_MAX;
        }
    }
}
