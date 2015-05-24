using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace MathCommon
{
    public class NormalFunctionArgumentsTable
    {
        private double[] _arguments, _values;

        public double this[double index]
        {
            get
            {
                if (index > 1)
                    throw new ArgumentOutOfRangeException("index", "Variance could not be greater than 1");
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index", "Variance could not be lower than 0");
                if (index < _arguments[0])
                    return _values[0];
                for (var i = 0; i < _arguments.Length - 1; i++)
                {
                    if (index >= _arguments[i] && index <= _arguments[i + 1])
                    {
                        return _values[i] +
                               (index - _arguments[i])/(_arguments[i + 1] - _arguments[i])*(_values[i + 1] - _values[i]);
                    }
                }
                return _values[_values.Length - 1];
            }
        }

        public NormalFunctionArgumentsTable()
        {
            var relativePath = ConfigurationManager.AppSettings["NormalTable"];
            var path = Directory.GetCurrentDirectory() + relativePath;
            var data = new List<string>();
            using (var file = File.OpenRead(path))
            {
                using (var stream = new StreamReader(file))
                {
                    while (!stream.EndOfStream)
                    {
                        data.Add(stream.ReadLine());
                    }
                }
            }
            var arguments = new List<double>();
            var values = new List<double>();
            foreach (
                var c in
                    data.Select(s => s.Split(';').Where(str => !string.IsNullOrWhiteSpace(str)).ToArray())
                        .Where(c => c.Length == 2))
            {
                arguments.Add(double.Parse(c[0]));
                values.Add(double.Parse(c[1]));
            }
            _arguments = arguments.ToArray();
            _values = values.ToArray();
        }
    }
}
