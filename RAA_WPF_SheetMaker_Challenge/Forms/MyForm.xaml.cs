using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Microsoft.Win32;


namespace RAA_WPF_SheetMaker_Challenge
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        private List<dSheets> sheetDataList;
        private string sheetNumber;
        private string sheetName;
        private bool isPlaceholder;
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
            DataList = new ObservableCollection<DataClass1>();
            DataContext = this;
            DataGrid.ItemsSource = dataList;
            //DataGrid.ItemsSource = GetData();
            
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
                if (parameter != null && parameter.HasValue)
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

            sheetDataList = new List<dSheets>();

            string[] sheetArray = File.ReadAllLines(sheetPath);
            for (int i = 1; i < sheetArray.Length; i++)
            {
                string[] cellData = sheetArray[i].Split(',');

                dSheets curSheetData = new dSheets();
                curSheetData.Number = cellData[0];
                curSheetData.Name = cellData[1];

                sheetDataList.Add(curSheetData);
            }
            foreach (dSheets sheet in sheetDataList)
            {
                DataList.Add(new DataClass1(sheet.Number, sheet.Name, false, null, null));
            }
            GetData();
            DataGrid.ItemsSource = DataList;
            //this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            DataList.Add(new DataClass1(sheetNumber,sheetName,false,null,null));
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem != null)
            {
                DataList.Remove(DataGrid.SelectedItem as DataClass1);
            }

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //todo Process data and close form
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Sheet Maker");
                foreach (DataClass1 curRow in DataList)
                {
                    ViewSheet newSheet = ViewSheet.Create(doc, tblockId);

                    //XYZ insertPoint = new XYZ(2, 1, 0);
                    //XYZ secondInsertPoint = new XYZ(0, 1, 0);

                    //Viewport newViewport = Viewport.Create((doc, newSheet.Id, selectedView.Id, insertPoint));
                }
                foreach (var sheet in GetData())
                {
                    ViewSheet newSheet = ViewSheet.Create(doc, tblockId);
                    newSheet.Name = sheet.Name;
                    newSheet.SheetNumber = sheet.Number;
                }
                tx.Commit();
                tx.Dispose();
            }

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
            SheetNumberItems.Add(sheetNumber);
            SheetNameItems.Add(sheetName);
            return DataList.ToList();
        }

        private static List<dSheets> SheetList()
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

        private static string OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        private void LoadDataFromSource()
        {
            
        }
    }
    public class DataClass1
    {
        //Defining the properties for the DataGrid - container for data
        public string SheetNumber { get; set; }
        public string SheetName { get; set; }
        public bool IsPlaceholder { get; set; }
        public List<ViewPlan> ViewPlanList { get; set; }
        public ElementId TBlockId { get; set; }
        public string Name { get; internal set; }
        public string Number { get; internal set; }

        public DataClass1(string sheetNumber, string sheetName, bool isPlaceholder, 
            List<ViewPlan> viewPlanList, ElementId tBlockId)
        {
            SheetNumber = sheetNumber;
            SheetName = sheetName;
            IsPlaceholder = isPlaceholder;
            ViewPlanList = viewPlanList;
            TBlockId = tBlockId;
        }
    }
    
}
