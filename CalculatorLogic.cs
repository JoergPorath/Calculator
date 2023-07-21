using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    enum Operator { Add, Sub, Mult, Div }
    class CalculatorLogic
    {
        // Rückgabewert der Berechnung zum Anzeigen eines (Benutzer-)Fehlers.
        public static readonly Double ERROR = Double.NaN;
        // Details zu einem aufgetreten Fehler (z.B. Teilen durch 0)
        internal String ErrorMessage { get; private set; } = String.Empty;
        private double? operandLeft;
        public double? OperandLeft {
            get { return operandLeft; } 
            internal set
            {
                operandLeft = value;
            }
        }
        private double? operandRight;
        public double? OperandRight
        {
            get { return operandRight; }
            internal set
            {
                operandRight = value;
            }
        }
        internal Operator? BinaryOperator { get; set; } = null;

        public CalculatorLogic()
        {
            OperandLeft = null;
            OperandRight = null;
        }

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
                    if ( OperandRight.Value == 0 )
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

        internal void Clear()
        {
            OperandLeft = null;
            OperandRight = null;
            BinaryOperator = null;
        }
    }
}
