using Managers;
using VContainer;
using VContainer.Unity;

namespace LifeTimeScope
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 전역적으로 사용되는 매니저들 등록
            builder.Register<DataManager>(Lifetime.Singleton);
        }
    }
}