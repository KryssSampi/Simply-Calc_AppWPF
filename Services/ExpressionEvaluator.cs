using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simply_Calc_AppWPF.core;

namespace Simply_Calc_AppWPF.Services
{

    /// <summary>
    /// Type de token reconnu lors du parsing.
    /// </summary>
    public enum TokenType
    {
        Number,
        Operator,
        Function,
        OpenParenthesis,
        CloseParenthesis,
        Constant
    }

    /// <summary>
    /// Représente un token (unité lexicale) dans une expression.
    /// </summary>
    public record Token
    {
        public TokenType Type { get; init; }
        public string Value { get; init; }
        public double? NumericValue { get; init; }

        public Token(TokenType type, string value, double? numericValue = null)
        {
            Type = type;
            Value = value;
            NumericValue = numericValue;
        }

        public override string ToString() => $"{Type}: {Value}";
    }

    /// <summary>
    /// Service d'évaluation d'expressions mathématiques.
    /// Convertit une chaîne comme "5+3×2" en résultat numérique.
    /// </summary>
    public class ExpressionEvaluator
    {
        private readonly CalculatorEngine _engine;

        public ExpressionEvaluator()
        {
            _engine = new CalculatorEngine();
        }

        // ==================== ÉVALUATION PRINCIPALE ====================

        /// <summary>
        /// Évalue une expression mathématique complète.
        /// </summary>
        /// <param name="expression">Expression à évaluer (ex: "5+3×sin(45)")</param>
        /// <returns>Résultat du calcul</returns>
        /// <exception cref="ArgumentException">Si l'expression est invalide</exception>
        public double Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Expression vide", nameof(expression));

            try
            {
                // 1. Tokenization
                var tokens = Tokenize(expression);

                // 2. Conversion en notation polonaise inverse (RPN)
                var rpn = ConvertToRPN(tokens);

                // 3. Évaluation du RPN
                return EvaluateRPN(rpn);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Erreur d'évaluation: {ex.Message}", nameof(expression), ex);
            }
        }

        // ==================== TOKENIZATION ====================

        /// <summary>
        /// Découpe une expression en tokens.
        /// </summary>
        /// <param name="expression">Expression source</param>
        /// <returns>Liste de tokens</returns>
        private List<Token> Tokenize(string expression)
        {
            var tokens = new List<Token>();
            int i = 0;

            while (i < expression.Length)
            {
                char c = expression[i];

                // Espaces - ignorer
                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                // Nombres
                if (char.IsDigit(c) || c == '.')
                {
                    var number = ExtractNumber(expression, ref i);
                    tokens.Add(new Token(TokenType.Number, number.ToString(), number));
                    continue;
                }

                // Opérateurs
                if (IsOperatorChar(c))
                {
                    tokens.Add(new Token(TokenType.Operator, c.ToString()));
                    i++;
                    continue;
                }

                // Parenthèses
                if (c == '(')
                {
                    tokens.Add(new Token(TokenType.OpenParenthesis, "("));
                    i++;
                    continue;
                }

                if (c == ')')
                {
                    tokens.Add(new Token(TokenType.CloseParenthesis, ")"));
                    i++;
                    continue;
                }

                // Fonctions et constantes
                if (char.IsLetter(c) || c == 'π')
                {
                    var word = ExtractWord(expression, ref i);

                    if (IsConstant(word))
                    {
                        double constantValue = GetConstantValue(word);
                        tokens.Add(new Token(TokenType.Constant, word, constantValue));
                    }
                    else if (IsFunction(word))
                    {
                        tokens.Add(new Token(TokenType.Function, word));
                    }
                    else
                    {
                        throw new ArgumentException($"Symbole inconnu: {word}");
                    }
                    continue;
                }

                throw new ArgumentException($"Caractère invalide: {c}");
            }

            return tokens;
        }

        /// <summary>
        /// Extrait un nombre (entier ou décimal) depuis une position donnée.
        /// </summary>
        private double ExtractNumber(string expression, ref int index)
        {
            var sb = new StringBuilder();
            bool hasDecimalPoint = false;

            while (index < expression.Length)
            {
                char c = expression[index];

                if (char.IsDigit(c))
                {
                    sb.Append(c);
                    index++;
                }
                else if ((c == '.' || c == ',') && !hasDecimalPoint)
                {
                    sb.Append('.');
                    hasDecimalPoint = true;
                    index++;
                }
                else
                {
                    break;
                }
            }

            if (double.TryParse(sb.ToString(), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            throw new ArgumentException($"Nombre invalide: {sb}");
        }

        /// <summary>
        /// Extrait un mot (fonction ou constante) depuis une position donnée.
        /// </summary>
        private string ExtractWord(string expression, ref int index)
        {
            var sb = new StringBuilder();

            while (index < expression.Length)
            {
                char c = expression[index];

                if (char.IsLetter(c) || c == 'π' || c == 'e')
                {
                    sb.Append(c);
                    index++;
                }
                else
                {
                    break;
                }
            }

            return sb.ToString();
        }

        // ==================== CONVERSION RPN (Shunting Yard) ====================

        /// <summary>
        /// Convertit une liste de tokens en notation polonaise inverse (RPN).
        /// Utilise l'algorithme Shunting Yard de Dijkstra.
        /// </summary>
        private List<Token> ConvertToRPN(List<Token> tokens)
        {
            var output = new List<Token>();
            var operatorStack = new Stack<Token>();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                    case TokenType.Constant:
                        output.Add(token);
                        break;

                    case TokenType.Function:
                        operatorStack.Push(token);
                        break;

                    case TokenType.Operator:
                        while (operatorStack.Count > 0 &&
                               operatorStack.Peek().Type == TokenType.Operator &&
                               GetPrecedence(operatorStack.Peek().Value) >= GetPrecedence(token.Value))
                        {
                            output.Add(operatorStack.Pop());
                        }
                        operatorStack.Push(token);
                        break;

                    case TokenType.OpenParenthesis:
                        operatorStack.Push(token);
                        break;

                    case TokenType.CloseParenthesis:
                        while (operatorStack.Count > 0 && operatorStack.Peek().Type != TokenType.OpenParenthesis)
                        {
                            output.Add(operatorStack.Pop());
                        }

                        if (operatorStack.Count == 0)
                            throw new ArgumentException("Parenthèses non équilibrées");

                        operatorStack.Pop(); // Retirer '('

                        // Si fonction avant la parenthèse
                        if (operatorStack.Count > 0 && operatorStack.Peek().Type == TokenType.Function)
                        {
                            output.Add(operatorStack.Pop());
                        }
                        break;
                }
            }

            // Vider la pile d'opérateurs
            while (operatorStack.Count > 0)
            {
                var op = operatorStack.Pop();
                if (op.Type == TokenType.OpenParenthesis)
                    throw new ArgumentException("Parenthèses non équilibrées");

                output.Add(op);
            }

            return output;
        }

        // ==================== ÉVALUATION RPN ====================

        /// <summary>
        /// Évalue une expression en notation polonaise inverse.
        /// </summary>
        private double EvaluateRPN(List<Token> rpnTokens)
        {
            var stack = new Stack<double>();

            foreach (var token in rpnTokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                    case TokenType.Constant:
                        if (token.NumericValue.HasValue)
                            stack.Push(token.NumericValue.Value);
                        else
                            throw new ArgumentException($"Valeur numérique manquante pour: {token.Value}");
                        break;

                    case TokenType.Operator:
                        if (stack.Count < 2)
                            throw new ArgumentException($"Opérandes insuffisants pour: {token.Value}");

                        double b = stack.Pop();
                        double a = stack.Pop();
                        double result = _engine.Calculate(a, b, token.Value);
                        stack.Push(result);
                        break;

                    case TokenType.Function:
                        if (stack.Count < 1)
                            throw new ArgumentException($"Opérande manquant pour: {token.Value}");

                        double value = stack.Pop();

                        // Conversion degrés → radians pour fonctions trigo
                        if (IsTrigonometricFunction(token.Value))
                        {
                            value = value * CalculatorConstants.DEGREES_TO_RADIANS_FACTOR;
                        }

                        double funcResult = _engine.ApplyFunction(token.Value, value);

                        // Conversion radians → degrés pour fonctions inverses
                        if (IsInverseTrigonometricFunction(token.Value))
                        {
                            funcResult = funcResult * CalculatorConstants.RADIANS_TO_DEGREES_FACTOR;
                        }

                        stack.Push(funcResult);
                        break;
                }
            }

            if (stack.Count != 1)
                throw new ArgumentException("Expression invalide");

            return stack.Pop();
        }

        // ==================== HELPERS ====================

        /// <summary>
        /// Vérifie si un caractère est un opérateur.
        /// </summary>
        private bool IsOperatorChar(char c)
        {
            return c is '+' or '-' or '×' or '÷' or '*' or '/';
        }

        /// <summary>
        /// Vérifie si un mot est une fonction connue.
        /// </summary>
        private bool IsFunction(string word)
        {
            return word is "sin" or "cos" or "tan" or "arcsin" or "arccos" or "arctan";
        }

        /// <summary>
        /// Vérifie si un mot est une constante.
        /// </summary>
        private bool IsConstant(string word)
        {
            return word is "π" or "e" or "pi";
        }

        /// <summary>
        /// Obtient la valeur numérique d'une constante.
        /// </summary>
        private double GetConstantValue(string constant)
        {
            return constant switch
            {
                "π" or "pi" => CalculatorConstants.PI,
                "e" => CalculatorConstants.E,
                _ => throw new ArgumentException($"Constante inconnue: {constant}")
            };
        }

        /// <summary>
        /// Obtient la précédence d'un opérateur (plus haute = prioritaire).
        /// </summary>
        private int GetPrecedence(string operatorSymbol)
        {
            return operatorSymbol switch
            {
                "+" or "-" => 1,
                "×" or "÷" or "*" or "/" => 2,
                _ => 0
            };
        }

        /// <summary>
        /// Vérifie si c'est une fonction trigonométrique directe.
        /// </summary>
        private bool IsTrigonometricFunction(string function)
        {
            return function is "sin" or "cos" or "tan";
        }

        /// <summary>
        /// Vérifie si c'est une fonction trigonométrique inverse.
        /// </summary>
        private bool IsInverseTrigonometricFunction(string function)
        {
            return function is "arcsin" or "arccos" or "arctan";
        }

        // ==================== VALIDATION ====================

        /// <summary>
        /// Valide une expression avant évaluation.
        /// </summary>
        /// <param name="expression">Expression à valider</param>
        /// <returns>True si valide, false sinon</returns>
        public bool ValidateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return false;

            try
            {
                var tokens = Tokenize(expression);

                // Vérifier équilibre des parenthèses
                int parenthesisCount = 0;
                foreach (var token in tokens)
                {
                    if (token.Type == TokenType.OpenParenthesis)
                        parenthesisCount++;
                    else if (token.Type == TokenType.CloseParenthesis)
                        parenthesisCount--;

                    if (parenthesisCount < 0)
                        return false;
                }

                if (parenthesisCount != 0)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tente d'évaluer une expression en retournant un résultat ou une erreur.
        /// </summary>
        /// <param name="expression">Expression à évaluer</param>
        /// <param name="result">Résultat si succès</param>
        /// <param name="errorMessage">Message d'erreur si échec</param>
        /// <returns>True si évaluation réussie</returns>
        public bool TryEvaluate(string expression, out double result, out string errorMessage)
        {
            result = 0;
            errorMessage = string.Empty;

            try
            {
                result = Evaluate(expression);
                return true;
            }
            catch (DivideByZeroException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            catch (ArgumentException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Erreur: {ex.Message}";
                return false;
            }
        }
    }
}
