using System;
using System.Diagnostics;

namespace Calculator
{
    /// <summary>
    /// Enum mit den vier Grundrechenarten
    /// </summary>
    enum Operator { Add, Sub, Mult, Div }

    /// <summary>
    /// Einfacher Rechner der zwei Operanden und einen Operator speichert und den Term auswertet.
    /// </summary>
    class CalculatorLogic
    {
        // Rückgabewert der Berechnung zum Anzeigen eines (Benutzer-)Fehlers.
        public static readonly Double ERROR = Double.NaN;
        // Details zu einem aufgetreten Fehler (z.B. Teilen durch 0)
        internal String ErrorMessage { get; private set; } = String.Empty;
        // linker Operand
        public double? OperandLeft { get; internal set; } = null;
        // rechter Operand
        public double? OperandRight { get; internal set; } = null;
        // zweiwertiger Operator (+, -, *, /)
        internal Operator? BinaryOperator { get; set; } = null;

        /// <summary>
        /// Auswertung des Terms. Bei Fehlern (Teilen durch 0) wird die Konstante ERROR (NaN) zurüggegeben und
        /// zusätzlich ein Hinweis in ErrorMessage hinterlegt.
        /// Der rechte Operand muss gesetzt sein!
        /// </summary>
        /// <returns>Ergebnis als Double, möglicherweis ERROR (NaN) bei Fehlern.</returns>
        internal Double Evaluate()
        {
            Debug.Assert(OperandRight != null, "Kein rechter Operand vorhanden!");
            if (OperandLeft == null)
            {
                return OperandRight.Value;
            }
            Double result = 0.0;
            switch (BinaryOperator)
            {
                case Operator.Add:
                    result = OperandLeft.Value + OperandRight.Value;
                    break;
                case Operator.Sub:
                    result = OperandLeft.Value - OperandRight.Value;
                    break;
                case Operator.Mult:
                    result = OperandLeft.Value * OperandRight.Value;
                    break;
                case Operator.Div:
                    if (OperandRight.Value == 0)
                    {
                        result = ERROR;
                        ErrorMessage = "Durch 0 darf man nicht teilen!";
                    }
                    else
                    {
                        result = OperandLeft.Value / OperandRight.Value;
                    }
                    break;
                case null:
                    result = OperandRight.Value;
                    break;
                default:
                    Debug.Assert(false, "Ungültiger Operator!");
                    break;
            }
            return result;
        }

        /// <summary>
        /// Setzt den Rechner zurück.
        /// </summary>
        internal void Clear()
        {
            OperandLeft = null;
            OperandRight = null;
            BinaryOperator = null;
        }
    }
}
