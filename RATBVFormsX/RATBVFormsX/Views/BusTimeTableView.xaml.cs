using RATBVFormsX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RATBVFormsX.Views
{
    public partial class BusTimeTableView : TabbedPage
    {
        public BusTimeTableView()
        {
            InitializeComponent();
        }

        private void BusTime_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            ((ListView)sender).SelectedItem = null; // de-select the row
        }
    }
}
