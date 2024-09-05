using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.ViewModels
{
    public static class ExtensionMethods
    {
        public static IEnumerable<ReportingTreeNode> GetTreeNodeHierarchy(this IEnumerable<ReportingTreeNode> treeNodes)
        {
            foreach (var tn in treeNodes)
            {
                foreach (var tnc in tn.ChildTreeNodes.GetTreeNodeHierarchy())
                    yield return tnc;

                yield return tn;
            }
        }


        public static IEnumerable<Report> GetReportNodeHierarchy(this IEnumerable<INodeItem> treeNodes)
        {
            foreach (var tn in treeNodes)
            {
                if (tn is ReportingTreeNode)
                {
                    var rtn = tn as ReportingTreeNode;

                    foreach (var tnc in rtn.ChildTreeNodes.GetReportNodeHierarchy())
                        yield return tnc;

                    foreach (var rep in rtn.Reports)
                        yield return rep;
                }
                else if(tn is Report)
                    yield return tn as Report;
            }
        }


        public static void ExpandAllParents(this ReportingTreeNode rep)
        {
            if(rep.ParentTreeNode != null)
            {
                rep.ParentTreeNode.ExpandAllParents();

                if (!rep.ParentTreeNode.IsExpanded)
                    rep.ParentTreeNode.IsExpanded = true;
            }
        }

        public static void ExpandAllParents(this Report report)
        {
            foreach (var rep in report.ReportingTreeNodes)
            {
                if (!rep.IsExpanded)
                    rep.IsExpanded = true;

                rep.ExpandAllParents();
            }
        }
    }
}
