using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simply_Calc_AppWPF.Helpers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Simply_Calc_AppWPF.core;

namespace Simply_Calc_AppWPF
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Gestionnaire d'événement de mise à jour du layout.
        /// </summary>
        private void MainWindow_LayoutUpdated(object sender, EventArgs e)
        {
            // Sécurité : afficher "0" si vide
            if (string.IsNullOrEmpty(resultatTexte.Text))
            {
                resultatTexte.Text = "0";
            }

            // Corriger "-" seul
            if (resultatTexte.Text == "-")
            {
                resultatTexte.Text = "0";
            }

            // Gestion dynamique taille police opérations
            if (operationTexte.Text.Length > CalculatorConstants.OPERATION_TEXT_LENGTH_THRESHOLD)
            {
                operationTexte.FontSize = CalculatorConstants.REDUCED_OPERATION_FONT_SIZE;
                operationTexte.TextWrapping = TextWrapping.Wrap;

                if (operationTexte.Text.Length > CalculatorConstants.OPERATION_TEXT_LENGTH_THRESHOLD_SEVERE)
                {
                    operationTexte.FontSize = CalculatorConstants.REDUCED_OPERATION_FONT_SIZE - 5;
                }
            }
        }

        /// <summary>
        /// Gestionnaire de changement de texte pour resultatTexte.
        /// </summary>
        private void resultatTexte_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ignorer si erreur
            if (_state.IsErrorState) return;

            // Vérification longueur max
            if (ParsingHelper.TryParseDouble(resultatTexte.Text, out double test) &&
                resultatTexte.Text.Length > CalculatorConstants.MAX_INPUT_LENGTH)
            {
                resultatTexte.Text = resultatTexte.Text[..CalculatorConstants.MAX_INPUT_LENGTH];
                resultatTexte.CaretIndex = resultatTexte.Text.Length;
            }

            // Parsing valeur entrée
            if (ParsingHelper.TryParseDouble(resultatTexte.Text, out double parsed))
            {
                if (!_state.IsEqualDone)
                {
                    if (_state.CurrentOperator != null)
                    {
                        // Calcul intermédiaire
                        ResultatBlock.Text = _state.CurrentOperator(_state.FirstValue, parsed).ToString();
                    }
                    else
                    {
                        ResultatBlock.Text = parsed.ToString();
                    }
                }
            }
            // Cas : sous-opérateur actif
            else if (_state.SubOperator != null && _state.IsSubOperatorSet)
            {
                try
                {
                    if (_state.IsSubOperatorSet)
                    {
                        double tempResult;
                        string inside = string.Empty;

                        // Extraire contenu après PreviousSubExpression
                        if (!string.IsNullOrEmpty(_state.PreviousSubExpression) &&
                            resultatTexte.Text.Length >= _state.PreviousSubExpression.Length)
                        {
                            inside = resultatTexte.Text.Substring(_state.PreviousSubExpression.Length);
                        }

                        // Si contenu est un nombre
                        if (ParsingHelper.TryParseDouble(inside, out double temp))
                        {
                            bool isArcFunction = IsInverseTrigonometric(_state.SubOperator);

                            if (!isArcFunction)
                            {
                                tempResult = _state.SubOperator(
                                    MathHelper.DegreesToRadians(temp),
                                    _state.SubSecondValue
                                );
                            }
                            else
                            {
                                double resultRadians = _state.SubOperator(_state.SubFirstValue, temp);
                                tempResult = MathHelper.RadiansToDegrees(resultRadians);

                                if (inside.Length < 9)
                                {
                                    tempResult = Math.Round(tempResult);
                                }
                            }

                            // Appliquer opérateur principal si actif
                            if (_state.IsOperatorSet)
                            {
                                ResultatBlock.Text = _state.CurrentOperator(_state.FirstValue, tempResult).ToString();
                            }
                            else
                            {
                                ResultatBlock.Text = tempResult.ToString();
                            }
                        }
                        // Cas : constante (π ou e)
                        else if (inside == "π" || inside == "e" || inside == "π)" || inside == "e)")
                        {
                            if (_state.CurrentOperator != null)
                            {
                                ResultatBlock.Text = _state.CurrentOperator(_state.FirstValue, _state.SubResult).ToString();
                            }
                            else
                            {
                                ResultatBlock.Text = _state.SubResult.ToString();
                            }
                        }
                    }
                }
                catch
                {
                    // Ignorer erreurs
                    return;
                }
            }
            // Cas : constante utilisée
            else if (_state.IsConstantUsed)
            {
                if (_state.CurrentOperator != null)
                {
                    ResultatBlock.Text = _state.CurrentOperator(_state.FirstValue, _state.SubConstant).ToString();
                }
                else
                {
                    ResultatBlock.Text = _state.SubConstant.ToString();
                }
            }
            // Sinon afficher résultat courant
            else
            {
                ResultatBlock.Text = _state.Result.ToString();
            }

            // Mise à jour buffer
            _state.InputBuffer = resultatTexte.Text == "0" ? string.Empty : resultatTexte.Text;
        }

        /// <summary>
        /// Gestionnaire de changement de texte pour operationTexte.
        /// </summary>
        private void OperationTexte_TextChanged(object sender, TextChangedEventArgs e)
        {
            string current = operationTexte.Text;

            // Déterminer partie ajoutée
            string added = string.Empty;
            if (current.Length > _state.OperationTextPrevious.Length)
            {
                added = current.Substring(_state.OperationTextPrevious.Length);

                string lastToken = string.Empty;
                string sign = string.Empty;

                // Parser ajout
                if (!string.IsNullOrEmpty(added))
                {
                    if (ParsingHelper.IsOperator(added.Last()))
                    {
                        lastToken = ParsingHelper.GetLastNumericToken(added[..^1]);
                        sign = added.Last().ToString();
                    }
                    else
                    {
                        lastToken = ParsingHelper.GetLastNumericToken(added);
                    }
                }

                // Reformater si nombre valide
                if (ParsingHelper.TryParseDouble(lastToken, out double parsed))
                {
                    string formatted = FormattingHelper.FormatResultStrict(parsed);

                    // Désabonner temporairement
                    operationTexte.TextChanged -= OperationTexte_TextChanged;

                    // Réécrire
                    operationTexte.Text = _state.OperationTextPrevious + formatted + sign;

                    // Réabonner
                    operationTexte.TextChanged += OperationTexte_TextChanged;
                }
            }

            // Mise à jour mémoire tampon
            _state.OperationTextPrevious = operationTexte.Text;
        }

        /// <summary>
        /// Gestionnaire de changement de texte pour ResultatBlock.
        /// </summary>
        private void ResultatBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Empêcher récursion
            ResultatBlock.TextChanged -= ResultatBlock_TextChanged;

            string entrance = ResultatBlock.Text;
            string withoutSeparators = entrance.Replace(",", "").Replace(".", "");

            // Si trop long
            if (withoutSeparators.Length > CalculatorConstants.MAX_DISPLAY_LENGTH)
            {
                if (ParsingHelper.TryParseDouble(entrance, out double parsed))
                {
                    // Notation scientifique si nécessaire
                    if (Math.Abs(parsed) > CalculatorConstants.SCIENTIFIC_NOTATION_THRESHOLD_HIGH ||
                        (Math.Abs(parsed) > 0 && Math.Abs(parsed) < CalculatorConstants.SCIENTIFIC_NOTATION_THRESHOLD_LOW))
                    {
                        int pow = (int)Math.Floor(Math.Log10(Math.Abs(parsed)));
                        string formatted = Math.Round((parsed / Math.Pow(10, pow)), 3).ToString() + "E" + pow;
                        ResultatBlock.Text = formatted;
                    }
                    else
                    {
                        // Couper à 15 caractères
                        ResultatBlock.Text = entrance.Substring(0, CalculatorConstants.MAX_DISPLAY_LENGTH);
                    }
                }
            }

            // Corriger "-0"
            if (ResultatBlock.Text == "-0")
            {
                ResultatBlock.Text = "0";
            }

            // Réattacher événement
            ResultatBlock.TextChanged += ResultatBlock_TextChanged;
        }

        /// <summary>
        /// Gestionnaire des touches clavier.
        /// </summary>
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // Chiffres 0-9
                case Key.D0 or Key.NumPad0:
                    zeroBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.D1 or Key.NumPad1:
                    unBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.D2 or Key.NumPad2:
                    deuxBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.D3 or Key.NumPad3:
                    troisBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.D4 or Key.NumPad4:
                    quatreBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.D5 or Key.NumPad5:
                    cinqBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.D6 or Key.NumPad6:
                    sixBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.D7 or Key.NumPad7:
                    septBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.D8 when Keyboard.Modifiers != ModifierKeys.Shift:
                case Key.NumPad8:
                    huitBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case Key.D9 or Key.NumPad9:
                    neufBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;

                // Point décimal
                case Key.OemPeriod or Key.Decimal:
                    pointBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;

                // Opérateurs
                case Key.OemMinus or Key.Subtract when Keyboard.Modifiers != ModifierKeys.Shift:
                    soustractionBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;

                case Key.OemPlus or Key.Add when Keyboard.Modifiers == ModifierKeys.Shift:
                    additionBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;

                case Key.D8 or Key.Multiply when Keyboard.Modifiers == ModifierKeys.Shift:
                    multiplicationBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;

                case Key.Oem2 or Key.Divide:
                    divisionBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;

                // Touches spéciales
                case Key.Back:
                    effacerDernierBoutton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;

                case Key.Delete:
                    effacerEntreeBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;

                case Key.E when Keyboard.Modifiers != ModifierKeys.Shift:
                    expBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;

                case Key.Enter:
                    egalBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;

                case Key.Escape:
                    effacerToutBouton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
            }
        }

    }
}