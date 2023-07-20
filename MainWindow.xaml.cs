using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            if (content.Length == 1 && content[0] >= '0' && content[0] <= '9')
            {
                int digit = Convert.ToInt32(content[0]);
                // TODO Calculator.EnterDigit(digit);
                display.Content += content;
            }
            else
            {
                // Verarbeitung von Buttons, die keine Ziffern sind, unabhängig vom Content (um z.B. auch Bilder zu ermöglichen):
                if (btn == btn_add)
                {

                }
                else if (btn == btn_subtract)
                {

                }
                else if (btn == btn_multiply)
                {

                }
                else if (btn == btn_divide)
                {

                }
                else if (btn == btn_evaluate)
                {
                    // TODO
                    display.Content += "=42";
                }
                else if (btn == btn_swapSign)
                {

                }
                else if (btn == btn_reciprocal)
                {

                }
                else if (btn == btn_square)
                {

                }
                else if (btn == btn_squareRoot)
                {

                }
                else if (btn == btn_percent)
                {

                }
                else if (btn == btn_decimal)
                {

                }
                else if (btn == btn_backspace)
                {
                    // TODO
                    String currentDisplay = display.Content.ToString() ?? "0";
                    display.Content = currentDisplay.Remove(currentDisplay.Length - 1);
                }
                else if (btn == btn_clearEntry)
                {
                    // TODO
                    display.Content = "0";
                }
                else if (btn == btn_clear)
                {
                    // TODO
                    display.Content = "0";
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
                switch (key)
                {
                    case Key.D0:
                    case Key.NumPad0:
                        return digit_0;
                    case Key.D1:
                    case Key.NumPad1:
                        return digit_1;
                    case Key.D2:
                    case Key.NumPad2:
                        return digit_2;
                    case Key.D3:
                    case Key.NumPad3:
                        return digit_3;
                    case Key.D4:
                    case Key.NumPad4:
                        return digit_4;
                    case Key.D5:
                    case Key.NumPad5:
                        return digit_5;
                    case Key.D6:
                    case Key.NumPad6:
                        return digit_6;
                    case Key.D7:
                    case Key.NumPad7:
                        return digit_7;
                    case Key.D8:
                    case Key.NumPad8:
                        return digit_8;
                    case Key.D9:
                    case Key.NumPad9:
                        return digit_9;
                    case Key.Back:
                        return btn_backspace;
                    case Key.Add:
                    case Key.OemPlus:
                        return btn_add;
                    case Key.Subtract:
                    case Key.OemMinus:
                        return btn_subtract;
                    case Key.Multiply:
                        return btn_multiply;
                    case Key.Divide:
                        return btn_divide;
                    case Key.Enter:
                        return btn_evaluate;
                    case Key.Decimal:
                    case Key.OemComma:
                        return btn_decimal;
                    case Key.Delete:
                        return btn_clearEntry;
                    case Key.Escape:
                        return btn_clear;
                    // TODO...
                    default:
                        //display.Content = key.ToString();
                        return null;
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                // Zuordnung bei gedrückter Shift-Taste
                switch (key)
                {
                    case Key.D5:
                        return btn_percent;
                    case Key.OemPlus:
                        return btn_multiply;
                    case Key.D7:
                        return btn_divide;
                    // TODO...
                    default:
                        return null;
                    
                }
            }
            else
            {
                // andere Modifiziertatsten (STRG, ALT, WINDOWS) werden ignoriert
                return null;
            }
        }
    }
}
