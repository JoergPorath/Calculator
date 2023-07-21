using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Inhalt des Displays als StringBuilder
        private StringBuilder displayContent = new StringBuilder("0");
        // Unterscheidung, ob eine Zahl verändert oder eine neue Zahl eingegeben wird
        private Boolean alreadyEnteringNumber = false;
        // Ob die Zahl im Display neu ist, oder noch das Ergebnis der letzten Berechnung
        private Boolean newNumberInDisplay = true;
        // Instanz (Singleton) der Rechen-Logik
        private readonly CalculatorLogic calculator = new CalculatorLogic();

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Verarbeitung von Maus-Klick-Ereignissen auf Buttons im Rechnen-Grid.
        /// </summary>
        /// <param name="sender">Aufrufer der Methode (nicht die Ereignisquelle)</param>
        /// <param name="e">Zusatzinformationen zum Ereignis</param>
        private void Calculator_Click(object sender, RoutedEventArgs e)
        {
            // Sicherstellen, dass das Ereignis von einem Button kommt und casten
            if (e.Source is not Button btn)
            {
                return;
            }

            // Zifferneingaben über die Content-Eigenschaft des Buttons zuordnen:
            String content = btn.Content.ToString() ?? "";
            if (content.Length == 1 && char.IsDigit(content[0])) // >= '0' && content[0] <= '9')
            {
                EnterDigit(content[0]);
            }
            else
            {
                // Verarbeitung von Buttons, die keine Ziffern sind, unabhängig vom Content (um z.B. auch Bilder zu ermöglichen):
                if (btn == btn_add)
                {
                    SetBinaryOperator(Operator.Add);
                }
                else if (btn == btn_subtract)
                {
                    SetBinaryOperator(Operator.Sub);
                }
                else if (btn == btn_multiply)
                {
                    SetBinaryOperator(Operator.Mult);
                }
                else if (btn == btn_divide)
                {
                    SetBinaryOperator(Operator.Div);
                }
                else if (btn == btn_evaluate)
                {
                    Evaluate();
                }
                else if (btn == btn_swapSign)
                {
                    SwapSign();
                }
                else if (btn == btn_reciprocal)
                {
                    // TODO
                }
                else if (btn == btn_square)
                {
                    // TODO
                }
                else if (btn == btn_squareRoot)
                {
                    // TODO
                }
                else if (btn == btn_percent)
                {
                    // TODO
                }
                else if (btn == btn_decimal)
                {
                    EnterDecimal();
                }
                else if (btn == btn_backspace)
                {
                    EnterBackspace();
                }
                else if (btn == btn_clearEntry)
                {
                    ClearEntry();
                }
                else if (btn == btn_clear)
                {
                    calculator.Clear();
                    ClearEntry();
                }
            }
        }

        /// <summary>
        /// Klappt bei ausreichend großem Fenster Verlauf bzw. Speicher auf der rechten Seite auf.
        /// </summary>
        /// <param name="sender">Anwendungsfenster</param>
        /// <param name="e">Zusatzinformationen zum Ereignis</param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AppWindow.Width < 577)
            {
                sideBoard.Width = new GridLength(0.0, GridUnitType.Star);
                btn_memList.Visibility = Visibility.Visible;
                btn_history.Visibility = Visibility.Visible;
            } else
            {
                sideBoard.Width = new GridLength(11.0, GridUnitType.Star);
                btn_memList.Visibility = Visibility.Collapsed;
                btn_history.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Verarbeitung von Tastatur-Eingaben.
        /// Zuordbare Buttons werden farblich markiert und ihre ClickEvents gefeurt.
        /// </summary>
        /// <param name="sender">Anwendungsfenster</param>
        /// <param name="e">Zusatzinformationen zum Ereignis</param>
        private void AppWindow_KeyDown(object sender, KeyEventArgs e)
        {
            Button? btn = SelectButton(e);
            if (btn != null)
            {
                btn.Background = Brushes.LightBlue;
                btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, btn));
            }
        }

        /// <summary>
        /// Loslassen von Tasten hebt die farbliche Markierung wieder auf.
        /// </summary>
        /// <param name="sender">Anwendungsfenster</param>
        /// <param name="e">Zusatzinformationen zum Ereignis</param>
        private void AppWindow_KeyUp(object sender, KeyEventArgs e)
        {
            Button? btn = SelectButton(e);
            if (btn != null)
            {
                btn.Background = Brushes.GhostWhite;
            }
        }

        /// <summary>
        /// Zuordnung von Tasten zu Buttons.
        /// </summary>
        /// <param name="e">Tastaturereignis</param>
        /// <returns>entsprechender Button oder null, wenn keine Zuordnung vorhanden ist</returns>
        private Button? SelectButton(KeyEventArgs e)
        {
            Key key = e.Key;
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                return key switch
                {
                    Key.D0 or Key.NumPad0 => digit_0,
                    Key.D1 or Key.NumPad1 => digit_1,
                    Key.D2 or Key.NumPad2 => digit_2,
                    Key.D3 or Key.NumPad3 => digit_3,
                    Key.D4 or Key.NumPad4 => digit_4,
                    Key.D5 or Key.NumPad5 => digit_5,
                    Key.D6 or Key.NumPad6 => digit_6,
                    Key.D7 or Key.NumPad7 => digit_7,
                    Key.D8 or Key.NumPad8 => digit_8,
                    Key.D9 or Key.NumPad9 => digit_9,
                    Key.Back => btn_backspace,
                    Key.Add or Key.OemPlus => btn_add,
                    Key.Subtract or Key.OemMinus => btn_subtract,
                    Key.Multiply => btn_multiply,
                    Key.Divide => btn_divide,
                    Key.Enter => btn_evaluate,
                    Key.Decimal or Key.OemComma => btn_decimal,
                    Key.Delete => btn_clearEntry,
                    Key.Escape => btn_clear,
                    _ => null,
                };
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                // Zuordnung bei gedrückter Shift-Taste
                return key switch
                {
                    Key.D5 => btn_percent,
                    Key.OemPlus => btn_multiply,
                    Key.D7 => btn_divide,
                    _ => null,
                };
            }
            else
            {
                // andere Modifiziertatsten (STRG, ALT, WINDOWS) werden ignoriert
                return null;
            }
        }

        /// <summary>
        /// Verarbeitung von Ziffer-Eingaben.
        /// </summary>
        /// <param name="digit">Ziffer als Zeichen (Muss einer Ziffer von 0 - 9 entsprechen!)</param>
        private void EnterDigit(char digit)
        {
            Debug.Assert(char.IsDigit(digit), digit + " ist keine Ziffer!");

            if (alreadyEnteringNumber)
            {
                if (displayContent.ToString() == "0")
                {
                    // Führende 0 nur vor einem Komma, sonst überschreiben
                    displayContent.Clear().Append(digit);
                } else
                {
                    displayContent.Append(digit);
                }
            }
            else
            {
                StartNewEntry();
                displayContent.Clear().Append(digit);
            }
            display.Content = displayContent.ToString();
        }

        /// <summary>
        /// Setzen der Dezimalstelle.
        /// Dies ist nur einmal in der Zahl möglich.
        /// Das Setzen eines Kommas an erster Stelle wird als "0," interpretiert.
        /// </summary>
        private void EnterDecimal()
        {
            if (alreadyEnteringNumber)
            {
                // kein doppeltes Komma
                if (!displayContent.ToString().Contains(","))
                {
                    displayContent.Append(',');
                }
            }
            else
            {
                StartNewEntry();
                displayContent.Clear().Append("0,");
            }
            display.Content = displayContent.ToString();
        }

        /// <summary>
        /// Beginnt die Eingabe einer neuen Zahl (Durch eingabe einer Ziffer, oder ",").
        /// </summary>
        private void StartNewEntry()
        {
            if (calculator.OperandRight != null)
            {
                // Letzter Term mit "=" abgeschlossen -> Neue Rechnung.
                calculator.OperandLeft = null;
            }
            alreadyEnteringNumber = true;
            newNumberInDisplay = true;
            calculator.OperandRight = null;
        }

        /// <summary>
        /// Löschen der zuletzt eingegebenen Ziffer oder des Dezimaltrennzeichens.
        /// </summary>
        private void EnterBackspace()
        {
            if (alreadyEnteringNumber && displayContent.Length > 0)
            {
                displayContent.Remove(displayContent.Length - 1, 1);
                if (displayContent.Length == 0)
                {
                    displayContent.Append("0");
                }
                display.Content = displayContent.ToString();
            }
        }

        /// <summary>
        /// Zurücksetzen des Displays.
        /// </summary>
        private void ClearEntry()
        {
            displayContent.Clear().Append('0');
            display.Content = displayContent.ToString();
            alreadyEnteringNumber = false;
            newNumberInDisplay = true;
        }

        /// <summary>
        /// Wechsel des Vorzeichens.
        /// </summary>
        private void SwapSign()
        {
            if (displayContent.ToString() != "0")
            {
                if (displayContent[0] == '-')
                {
                    displayContent.Remove(0, 1);
                }
                else
                {
                    displayContent.Insert(0, '-');
                }
                display.Content = displayContent.ToString();
            }            
        }

        /// <summary>
        /// Setzen eines zweiwertigen Operators (+, -, *, /).
        /// Führt bei einer zuvor eingegeben Zahl zur Auswertung des linken Terms.
        /// </summary>
        /// <param name="binayOperator">Operator (+, -, *, /) als Enum-Wert</param>
        private void SetBinaryOperator(Operator binayOperator)
        {
            if (newNumberInDisplay) // (ohne Änderung des Operanden wird nur der Operator verändert)
            {
                // Bisherigen Term auswerten
                double value = Convert.ToDouble(displayContent.ToString());
                alreadyEnteringNumber = false;
                newNumberInDisplay = false;
                if (calculator.OperandLeft == null)
                {
                    calculator.OperandLeft = value;
                }
                else
                {
                    calculator.OperandRight = value;
                    Calculate();
                }
            }
            calculator.BinaryOperator = binayOperator;
            calculator.OperandRight = null;
        }

        /// <summary>
        /// Auswertung des Terms (durch "=").
        /// </summary>
        private void Evaluate()
        {
            if (calculator.OperandRight == null)
            {
                double value = Convert.ToDouble(displayContent.ToString());
                calculator.OperandRight = value;
            }
            Calculate();
            alreadyEnteringNumber = false;
            newNumberInDisplay = false;
        }

        /// <summary>
        /// Berechnung durch die CalculatorLogic-Instanz und ggf. Anziege von Fehlern (z.B. Teilen durch 0).
        /// </summary>
        private void Calculate()
        {
            Double result = calculator.Evaluate();

            if (result.Equals(CalculatorLogic.ERROR))
            {
                display.Content = "ERROR";
                termDisplay.Content = calculator.ErrorMessage;
                // TODO Anzeige muss wieder aufeghoben werden...
            }
            else
            {
                // TODO schöner Lösung zum Formatieren finden!
                if (Math.Abs(result) > 999999999999 || Math.Abs(result) < 0.0000000001)
                {
                    displayContent.Clear().Append(result.ToString("G8"));
                }
                else
                {
                    displayContent.Clear().Append(result.ToString("G12"));
                }
                display.Content = displayContent.ToString();
                // TODO Term anzeigen!
                calculator.OperandLeft = result;
            }
        }
    }
}
