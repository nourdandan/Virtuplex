using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace Virtuplex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Helper helper = new Helper();
        const int bufferSize = 1000;
        Stopwatch sw = new Stopwatch();
        public MainWindow()
        {
            InitializeComponent();
        }



        public async void OnClick1(object sender, RoutedEventArgs e)
        {
            try
            {
                tbAlternativeInput.Text += "---- Starting Calculation Event  ---- \r\n";
                CalculateEvent(tbFileInput.Text, string.Concat(tbFileOutput.Text,@"\",tbFileName.Text));
                tbAlternativeInput.Text += "---- In Proccess  ---- \r\n";
            }

            catch(Exception ex)
            {
                tbOutputText.Text = $"Error has occured : {Environment.NewLine} {ex.Message}";
            }
        }

        async Task<string[]> ReadFile(string inputFile)
        {
            using(var stream = File.Open(inputFile, FileMode.Open))
            {
                using(var sr =  new StreamReader(stream))
                {
                    tbAlternativeInput.Text += "---- File reading started ---- \r\n";
                    await Task.Delay(1000);
                    var fileText =  sr.ReadToEndAsync();
                    tbAlternativeInput.Text += "---- File reading In Processs  ---- \r\n";
                    await fileText;
                    tbAlternativeInput.Text += "---- File reading completed  ---- \r\n";
                    return fileText.Result.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                }
            }
        }

        async Task WriteFile(string outputFile,List<string> arrayToWrite)
        {
            using (var stream = File.Open(outputFile, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(stream))
                {
                    tbAlternativeInput.Text += "---- File Writing started  ---- \r\n";
                    arrayToWrite.ForEach(result => sw.WriteLineAsync(result));
                    tbAlternativeInput.Text += "---- File Writing completed  ---- \r\n";
                }
            }
        }

        async Task CalculateEvent(string inputFile , string outputFile)
        {
            var sb = new StringBuilder();
            sw.Reset();
            sw.Start();
            var inputLines = await ReadFile(inputFile);
            tbAlternativeInput.Text += $"---- Read file in {sw.ElapsedMilliseconds}  ---- \r\n";
            sw.Stop();
            var count = 0;
            var bufferData = new List<string>();
            foreach ( var inputLine in  inputLines)
            {
                if (!string.IsNullOrEmpty(inputLine))
                {
                    count++;
                    tbAlternativeInput.Text += $"---- Currently Evaluating line {count}  ---- \r\n";
                    sw.Reset();
                    sw.Start();
                    var evaluationResult = helper.Evaluate(inputLine);
                    tbAlternativeInput.Text += "---- long evaluation in process  ---- \r\n";
                    if (bufferData.Count() == bufferSize)
                    {
                        await WriteFile(outputFile, bufferData);
                        bufferData.Clear();
                    }
                    var result = $"{await evaluationResult}";
                    tbAlternativeInput.Text += $"---- Finished Evaluating {count} ,  finished in {sw.ElapsedMilliseconds}  ---- \r\n";
                    tbOutputText.Text += $"{result} -line {count} \r\n";
                    bufferData.Add(result);
                }
            }
            await WriteFile(outputFile, bufferData);
            bufferData.Clear();
            tbAlternativeInput.Text += "---- Process Finished  ---- \r\n";
        }
    }
}
