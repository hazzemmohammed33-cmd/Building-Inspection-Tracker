using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Windows.Input;
using WpfApp2Building_Inspection_Tracker.Commands;
using WpfApp2Building_Inspection_Tracker.Models;

namespace WpfApp2Building_Inspection_Tracker.ViewModels
{
    public class NewInspectionViewModel : ViewModelBase
    {
        // Shared data (between Tab1 & Tab2)
        private readonly ObservableCollection<Inspection> _sharedInspections;

        // Draft Header Fields (user inputs BEFORE saving inspection)
        private string _projectName;
        private string _inspectorName;
        private string _selectedType;
        private DateTime _selectedDate = DateTime.Today;
        //Items Fields (befor adding items)
        private string _newItemName;
        private string _selectedStatus = "Pass";
        private string _newItemNotes;
        //==================================================
       // Bindable Properties Header
        public string ProjectName
        {
            get => _projectName;
            set { if (SetProperty(ref _projectName, value));}
        }

        public string InspectorName
        {
            get => _inspectorName;
            set { if (SetProperty(ref _inspectorName, value)); }
        }

        public string SelectedType
        {
            get => _selectedType;
            set { if (SetProperty(ref _selectedType, value)) ; }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }
        // Bindable Properties  Items
        public string NewItemName
        {
            get => _newItemName;
            set { if (SetProperty(ref _newItemName, value)); }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        public string NewItemNotes
        {
            get => _newItemNotes;
            set => SetProperty(ref _newItemNotes, value);
        }
        // Collections for UI binding
        public ObservableCollection<InspectionItem> ChecklistItems { get; }
        public List<string> InspectionTypes { get; } // list because it is a fixed value and it will not change
        public List<string> StatusOptions { get; }

        // Live statistics 
        public int passedCount => ChecklistItems.Count(i => i.status == "Pass");
        public int failedCount => ChecklistItems.Count(i => i.status == "Fail");
        public int NACount => ChecklistItems.Count(i => i.status == "N/A");
        public int totalItems => ChecklistItems.Count;

        public double PassRate
        {
            get
            {
                var items = ChecklistItems.Where(i => i.status != "N/A");
                int passedItems = items.Count(i => i.status == "Pass");
                int totalItems = items.Count();

                return totalItems == 0 ? 0 : (double)passedItems / totalItems * 100;
            }
        }
        //============================================================
        //Commands
        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand SubmitCommand { get; }
        public ICommand ClearCommand { get; }

        //Command Logic 
        private bool CanAddItem()
            => !string.IsNullOrWhiteSpace(NewItemName);

        private void AddItem()
        {
            ChecklistItems.Add(new InspectionItem
            {
                itemName = NewItemName,
                status = SelectedStatus,
                notes = NewItemNotes
            });

            NewItemName = string.Empty;
            NewItemNotes = string.Empty;

            LiveStats();
        }
        private void RemoveItem(object item)
        {
            var inspectionItem = item as InspectionItem;
            if (inspectionItem != null)
            {
                ChecklistItems.Remove(inspectionItem);
                LiveStats();
            }
        }
        private bool CanSubmit()
        {
            return !string.IsNullOrWhiteSpace(ProjectName)
                && !string.IsNullOrWhiteSpace(InspectorName)
                && !string.IsNullOrWhiteSpace(SelectedType)
                && ChecklistItems.Count > 0;
        }

        private void SubmitInspection()
        {
            var inspection = new Inspection
            {
                projectName = ProjectName,
                inspectorName = InspectorName,
                inspectionType = SelectedType,
                inspictionDate = SelectedDate,

                Items = new ObservableCollection<InspectionItem>(ChecklistItems)
            };
            // Add to shared history list
            _sharedInspections.Add(inspection);
            ClearForm();
        }

        private void ClearForm()
        {
            ProjectName = string.Empty;
            InspectorName = string.Empty;
            SelectedType = null;
            SelectedDate = DateTime.Today;
            NewItemName = string.Empty;
            SelectedStatus = "Pass";
            NewItemNotes = string.Empty;
            ChecklistItems.Clear();
            LiveStats();
        }

        public NewInspectionViewModel(ObservableCollection<Inspection> sharedInspections)
        {
            _sharedInspections = sharedInspections;

            ChecklistItems = new ObservableCollection<InspectionItem>();
            ChecklistItems.CollectionChanged += (s, e) => LiveStats(); 

            InspectionTypes = new List<string> { "Structural", "MEP", "Safety", "Finishing", "Electrical" }; // fixed value and it will not change
            StatusOptions = new List<string> { "Pass", "Fail", "N/A" };

            AddItemCommand = new RelayCommand(_ => AddItem(), _ => CanAddItem());
            RemoveItemCommand = new RelayCommand(item => RemoveItem(item));
            SubmitCommand = new RelayCommand(_ => SubmitInspection(), _ => CanSubmit());
            ClearCommand = new RelayCommand(_ => ClearForm());
        }
        //live stats refresh method
        private void LiveStats()
        {
            OnPropertyChanged(nameof(totalItems));
            OnPropertyChanged(nameof(passedCount));
            OnPropertyChanged(nameof(failedCount));
            OnPropertyChanged(nameof(NACount));
            OnPropertyChanged(nameof(PassRate));
        }

    }
}
