// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the BaseViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RATBVFormsX.ViewModels
{
    using System;
    using System.Linq.Expressions;

    using Cirrious.CrossCore;
    using Cirrious.MvvmCross.ViewModels;
    using RATBVFormsX.Services;
    using Acr.MvvmCross.Plugins.UserDialogs;
    using Acr.MvvmCross.Plugins.Network;

    /// <summary>
    ///    Defines the BaseViewModel type.
    /// </summary>
    public abstract class BaseViewModel : MvxViewModel
    {
        protected IBusDataService _busDataService;
        protected IBusWebService _busWebService;

        protected IUserDialogService _dialogService;
        protected INetworkService _networkService;

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>An instance of the service.</returns>
        public TService GetService<TService>() where TService : class
        {
            return Mvx.Resolve<TService>();
        }

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="backingStore">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="property">The property.</param>
        protected void SetProperty<T>(
            ref T backingStore,
            T value,
            Expression<Func<T>> property)
        {
            if (Equals(backingStore, value))
                return;

            backingStore = value;

            this.RaisePropertyChanged(property);
        }

        protected bool IsInternetAvailable()
        {
            //_networkService.Subscribe(x => x.Status.)
            // this does not work when disconnecting the wifi, I need to subscribe to the event in order to recheck connectivity
            //if (!_networkService.IsMobile && !_networkService.IsWifi)
            //{
            //    _dialogService.Toast("No Internet connection detected");

            //    return false;
            //}

            return true;
        }
    }
}
