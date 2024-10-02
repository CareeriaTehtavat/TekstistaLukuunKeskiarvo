using HelloWorld; // Ensure this is the correct namespace for the Program class
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HelloWorldTest
{
    public class UnitTest1
    {


        //Harjoitus - PeruslaskutNumeromuuttujilla
        [Fact]
        [Trait("TestGroup", "TekstistaLukuunKeskiarvo")]
        public void TekstistaLukuunKeskiarvo()
        {
            // Arrange
            using var sw = new StringWriter();
            Console.SetOut(sw);


            // Set a timeout of 30 seconds for the test execution
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                // Act
                Task task = Task.Run(() =>
                {
                    // Run the program
                    HelloWorld.Program.Main(new string[0]);
                }, cancellationTokenSource.Token);

                task.Wait(cancellationTokenSource.Token);  // Wait for the task to complete or timeout

                // Get the output that was written to the console
                var result = sw.ToString().TrimEnd(); // Trim only the end of the string
                var resultLines = result.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                // Define a regex pattern to match the structure, ignoring specific values
                string teksti1 = "10,4";
                string teksti2 = "8,2";
                string teksti3 = "5,1";
                // Parse inputs as float
                float floatLuku1 = float.Parse(teksti1);
                float floatLuku2 = float.Parse(teksti2);
                float floatLuku3 = float.Parse(teksti3);
                float floatKeskiarvo = (floatLuku1 + floatLuku2 + floatLuku3) / 3;

                // Parse inputs as double
                double doubleLuku1 = Convert.ToDouble(teksti1);
                double doubleLuku2 = Convert.ToDouble(teksti2);
                double doubleLuku3 = Convert.ToDouble(teksti3);
                double doubleKeskiarvo = (doubleLuku1 + doubleLuku2 + doubleLuku3) / 3;

                // Expected text for both float and double results
                string expectedFloatText = $"Tekstina syotettyjen lukujen {teksti1} {teksti2} ja {teksti3} keskiarvo on {floatKeskiarvo}";
                string expectedDoubleText = $"Tekstina syotettyjen lukujen {teksti1} {teksti2} ja {teksti3} keskiarvo on {doubleKeskiarvo}";

                // Assert: Check if the result matches either the float or double expected output
                bool matchesFloat = LineContainsIgnoreSpaces(expectedFloatText, resultLines[0]);
                bool matchesDouble = LineContainsIgnoreSpaces(expectedDoubleText, resultLines[0]);

                Assert.True(matchesFloat || matchesDouble,
                    "The result does not match either float or double calculated value. " +
                    $"Actual: {resultLines[0]} / Expected float: {expectedFloatText} / Expected double: {expectedDoubleText}");
            }
            catch (OperationCanceledException)
            {
                Assert.True(false, "The operation was canceled due to timeout.");
            }
            catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
            {
                Assert.True(false, "The operation was canceled due to timeout.");
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
        private bool LineContainsIgnoreSpaces(string expectedText, string line)
        {
            // Remove all whitespace and convert to lowercase
            string normalizedLine = Regex.Replace(line, @"[\s.,]+", "").ToLower();
            string normalizedExpectedText = Regex.Replace(expectedText, @"[\s.,]+", "").ToLower();

            // Create a regex pattern to allow any character for "ä", "ö", "a", and "o"
            string pattern = Regex.Escape(normalizedExpectedText)
                                  .Replace("ö", ".")  // Allow any character for "ö"
                                  .Replace("ä", ".")  // Allow any character for "ä"
                                  .Replace("a", ".")  // Allow any character for "a"
                                  .Replace("o", ".");  // Allow any character for "o"

            // Check if the line matches the pattern, ignoring case
            return Regex.IsMatch(normalizedLine, pattern, RegexOptions.IgnoreCase);
        }


        private int CountWords(string line)
        {
            return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private bool CompareLines(string[] actualLines, string[] expectedLines)
        {
            if (actualLines.Length != expectedLines.Length)
            {
                return false;
            }

            for (int i = 0; i < actualLines.Length; i++)
            {
                if (actualLines[i] != expectedLines[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}


