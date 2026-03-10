using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace WpfApp2Building_Inspection_Tracker.Models
{
    public class Inspection
    {
        // Header Feilds
        public string projectName { get; set; }
        public string inspectorName { get; set; }
        public string projectType { get; set; }
        public string inspectionType { get; set; }
        public DateTime inspictionDate { get; set; }
        //------------------------------------------------
        // Check List 
        public ObservableCollection<InspectionItem> Items { get; set; } = new ObservableCollection<InspectionItem>();

        // Computing 
        public double passRate
        {
            get
            {
                var items = Items.Where(i => i.status != "N/A");
                int passedItems = items.Count(i => i.status == "Pass");
                int totalItems = items.Count();

                return totalItems == 0 ? 0 : (double)passedItems / totalItems * 100;

            }
        }

        public string result
        {
            get
            {
                if (passRate >= 80)
                {
                    return "Passed";
                }
                return "Failed";
            }
        }
    }
}
