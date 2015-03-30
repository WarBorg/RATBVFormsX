using RATBVFormsX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RATBVFormsX.Views
{
    public partial class BusLinesView : TabbedPage
    {
        private BusLinesViewModel ViewModel;

        public BusLinesView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            ViewModel = this.BindingContext as BusLinesViewModel;
        }

        private void BusLine_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            ViewModel.ShowSelectedBusLineStationsCommand.Execute(e.SelectedItem);

            ((ListView)sender).SelectedItem = null; // de-select the row
        }
    }
}
