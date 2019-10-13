using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using windows_party.DataContext.Auth;
using windows_party.DataContext.Server;
using windows_party.Helpers;
using windows_party.Login;
using windows_party.ServerList;

namespace windows_party
{
    public class AppBootstrapper : BootstrapperBase
    {
        #region Logger
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region private fields
        SimpleContainer container;
        #endregion

        #region constructor/destructor
        public AppBootstrapper()
        {
            // attach our logger wrapper to the framework logging
            AttachCaliburnMicroLogging();

#if DEBUG
            // if debugging configuration, enable debug level logging
            EnableDebugLogging();
#endif

            Logger.Debug("Initializing the AppBootstrapper");

            Initialize();
        }
        #endregion

        #region Bootstrapper configuration
        protected override void Configure()
        {
            Logger.Debug("Configuring the bootstrapper");

            // adding password box helper to convention manager
            ConventionManager.AddElementConvention<PasswordBox>(PasswordBoxHelper.BoundPasswordProperty, "Password", "PasswordChanged");

            // add a special value entry for key press
            MessageBinder.SpecialValues.Add("$pressedkey", (context) =>
            {
                if (context.EventArgs is KeyEventArgs keyArgs)
                    return keyArgs.Key;

                return null;
            });

            Logger.Debug("Initializing the IoC container");

            // initialize the IoC container
            container = new SimpleContainer();

            // main singleton components
            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();

            // helper things
            container.PerRequest<BackgroundWorker, BackgroundWorker>();

            // data contexts
            container.PerRequest<IAuth, PartyAuth>();
            container.PerRequest<IServer, PartyServer>();

            // interactive components (screens)
            container.PerRequest<ILogin, LoginViewModel>();
            container.PerRequest<IServerList, ServerListViewModel>();

            // main application window (shell)
            container.PerRequest<IMain, MainViewModel>();
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
            Logger.Debug("Activating MainView");

            IMain mainViewModel = IoC.Get<IMain>();
            BuildUp(mainViewModel);

            IWindowManager windowManager = IoC.Get<IWindowManager>();
            windowManager.ShowWindow(mainViewModel);

            Logger.Info("Application is running");
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            Logger.Info("Application is shutting down");

            base.OnExit(sender, e);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Fatal("Fatal unhandled exception {exception} detected. Message: {message}", e.Exception.GetType(), e.Exception.Message);

            base.OnUnhandledException(sender, e);
        }
        #endregion

        #region private helpers
        private void AttachCaliburnMicroLogging()
        {
            LogManager.GetLog = type => new FrameworkLogger(type);
        }
        
        private void EnableDebugLogging()
        {
            foreach (NLog.Config.LoggingRule rule in NLog.LogManager.Configuration.LoggingRules)
            {
                rule.EnableLoggingForLevel(NLog.LogLevel.Debug);
            }

            // Call to update existing Loggers created with GetLogger() or GetCurrentClassLogger()
            NLog.LogManager.ReconfigExistingLoggers();
        }
#endregion
    }
}