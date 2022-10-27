using Microsoft.Practices.Unity;
using Project_1.Models.Constraints;
using Project_1.Models.Repositories;
using Project_1.Models.Repositories.Abstract;
using Project_1.Models.Shapes.Abstract;
using Project_1.Presenters;
using Project_1.Views;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Project_1
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var container = new UnityContainer();
            container.RegisterType<ICanvas, Canvas>();
            container.RegisterType<IDrawer, Drawer>();
            container.RegisterType<IShapeRepository, ShapeRepository>();
            container.RegisterType<IEdgeConstraintRepository<FixedLength, float>, FixedLengthRepository>();
            container.RegisterType<IEdgeConstraintRepository<Perpendicular, IEdge>, PerpendicularRepository>();
            container.RegisterType<IConstraintRepositories, ConstraintRepositories>();

            var canvas = container.Resolve<Canvas>();
            Application.Run(canvas.GetForm());
        }
    }
}
