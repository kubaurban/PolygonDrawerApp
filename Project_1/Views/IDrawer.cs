using Project_1.Helpers.UI;
using Project_1.Models.Constraints.Abstract;
using Project_1.Models.Shapes.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Project_1.Views
{
    public interface IDrawer : IUserActionHandler
    {
        DrawerMode Mode { get; }

        event EventHandler ModeChangedHandler;
        event EventHandler EdgeInsertPointClickedHandler;
        event EventHandler EdgeSetLengthClickedHandler;
        event EventHandler SelectedRelationChangedHandler;
        event EventHandler RelationDeleteHandler;
        event EventHandler EdgeDeleteFixedLengthHandler;
        event MouseEventHandler MouseMiddleHandler;

        void DrawLine(PointF p1, PointF p2, Color? color = null);
        void DrawPoint(PointF p);
        void DrawPolygon(IPolygon polygon);
        void DrawPolygons(IEnumerable<IPolygon> polygons);
        void Write(PointF pointF, string text, Color? color = null);
        void ClearArea();
        void RefreshArea();
        void ShowManageEdgeMenu(PointF point, bool isFixed);
        void EnableRelationsBoxVisibility();
        void DisableRelationsBoxVisibility();
        void SetRelationsListDataSource(IList<IEdgeConstraint<IEdge>> relations);
        IEdgeConstraint<IEdge> GetSelectedRelation();
        void UnsetSelectedRelation();
    }
}
