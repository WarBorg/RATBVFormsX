using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Platform;
using UIKit;
using Cirrious.MvvmCross.Touch.Views.Presenters;

namespace RATBVFormsX.iOS
{
	public class Setup : MvxTouchSetup
	{
		public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
		{
		}

        protected override IMvxTouchViewPresenter CreatePresenter()
        {
            return new MvxFormsTouchViewPresenter(Window);
        }

		protected override IMvxApplication CreateApp()
		{
            return new App();
		}

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }
	}
}