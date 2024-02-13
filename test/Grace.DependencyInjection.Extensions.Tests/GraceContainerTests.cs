using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Grace.DependencyInjection.Extensions.Tests
{
    public class GraceContainerTests : DependencyInjectionSpecificationTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            void Configuration(InjectionScopeConfiguration x)
            {
                x.Behaviors.AllowInstanceAndFactoryToReturnNull = true;
                x.AutoRegisterUnknown = false;
            }

            return new DependencyInjectionContainer(Configuration)
            //return new DependencyInjectionContainer()
                .Populate(serviceCollection);
        }

        [Fact]
        public void GetServices()
        {
            var services = new ServiceCollection()
                //.AddSingleton(sp => new NamedService<StrategyIndex> { Name = "strat1", Value = new StrategyIndex() })
                //.AddSingleton(sp => new NamedService<StrategyIndex> { Name = "strat2", Value = new StrategyIndex() })
                //.AddSingleton(sp => new NamedService<IHeroesIndex> { Name = "wow", Value = new HeroesIndex() })
                //.AddKeyedSingleton("strat", (sp, k) => new NamedService<StrategyIndex> { Name = "statA", Value = new StrategyIndex() })
                //.AddKeyedSingleton("strat", (sp, k) => new NamedService<StrategyIndex> { Name = "statB", Value = new StrategyIndex() })
                //.AddSingleton(sp => new NamedService<IHeroesIndex> { Name = "name", Value = new HeroesIndex() })
                //.AddSingleton(sp => new NamedService<IHeroesIndex> { Name = "name", Value = new HeroesIndex() })
                ;
            var providers = CreateServiceProvider(services);

            var strategy = providers.GetService<StrategyIndex>();

            var q = providers.GetServices<NamedService<IHeroesIndex>>();// ?? Enumerable.Empty<NamedService<IHeroesIndex>>();
            Assert.NotEmpty(q);

            // returns 1 single entry when no services are registered with null data (not expected)
            var strats = providers.GetServices<NamedService<StrategyIndex>>();// ?? Enumerable.Empty<NamedService<IHeroesIndex>>();

            // returns empty when no services are registered (expected)
            var stratsKeyed = providers.GetKeyedServices<NamedService<StrategyIndex>>("strat");
        }

        [Fact]
        public async Task Scope_DisposeAsync_ShouldNotDisposeSingleton()
        {
            var services = new ServiceCollection()
                .AddKeyedSingleton<StrategyIndexController>("hots", (sp, key) => new(key as string))
                .AddScoped<StrategyIndex>()
                ;
            var providers = CreateServiceProvider(services);

            var rootSingletonController = providers.GetKeyedService<StrategyIndexController>("hots");
            var rootScopedStrategy = providers.GetService<StrategyIndex>();

            var serviceScope = providers.CreateScope();

            var scopedStrategy = serviceScope.ServiceProvider.GetService<StrategyIndex>();

            var scopeSingletonController = serviceScope.ServiceProvider.GetKeyedService<StrategyIndexController>("hots");

            Assert.NotEqual(rootScopedStrategy, scopedStrategy);
            Assert.Equal(rootSingletonController, scopeSingletonController);

            //serviceScope.Dispose();

            switch (serviceScope)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }

        }
    }
}

public class NamedService<TService>
{
    public NamedService(string name, TService service)
    {
        Name = name;
        Service = service;
    }


    public string Name { get; }
    public TService Service { get; }
}

public interface IHeroesIndex
{
}

public class HeroesIndex : IHeroesIndex
{

}
public class StrategyIndex : IDisposable
{
    public void Dispose()
    {
        //throw new NotImplementedException();
    }
}

public class StrategyIndexController(string key) : IDisposable
{
    public string Key { get; } = key;

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}