using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Virtuplex
{
    public class Helper
    {

        private Dictionary<string, Func<int, int, int>> _operations = new Dictionary<string, Func<int, int, int>>
        {
            {"*",(a1, a2) => a1 * a2 },
            {"/",(a1, a2) => a1 / a2 },
            {"+",(a1, a2) => a1 + a2},
            {"-",(a1, a2) => a1 - a2 }
        };

        public async Task<string> Evaluate(string lineInput)
        {
            string message = "";
            if (lineInput.Any(c => !Validate(c, out message)))
            {
                return message;
            }
            //assuming we always have spaces between elements since no paranthesis are involved 2 + -3 and not 2+(-3) , otherwise 2+-3 is problematic on parse
            List<string> input = lineInput.Split(' ').ToList();

            try
            {
                //await Task.Delay(1000); //test with delay
                var CounterFlag = 0;
                //operators have priority , we start with highest and finish with lowest
                foreach (var op in PrecedenceChar())
                {
                    //linear search which shrinks since list gets smaller and we don't have to start from beggining for that operator
                    while (CounterFlag < input.Count)
                    {
                        if (input[CounterFlag] == op)
                        {
                            //replace left of operatot by result
                            input[CounterFlag - 1] = _operations[op](int.Parse(input[CounterFlag - 1]), int.Parse(input[CounterFlag + 1])).ToString();
                            //remove both operator and what used to be right of operator
                            input.RemoveAt(CounterFlag);
                            input.RemoveAt(CounterFlag);
                        }
                        else
                            CounterFlag++;
                    }
                    CounterFlag = 0;
                }
                message = input[0];

                return message;
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        bool Validate(char c, out string message)
        {
            if (_operators.Contains(c))
            {
                message = "valid";
                return true;
            }
            if (char.IsLetter(c) || char.IsPunctuation(c))
            {
                message = $"Error - Invalid character: {c}";
                return false;
            }
            message = "valid";
            return true;
        }

        private List<char> _operators = new List<char> { '*', '/', '+', '-' };
        static IEnumerable<string> PrecedenceChar()
        {
            yield return "*";
            yield return "/";
            yield return "+";
            yield return "-";
        }
    }
}
