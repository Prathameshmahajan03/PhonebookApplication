namespace PhonebookApplication.Exceptions
{
    public class DuplicatePhoneException : Exception
    {
        public DuplicatePhoneException(string message)
            : base(message)
        {
        }
    }
}