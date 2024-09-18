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

                double luku1 = Convert.ToDouble(teksti1);
                double luku2 = Convert.ToDouble(teksti2);
                double luku3 = Convert.ToDouble(teksti3);

                double keskiarvo = (luku1 + luku2 + luku3) / 3;


                // Assert: Check if the result matches the expected structure
                Assert.True(LineContainsIgnoreSpaces(resultLines[0], "Tekstinä syötettyjen lukujen " + teksti1 + " " + teksti2 + " ja " + teksti3 + " keskiarvo on " + keskiarvo), "Line does not contain expected text: " + resultLines[0] + ", expected text: " + "11");
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
        private bool LineContainsIgnoreSpaces(string line, string expectedText)
        {
            // Remove all whitespace from the line and the expected text
            string normalizedLine = Regex.Replace(line, @"\s+", "");
            string normalizedExpectedText = Regex.Replace(expectedText, @"\s+", "");
            return normalizedLine.Contains(normalizedExpectedText);
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



