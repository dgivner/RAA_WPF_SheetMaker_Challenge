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
        string tblockTypeName;
        private List<ViewPlan> viewPlanList;
        private List<string> tBlockNames;

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
            dataList.Add(new DataClass1(sheetNumber,sheetName,true, tBlockNames, viewPlanList));

            List<string> viewNames = MyForm.GetViewPlansByName(doc);
            //List<Element> tBlockNames = MyForm.GetTitleBlocksByName(doc, tBlockNames);

            // open form
            MyForm currentForm = new MyForm(doc, dataList)
            {
                Width = 800,
                Height = 450,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            //if (currentForm.DialogResult == true)
            //{
            //    List<DataClass1> dataList = currentForm.GetData();

            //    foreach (DataClass1 curClass1 in dataList)
            //    {
            //        TaskDialog.Show("Titleblocks", curClass1.TitleBlockList.ToString());
            //        TaskDialog.Show("Views", curClass1.ViewPlanList.ToString());
            //    }
            //}
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
