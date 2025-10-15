using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Simply_Calc_AppWPF.core;
using Simply_Calc_AppWPF.Helpers;

namespace Simply_Calc_AppWPF
{
    /// <summary>
    /// Partial class gérant les opérateurs arithmétiques (+, -, ×, ÷).
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Gère le clic sur un bouton d'opérateur binaire.
        /// </summary>
        /// <param name="symbol">Symbole de l'opérateur</param>
        private void OperatorButton_Click(string symbol)
        {
            // 1. Validation initiale
            if (_state.IsErrorState)
            {
                ClearErrorStyles();
                return;
            }

            if (string.IsNullOrEmpty(symbol))
                return;

            // 2. Fermeture parenthèse si ouverte
            if (_state.IsParenthesisOpen)
            {
                CloseParenthesis();
            }

            // 3. Cas où "=" a déjà été fait
            if (_state.IsEqualDone)
            {
                ResetCalculatorExcept(nameof(_state.Result));
                _state.IsEqualDone = false;
                _state.FirstValue = _state.Result;

                operationTexte.Text = $"{_state.FirstValue}{symbol}";
                ResultatBlock.Text = _state.Result.ToString();

                _state.CurrentOperatorSymbol = symbol;
                _state.CurrentOperator = _operations[symbol];
                _state.IsOperatorSet = true;
                return;
            }

            // 4. Gestion remplacement de symbole si buffer vide
            if (string.IsNullOrEmpty(_state.InputBuffer) && _state.IsOperatorSet)
            {
                if (!string.IsNullOrEmpty(operationTexte.Text) &&
                    ParsingHelper.IsOperator(operationTexte.Text[^1]))
                {
                    operationTexte.Text = operationTexte.Text[..^1] + symbol;
                }

                _state.CurrentOperatorSymbol = symbol;
                _state.CurrentOperator = _operations[symbol];
                return;
            }

            // 5. Parsing de l'entrée courante
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

            // 6. Calcul et mise à jour
            if (!_state.IsOperatorSet)
            {
                _state.FirstValue = parsed;
            }
            else
            {
                _state.SecondValue = parsed;
                Compute();
                _state.FirstValue = _state.Result;
            }

            // 7. Définir nouvel opérateur
            if (_operations.ContainsKey(symbol))
            {
                _state.CurrentOperator = _operations[symbol];
                _state.IsOperatorSet = true;
                _state.CurrentOperatorSymbol = symbol;

                // Mise à jour affichage
                if (!string.IsNullOrEmpty(_state.InputBuffer))
                {
                    if (_state.InputBuffer[0] == '-')
                    {
                        operationTexte.Text += $"({_state.InputBuffer}){symbol}";
                    }
                    else
                    {
                        operationTexte.Text += $"{_state.InputBuffer}{symbol}";
                    }
                }

                resultatTexte.Text = string.Empty;
            }
            else
            {
                _state.CurrentOperator = null;
                _state.IsOperatorSet = false;
                _state.CurrentOperatorSymbol = string.Empty;
            }
        }
    }
}


