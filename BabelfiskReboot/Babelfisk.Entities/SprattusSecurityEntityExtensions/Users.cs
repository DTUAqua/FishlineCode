using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anchor.Core;

namespace Babelfisk.Entities.SprattusSecurity
{
    public partial class Users
    {

        public bool IsAdmin
        {
            get { return HasRole("Admin"); }
        }

        public bool IsEditor
        {
            get { return HasTask(SecurityTask.ModifyData); }
        }

        public bool IsReader
        {
            get { return HasTask(SecurityTask.ReadData); }
        }


        public bool HasViewReportsTask
        {
            get { return HasTask(SecurityTask.ViewReports); }
        }

        public bool HasViewLookupsTask
        {
            get
            {
                return HasTask(SecurityTask.ViewLookups);
            }
        }


        public bool HasEditLookupsTask
        {
            get
            {
                return HasTask(SecurityTask.EditLookups);
            }
        }


        public bool HasGoOfflineTask
        {
            get { return HasTask(SecurityTask.GoOffline); }
        }


        public bool HasExportToWarehouseTask
        {
            get { return HasTask(SecurityTask.ExportToWarehouse); }
        }

        public bool HasExportDataTask
        {
            get { return HasTask(SecurityTask.ExportData); }
        }

        public bool HasAddEditSDEventsAndSamplesTask
        {
            get { return HasTask(SecurityTask.AddEditSDEventsAndSamples); }
        }

        public bool HasViewSDEventsAndSamplesTask
        {
            get { return HasTask(SecurityTask.ViewSDEventsAndSamples); }
        }

        public bool HasEditSDAnnotationsTask
        {
            get { return HasTask(SecurityTask.EditSDAnnotations); }
        }

        public bool HasDeleteAnimalsTask
        {
            get { return HasTask(SecurityTask.DeleteAnimals); }
        }

        public string UIDisplay
        {
            get
            {
                return string.Format("{0} - {1} {2}", UserName, FirstName, LastName);
            }
        }


        public List<FishLineTasks> GetFishLineTasks
        {
            get
            {
                return Role == null ? new List<FishLineTasks>() : Role.SelectMany(x => x.FishLineTasks).DistinctBy(x => x.FishLineTaskId).ToList();
            }
        }


        public bool HasTask(SecurityTask st)
        {
            if(Role != null)
            {
                var res = Role.SelectMany(x => x.FishLineTasks).Where(x => x.Value != null && x.Value.Equals(st.ToString(), StringComparison.InvariantCultureIgnoreCase)).Any();
                return res;
            }
          //  SecurityTask.
        //    return Role != null && Role.SecurityTasks != null && Role.SecurityTasks.Contains(st);
            return false;
        }


        public bool HasOneOrMoreTasks(params SecurityTask[] st)
        {
            if (Role != null)
            {
                var q = Role.SelectMany(x => x.FishLineTasks).Where(x => x.Value != null).ToList();
                var res = st.Select(x => x.ToString()).Where(x => q.Any(t => t.Value.Equals(x, StringComparison.InvariantCultureIgnoreCase))).Any();
                return res;
            }

            return false;
        }


        public bool HasRole(string strRole)
        {
            if (Role != null)
            {
                var res = Role.Where(x => x.Role1 != null && x.Role1.Equals(strRole, StringComparison.InvariantCultureIgnoreCase)).Any();
                return res;
            }
          // return Role != null && Role.Role1 != null && Role.Role1.Equals(strRole, StringComparison.InvariantCultureIgnoreCase);

            return false;
        }
    }
}
