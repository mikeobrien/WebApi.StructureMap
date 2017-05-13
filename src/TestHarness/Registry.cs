namespace TestHarness
{
    public class Registry : StructureMap.Registry
    {
        public Registry()
        {
            ForSingletonOf<ISingletonDependency>().Use<SingletonDependency>();
            For<ITransientDependency>().Use<TransientDependency>();
        }
    }
}