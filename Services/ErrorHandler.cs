using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simply_Calc_AppWPF.core;

namespace Simply_Calc_AppWPF.Services
{

    /// <summary>
    /// Type d'erreur calculatrice.
    /// </summary>
    public enum ErrorType
    {
        None,
        DivisionByZero,
        SyntaxError,
        MathError,
        DomainError,
        OverflowError,
        UnderflowError,
        InvalidOperation,
        Unknown
    }

    /// <summary>
    /// Sévérité de l'erreur.
    /// </summary>
    public enum ErrorSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Informations détaillées sur une erreur.
    /// </summary>
    public record ErrorInfo
    {
        /// <summary>
        /// Type d'erreur.
        /// </summary>
        public ErrorType Type { get; init; }

        /// <summary>
        /// Message d'erreur principal.
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// Message d'erreur détaillé (optionnel).
        /// </summary>
        public string? DetailedMessage { get; init; }

        /// <summary>
        /// Sévérité de l'erreur.
        /// </summary>
        public ErrorSeverity Severity { get; init; }

        /// <summary>
        /// Horodatage de l'erreur.
        /// </summary>
        public DateTime Timestamp { get; init; }

        /// <summary>
        /// Exception source (si applicable).
        /// </summary>
        public Exception? SourceException { get; init; }

        /// <summary>
        /// Contexte additionnel (expression, valeurs, etc.).
        /// </summary>
        public Dictionary<string, object>? Context { get; init; }

        public ErrorInfo()
        {
            Type = ErrorType.None;
            Message = string.Empty;
            Severity = ErrorSeverity.Info;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Severity}] {Type}: {Message}";
        }
    }

    /// <summary>
    /// Gestionnaire centralisé des erreurs de la calculatrice.
    /// Gère la détection, le logging et la présentation des erreurs.
    /// </summary>
    public class ErrorHandler
    {
        private readonly List<ErrorInfo> _errorHistory;
        private readonly int _maxHistorySize;
        private string? _logFilePath;

        /// <summary>
        /// Historique des erreurs (lecture seule).
        /// </summary>
        public IReadOnlyList<ErrorInfo> ErrorHistory => _errorHistory.AsReadOnly();

        /// <summary>
        /// Dernière erreur enregistrée.
        /// </summary>
        public ErrorInfo? LastError => _errorHistory.Count > 0 ? _errorHistory[^1] : null;

        /// <summary>
        /// Indique si une erreur a été enregistrée récemment.
        /// </summary>
        public bool HasErrors => _errorHistory.Count > 0;

        public ErrorHandler(int maxHistorySize = 100, string? logFilePath = null)
        {
            _maxHistorySize = maxHistorySize;
            _errorHistory = new List<ErrorInfo>();
            _logFilePath = logFilePath;
        }

        // ==================== GESTION DES ERREURS ====================

        /// <summary>
        /// Traite une exception et retourne les informations d'erreur correspondantes.
        /// </summary>
        /// <param name="exception">Exception à traiter</param>
        /// <param name="context">Contexte additionnel</param>
        /// <returns>Informations d'erreur structurées</returns>
        public ErrorInfo HandleException(Exception exception, Dictionary<string, object>? context = null)
        {
            ErrorInfo errorInfo = exception switch
            {
                DivideByZeroException divEx => new ErrorInfo
                {
                    Type = ErrorType.DivisionByZero,
                    Message = CalculatorConstants.ERROR_DIVISION_BY_ZERO,
                    DetailedMessage = divEx.Message,
                    Severity = ErrorSeverity.Error,
                    Timestamp = DateTime.Now,
                    SourceException = divEx,
                    Context = context
                },

                ArgumentException argEx => new ErrorInfo
                {
                    Type = ErrorType.SyntaxError,
                    Message = CalculatorConstants.ERROR_SYNTAX,
                    DetailedMessage = argEx.Message,
                    Severity = ErrorSeverity.Error,
                    Timestamp = DateTime.Now,
                    SourceException = argEx,
                    Context = context
                },

                OverflowException overEx => new ErrorInfo
                {
                    Type = ErrorType.OverflowError,
                    Message = "Dépassement de capacité",
                    DetailedMessage = overEx.Message,
                    Severity = ErrorSeverity.Error,
                    Timestamp = DateTime.Now,
                    SourceException = overEx,
                    Context = context
                },

                InvalidOperationException invEx => new ErrorInfo
                {
                    Type = ErrorType.InvalidOperation,
                    Message = "Opération invalide",
                    DetailedMessage = invEx.Message,
                    Severity = ErrorSeverity.Error,
                    Timestamp = DateTime.Now,
                    SourceException = invEx,
                    Context = context
                },

                _ => new ErrorInfo
                {
                    Type = ErrorType.Unknown,
                    Message = CalculatorConstants.ERROR_UNKNOWN,
                    DetailedMessage = exception.Message,
                    Severity = ErrorSeverity.Error,
                    Timestamp = DateTime.Now,
                    SourceException = exception,
                    Context = context
                }
            };

            RecordError(errorInfo);
            return errorInfo;
        }

        /// <summary>
        /// Crée et enregistre une erreur personnalisée.
        /// </summary>
        /// <param name="type">Type d'erreur</param>
        /// <param name="message">Message principal</param>
        /// <param name="severity">Sévérité</param>
        /// <param name="detailedMessage">Message détaillé optionnel</param>
        /// <param name="context">Contexte optionnel</param>
        /// <returns>Informations d'erreur créées</returns>
        public ErrorInfo CreateError(
            ErrorType type,
            string message,
            ErrorSeverity severity = ErrorSeverity.Error,
            string? detailedMessage = null,
            Dictionary<string, object>? context = null)
        {
            var errorInfo = new ErrorInfo
            {
                Type = type,
                Message = message,
                DetailedMessage = detailedMessage,
                Severity = severity,
                Timestamp = DateTime.Now,
                Context = context
            };

            RecordError(errorInfo);
            return errorInfo;
        }

        /// <summary>
        /// Enregistre une erreur dans l'historique.
        /// </summary>
        /// <param name="errorInfo">Informations d'erreur</param>
        private void RecordError(ErrorInfo errorInfo)
        {
            // Ajouter à l'historique
            _errorHistory.Add(errorInfo);

            // Limiter la taille de l'historique
            while (_errorHistory.Count > _maxHistorySize)
            {
                _errorHistory.RemoveAt(0);
            }

            // Logger si chemin configuré
            if (!string.IsNullOrEmpty(_logFilePath))
            {
                LogError(errorInfo);
            }
        }

        // ==================== MESSAGES D'ERREUR ====================

        /// <summary>
        /// Obtient le message d'erreur approprié pour un type donné.
        /// </summary>
        /// <param name="type">Type d'erreur</param>
        /// <returns>Message d'erreur standard</returns>
        public string GetErrorMessage(ErrorType type)
        {
            return type switch
            {
                ErrorType.DivisionByZero => CalculatorConstants.ERROR_DIVISION_BY_ZERO,
                ErrorType.SyntaxError => CalculatorConstants.ERROR_SYNTAX,
                ErrorType.MathError => CalculatorConstants.ERROR_MATH,
                ErrorType.DomainError => CalculatorConstants.ERROR_DOMAIN,
                ErrorType.OverflowError => "Nombre trop grand",
                ErrorType.UnderflowError => "Nombre trop petit",
                ErrorType.InvalidOperation => "Opération non valide",
                ErrorType.Unknown => CalculatorConstants.ERROR_UNKNOWN,
                _ => "Erreur"
            };
        }

        /// <summary>
        /// Obtient la taille de police appropriée pour un message d'erreur.
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <returns>Taille de police</returns>
        public int GetErrorFontSize(string message)
        {
            return CalculatorConstants.GetErrorFontSize(message);
        }

        // ==================== VALIDATION ====================

        /// <summary>
        /// Valide un résultat et génère une erreur si invalide.
        /// </summary>
        /// <param name="result">Résultat à valider</param>
        /// <param name="operation">Description de l'opération</param>
        /// <returns>ErrorInfo si invalide, null si valide</returns>
        public ErrorInfo? ValidateResult(double result, string operation)
        {
            if (double.IsNaN(result))
            {
                return CreateError(
                    ErrorType.MathError,
                    CalculatorConstants.ERROR_MATH,
                    ErrorSeverity.Error,
                    "Le résultat est NaN (Not a Number)",
                    new Dictionary<string, object> { { "operation", operation } }
                );
            }

            if (double.IsInfinity(result))
            {
                return CreateError(
                    ErrorType.OverflowError,
                    "Résultat infini",
                    ErrorSeverity.Error,
                    "Le résultat dépasse les limites numériques",
                    new Dictionary<string, object> { { "operation", operation } }
                );
            }

            return null;
        }

        /// <summary>
        /// Valide une division et génère une erreur si division par zéro.
        /// </summary>
        /// <param name="dividend">Dividende</param>
        /// <param name="divisor">Diviseur</param>
        /// <returns>ErrorInfo si division par zéro, null si valide</returns>
        public ErrorInfo? ValidateDivision(double dividend, double divisor)
        {
            if (Math.Abs(divisor) < CalculatorConstants.FLOATING_POINT_EPSILON)
            {
                return CreateError(
                    ErrorType.DivisionByZero,
                    CalculatorConstants.ERROR_DIVISION_BY_ZERO,
                    ErrorSeverity.Error,
                    $"Tentative de diviser {dividend} par zéro"
                );
            }

            return null;
        }

        /// <summary>
        /// Valide une valeur pour une fonction arc (arcsin, arccos).
        /// </summary>
        /// <param name="value">Valeur à valider</param>
        /// <param name="functionName">Nom de la fonction</param>
        /// <returns>ErrorInfo si hors domaine, null si valide</returns>
        public ErrorInfo? ValidateArcFunction(double value, string functionName)
        {
            if (!CalculatorConstants.IsValidForArcFunction(value))
            {
                return CreateError(
                    ErrorType.DomainError,
                    CalculatorConstants.ERROR_DOMAIN,
                    ErrorSeverity.Error,
                    $"{functionName} requiert une valeur entre -1 et 1 (reçu: {value})",
                    new Dictionary<string, object>
                    {
                        { "function", functionName },
                        { "value", value }
                    }
                );
            }

            return null;
        }

        // ==================== LOGGING ====================

        /// <summary>
        /// Configure le chemin du fichier de log.
        /// </summary>
        /// <param name="filePath">Chemin du fichier</param>
        public void SetLogFilePath(string? filePath)
        {
            _logFilePath = filePath;
        }

        /// <summary>
        /// Écrit une erreur dans le fichier de log.
        /// </summary>
        /// <param name="errorInfo">Informations d'erreur</param>
        public void LogError(ErrorInfo errorInfo)
        {
            if (string.IsNullOrEmpty(_logFilePath))
                return;

            try
            {
                var logEntry = new StringBuilder();
                logEntry.AppendLine($"[{errorInfo.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{errorInfo.Severity}] {errorInfo.Type}");
                logEntry.AppendLine($"Message: {errorInfo.Message}");

                if (!string.IsNullOrEmpty(errorInfo.DetailedMessage))
                    logEntry.AppendLine($"Détails: {errorInfo.DetailedMessage}");

                if (errorInfo.Context != null && errorInfo.Context.Count > 0)
                {
                    logEntry.AppendLine("Contexte:");
                    foreach (var kvp in errorInfo.Context)
                    {
                        logEntry.AppendLine($"  {kvp.Key}: {kvp.Value}");
                    }
                }

                if (errorInfo.SourceException != null)
                {
                    logEntry.AppendLine($"Exception: {errorInfo.SourceException.GetType().Name}");
                    logEntry.AppendLine($"Stack Trace: {errorInfo.SourceException.StackTrace}");
                }

                logEntry.AppendLine(new string('-', 80));

                File.AppendAllText(_logFilePath, logEntry.ToString(), Encoding.UTF8);
            }
            catch
            {
                // Ignorer erreurs de logging pour éviter boucles infinies
            }
        }

        // ==================== STATISTIQUES ====================

        /// <summary>
        /// Compte le nombre d'erreurs par type.
        /// </summary>
        /// <returns>Dictionnaire type → count</returns>
        public Dictionary<ErrorType, int> GetErrorStatistics()
        {
            var stats = new Dictionary<ErrorType, int>();

            foreach (var error in _errorHistory)
            {
                if (stats.ContainsKey(error.Type))
                    stats[error.Type]++;
                else
                    stats[error.Type] = 1;
            }

            return stats;
        }

        /// <summary>
        /// Obtient les erreurs d'une sévérité donnée.
        /// </summary>
        /// <param name="severity">Sévérité recherchée</param>
        /// <returns>Liste des erreurs correspondantes</returns>
        public IEnumerable<ErrorInfo> GetErrorsBySeverity(ErrorSeverity severity)
        {
            return _errorHistory.Where(e => e.Severity == severity);
        }

        /// <summary>
        /// Obtient les erreurs dans une plage de dates.
        /// </summary>
        /// <param name="startDate">Date de début</param>
        /// <param name="endDate">Date de fin</param>
        /// <returns>Liste des erreurs correspondantes</returns>
        public IEnumerable<ErrorInfo> GetErrorsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _errorHistory.Where(e => e.Timestamp >= startDate && e.Timestamp <= endDate);
        }

        /// <summary>
        /// Obtient le type d'erreur le plus fréquent.
        /// </summary>
        /// <returns>Type d'erreur le plus courant ou null</returns>
        public ErrorType? GetMostCommonErrorType()
        {
            if (_errorHistory.Count == 0)
                return null;

            var stats = GetErrorStatistics();
            return stats.OrderByDescending(kvp => kvp.Value).First().Key;
        }

        // ==================== GESTION HISTORIQUE ====================

        /// <summary>
        /// Efface l'historique des erreurs.
        /// </summary>
        public void ClearHistory()
        {
            _errorHistory.Clear();
        }

        /// <summary>
        /// Supprime les erreurs plus anciennes qu'une date donnée.
        /// </summary>
        /// <param name="cutoffDate">Date limite</param>
        /// <returns>Nombre d'erreurs supprimées</returns>
        public int RemoveOldErrors(DateTime cutoffDate)
        {
            int countBefore = _errorHistory.Count;
            _errorHistory.RemoveAll(e => e.Timestamp < cutoffDate);
            return countBefore - _errorHistory.Count;
        }

        // ==================== EXPORT ====================

        /// <summary>
        /// Génère un rapport textuel de l'historique des erreurs.
        /// </summary>
        /// <returns>Rapport formaté</returns>
        public string GenerateErrorReport()
        {
            var report = new StringBuilder();
            report.AppendLine("=== RAPPORT D'ERREURS ===");
            report.AppendLine($"Généré le: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Nombre total d'erreurs: {_errorHistory.Count}");
            report.AppendLine();

            // Statistiques
            report.AppendLine("--- Statistiques par type ---");
            var stats = GetErrorStatistics();
            foreach (var kvp in stats.OrderByDescending(x => x.Value))
            {
                report.AppendLine($"{kvp.Key}: {kvp.Value} occurrence(s)");
            }
            report.AppendLine();

            // Détails des erreurs
            report.AppendLine("--- Détails des erreurs ---");
            for (int i = 0; i < _errorHistory.Count; i++)
            {
                var error = _errorHistory[i];
                report.AppendLine($"{i + 1}. [{error.Timestamp:HH:mm:ss}] {error}");
                if (!string.IsNullOrEmpty(error.DetailedMessage))
                    report.AppendLine($"   Détails: {error.DetailedMessage}");
            }

            return report.ToString();
        }

        /// <summary>
        /// Exporte l'historique des erreurs vers un fichier.
        /// </summary>
        /// <param name="filePath">Chemin du fichier de destination</param>
        public void ExportToFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            string report = GenerateErrorReport();
            File.WriteAllText(filePath, report, Encoding.UTF8);
        }

        // ==================== UTILITAIRES ====================

        /// <summary>
        /// Représentation textuelle du gestionnaire d'erreurs.
        /// </summary>
        public override string ToString()
        {
            return $"ErrorHandler: {_errorHistory.Count} erreurs enregistrées";
        }
    }
}
