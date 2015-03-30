using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.ViewModels;
using RATBVFormsX.ViewModels;

namespace RATBVFormsX
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<BusLinesViewModel>();
        }
    }
}