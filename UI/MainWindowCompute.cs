using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simply_Calc_AppWPF.Helpers;
using Simply_Calc_AppWPF.Services;
using System.Windows;
using Simply_Calc_AppWPF.core;
using System.Globalization;

namespace Simply_Calc_AppWPF
{
    /// <summary>
    /// Partial class contenant la logique de calcul principale.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Effectue le calcul avec l'opérateur courant.
        /// </summary>
        private void Compute()
        {
            if (_state.CurrentOperator == null) return;

            _state.OperationCount++;
            double result;

            try
            {
                // Appliquer l'opérateur
                result = _state.CurrentOperator(_state.FirstValue, _state.SecondValue);
            }
            catch (Exception)
            {
                // Erreur d'exécution (overflow, etc.)
                ShowError(CalculatorConstants.ERROR_MATH, true);
                return;
            }

            // Vérifier validité du résultat
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                // Cas spécifique : division par zéro
                if (_state.CurrentOperatorSymbol == CalculatorConstants.OPERATOR_DIVISION &&
                    Math.Abs(_state.SecondValue) < CalculatorConstants.FLOATING_POINT_EPSILON)
                {
                    ShowError(CalculatorConstants.ERROR_DIVISION_BY_ZERO, true);
                }
                else
                {
                    ShowError(CalculatorConstants.ERROR_MATH, true);
                }

                _state.Result = 0;
            }
            else
            {
                // Résultat valide
                _state.Result = result;
                ResultatBlock.Text = _state.Result.ToString(CultureInfo.CurrentCulture);
                ClearErrorStyles();
            }

            // Reset flags
            _state.IsOperatorSet = false;
            _state.IsOperationJustDone = true;
        }
    }
}
