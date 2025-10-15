using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simply_Calc_AppWPF.Models
{
    /// <summary>
    /// Représente l'état complet de la calculatrice à un instant donné.
    /// Encapsule toutes les variables d'état pour faciliter la gestion et les tests.
    /// </summary>
    public class CalculatorState
    {
        // ==================== VALEURS PRINCIPALES ====================

        /// <summary>
        /// Résultat global courant de la calculatrice.
        /// Mis à jour après chaque calcul ou touche "=".
        /// </summary>
        public double Result { get; set; }

        /// <summary>
        /// Première valeur d'une opération binaire (ex: 5 dans "5 + 3").
        /// </summary>
        public double FirstValue { get; set; }

        /// <summary>
        /// Deuxième valeur d'une opération binaire (ex: 3 dans "5 + 3").
        /// </summary>
        public double SecondValue { get; set; }

        // ==================== OPÉRATEURS ====================

        /// <summary>
        /// Référence vers l'opérateur binaire courant (+, -, ×, ÷).
        /// </summary>
        public Func<double, double, double>? CurrentOperator { get; set; }

        /// <summary>
        /// Symbole textuel de l'opérateur sélectionné (pour affichage).
        /// </summary>
        public string CurrentOperatorSymbol { get; set; } = string.Empty;  

        /// <summary>
        /// Référence vers l'opérateur de sous-opération (sin, cos, tan, etc.).
        /// </summary>
        public Func<double, double?, double>? SubOperator { get; set; }

        // ==================== ÉTATS BOOLÉENS ====================

        /// <summary>
        /// Indique si un opérateur binaire a été sélectionné.
        /// </summary>
        public bool IsOperatorSet { get; set; } = false;

        /// <summary>
        /// Indique si un sous-opérateur (fonction) a été sélectionné.
        /// </summary>
        public bool IsSubOperatorSet { get; set; } =false;

        /// <summary>
        /// Indique si une parenthèse ouvrante est en attente de fermeture.
        /// </summary>
        public bool IsParenthesisOpen { get; set; } = false;

        /// <summary>
        /// Indique si la dernière action a été de fermer une parenthèse.
        /// </summary>
        public bool IsParenthesisJustClosed { get; set; } = false;

        /// <summary>
        /// Indique si une constante (π ou e) est utilisée dans la saisie.
        /// </summary>
        public bool IsConstantUsed { get; set; } = false;

        /// <summary>
        /// Indique si une opération vient d'être effectuée.
        /// Empêche une saisie incohérente immédiatement après.
        /// </summary>
        public bool IsOperationJustDone { get; set; } = false;

        /// <summary>
        /// Indique si la touche "=" a été pressée.
        /// Permet de réinitialiser ou enchaîner les calculs.
        /// </summary>
        public bool IsEqualDone { get; set; } = false;

        /// <summary>
        /// Indique si une erreur est actuellement affichée.
        /// Empêche toute saisie incohérente pendant l'affichage de l'erreur.
        /// </summary>
        public bool IsErrorState { get; set; } = false;

        /// <summary>
        /// Indique si une sous-opération a démarré.
        /// </summary>
        public bool IsSubOperationStarted { get; set; } = false;

        // ==================== SOUS-OPÉRATIONS ====================

        /// <summary>
        /// Résultat temporaire de la sous-opération (fonction).
        /// </summary>
        public double SubResult { get; set; }

        /// <summary>
        /// Première valeur de la sous-opération.
        /// </summary>
        public double SubFirstValue { get; set; }

        /// <summary>
        /// Deuxième valeur de la sous-opération (optionnelle, peut être null).
        /// Utilisée comme multiplicateur pour les fonctions.
        /// </summary>
        public double? SubSecondValue { get; set; }

        /// <summary>
        /// Constante temporaire utilisée dans un sous-calcul (π ou e).
        /// </summary>
        public double SubConstant { get; set; }

        /// <summary>
        /// Sauvegarde de l'état précédent du texte avant une sous-opération.
        /// </summary>
        public string PreviousSubExpression { get; set; } = string.Empty;

        // ==================== BUFFERS ET COMPTEURS ====================

        /// <summary>
        /// Tampon de saisie utilisateur courant (zone texte avant validation).
        /// </summary>
        public string InputBuffer { get; set; } = string.Empty ;

        /// <summary>
        /// Mémoire tampon de l'affichage précédent pour détecter ce qui a été ajouté.
        /// </summary>
        public string OperationTextPrevious { get; set; } = string.Empty;

        /// <summary>
        /// Compteur du nombre d'opérations effectuées.
        /// Utile pour diagnostics ou suivi.
        /// </summary>
        public int OperationCount { get; set; }
        public string ErrorMessage { get; internal set; } = string.Empty;
        public object LastResult { get; internal set; }

        // ==================== CONSTRUCTEURS ====================

        /// <summary>
        /// Constructeur par défaut initialisant tous les états à leurs valeurs par défaut.
        /// </summary>
        public CalculatorState()
        {
            // Valeurs numériques
            Result = 0;
            FirstValue = 0;
            SecondValue = 0;
            SubResult = 0;
            SubFirstValue = 0;
            SubSecondValue = null;
            SubConstant = 1.0;
            OperationCount = 0;

            // Opérateurs
            CurrentOperator = null;
            CurrentOperatorSymbol = string.Empty;
            SubOperator = null;

            // États booléens
            IsOperatorSet = false;
            IsSubOperatorSet = false;
            IsParenthesisOpen = false;
            IsParenthesisJustClosed = false;
            IsConstantUsed = false;
            IsOperationJustDone = false;
            IsEqualDone = false;
            IsErrorState = false;
            IsSubOperationStarted = false;

            // Buffers
            InputBuffer = string.Empty;
            OperationTextPrevious = string.Empty;
            PreviousSubExpression = string.Empty;
        }

        // ==================== MÉTHODES DE RÉINITIALISATION ====================

        /// <summary>
        /// Réinitialise l'état complet de la calculatrice.
        /// </summary>
        /// <param name="exceptions">Noms des propriétés à ne PAS réinitialiser</param>
        public void Reset(params string[] exceptions)
        {
            var exceptionsSet = new HashSet<string>(exceptions);

            // Reset valeurs numériques
            if (!exceptionsSet.Contains(nameof(Result))) Result = 0;
            if (!exceptionsSet.Contains(nameof(FirstValue))) FirstValue = 0;
            if (!exceptionsSet.Contains(nameof(SecondValue))) SecondValue = 0;
            if (!exceptionsSet.Contains(nameof(SubResult))) SubResult = 0;
            if (!exceptionsSet.Contains(nameof(SubFirstValue))) SubFirstValue = 0;
            if (!exceptionsSet.Contains(nameof(SubSecondValue))) SubSecondValue = null;
            if (!exceptionsSet.Contains(nameof(SubConstant))) SubConstant = 1.0;
            if (!exceptionsSet.Contains(nameof(OperationCount))) OperationCount = 0;

            // Reset opérateurs
            if (!exceptionsSet.Contains(nameof(CurrentOperator))) CurrentOperator = null;
            if (!exceptionsSet.Contains(nameof(CurrentOperatorSymbol))) CurrentOperatorSymbol = string.Empty;
            if (!exceptionsSet.Contains(nameof(SubOperator))) SubOperator = null;

            // Reset états booléens
            if (!exceptionsSet.Contains(nameof(IsOperatorSet))) IsOperatorSet = false;
            if (!exceptionsSet.Contains(nameof(IsSubOperatorSet))) IsSubOperatorSet = false;
            if (!exceptionsSet.Contains(nameof(IsParenthesisOpen))) IsParenthesisOpen = false;
            if (!exceptionsSet.Contains(nameof(IsParenthesisJustClosed))) IsParenthesisJustClosed = false;
            if (!exceptionsSet.Contains(nameof(IsConstantUsed))) IsConstantUsed = false;
            if (!exceptionsSet.Contains(nameof(IsOperationJustDone))) IsOperationJustDone = false;
            if (!exceptionsSet.Contains(nameof(IsEqualDone))) IsEqualDone = false;
            if (!exceptionsSet.Contains(nameof(IsErrorState))) IsErrorState = false;
            if (!exceptionsSet.Contains(nameof(IsSubOperationStarted))) IsSubOperationStarted = false;

            // Reset buffers
            if (!exceptionsSet.Contains(nameof(InputBuffer))) InputBuffer = string.Empty;
            if (!exceptionsSet.Contains(nameof(OperationTextPrevious))) OperationTextPrevious = string.Empty;
            if (!exceptionsSet.Contains(nameof(PreviousSubExpression))) PreviousSubExpression = string.Empty;
        }

        /// <summary>
        /// Réinitialise uniquement les états liés aux sous-opérations.
        /// </summary>
        public void ResetSubOperation()
        {
            SubOperator = null;
            IsSubOperatorSet = false;
            IsSubOperationStarted = false;
            SubResult = 0;
            SubFirstValue = 0;
            SubSecondValue = null;
            SubConstant = 1.0;
            PreviousSubExpression = string.Empty;
            IsParenthesisOpen = false;
            IsParenthesisJustClosed = false;
        }

        /// <summary>
        /// Réinitialise uniquement les états liés aux opérateurs principaux.
        /// </summary>
        public void ResetMainOperation()
        {
            CurrentOperator = null;
            CurrentOperatorSymbol = string.Empty;
            IsOperatorSet = false;
            FirstValue = 0;
            SecondValue = 0;
        }

        /// <summary>
        /// Réinitialise l'état d'erreur.
        /// </summary>
        public void ClearError()
        {
            IsErrorState = false;
        }

        // ==================== MÉTHODES UTILITAIRES ====================

        /// <summary>
        /// Clone l'état actuel pour créer une copie indépendante.
        /// </summary>
        /// <returns>Nouvelle instance avec les mêmes valeurs</returns>
        public CalculatorState Clone()
        {
            return new CalculatorState
            {
                // Valeurs numériques
                Result = this.Result,
                FirstValue = this.FirstValue,
                SecondValue = this.SecondValue,
                SubResult = this.SubResult,
                SubFirstValue = this.SubFirstValue,
                SubSecondValue = this.SubSecondValue,
                SubConstant = this.SubConstant,
                OperationCount = this.OperationCount,

                // Opérateurs
                CurrentOperator = this.CurrentOperator,
                CurrentOperatorSymbol = this.CurrentOperatorSymbol,
                SubOperator = this.SubOperator,

                // États booléens
                IsOperatorSet = this.IsOperatorSet,
                IsSubOperatorSet = this.IsSubOperatorSet,
                IsParenthesisOpen = this.IsParenthesisOpen,
                IsParenthesisJustClosed = this.IsParenthesisJustClosed,
                IsConstantUsed = this.IsConstantUsed,
                IsOperationJustDone = this.IsOperationJustDone,
                IsEqualDone = this.IsEqualDone,
                IsErrorState = this.IsErrorState,
                IsSubOperationStarted = this.IsSubOperationStarted,

                // Buffers
                InputBuffer = this.InputBuffer,
                OperationTextPrevious = this.OperationTextPrevious,
                PreviousSubExpression = this.PreviousSubExpression
            };
        }

        /// <summary>
        /// Vérifie si l'état est dans une configuration valide.
        /// </summary>
        /// <returns>True si état cohérent</returns>
        public bool IsValid()
        {
            // Vérifications de cohérence
            if (IsOperatorSet && CurrentOperator == null)
                return false;

            if (IsSubOperatorSet && SubOperator == null)
                return false;

            if (IsParenthesisJustClosed && IsParenthesisOpen)
                return false;

            return true;
        }

        /// <summary>
        /// Représentation textuelle de l'état (pour debug).
        /// </summary>
        /// <returns>Résumé de l'état actuel</returns>
        public override string ToString()
        {
            return $"Result={Result}, FirstVal={FirstValue}, Operator={CurrentOperatorSymbol}, " +
                   $"IsOpSet={IsOperatorSet}, IsEqual={IsEqualDone}, IsError={IsErrorState}";
        }
    }
}
