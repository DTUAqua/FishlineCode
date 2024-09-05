using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.SprattusSecurity
{
    public partial class Role
    {
        private List<SecurityTask> _lstTasks = null;


        /// <summary>
        /// Retrieve a list of tasks associated to the current role
        /// </summary>
        public List<SecurityTask> SecurityTasks
        {
            get
            {
                if (_lstTasks == null)
                {
                    _lstTasks = new List<SecurityTask>();
                    _lstTasks.Add(SecurityTask.ReadData);

                    if (FishLineTasks != null && FishLineTasks.Count > 0)
                        _lstTasks = FishLineTasks.Select(x => (SecurityTask)Enum.Parse(typeof(SecurityTask), x.Value)).ToList();
                }

                return _lstTasks;
            }
        }


        /// <summary>
        /// Checks whether the role has a given task.
        /// </summary>
        public bool HasTask(SecurityTask task)
        {
            return SecurityTasks.Where(x => x.Equals(task)).Any();
        }
    }
}
