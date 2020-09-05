using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

//todo (?)Change the border colour around focused textbox
//todo Change top-right icon

namespace NumeralSystemConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Makes window unable to resize
            this.ResizeMode = ResizeMode.NoResize;

            // Listens for key pressed, for ENTER support
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        // Checks if ENTER was pressed and tries to convert entered number
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // When ENTER pressed
            if (e.Key == Key.Enter)
            {
                // Reset result box
                lowerNumberBox.Text = "";

                // Replace comma with dot and slice zeros at the end
                upperNumberBox.Text = upperNumberBox.Text.Replace('.', ',');
                if (upperNumberBox.Text.Contains(','))
                {
                    upperNumberBox.Text = upperNumberBox.Text.TrimEnd('0');
                    // If comma at the end, delete it
                    if (upperNumberBox.Text[upperNumberBox.Text.Length - 1] == ',')
                    {
                        upperNumberBox.Text = upperNumberBox.Text.Substring(0, upperNumberBox.Text.Length - 1);
                    }
                }

                // Slice out zeros at the beginning of inputs (but it can be fraction 0,5 etc.)
                upperSystemBox.Text = upperSystemBox.Text.TrimStart('0');
                lowerSystemBox.Text = lowerSystemBox.Text.TrimStart('0');
                upperNumberBox.Text = upperNumberBox.Text.TrimStart('0');
                if (upperNumberBox.Text[0] == ',')
                {
                    upperNumberBox.Text = upperNumberBox.Text.Insert(0, "0");
                }

                // Checks if input(numeral systems and original number) are valid
                if (!checkInput(upperNumberBox.Text, upperSystemBox.Text, lowerSystemBox.Text))
                {
                    return;
                }

                // Converts after making sure values are correct
                string conversionResult = Number.Convert(new Number(upperNumberBox.Text, Int32.Parse(upperSystemBox.Text)), Int32.Parse(lowerSystemBox.Text));

                // Displays result in proper text box
                lowerNumberBox.Text = conversionResult;

                displayInfo("Conversion successful");
            }
        }

        // Displays info e.g. about errors
        public async void displayInfo(string info, bool error = false)
        {
            // Makes sure another animation isn't in progress
            if (infoBox.Text != "Press ENTER to convert")
            {
                return;
            }

            int fadeSpeed = 20; // Time in ms between phases
            int infoDisplayTime = 2500; // Time in ms, how long will info be dispolayed

            // Handmade animation of fade in and out

            for (int i = 0; i < 10; i++)
            {
                infoBox.Opacity -= 0.1;
                await Task.Delay(fadeSpeed);
            }

            // Change info color to orange if it's an error
            if (error)
            {
                infoBox.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(233, 132, 0));
            }
            infoBox.Text = info;

            for (int i = 0; i < 10; i++)
            {
                infoBox.Opacity += 0.1;
                await Task.Delay(fadeSpeed);
            }

            await Task.Delay(infoDisplayTime);

            for (int i = 0; i < 10; i++)
            {
                infoBox.Opacity -= 0.1;
                await Task.Delay(fadeSpeed);
            }

            // Switching back to original values
            infoBox.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(169, 169, 169));
            infoBox.Text = "Press ENTER to convert";

            for (int i = 0; i < 10; i++)
            {
                infoBox.Opacity += 0.1;
                await Task.Delay(fadeSpeed);
            }
        }

        // Checks if input (numeral systems and original number) are valid
        public bool checkInput(string originalVal, string originalSys, string targetSys)
        {
            // Check if every value is filled
            if (originalVal == "")
            {
                this.displayInfo("Input the number you want to convert", true);
                return false;
            }
            if (originalSys == "")
            {
                this.displayInfo("Input numeral system of the original number", true);
                return false;
            }
            if (targetSys == "")
            {
                this.displayInfo("Input numeral system you want to convert the number to", true);
                return false;
            }

            // Check both original and target numreal systems
            int originalSystem, targetSystem;
            bool isNumeric = int.TryParse(originalSys, out originalSystem);
            bool isNumeric2 = int.TryParse(targetSys, out targetSystem);

            if (!isNumeric || !isNumeric2)
            {
                this.displayInfo("Invalid characters in numeral system", true);
                return false;
            }

            if (!((originalSystem >= 2 && originalSystem <= 36) && (targetSystem >= 2 && targetSystem <= 36)))
            {
                this.displayInfo("Numeral systems available: 2 - 36", true);
                return false;
            }

            // Check if number is in original numeral system
            bool dotAlready = false;

            for (int i = 0; i < originalVal.Length; i++)
            {
                bool valid = true;
                byte asciiCode = Encoding.Default.GetBytes(originalVal)[i];

                // In case of number
                if (asciiCode >= 48 && asciiCode <= 57 && originalSystem <= asciiCode - 48)
                {
                    valid = false;
                }
                // In case of upper case letter
                else if (asciiCode >= 65 && asciiCode <= 90 && originalSystem <= asciiCode - 55)
                {
                    valid = false;
                }
                // In case of lower case letter
                else if (asciiCode >= 97 && asciiCode <= 122 && originalSystem <= asciiCode - 87)
                {
                    valid = false;
                }
                // In case of comma or dot
                else if (asciiCode == 44 || asciiCode == 46)
                {
                    if (i == 0)
                    {
                        this.displayInfo("Comma/dot can't be inserted at the beginning of a number", true);
                        return false;
                    }
                    else if (dotAlready)
                    {
                        this.displayInfo("Comma/dot can't be inserted twice", true);
                        return false;
                    }
                    dotAlready = true;
                }
                // In case of other character
                else if (asciiCode < 48 || (asciiCode > 57 && asciiCode < 65) || (asciiCode > 90 && asciiCode < 97) || asciiCode > 122)
                {
                    valid = false;
                }

                if (!valid)
                {
                    this.displayInfo("This number is not valid in given numeral system", true);
                    return false;
                }
            }

            return true;
        }

        // When result box clicked, copy value to clickboard and display info about it
        private void lowerNumberBoxClicked(object sender, MouseButtonEventArgs e)
        {
            // Checks whether result is displayed
            if (lowerNumberBox.Text != "")
            {
                this.displayInfo("Result copied to clipboard");
                Clipboard.SetText(lowerNumberBox.Text);
            }
        }
    }
}