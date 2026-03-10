using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp2Building_Inspection_Tracker.Commands;
using WpfApp2Building_Inspection_Tracker.Models;

namespace WpfApp2Building_Inspection_Tracker.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Inspection> _sharedInspections;

        private string _filterType = "All";
        private string _filterResult = "All";
        private Inspection _selectedInspection;

        public string FilterType
        {
            get => _filterType;
            set
            {
                if (SetProperty(ref _filterType, value))
                    ApplyFilter();
            }
        }

        public string FilterResult
        {
            get => _filterResult;
            set
            {
                if (SetProperty(ref _filterResult, value))
                    ApplyFilter();
            }
        }

        public Inspection SelectedInspection
        {
            get => _selectedInspection;
            set => SetProperty(ref _selectedInspection, value);
        }

        // Collections for UI binding
        public ObservableCollection<Inspection> FilteredInspections { get; }

        public int TotalInspections => _sharedInspections.Count;

        public double AveragePassRate
            => _sharedInspections.Count == 0 ? 0 : _sharedInspections.Average(i => i.passRate);

        public ICommand DeleteCommand { get; }

        private void Delete(object item)
        {
            var inspection = item as Inspection;
            if (inspection != null)
            {
                _sharedInspections.Remove(inspection);
            }
        }

        private void ApplyFilter()
        {
            var query = _sharedInspections.AsEnumerable(); // to use linq
            // Filter by type 
            if (FilterType != "All")
                query = query.Where(i => i.inspectionType == FilterType);
            // Filter by result
            if (FilterResult != "All")
                query = query.Where(i => i.result == FilterResult);

            FilteredInspections.Clear();
            foreach (var item in query)
                FilteredInspections.Add(item);

            OnPropertyChanged(nameof(TotalInspections));
            OnPropertyChanged(nameof(AveragePassRate));
        }

        public HistoryViewModel(ObservableCollection<Inspection> sharedInspections)
        {
            _sharedInspections = sharedInspections;

            FilteredInspections = new ObservableCollection<Inspection>(_sharedInspections);

            _sharedInspections.CollectionChanged += (s, e) => ApplyFilter();

            DeleteCommand = new RelayCommand(item => Delete(item));
        }
    }
}

