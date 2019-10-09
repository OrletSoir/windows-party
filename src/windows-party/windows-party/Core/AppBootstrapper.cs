using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Caliburn.Micro;
using windows_party.Login;
using windows_party.ServerList;

namespace windows_party
{
    public class AppBootstrapper : BootstrapperBase
    {
        #region private fields
        SimpleContainer container;
        #endregion

        #region constructor/destructor
        public AppBootstrapper()
        {
            Initialize();

            // adding password box helper to convention manager
            ConventionManager.AddElementConvention<PasswordBox>(PasswordBoxHelper.BoundPasswordProperty, "Password", "PasswordChanged");
        }
        #endregion

        #region IoC container population
        protected override void Configure()
        {
            container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();

            container.PerRequest<IMain, MainViewModel>();
            container.PerRequest<ILogin, LoginViewModel>();
            container.PerRequest<IServerList, ServerListViewModel>();
        }
        #endregion

        #region IoC container composition overrides
        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
        #endregion

        #region app startup/cleanup actions
        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            IMain mainViewModel = IoC.Get<IMain>();
            BuildUp(mainViewModel);

            IWindowManager windowManager = IoC.Get<IWindowManager>();
            windowManager.ShowWindow(mainViewModel);
        }
        #endregion
    }
}