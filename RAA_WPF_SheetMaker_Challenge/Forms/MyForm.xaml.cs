using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Microsoft.Win32;


namespace RAA_WPF_SheetMaker_Challenge
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        
        private string sheetNumber;
        private string sheetName;
        private bool isPlaceholder;
        
        private List<ViewPlan> viewPlanList;
        private List<string> tBlockNames;
        private Document doc;
        private ElementId tblockId;

        ObservableCollection<DataClass1> DataList { get; set; }
        ObservableCollection<string> SheetNumberItems { get; set; }
        ObservableCollection<string> SheetNameItems { get; set; }
        ObservableCollection<bool> IsPlaceholder { get; set; }
        ObservableCollection<string> TitleBlockItems { get; set; }
        ObservableCollection<string> ViewNameItems { get; set; }



        public MyForm(Document doc, List<DataClass1> dataList)
        {
            InitializeComponent();
            DataGrid.ItemsSource = dataList;
            //DataGrid.ItemsSource = GetData();
            //dataList = new ObservableCollection<DataClass1>();
            SheetNumberItems = new ObservableCollection<string>();
            //SheetNumberItem = sheetNumber;
            
            SheetNameItems = new ObservableCollection<string>();
            //SheetNameItem = sheetName;
            
            IsPlaceholder = new ObservableCollection<bool>();

            TitleBlockItems = new ObservableCollection<string>(GetTitleBlocksByName(doc));
            
            TitleBlockItem.ItemsSource = TitleBlockItems;


            ViewNameItems = new ObservableCollection<string>(GetViewPlansByName(doc));
            ViewItem.ItemsSource = ViewNameItems;

        }
        //Methods can be moved to a utils or collector class
        public static List<string> GetTitleBlocksByName(Document doc)//Get Title Block by Type Name
        {
            List<string> tBlockNames = new List<string>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            ICollection<Element> tBlocks = collector.OfClass(typeof(FamilyInstance)).ToElements();
            foreach (Element titleBlock in tBlocks)
            {
                Parameter parameter = titleBlock.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_NAME);
                if(parameter != null && parameter.HasValue)
                tBlockNames.Add(parameter.AsString());
            }
            return tBlockNames;
        }
        public static List<string> GetViewPlansByName(Document doc) //Get Views by View Name
        {
            List<string> viewNames = new List<string>();
            FilteredElementCollector collecter = new FilteredElementCollector(doc);
            ICollection<Element> views = collecter.OfClass(typeof(View)).ToElements();

            foreach(Element view in views)
            {viewNames.Add(view.Name);}
            return viewNames;
        }
        //Button Events
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            string sheetPath = OpenFile();
            //var fileName = OpenFile();
            //Read Sheets excel file for data
            List<dSheets> sheetDataList = new List<dSheets>();

            string[] sheetArray = File.ReadAllLines(sheetPath);

            foreach (string sheetString in sheetArray)
            {
                string[] cellData = sheetString.Split(',');

                dSheets curSheetData = new dSheets();
                curSheetData.Number = cellData[0];
                curSheetData.Name = cellData[1];

                sheetDataList.Add(curSheetData);
            }
            sheetDataList.RemoveAt(0);
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            DataList.Add(new DataClass1(sheetNumber,sheetName,isPlaceholder,tBlockNames,viewPlanList));
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (DataClass1 curRow in DataList)
                {
                    if (DataGrid.SelectedItem == curRow)
                        DataList.Remove(curRow);
                }
            }
            catch (Exception)
            { }
            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //using (Transaction tx = new Transaction(doc))
            //{
            //    tx.Start("Sheet Maker");
            //    foreach (DataClass1 curRow in DataList)
            //    {
            //        ViewSheet newSheet = ViewSheet.Create((doc, tblockId));

            //        XYZ insertPoint = new XYZ(2, 1, 0);
            //        XYZ secondInsertPoint = new XYZ(0, 1, 0);

            //        Viewport newViewport = Viewport.Create((doc, newSheet.Id, View.Id, insertPoint));
            //    }
            //    foreach (var sheet in GetData())
            //    {
            //        ViewSheet newSheet = ViewSheet.Create(doc, tblockId);
            //        newSheet.Name = sheet.Name;
            //        newSheet.SheetNumber = sheet.Number;
            //    }
            //    tx.Commit();
            //    tx.Dispose();
            //}
            
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        public List<DataClass1> GetData()
        {
            List<dSheets> SheetList()
            {
                string sheetsFilePath = OpenFile();

                List<dSheets> sheets = new List<dSheets>();
                string[] sheetsArray = File.ReadAllLines(sheetsFilePath);
                foreach (var sheetsRowString in sheetsArray)
                {
                    string[] sheetsCellString = sheetsRowString.Split(',');
                    var sheet = new dSheets
                    {
                        Number = sheetsCellString[0],
                        Name = sheetsCellString[1]
                    };

                    sheets.Add(sheet);
                }

                return sheets;
            }
            return DataList.ToList();
        }

        private static string OpenFile()
        {
            OpenFileDialog selectFile = new OpenFileDialog();
            selectFile.InitialDirectory = "C:\\";
            selectFile.Filter = "Excel|*.xlsx";
            selectFile.Multiselect = false;

            string fileName = "";
            if ((bool)selectFile.ShowDialog())
            {
                fileName = selectFile.FileName;
            }

            return fileName;
        }

    }
    public class DataClass1
    {

        //Defining the properties for the DataGrid - container for data
        public string SheetNumber { get; set; }
        public string SheetName { get; set; }
        public bool IsPlaceholder { get; set; }
        public List<string> TitleBlockList { get; set; }
        public List<ViewPlan> ViewPlanList { get; set; }
        public string Name { get; internal set; }
        public string Number { get; internal set; }

        public DataClass1(string sheetNumber, string sheetName, bool isPlaceholder, List<string> titleBlockList,
            List<ViewPlan> viewPlanList)
        {
            SheetNumber = sheetNumber;
            SheetName = sheetName;
            IsPlaceholder = isPlaceholder;
            TitleBlockList = titleBlockList;
            ViewPlanList = viewPlanList;
        }

    }
    
}
