using Project_1.Models.Repositories;
using Project_1.Presenters;
using Project_1.Views;
using System;
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

            var drawer = new Drawer();
            var shapesRepository = new ShapeRepository();
            var relationsRepository = new ConstraintRepository();
            
            new Canvas(drawer, shapesRepository, relationsRepository);

            Application.Run(drawer);
        }
    }
}
