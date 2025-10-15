using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Simply_Calc_AppWPF.Helpers;
using System.Windows.Controls;
using System.Windows.Media;
using Simply_Calc_AppWPF.core;
using System.Globalization;

namespace Simply_Calc_AppWPF
{

    public partial class MainWindow : Window
    {
        /// <summary>
        /// Gère le clic sur le bouton "=" (égal).
        /// </summary>
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (_state.IsEqualDone) return;

            if (_state.IsErrorState)
            {
                ClearErrorStyles();
                return;
            }

            if (_state.IsParenthesisOpen)
            {
                CloseParenthesis();
            }

            // Parsing valeur courante
            double parsed;
            if (!ParsingHelper.TryParseDouble(_state.InputBuffer, out parsed))
            {
                if (!_state.IsParenthesisJustClosed)
                {
                    if (!_state.IsConstantUsed)
                    {
                        ShowError(CalculatorConstants.ERROR_SYNTAX, false);
                        return;
                    }
                    else
                    {
                        parsed = _state.SubConstant;
                        _state.IsConstantUsed = false;
                    }
                }
                else
                {
                    parsed = _state.SubResult;
                    _state.IsParenthesisJustClosed = false;
                }
            }

            // Calcul final
            if ((_state.CurrentOperator == null && !_state.IsOperatorSet) ||
                string.IsNullOrEmpty(_state.InputBuffer))
            {
                _state.Result = parsed;
            }

            if (_state.IsOperatorSet)
            {
                _state.SecondValue = parsed;
                Compute();
            }

            // Mise à jour affichage si pas d'erreur
            if (!_state.IsErrorState)
            {
                _state.IsEqualDone = true;
                operationTexte.Text += resultatTexte.Text;

                ResultatBlock.Text = double.IsInfinity(_state.Result) || double.IsNaN(_state.Result)
                    ? "Impossible"
                    : _state.Result.ToString(CultureInfo.CurrentCulture);

                resultatTexte.Text = ResultatBlock.Text;
                _state.InputBuffer = ResultatBlock.Text == "Impossible"
                    ? string.Empty
                    : _state.Result.ToString(CultureInfo.CurrentCulture);

                _state.FirstValue = _state.Result;
                _state.IsOperatorSet = false;
                _state.CurrentOperator = null;

                // Ajouter à l'historique
                if (_history != null && ResultatBlock.Text != "Impossible")
                {
                    _history.Add(operationTexte.Text, _state.Result);
                }
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton "Back" (supprimer dernier caractère).
        /// </summary>
        private void DeleteLast_Click(object sender, RoutedEventArgs e)
        {
            if (_state.IsErrorState)
            {
                ClearErrorStyles();
                return;
            }

            if (_state.IsEqualDone) return;

            if (!string.IsNullOrEmpty(_state.InputBuffer))
            {
                char lastChar = resultatTexte.Text[^1];

                // Cas : suppression d'une parenthèse ouvrante
                if (lastChar == '(' && _state.SubOperator != null)
                {
                    string funcKey = GetFunctionKey(_state.SubOperator);
                    if (!string.IsNullOrEmpty(funcKey))
                    {
                        resultatTexte.Text = resultatTexte.Text[..^(funcKey.Length + 1)];
                    }

                    // Reset sous-opération
                    _state.IsParenthesisOpen = false;
                    _state.IsParenthesisJustClosed = false;
                    _state.IsSubOperatorSet = false;
                    _state.IsSubOperationStarted = false;
                    _state.SubOperator = null;
                    _state.PreviousSubExpression = string.Empty;
                    _state.SubFirstValue = 0;
                    _state.SubSecondValue = null;
                    _state.SubResult = 0;
                    return;
                }

                // Cas : suppression d'une constante
                if (lastChar == 'e' || lastChar == 'π')
                {
                    _state.IsConstantUsed = false;
                    _state.SubConstant = 1;
                }

                // Cas : suppression parenthèse fermante
                if (lastChar == ')')
                {
                    _state.IsParenthesisJustClosed = false;
                    _state.IsParenthesisOpen = true;
                    _state.IsSubOperatorSet = true;
                }

                // Suppression caractère
                _state.InputBuffer = _state.InputBuffer.Length > 1
                    ? _state.InputBuffer[..^1]
                    : string.Empty;

                resultatTexte.Text = string.IsNullOrEmpty(_state.InputBuffer) ? "0" : _state.InputBuffer;
            }
            else
            {
                // Suppression dans operationTexte si buffer vide
                if (!string.IsNullOrEmpty(operationTexte.Text) &&
                    !ParsingHelper.IsOperator(operationTexte.Text[^1]))
                {
                    operationTexte.Text = operationTexte.Text[..^1];
                }
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton "CE" (Clear Entry).
        /// </summary>
        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            if (_state.IsEqualDone) return;

            ClearErrorStyles();

            // Zone de saisie à zéro
            resultatTexte.Text = "0";
            _state.InputBuffer = string.Empty;

            // Reset sous-opérations
            _state.IsParenthesisOpen = false;
            _state.IsParenthesisJustClosed = false;
            _state.IsSubOperatorSet = false;
            _state.IsSubOperationStarted = false;
            _state.SubOperator = null;
            _state.PreviousSubExpression = string.Empty;
            _state.SubFirstValue = 0;
            _state.SubSecondValue = null;
            _state.SubResult = 0;
            _state.IsOperationJustDone = false;
        }

        /// <summary>
        /// Gère le clic sur le bouton "C" (Clear All).
        /// </summary>
        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            ClearErrorStyles();
            ResetCalculatorExcept();

            _state.InputBuffer = string.Empty;
            _state.FirstValue = 0;
            _state.SecondValue = 0;
            _state.Result = 0;
            _state.IsOperatorSet = false;
            _state.CurrentOperator = null;
            _state.OperationCount = 0;

            operationTexte.Text = string.Empty;
            resultatTexte.Text = "0";
            ResultatBlock.Text = "0";
        }

        /// <summary>
        /// Gère le clic sur le bouton "+/-" (changement de signe).
        /// </summary>
        private void ChangeSign_Click(object sender, RoutedEventArgs e)
        {
            if (_state.IsEqualDone)
            {
                ResetCalculatorExcept(nameof(_state.Result));
                resultatTexte.Text = (-_state.Result).ToString();
                return;
            }

            if (string.IsNullOrEmpty(_state.InputBuffer)) return;

            if (_state.InputBuffer.StartsWith("-"))
            {
                _state.InputBuffer = _state.InputBuffer[1..];
            }
            else
            {
                _state.InputBuffer = "-" + _state.InputBuffer;
            }

            if (_state.IsConstantUsed)
            {
                _state.SubConstant = -_state.SubConstant;
            }

            resultatTexte.Text = _state.InputBuffer;
        }

        /// <summary>
        /// Obtient la clé d'une fonction depuis son delegate.
        /// </summary>
        private string GetFunctionKey(Func<double, double?, double> func)
        {
            if (func == null) return string.Empty;

            foreach (var kvp in _functions)
            {
                if (kvp.Value.Method == func.Method)
                {
                    return kvp.Key;
                }
            }

            return string.Empty;
        }
    }
}
     

