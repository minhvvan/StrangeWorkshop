using System;
using Managers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LifeTimeScope
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private DataManager _dataManager;
        [SerializeField] private EventManager _eventManager;
        [SerializeField] private LoadingManager _loadingManager;
        [SerializeField] private RecipeManager _recipeManager;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            // 전역적으로 사용되는 매니저들 등록
            builder.RegisterComponent(_dataManager);
            builder.RegisterComponent(_eventManager);
            builder.RegisterComponent(_loadingManager);
            builder.RegisterComponent(_recipeManager);
        }
        
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}