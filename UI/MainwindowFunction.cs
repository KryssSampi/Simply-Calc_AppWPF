using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Simply_Calc_AppWPF.core;
using Simply_Calc_AppWPF.Helpers;
using System.Windows.Controls;

namespace Simply_Calc_AppWPF
{
    /// <summary>
    /// Partial class gérant les fonctions scientifiques et constantes.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Gère le clic sur un bouton de fonction (sin, cos, tan, etc.).
        /// </summary>
        private void FuncButton_Click(object sender, RoutedEventArgs e)
        {
            if (_state.IsErrorState)
            {
                ClearErrorStyles();
                return;
            }

            if (sender is not Button btn) return;
            string token = btn.Content?.ToString() ?? "";

            // Gestion constante utilisée
            if (_state.IsConstantUsed)
            {
                OperatorButton_Click(CalculatorConstants.OPERATOR_MULTIPLICATION);
                _state.IsConstantUsed = false;
            }

            // Gestion égal done
            if (_state.IsEqualDone)
            {
                ResetCalculatorExcept(nameof(_state.Result));
                resultatTexte.Text = token + "(";
                _state.PreviousSubExpression = resultatTexte.Text;
                _state.InputBuffer = resultatTexte.Text;
                _state.SubOperator = _functions.ContainsKey(token.ToLower()) ? _functions[token.ToLower()] : null;
                _state.IsSubOperatorSet = _state.SubOperator != null;
                _state.IsParenthesisOpen = true;
                resultatTexte.Text += _state.Result.ToString();
                CloseParenthesis();
                Equals_Click(null, null);
                return;
            }

            // Fermer parenthèse si ouverte
            if (_state.IsParenthesisOpen)
            {
                CloseParenthesis();
                OperatorButton_Click(CalculatorConstants.OPERATOR_MULTIPLICATION);
                _state.IsParenthesisJustClosed = false;
            }

            if (resultatTexte.Text == "0")
                resultatTexte.Text = string.Empty;

            // Vérification longueur max
            if (resultatTexte.Text.Length + token.Length + 1 >= CalculatorConstants.MAX_OPERATION_LENGTH)
                return;

            if (!_state.IsErrorState)
            {
                // Parsing SubSecondVal
                if (!string.IsNullOrEmpty(resultatTexte.Text) && resultatTexte.Text != "0")
                {
                    if (ParsingHelper.TryParseDouble(resultatTexte.Text, out double parsed))
                    {
                        _state.SubSecondValue = parsed;
                    }
                    else
                    {
                        _state.SubSecondValue = null;
                    }
                }
                else
                {
                    _state.SubSecondValue = null;
                }

                // Mise à jour interface
                resultatTexte.Text += token + "(";
                _state.PreviousSubExpression = resultatTexte.Text;
                _state.InputBuffer = resultatTexte.Text;
                _state.SubOperator = _functions.ContainsKey(token.ToLower()) ? _functions[token.ToLower()] : null;
                _state.IsSubOperatorSet = _state.SubOperator != null;
                _state.IsParenthesisOpen = true;
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton π (Pi).
        /// </summary>
        private void Pi_Click(object sender, RoutedEventArgs e)
        {
            if (_state.IsConstantUsed)
            {
                OperatorButton_Click(CalculatorConstants.OPERATOR_MULTIPLICATION);
                _state.IsConstantUsed = false;
            }

            double multiplier = 0;
            _state.IsConstantUsed = true;

            if (_state.IsEqualDone)
            {
                OperatorButton_Click(CalculatorConstants.OPERATOR_MULTIPLICATION);
            }

            if (_state.IsParenthesisOpen)
            {
                _state.SubFirstValue = CalculatorConstants.PI;
                resultatTexte.Text += CalculatorConstants.PI_SYMBOL;
                CloseParenthesis();
                _state.IsConstantUsed = false;
                return;
            }

            if (resultatTexte.Text == "0")
            {
                _state.SubConstant = CalculatorConstants.PI;
                resultatTexte.Text = CalculatorConstants.PI_SYMBOL;
            }
            else
            {
                if (ParsingHelper.TryParseDouble(resultatTexte.Text, out double parsed))
                    multiplier = parsed;

                _state.SubConstant = CalculatorConstants.PI * multiplier;
                resultatTexte.Text += CalculatorConstants.PI_SYMBOL;
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton e (Euler).
        /// </summary>
        private void Exp_Click(object sender, RoutedEventArgs e)
        {
            if (_state.IsConstantUsed)
            {
                OperatorButton_Click(CalculatorConstants.OPERATOR_MULTIPLICATION);
                _state.IsConstantUsed = false;
            }

            double multiplier = 0;
            _state.IsConstantUsed = true;

            if (_state.IsEqualDone)
            {
                OperatorButton_Click(CalculatorConstants.OPERATOR_MULTIPLICATION);
            }

            if (_state.IsParenthesisOpen)
            {
                _state.SubFirstValue = CalculatorConstants.E;
                resultatTexte.Text += CalculatorConstants.E_SYMBOL;
                CloseParenthesis();
                _state.IsConstantUsed = false;
                return;
            }

            if (resultatTexte.Text == "0")
            {
                _state.SubConstant = CalculatorConstants.E;
                resultatTexte.Text = CalculatorConstants.E_SYMBOL;
            }
            else
            {
                if (ParsingHelper.TryParseDouble(resultatTexte.Text, out double parsed))
                    multiplier = parsed;

                _state.SubConstant = CalculatorConstants.E * multiplier;
                resultatTexte.Text += CalculatorConstants.E_SYMBOL;
            }
        }

        /// <summary>
        /// Ferme une parenthèse et calcule la sous-expression.
        /// </summary>
        private void CloseParenthesis()
        {
            if (!_state.IsParenthesisOpen) return;

            string inside = string.Empty;

            // Extraire contenu après PreviousSubExpression
            if (!string.IsNullOrEmpty(_state.PreviousSubExpression) &&
                resultatTexte.Text.Length >= _state.PreviousSubExpression.Length)
            {
                inside = resultatTexte.Text.Substring(_state.PreviousSubExpression.Length);
            }

            // Fermeture logique
            _state.IsParenthesisOpen = false;

            if (_state.IsSubOperatorSet && _state.SubOperator != null)
            {
                if (ParsingHelper.TryParseDouble(inside, out double parsed))
                {
                    _state.SubFirstValue = parsed;

                    // Vérifier si fonction inverse trigo
                    bool isArcFunction = IsInverseTrigonometric(_state.SubOperator);

                    if (!isArcFunction)
                    {
                        // Convertir degrés → radians
                        _state.SubResult = _state.SubOperator(
                            MathHelper.DegreesToRadians(_state.SubFirstValue),
                            _state.SubSecondValue
                        );
                    }
                    else
                    {
                        // Fonction arc : entrée en valeur, sortie en degrés
                        double resultRadians = _state.SubOperator(_state.SubFirstValue, _state.SubSecondValue);
                        _state.SubResult = MathHelper.RadiansToDegrees(resultRadians);

                        // Arrondir si petit nombre
                        if (inside.Length > 9)
                        {
                            _state.SubResult = Math.Round(_state.SubResult);
                        }
                    }
                }
                else
                {
                    // Si pas de valeur numérique
                    if (!_state.IsConstantUsed)
                    {
                        ShowError(CalculatorConstants.ERROR_SYNTAX, false);
                        return;
                    }
                    else
                    {
                        _state.SubResult = _state.SubOperator(_state.SubFirstValue, _state.SubSecondValue);
                    }
                }
            }

            if (!_state.IsErrorState)
            {
                resultatTexte.Text += ")";
                _state.IsSubOperatorSet = false;
                _state.IsParenthesisJustClosed = true;
            }
        }

        /// <summary>
        /// Vérifie si une fonction est trigonométrique inverse.
        /// </summary>
        private bool IsInverseTrigonometric(Func<double, double?, double> fonction)
        {
            if (fonction == null) return false;

            // Comparer avec les fonctions arc connues
            return fonction.Method == _functions[CalculatorConstants.FUNCTION_ARCSIN].Method ||
                   fonction.Method == _functions[CalculatorConstants.FUNCTION_ARCCOS].Method ||
                   fonction.Method == _functions[CalculatorConstants.FUNCTION_ARCTAN].Method;
        }
    }
}
