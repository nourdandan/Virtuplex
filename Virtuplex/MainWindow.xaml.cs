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
                //dont await so we don't block the wpf guid
                CalculateEvent(tbFileInput.Text, string.Concat(tbFileOutput.Text, @"\", tbFileName.Text));
                tbAlternativeInput.Text += "---- In Proccess  ---- \r\n";
            }

            //catch exceptions from the parallel async 
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    tbOutputText.Text = $"Error has occured : {Environment.NewLine} {innerEx.Message}";
                }
            }

            catch (Exception ex)
            {
                tbOutputText.Text = $"Error has occured : {Environment.NewLine} {ex.Message}";
            }
        }

        async Task CalculateEvent(string inputFile, string outputFile)
        {
            var fileReadResult = ReadFile(inputFile);
            await fileReadResult;
            //wait until reading is done ,alternative method would be yo yield result
            var res = EvaluateTasks(fileReadResult.Result.ToList());
            var sb = new StringBuilder();
            while (res.Count > 0)
            {
                //write when buffer size reached
                var size = res.Count > bufferSize ? bufferSize : res.Count;
                var batch = await Task.WhenAll(res.GetRange(0, size));
                batch.ToList().ForEach(x => sb.AppendLine(x));
                await WriteFile(outputFile, sb.ToString());
                res.RemoveRange(0, size);
                sb.Clear();
            }
        }

        //set up tasks in list 
        List<Task<string>> EvaluateTasks(IEnumerable<string> lines)
        {
            var listOfTasks = new List<Task<string>>();
            tbAlternativeInput.Text += "---- Eval Starting  ---- \r\n";
            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                    listOfTasks.Add(helper.Evaluate(line));
            }
            return listOfTasks;
        }

        async Task<string[]> ReadFile(string inputFile)
        {
            using (var stream = File.Open(inputFile, FileMode.Open))
            {
                using (var sr = new StreamReader(stream))
                {
                    tbAlternativeInput.Text += "---- File reading started ---- \r\n";
                    //await Task.Delay(1000);
                    var fileText = sr.ReadToEndAsync();
                    tbAlternativeInput.Text += "---- File reading In Processs  ---- \r\n";
                    await fileText;
                    tbAlternativeInput.Text += "---- File reading completed  ---- \r\n";
                    return fileText.Result.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                }
            }
        }

        async Task WriteFile(string outputFile, string arrayToWrite)
        {
            using (var stream = File.Open(outputFile, FileMode.Append, FileAccess.Write))
            {
                using (var sw = new StreamWriter(stream))
                {
                    tbAlternativeInput.Text += "---- File Writing started  ---- \r\n";
                    await sw.WriteAsync(arrayToWrite);
                    tbAlternativeInput.Text += "---- File Writing completed  ---- \r\n";
                }
            }
        }

    }
}
