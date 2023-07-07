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
        ObservableCollection<Element> TitleBlockItems { get; set; }
        ObservableCollection<View> ViewNameItems { get; set; }



        public MyForm(List<Element> TblockList, List<View> ViewList)
        {
            InitializeComponent();
            DataList = new ObservableCollection<DataClass1>();
            TitleBlockItems = new ObservableCollection<Element>(TblockList);
            ViewNameItems = new ObservableCollection<View>(ViewList);

            DataGrid.ItemsSource = DataList;
            TitleBlockItem.ItemsSource = TitleBlockItems;
            ViewItem.ItemsSource = ViewNameItems;

        }
        //Methods can be moved to a utils or collector class
        
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
                DataList.Add(new DataClass1());
            }
            GetData();
            DataGrid.ItemsSource = DataList;
            //this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            DataList.Add(new DataClass1());
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
       
        public bool IsPlaceholder { get; set; }
        public List<View> SelectedView { get; set; }
        public Element TBlockId { get; set; }
        public string Name { get; internal set; }
        public string Number { get; internal set; }

    }
    
}
