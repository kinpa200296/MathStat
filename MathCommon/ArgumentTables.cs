namespace MathCommon
{
    public static class ArgumentTables
    {
        private static Xi2FunctionArgumentTable _xi2;
        private static StudentFunctionArgumentTable _student;
        private static NormalFunctionArgumentsTable _normal;

        public static Xi2FunctionArgumentTable Xi2
        {
            get { return _xi2 ?? (_xi2 = new Xi2FunctionArgumentTable()); }
        }

        public static StudentFunctionArgumentTable Student
        {
            get { return _student ?? (_student = new StudentFunctionArgumentTable()); }
        }

        public static NormalFunctionArgumentsTable Normal
        {
            get { return _normal ?? (_normal = new NormalFunctionArgumentsTable()); }
        }
    }
}
