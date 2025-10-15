using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simply_Calc_AppWPF.Helpers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Simply_Calc_AppWPF.core;
using Simply_Calc_AppWPF.Services;

namespace Simply_Calc_AppWPF
{
    public partial class MainWindow : Window
    {
            /// <summary>
            /// Affiche un message d'erreur dans l'interface.
            /// </summary>
            /// <param name="message">Message d'erreur</param>
            /// <param name="showInResultFinal">True pour afficher dans ResultatBlock, false pour resultatTexte</param>
            private void ShowError(string message, bool showInResultFinal)
            {
                // Sélection taille de police
                int fontSize = message switch
                {
                    "Syntax error" => CalculatorConstants.ERROR_FONT_SIZE_SYNTAX,
                    "Math error" => CalculatorConstants.ERROR_FONT_SIZE_MATH,
                    "Impossible de diviser par 0" => CalculatorConstants.ERROR_FONT_SIZE_DIVISION,
                    _ => CalculatorConstants.ERROR_FONT_SIZE_DEFAULT
                };

                if (showInResultFinal)
                {
                    // Affichage dans ResultatBlock
                    ResultatBlock.Text = message;
                    ResultatBlock.FontStyle = FontStyles.Italic;
                    ResultatBlock.FontSize = fontSize;
                    ResultatBlock.Foreground = Brushes.Red;
                }
                else
                {
                    // Affichage dans resultatTexte
                    resultatTexte.Text = message;
                    resultatTexte.FontStyle = FontStyles.Italic;
                    resultatTexte.FontSize = fontSize;
                    resultatTexte.Foreground = Brushes.Red;
                }

                // Mise en état d'erreur
                _state.IsErrorState = true;
                _state.IsOperatorSet = false;
                _state.CurrentOperator = null;

                // Logger l'erreur
                _errorHandler?.CreateError(
                    ErrorType.SyntaxError,
                    message,
                    ErrorSeverity.Error
                );
            }

            /// <summary>
            /// Nettoie les styles d'erreur et réinitialise l'affichage.
            /// </summary>
            private void ClearErrorStyles()
            {
                if (!_state.IsErrorState) return;

                // Reset resultatTexte
                resultatTexte.FontStyle = FontStyles.Normal;
                resultatTexte.FontSize = _defaultResultatFontSize;
                resultatTexte.Foreground = Brushes.White;

                // Reset ResultatBlock
                ResultatBlock.FontStyle = FontStyles.Italic;
                ResultatBlock.FontSize = _defaultResultBlockFontSize;
                ResultatBlock.Foreground = Brushes.DimGray;

                // Reset operationTexte
                operationTexte.FontStyle = FontStyles.Normal;
                operationTexte.FontSize = _defaultOperationFontSize;

                // Quitter état d'erreur
                _state.IsErrorState = false;
                ResetCalculatorExcept();
            }

            /// <summary>
            /// Réinitialise la calculatrice sauf les propriétés spécifiées.
            /// </summary>
            /// <param name="exceptions">Noms des propriétés à ne pas réinitialiser</param>
            private void ResetCalculatorExcept(params string[] exceptions)
            {
                var exceptionsSet = new HashSet<string>(exceptions);

                // Nettoyer affichages
                if (!exceptionsSet.Contains(nameof(resultatTexte)))
                    resultatTexte.Text = "0";

                if (!exceptionsSet.Contains(nameof(operationTexte)))
                    operationTexte.Text = string.Empty;

                if (!exceptionsSet.Contains(nameof(ResultatBlock)))
                    ResultatBlock.Text = "0";

                // Réinitialiser états numériques
                if (!exceptionsSet.Contains(nameof(_state.Result)))
                    _state.Result = 0;

                if (!exceptionsSet.Contains(nameof(_state.FirstValue)))
                    _state.FirstValue = 0;

                if (!exceptionsSet.Contains(nameof(_state.SecondValue)))
                    _state.SecondValue = 0;

                if (!exceptionsSet.Contains(nameof(_state.OperationCount)))
                    _state.OperationCount = 0;

                if (!exceptionsSet.Contains(nameof(_state.SubResult)))
                    _state.SubResult = 0;

                if (!exceptionsSet.Contains(nameof(_state.SubFirstValue)))
                    _state.SubFirstValue = 0;

                if (!exceptionsSet.Contains(nameof(_state.SubSecondValue)))
                    _state.SubSecondValue = null;

                // Réinitialiser opérateurs principaux
                if (!exceptionsSet.Contains(nameof(_state.CurrentOperator)))
                    _state.CurrentOperator = null;

                if (!exceptionsSet.Contains(nameof(_state.CurrentOperatorSymbol)))
                    _state.CurrentOperatorSymbol = string.Empty;

                if (!exceptionsSet.Contains(nameof(_state.IsOperatorSet)))
                    _state.IsOperatorSet = false;

                // Réinitialiser sous-opérateurs
                if (!exceptionsSet.Contains(nameof(_state.SubOperator)))
                    _state.SubOperator = null;

                if (!exceptionsSet.Contains(nameof(_state.IsSubOperatorSet)))
                    _state.IsSubOperatorSet = false;

                if (!exceptionsSet.Contains(nameof(_state.IsSubOperationStarted)))
                    _state.IsSubOperationStarted = false;

                // Réinitialiser gestion parenthèses
                if (!exceptionsSet.Contains(nameof(_state.IsParenthesisOpen)))
                    _state.IsParenthesisOpen = false;

                if (!exceptionsSet.Contains(nameof(_state.IsParenthesisJustClosed)))
                    _state.IsParenthesisJustClosed = false;

                if (!exceptionsSet.Contains(nameof(_state.PreviousSubExpression)))
                    _state.PreviousSubExpression = string.Empty;

                // Réinitialiser buffers
                if (!exceptionsSet.Contains(nameof(_state.InputBuffer)))
                    _state.InputBuffer = string.Empty;

                if (!exceptionsSet.Contains(nameof(_state.IsOperationJustDone)))
                    _state.IsOperationJustDone = false;

                if (!exceptionsSet.Contains(nameof(_state.IsEqualDone)))
                    _state.IsEqualDone = false;

                // Réinitialiser états d'erreur
                if (!exceptionsSet.Contains(nameof(_state.IsErrorState)))
                    _state.IsErrorState = false;

                // Réinitialiser constantes
                if (!exceptionsSet.Contains(nameof(_state.IsConstantUsed)))
                    _state.IsConstantUsed = false;

                if (!exceptionsSet.Contains(nameof(_state.SubConstant)))
                    _state.SubConstant = 1.0;
            }
        }
    }
