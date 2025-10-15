using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simply_Calc_AppWPF.core
{
    /// <summary>
    /// Représente une opération individuelle dans l'historique.
    /// </summary>
    public record Operation
    {
        /// <summary>
        /// Expression calculée (ex: "5+3×2").
        /// </summary>
        public string Expression { get; init; }

        /// <summary>
        /// Résultat du calcul.
        /// </summary>
        public double Result { get; init; }

        /// <summary>
        /// Horodatage de l'opération.
        /// </summary>
        public DateTime Timestamp { get; init; }

        /// <summary>
        /// Constructeur avec valeurs par défaut.
        /// </summary>
        public Operation()
        {
            Expression = string.Empty;
            Result = 0;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Constructeur complet.
        /// </summary>
        /// <param name="expression">Expression calculée</param>
        /// <param name="result">Résultat</param>
        public Operation(string expression, double result)
        {
            Expression = expression;
            Result = result;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Représentation textuelle formatée de l'opération.
        /// </summary>
        /// <returns>Chaîne formatée "expression = result (timestamp)"</returns>
        public override string ToString()
        {
            return $"{Expression} = {Result:G} ({Timestamp:HH:mm:ss})";
        }
    }

    /// <summary>
    /// Gestionnaire d'historique des opérations de la calculatrice.
    /// Permet de stocker, récupérer et exporter l'historique des calculs.
    /// </summary>
    public class OperationHistory
    {
        private readonly List<Operation> _history;
        private readonly int _maxCapacity;

        /// <summary>
        /// Nombre d'opérations actuellement dans l'historique.
        /// </summary>
        public int Count => _history.Count;

        /// <summary>
        /// Capacité maximale de l'historique (nombre d'opérations conservées).
        /// </summary>
        public int MaxCapacity => _maxCapacity;

        /// <summary>
        /// Indique si l'historique est vide.
        /// </summary>
        public bool IsEmpty => _history.Count == 0;

        /// <summary>
        /// Accès en lecture seule à l'historique complet.
        /// </summary>
        public IReadOnlyList<Operation> History => _history.AsReadOnly();

        /// <summary>
        /// Initialise un nouvel historique avec capacité par défaut.
        /// </summary>
        /// <param name="maxCapacity">Capacité maximale (défaut: 100)</param>
        public OperationHistory(int maxCapacity = 100)
        {
            if (maxCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "La capacité doit être positive");

            _maxCapacity = maxCapacity;
            _history = new List<Operation>(maxCapacity);
        }

        // ==================== AJOUT ET SUPPRESSION ====================

        /// <summary>
        /// Ajoute une nouvelle opération à l'historique.
        /// Si la capacité maximale est atteinte, supprime la plus ancienne.
        /// </summary>
        /// <param name="operation">Opération à ajouter</param>
        public void Add(Operation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            // Si capacité atteinte, supprimer la plus ancienne
            if (_history.Count >= _maxCapacity)
            {
                _history.RemoveAt(0);
            }

            _history.Add(operation);
        }

        /// <summary>
        /// Ajoute une opération à partir d'une expression et d'un résultat.
        /// </summary>
        /// <param name="expression">Expression calculée</param>
        /// <param name="result">Résultat du calcul</param>
        public void Add(string expression, double result)
        {
            Add(new Operation(expression, result));
        }

        /// <summary>
        /// Supprime une opération spécifique de l'historique.
        /// </summary>
        /// <param name="operation">Opération à supprimer</param>
        /// <returns>True si suppression réussie</returns>
        public bool Remove(Operation operation)
        {
            return _history.Remove(operation);
        }

        /// <summary>
        /// Supprime l'opération à l'index spécifié.
        /// </summary>
        /// <param name="index">Index de l'opération (0-based)</param>
        /// <returns>True si suppression réussie</returns>
        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= _history.Count)
                return false;

            _history.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Vide complètement l'historique.
        /// </summary>
        public void Clear()
        {
            _history.Clear();
        }

        // ==================== RÉCUPÉRATION ====================

        /// <summary>
        /// Récupère la dernière opération effectuée.
        /// </summary>
        /// <returns>Dernière opération ou null si historique vide</returns>
        public Operation? GetLast()
        {
            return _history.Count > 0 ? _history[^1] : null;
        }

        /// <summary>
        /// Récupère les N dernières opérations.
        /// </summary>
        /// <param name="count">Nombre d'opérations à récupérer</param>
        /// <returns>Liste des dernières opérations</returns>
        public IEnumerable<Operation> GetLast(int count)
        {
            if (count <= 0)
                return Enumerable.Empty<Operation>();

            int startIndex = Math.Max(0, _history.Count - count);
            return _history.Skip(startIndex).ToList();
        }

        /// <summary>
        /// Récupère l'opération à l'index spécifié.
        /// </summary>
        /// <param name="index">Index de l'opération (0-based)</param>
        /// <returns>Opération trouvée ou null</returns>
        public Operation? GetAt(int index)
        {
            if (index < 0 || index >= _history.Count)
                return null;

            return _history[index];
        }

        /// <summary>
        /// Récupère toutes les opérations contenant une expression spécifique.
        /// </summary>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <param name="caseSensitive">Sensible à la casse (défaut: false)</param>
        /// <returns>Liste des opérations correspondantes</returns>
        public IEnumerable<Operation> Search(string searchTerm, bool caseSensitive = false)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Operation>();

            var comparison = caseSensitive
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            return _history.Where(op => op.Expression.Contains(searchTerm, comparison)).ToList();
        }

        /// <summary>
        /// Récupère les opérations dans une plage de dates.
        /// </summary>
        /// <param name="startDate">Date de début</param>
        /// <param name="endDate">Date de fin</param>
        /// <returns>Liste des opérations dans la plage</returns>
        public IEnumerable<Operation> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _history.Where(op => op.Timestamp >= startDate && op.Timestamp <= endDate).ToList();
        }

        // ==================== STATISTIQUES ====================

        /// <summary>
        /// Calcule la moyenne des résultats dans l'historique.
        /// </summary>
        /// <returns>Moyenne ou null si historique vide</returns>
        public double? GetAverageResult()
        {
            if (_history.Count == 0)
                return null;

            return _history.Average(op => op.Result);
        }

        /// <summary>
        /// Récupère le résultat minimum de l'historique.
        /// </summary>
        /// <returns>Résultat minimum ou null si historique vide</returns>
        public double? GetMinResult()
        {
            if (_history.Count == 0)
                return null;

            return _history.Min(op => op.Result);
        }

        /// <summary>
        /// Récupère le résultat maximum de l'historique.
        /// </summary>
        /// <returns>Résultat maximum ou null si historique vide</returns>
        public double? GetMaxResult()
        {
            if (_history.Count == 0)
                return null;

            return _history.Max(op => op.Result);
        }

        /// <summary>
        /// Compte le nombre d'opérations effectuées dans les dernières N heures.
        /// </summary>
        /// <param name="hours">Nombre d'heures</param>
        /// <returns>Nombre d'opérations</returns>
        public int GetOperationCountInLastHours(int hours)
        {
            if (hours <= 0)
                return 0;

            DateTime threshold = DateTime.Now.AddHours(-hours);
            return _history.Count(op => op.Timestamp >= threshold);
        }

        // ==================== EXPORT ====================

        /// <summary>
        /// Exporte l'historique vers un fichier CSV.
        /// Format: Expression,Result,Timestamp
        /// </summary>
        /// <param name="filePath">Chemin du fichier de destination</param>
        /// <param name="includeHeaders">Inclure ligne d'en-têtes (défaut: true)</param>
        public void ExportToCsv(string filePath, bool includeHeaders = true)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            var sb = new StringBuilder();

            // En-têtes
            if (includeHeaders)
            {
                sb.AppendLine("Expression,Result,Timestamp");
            }

            // Données
            foreach (var operation in _history)
            {
                sb.AppendLine($"\"{operation.Expression}\",{operation.Result},{operation.Timestamp:yyyy-MM-dd HH:mm:ss}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Exporte l'historique vers un fichier texte formaté.
        /// </summary>
        /// <param name="filePath">Chemin du fichier de destination</param>
        public void ExportToText(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            var sb = new StringBuilder();
            sb.AppendLine("=== HISTORIQUE DES CALCULS ===");
            sb.AppendLine($"Généré le: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Nombre d'opérations: {_history.Count}");
            sb.AppendLine();

            for (int i = 0; i < _history.Count; i++)
            {
                var op = _history[i];
                sb.AppendLine($"{i + 1}. {op}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Exporte l'historique vers un fichier JSON.
        /// </summary>
        /// <param name="filePath">Chemin du fichier de destination</param>
        public void ExportToJson(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"exportDate\": \"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\",");
            sb.AppendLine($"  \"operationCount\": {_history.Count},");
            sb.AppendLine("  \"operations\": [");

            for (int i = 0; i < _history.Count; i++)
            {
                var op = _history[i];
                sb.Append("    {");
                sb.Append($"\"expression\": \"{EscapeJsonString(op.Expression)}\", ");
                sb.Append($"\"result\": {op.Result}, ");
                sb.Append($"\"timestamp\": \"{op.Timestamp:yyyy-MM-dd HH:mm:ss}\"");
                sb.Append("}");

                if (i < _history.Count - 1)
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }

            sb.AppendLine("  ]");
            sb.AppendLine("}");

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Échappe les caractères spéciaux pour JSON.
        /// </summary>
        private static string EscapeJsonString(string input)
        {
            return input.Replace("\\", "\\\\")
                       .Replace("\"", "\\\"")
                       .Replace("\n", "\\n")
                       .Replace("\r", "\\r")
                       .Replace("\t", "\\t");
        }

        // ==================== IMPORT ====================

        /// <summary>
        /// Importe l'historique depuis un fichier CSV.
        /// Format attendu: Expression,Result,Timestamp
        /// </summary>
        /// <param name="filePath">Chemin du fichier source</param>
        /// <param name="clearExisting">Effacer l'historique existant (défaut: false)</param>
        /// <param name="skipHeaders">Ignorer première ligne d'en-têtes (défaut: true)</param>
        /// <returns>Nombre d'opérations importées</returns>
        public int ImportFromCsv(string filePath, bool clearExisting = false, bool skipHeaders = true)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Fichier introuvable", filePath);

            if (clearExisting)
                Clear();

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            int startIndex = skipHeaders ? 1 : 0;
            int importedCount = 0;

            for (int i = startIndex; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    var parts = ParseCsvLine(line);
                    if (parts.Length >= 2)
                    {
                        string expression = parts[0];
                        if (double.TryParse(parts[1], out double result))
                        {
                            var operation = new Operation(expression, result);

                            // Si timestamp fourni, l'utiliser
                            if (parts.Length >= 3 && DateTime.TryParse(parts[2], out DateTime timestamp))
                            {
                                operation = operation with { Timestamp = timestamp };
                            }

                            Add(operation);
                            importedCount++;
                        }
                    }
                }
                catch
                {
                    // Ignorer lignes invalides
                    continue;
                }
            }

            return importedCount;
        }

        /// <summary>
        /// Parse une ligne CSV en tenant compte des guillemets.
        /// </summary>
        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var currentField = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            result.Add(currentField.ToString());
            return result.ToArray();
        }

        // ==================== UTILITAIRES ====================

        /// <summary>
        /// Crée une copie de l'historique actuel.
        /// </summary>
        /// <returns>Nouvel historique avec copie des opérations</returns>
        public OperationHistory Clone()
        {
            var clone = new OperationHistory(_maxCapacity);
            foreach (var operation in _history)
            {
                clone.Add(operation);
            }
            return clone;
        }

        /// <summary>
        /// Représentation textuelle de l'historique.
        /// </summary>
        /// <returns>Résumé de l'historique</returns>
        public override string ToString()
        {
            return $"Historique: {_history.Count}/{_maxCapacity} opérations";
        }
    }
}
