using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace MathCommon
{
    public class StudentFunctionArgumentTable
    {
        private double[] _arguments1;
        private int _argumnets2Count;
        private double[][] _values;

        public int MaxNumberOfFreedoms { get { return _argumnets2Count; } }

        public double this[double index1, int index2]
        {
            get
            {
                if (index1 > 1)
                    throw new ArgumentOutOfRangeException("index1", "Variance could not be greater than 1");
                if (index1 < 0)
                    throw new ArgumentOutOfRangeException("index1", "Variance could not be lower than 0");
                if (index2 < 1)
                    throw new ArgumentOutOfRangeException("index2",
                        "Number of degrees of freedom could not be lower than 1");
                if (index2 > _argumnets2Count)
                    throw new ArgumentOutOfRangeException("index2",
                        "Number of degrees of freedom is too big consider using table for normal function");
                if (index1 < _arguments1[0])
                    return _values[0][index2 - 1];
                for (var i = 0; i < _arguments1.Length - 1; i++)
                {
                    if (index1 >= _arguments1[i] && index1 <= _arguments1[i + 1])
                    {
                        return _values[i][index2 - 1] +
                               (index1 - _arguments1[i])/(_arguments1[i + 1] - _arguments1[i])*
                               (_values[i + 1][index2 - 1] - _values[i][index2 - 1]);
                    }
                }
                return _values[_values.Length - 1][index2 - 1];
            }
        }

        public StudentFunctionArgumentTable()
        {
            var relativePath = ConfigurationManager.AppSettings["StudentTable"];
            var path = Directory.GetCurrentDirectory() + relativePath;
            var data = new List<string>();
            string header;
            using (var file = File.OpenRead(path))
            {
                using (var stream = new StreamReader(file))
                {
                    header = stream.ReadLine();
                    while (!stream.EndOfStream)
                    {
                        data.Add(stream.ReadLine());
                    }
                }
            }
            var h = header.Split(';').Where(str =>!string.IsNullOrWhiteSpace(str)).ToArray();
            _argumnets2Count = h.Length - 1;
            var arguments = new List<double>();
            var values = new List<double[]>();
            foreach (
                var c in
                    data.Select(s => s.Split(';').Where(str => !string.IsNullOrWhiteSpace(str)).ToArray())
                        .Where(c => c.Length == _argumnets2Count + 1))
            {
                arguments.Add(double.Parse(c[0]));
                var temp = new double[_argumnets2Count];
                for (var i = 0; i < _argumnets2Count; i++)
                {
                    temp[i] = double.Parse(c[i + 1]);
                }
                values.Add(temp);
            }
            _arguments1 = arguments.ToArray();
            _values = values.ToArray();
        }
    }
}
