using Microsoft.Practices.Unity.InterceptionExtension;

namespace Model.InterceptionAttribute
{
    public class TestCallHandler : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            // Invoke the next handler in the chain
            var result = getNext().Invoke(input, getNext);

            // After invoking the method on the original target
            if (result.Exception != null)
            {
                StaticCounterHelper.ExceptionCounter++;
            }
            else
            {
                StaticCounterHelper.SuccessCounter++;
            }

            return result;
        }

        public int Order
        {
            get;
            set;
        }
    }
}
