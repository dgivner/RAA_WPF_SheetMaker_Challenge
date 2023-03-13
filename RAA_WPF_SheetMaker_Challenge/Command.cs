#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace RAA_WPF_SheetMaker_Challenge
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        private string sheetNumber;
        private string sheetName;
        private List<ViewPlan> viewPlanList;
        private ElementId tBlockId;

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // put any code needed for the form here
            List<DataClass1> dataList = new List<DataClass1>();
            dataList.Add(new DataClass1(sheetNumber,sheetName,false, viewPlanList, tBlockId));

            List<string> viewNames = MyForm.GetViewPlansByName(doc);
            List<string> tBlockNames = MyForm.GetTitleBlocksByName(doc);

            // open form
            MyForm currentForm = new MyForm(doc, dataList)
            {
                Width = 800,
                Height = 450,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            
            // get form data and do something

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
