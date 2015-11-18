using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Model.InterceptionAttribute
{
    public class TestAttribute : HandlerAttribute
    {
        public TestAttribute()
        { }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new TestCallHandler();
        }
    }
}
