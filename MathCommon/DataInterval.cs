namespace MathCommon
{
    public class DataInterval
    {
        public double[] SampleData;

        public double Left { get; set; }

        public double Right { get; set; }

        public double Length
        {
            get { return Right - Left; }
        }

        public int Count
        {
            get { return SampleData.Length; }
        }

        public int InitialSampleSize { get; set; }

        public double Probability
        {
            get { return ((double) Count)/InitialSampleSize; }
        }

        public double ProbabilityDensity
        {
            get { return Probability/Length; }
        }
    }
}
