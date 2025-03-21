using APIApplication.Model;

namespace APIApplication.Exception
{
    public class ValidateException : System.Exception
    {
        public ValidateException(String field, Object value) : base($"Giá trị không phù hợp: {value} của trường {field}")
        {
        }
    }
}
