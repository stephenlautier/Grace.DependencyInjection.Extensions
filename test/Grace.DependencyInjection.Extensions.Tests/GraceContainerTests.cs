using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;
using System;
using Xunit;

namespace Grace.DependencyInjection.Extensions.Tests
{
    public class GraceContainerTests : DependencyInjectionSpecificationTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            void Configuration(InjectionScopeConfiguration x) => x.Behaviors.AllowInstanceAndFactoryToReturnNull = true;

            //return new DependencyInjectionContainer(Configuration)
            return new DependencyInjectionContainer()
                .Populate(serviceCollection);
        }

        [Fact]
        public void GetServices()
        {
            var services = new ServiceCollection()
                //.AddSingleton(sp => new NamedService<StrategyIndex> { Name = "strat1", Value = new StrategyIndex() })
                //.AddSingleton(sp => new NamedService<StrategyIndex> { Name = "strat2", Value = new StrategyIndex() })
                .AddSingleton(sp => new NamedService<IHeroesIndex> { Name = "wow", Value = new HeroesIndex() })
                //.AddKeyedSingleton("strat", (sp, k) => new NamedService<StrategyIndex> { Name = "statA", Value = new StrategyIndex() })
                //.AddKeyedSingleton("strat", (sp, k) => new NamedService<StrategyIndex> { Name = "statB", Value = new StrategyIndex() })
                //.AddSingleton(sp => new NamedService<IHeroesIndex> { Name = "name", Value = new HeroesIndex() })
                ;
            var providers = CreateServiceProvider(services);

            var q = providers.GetServices<NamedService<IHeroesIndex>>();// ?? Enumerable.Empty<NamedService<IHeroesIndex>>();
            Assert.NotEmpty(q);

            // returns 1 single entry when no services are registered with null data (not expected)
            var strats = providers.GetServices<NamedService<StrategyIndex>>();// ?? Enumerable.Empty<NamedService<IHeroesIndex>>();

            // returns empty when no services are registered (expected)
            var stratsKeyed = providers.GetKeyedServices<NamedService<StrategyIndex>>("strat");
        }
    }
}

public class NamedService<T>
{
    public string Name { get; set; }
    public T Value { get; set; }
}

public interface IHeroesIndex
{
}

public class HeroesIndex : IHeroesIndex
{

}
public class StrategyIndex
{

}