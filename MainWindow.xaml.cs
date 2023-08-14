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
        private readonly StringBuilder displayContent = new StringBuilder("0");
        // Unterscheidung, ob eine Zahl verändert oder eine neue Zahl eingegeben wird
        private bool isAlreadyEnteringNumber;
        private bool IsAlreadyEnteringNumber {
            get { 
                return isAlreadyEnteringNumber;
            }
            set {
                isAlreadyEnteringNumber = value;
                btn_backspace.IsEnabled = value;
            }
        }
        // Ob die Zahl im Display neu ist, oder noch das Ergebnis der letzten Berechnung
        private bool isNewNumberInDisplay = true;
        // Ob gerade ein Fehler angezeigt wird;
        private bool isShowingError = false;
        // Instanz (Singleton) der Rechen-Logik
        private readonly CalculatorLogic calculator = new CalculatorLogic();

        public MainWindow()
        {
            InitializeComponent();
            IsAlreadyEnteringNumber = false;
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
                    termDisplay.Content = "";
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

            if (isShowingError)
            {
                ClearEntry();
            }

            if (isAlreadyEnteringNumber)
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
            UpdateDisplay();
        }

        /// <summary>
        /// Setzen der Dezimalstelle.
        /// Dies ist nur einmal in der Zahl möglich.
        /// Das Setzen eines Kommas an erster Stelle wird als "0," interpretiert.
        /// </summary>
        private void EnterDecimal()
        {
            if (isShowingError)
            {
                ClearEntry();
            }

            if (IsAlreadyEnteringNumber)
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
            UpdateDisplay();
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
            IsAlreadyEnteringNumber = true;
            isNewNumberInDisplay = true;
            calculator.OperandRight = null;
        }

        /// <summary>
        /// Löschen der zuletzt eingegebenen Ziffer oder des Dezimaltrennzeichens.
        /// </summary>
        private void EnterBackspace()
        {
            if (IsAlreadyEnteringNumber && displayContent.Length > 0)
            {
                displayContent.Remove(displayContent.Length - 1, 1);
                if (displayContent.Length == 0)
                {
                    displayContent.Append('0');
                }
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Zurücksetzen des Displays.
        /// </summary>
        private void ClearEntry()
        {
            if (isShowingError)
            {
                ShowErrorMessage(false);
                calculator.Clear();
            }
            displayContent.Clear().Append('0');
            UpdateDisplay();
            IsAlreadyEnteringNumber = false;
            isNewNumberInDisplay = true;
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
                UpdateDisplay();
            }            
        }

        /// <summary>
        /// Setzen eines zweiwertigen Operators (+, -, *, /).
        /// Führt bei einer zuvor eingegeben Zahl zur Auswertung des linken Terms.
        /// </summary>
        /// <param name="binayOperator">Operator (+, -, *, /) als Enum-Wert</param>
        private void SetBinaryOperator(Operator binayOperator)
        {
            if (isNewNumberInDisplay) // (ohne Änderung des Operanden wird nur der Operator verändert)
            {
                // Bisherigen Term auswerten
                double value = Convert.ToDouble(displayContent.ToString());
                IsAlreadyEnteringNumber = false;
                isNewNumberInDisplay = false;
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

            ShowCurrentTerm();
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
            ShowCurrentTerm(withEvaluation: true);
            Calculate();
            IsAlreadyEnteringNumber = false;
            isNewNumberInDisplay = false;
        }

        /// <summary>
        /// Berechnung durch die CalculatorLogic-Instanz und ggf. Anziege von Fehlern (z.B. Teilen durch 0).
        /// </summary>
        private void Calculate()
        {
            Double result = calculator.Evaluate();

            if (result.Equals(CalculatorLogic.ERROR))
            {
                ShowErrorMessage(true);
            }
            else
            {
                displayContent.Clear().Append(result);
                UpdateDisplay();
                calculator.OperandLeft = result;
            }
        }

        /// <summary>
        /// Aktualisiert die Anzeige und passt dabei die Schriftgröße an.
        /// </summary>
        private void UpdateDisplay()
        {
            if (displayContent.Length > 22)
            {
                display.FontSize = 24;
            }
            else if (displayContent.Length > 19)
            {
                display.FontSize = 26;
            }
            else if (displayContent.Length > 15)
            {
                display.FontSize = 30;
            }
            else if (displayContent.Length > 12)
            {
                display.FontSize = 36;
            }
            else
            {
                display.FontSize = 48;
            }
            display.Content = displayContent.ToString();
        }

        /// <summary>
        /// Zeigt eine Fehlermeldung im Display an, bzw. löscht diese und (de)aktiviert alle Buttons außer Clear und die zur Eingabe einer neuen Zahl.
        /// </summary>
        /// <param name="currentError">ob jetzt ein Fehler angezeigt werden soll</param>
        private void ShowErrorMessage(bool currentError)
        {
            if (currentError)
            {
                display.FontSize = 48;
                display.Content = "ERROR";
                termDisplay.Content = calculator.ErrorMessage;
            }
            else
            {
                termDisplay.Content = "";
            }
            // Buttons (de)aktivieren
            btn_add.IsEnabled = !currentError;
            btn_subtract.IsEnabled = !currentError;
            btn_multiply.IsEnabled = !currentError;
            btn_divide.IsEnabled = !currentError;
            btn_reciprocal.IsEnabled = !currentError;
            btn_square.IsEnabled = !currentError;
            btn_squareRoot.IsEnabled = !currentError;
            btn_swapSign.IsEnabled = !currentError;
            btn_percent.IsEnabled = !currentError;
            btn_evaluate.IsEnabled = !currentError;
            btn_backspace.IsEnabled = !currentError;

            isShowingError = currentError;
        }

        /// <summary>
        /// Zeigt den aktuellen Term an.
        /// </summary>
        /// <param name="withEvaluation">ob auch der rechte Operand und das "=" angezeigt werden sollen (default: false)</param>
        private void ShowCurrentTerm(bool withEvaluation = false)
        {
            if (isShowingError)
            {
                return;
            }

            StringBuilder term = new StringBuilder();
            if (calculator.OperandLeft != null)
            {
                term.Append(calculator.OperandLeft.ToString() + " ");

                switch (calculator.BinaryOperator)
                {
                    case Operator.Add:
                        term.Append('+');
                        break;
                    case Operator.Sub:
                        term.Append('-');
                        break;
                    case Operator.Mult:
                        term.Append('x');
                        break;
                    case Operator.Div:
                        term.Append('÷');
                        break;
                    case null:
                        break;
                    default:
                        Debug.Assert(false, "Ungültiger Operator!");
                        break;
                }
            }

            if (withEvaluation)
            {
                Debug.Assert(calculator.OperandRight is not null, "Der rechte Operand sollte bei Aufruf von ShowCurrentTerm(true) gesetzt sein!");
                term.Append(" " + calculator.OperandRight.ToString() + " =");
            }

            if (term.Length > 37)
            {
                termDisplay.FontSize = 14;
            }
            else
            {
                termDisplay.FontSize = 16;
            }
            termDisplay.Content = term.ToString();
        }
    }
}
