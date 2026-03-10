using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2Building_Inspection_Tracker.Models;
using WpfApp2Building_Inspection_Tracker.ViewModels;

namespace WpfApp2Building_Inspection_Tracker.Views
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ObservableCollection<Inspection> sharedInspections = new ObservableCollection<Inspection>();

            NewInspectionTab.DataContext = new NewInspectionViewModel(sharedInspections);
            HistoryTab.DataContext = new HistoryViewModel(sharedInspections);
        }
    }
}
