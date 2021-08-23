using System;
using System.Windows;
using Caliburn.Micro;
using Ninject;

namespace AeroHockey.Shell
{
    public class Bootstrapper : BootstrapperBase
    {
        protected readonly IKernel Kernel = new StandardKernel();

        public Bootstrapper(bool useApplication = true)
            : base(useApplication)
        {
            Initialize();
        }
        
        protected override void OnExit(object sender, EventArgs e)
        {
            Kernel?.Dispose();
            base.OnExit(sender, e);
        }

        protected override void Configure()
        {
            LoadModules();
        }

        protected virtual void LoadModules()
        {
            Kernel.Bind<IWindowManager, WindowManager>().To<WindowManager>().InSingletonScope();

            Kernel.Bind<IEventAggregator, EventAggregator>().To<EventAggregator>().InSingletonScope();
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
