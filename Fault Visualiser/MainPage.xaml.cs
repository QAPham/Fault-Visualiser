using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using SQLitePCL;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Printing;
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Fault_Visualiser
{
    
    public sealed partial class MainPage : Page
    {
        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;
        List<Page> pages = new List<Page>();
        private int position = 0;
        private bool mode750 = true;
        //connect to database
        public SQLitePCL.SQLiteConnection conn750 = new SQLiteConnection("Loadcell750.db");
        public SQLitePCL.SQLiteConnection conn1500 = new SQLiteConnection("Loadcell1500.db");
        public MainPage()
        {
            this.InitializeComponent();
            //remove title bar
            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            //create tables if they haven't been created
            string sSQL = @"CREATE TABLE IF NOT EXISTS Loadcell750 
            (ID Integer Primary Key Autoincrement NOT NULL, Date_Measured DATE, Time_Measured TIME, Value1 float,Value2 float,Value3 float,Value4 float);";
            ISQLiteStatement cnStatement = conn750.Prepare(sSQL);
            cnStatement.Step();

            string sSQL1500 = @"CREATE TABLE IF NOT EXISTS Loadcell1500 
            (ID Integer Primary Key Autoincrement NOT NULL, Date_Measured DATE, Time_Measured TIME, Value1 float,Value2 float,Value3 float,Value4 float);";
            cnStatement = conn1500.Prepare(sSQL1500);
            cnStatement.Step();

            //This was deem not surficiant placement and use of this code

            //delete old data (>3 months old)
            //string sSQLDelete = @"DELETE FROM Loadcell750 WHERE Date_Measured = '" + DateTime.Today.AddMonths(-3).AddDays(-1).ToString("yyyy-MM-dd") + "'";
            //cnStatement = conn750.Prepare(sSQLDelete);
            //cnStatement.Step();

            //sSQLDelete = @"DELETE FROM Loadcell1500 WHERE Date_Measured = '" + DateTime.Today.AddMonths(-3).AddDays(-1).ToString("yyyy-MM-dd") + "'";
            //cnStatement = conn1500.Prepare(sSQLDelete);
            //cnStatement.Step();

            //display graph
            LoadChartContents(DateTime.Today.ToString("yyyy-MM-dd"));
        }
        //create each series
        public class LoadCell
        {
            public string Name
            {
                get;
                set;
            
            }
            public float Amount
            {
                get;
                set;
            }
        }
        public class LoadCell1
        {
            public string Name
            {
                get;
                set;

            }
            public float Amount
            {
                get;
                set;
            }
        }
        public class LoadCell2
        {
            public string Name
            {
                get;
                set;

            }
            public float Amount
            {
                get;
                set;
            }
        }
        public class LoadCell3
        {
            public string Name
            {
                get;
                set;

            }
            public float Amount
            {
                get;
                set;
            }
        }
        public class Tor
        {
            public string Name
            {
                get;
                set;

            }
            public float Amount
            {
                get;
                set;
            }
        }
        public class Tor2
        {
            public string Name
            {
                get;
                set;

            }
            public float Amount
            {
                get;
                set;
            }
        }
        public class Max
        {
            public string Name
            {
                get;
                set;

            }
            public float Amount
            {
                get;
                set;
            }
        }
        public class Min
        {
            public string Name
            {
                get;
                set;

            }
            public float Amount
            {
                get;
                set;
            }
        }

        private void LoadChartContents(string d)
        {
            try
            {
                //list of data for graph
                List<LoadCell> records = new List<LoadCell>();
                List<LoadCell1> records1 = new List<LoadCell1>();
                List<LoadCell2> records2 = new List<LoadCell2>();
                List<LoadCell3> records3 = new List<LoadCell3>();
                List<Tor> torMax = new List<Tor>();
                List<Tor2> torMin = new List<Tor2>();
                List<Max> max = new List<Max>();
                List<Min> min = new List<Min>();

                //create select statements for data
                string sSQL;
                ISQLiteStatement dbState;
                if (mode750 == true)
                {
                    if (d == DateTime.Today.ToString("yyyy-MM-dd"))
                    {
                        sSQL = @"select * from (SELECT * from Loadcell750 order by ID desc limit 8) T order by ID";
                    }
                    else
                    {
                        sSQL = @"SELECT * FROM Loadcell750 where Date_Measured = '" + d + "'";
                    }
                    dbState = conn750.Prepare(sSQL);
                }
                else
                {
                    if (d == DateTime.Today.ToString("yyyy-MM-dd"))
                    {
                        sSQL = @"select * from (SELECT * from Loadcell1500 order by ID desc limit 8) T order by ID";
                    }
                    else
                    {
                        sSQL = @"SELECT * FROM Loadcell1500 where Date_Measured = '" + d + "'";
                    }
                    dbState = conn1500.Prepare(sSQL);
                }

                //fetch results
                while (dbState.Step() == SQLiteResult.ROW)
                {
                    if (dbState["Date_Measured"].ToString() != null || dbState["Date_Measured"].ToString() != "")
                    {
                        //get and set data to store in lists
                        string date = dbState["Date_Measured"].ToString();
                        string time = dbState["Time_Measured"].ToString();
                        float v1 = float.Parse(dbState["Value1"].ToString());
                        float v2 = float.Parse(dbState["Value2"].ToString());
                        float v3 = float.Parse(dbState["Value3"].ToString());
                        float v4 = float.Parse(dbState["Value4"].ToString());
                        records.Add(new LoadCell()
                        {
                            Name = time,
                            Amount = v1
                        });
                        records1.Add(new LoadCell1()
                        {
                            Name = time,
                            Amount = v2
                        });
                        records2.Add(new LoadCell2()
                        {
                            Name = time,
                            Amount = v3
                        });
                        records3.Add(new LoadCell3()
                        {
                            Name = time,
                            Amount = v4
                        });
                        float maxTor = 10.2f;
                        float minTor = 10f;
                        if (mode750 == false)
                        {
                            maxTor = 14.75f;
                            minTor = 14.55f;
                        }
                        torMax.Add(new Tor()
                        {
                            Name = time,
                            Amount = maxTor
                        });
                        torMin.Add(new Tor2()
                        {
                            Name = time,
                            Amount = minTor
                        });

                        float maxAmount = 10.3f;
                        float minAmount = 9.9f;
                        if (mode750 == false)
                        {
                            maxAmount = 14.85f;
                            minAmount = 14.45f;
                        }

                        max.Add(new Max()
                        {
                            Name = time,
                            Amount = maxAmount
                        });
                        min.Add(new Min()
                        {
                            Name = time,
                            Amount = minAmount
                        });
                    }
                }
                lineChart.Width = 1000;
                //display series on graph
                (lineChart.Series[0] as LineSeries).ItemsSource = records;
                (lineChart.Series[1] as LineSeries).ItemsSource = records1;
                (lineChart.Series[2] as LineSeries).ItemsSource = records2;
                (lineChart.Series[3] as LineSeries).ItemsSource = records3;
                (lineChart.Series[4] as LineSeries).ItemsSource = torMax;
                (lineChart.Series[5] as LineSeries).ItemsSource = torMin;
                (lineChart.Series[6] as LineSeries).ItemsSource = max;
                (lineChart.Series[7] as LineSeries).ItemsSource = min;

                customChart();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void LoadChartContentRange(string start, string end)
        {
            try
            {
                //list of data for graph
                List<LoadCell> records = new List<LoadCell>();
                List<LoadCell1> records1 = new List<LoadCell1>();
                List<LoadCell2> records2 = new List<LoadCell2>();
                List<LoadCell3> records3 = new List<LoadCell3>();
                List<Tor> torMax = new List<Tor>();
                List<Tor2> torMin = new List<Tor2>();
                List<Max> max = new List<Max>();
                List<Min> min = new List<Min>();

                //create select statements for data
                string sSQL;
                ISQLiteStatement dbState;
                if (mode750 == true)
                {
                   
                    sSQL = @"SELECT * FROM Loadcell750 where Date_Measured >= '" + start +"' AND Date_Measured <= '" + end + "'";
                    dbState = conn750.Prepare(sSQL);
                }
                else
                {
                    sSQL = @"SELECT * FROM Loadcell1500 where Date_Measured >= '" + start + "' AND Date_Measured <= '" + end + "'";
                    dbState = conn1500.Prepare(sSQL);
                }
                string current = "";
                int count = 1;
                string name = "";
                
                //fetch results
                while (dbState.Step() == SQLiteResult.ROW)
                {
                    if (dbState["Date_Measured"].ToString() != null || dbState["Date_Measured"].ToString() != "")
                    {
                        //get and set data to store in lists
                        string date = dbState["Date_Measured"].ToString();
                        string time = dbState["Time_Measured"].ToString();
                        float v1 = float.Parse(dbState["Value1"].ToString());
                        float v2 = float.Parse(dbState["Value2"].ToString());
                        float v3 = float.Parse(dbState["Value3"].ToString());
                        float v4 = float.Parse(dbState["Value4"].ToString());
                        if (current != date)
                        {
                            current = date;
                            name = time + "\n" + current;
                        }
                        else
                        {
                            name = time;
                        }

                        records.Add(new LoadCell()
                        {
                            Name = name,
                            Amount = v1
                        });
                        records1.Add(new LoadCell1()
                        {
                            Name = name,
                            Amount = v2
                        });
                        records2.Add(new LoadCell2()
                        {
                            Name = name,
                            Amount = v3
                        });
                        records3.Add(new LoadCell3()
                        {
                            Name = name,
                            Amount = v4
                        });
                        float maxTor = 10.2f;
                        float minTor = 10f;
                        if (mode750 == false)
                        {
                            maxTor = 14.75f;
                            minTor = 14.55f;
                        }
                        torMax.Add(new Tor()
                        {
                            Name = name,
                            Amount = maxTor
                        });
                        torMin.Add(new Tor2()
                        {
                            Name = name,
                            Amount = minTor
                        });
                        
                        float maxAmount = 10.3f;
                        float minAmount = 9.9f;
                        if (mode750 == false)
                        {
                            maxAmount = 14.85f;
                            minAmount = 14.45f;
                        }

                        max.Add(new Max()
                        {
                            Name = name,
                            Amount = maxAmount
                        });
                        min.Add(new Min()
                        {
                            Name = name,
                            Amount = minAmount
                        });
                    }
                    
                    count++;
                }
                if (count > 16)
                {
                    lineChart.Width = count /16 * 1000;
                }
                else
                {
                    lineChart.Width = 1000;
                }
                //display series on graph
                (lineChart.Series[0] as LineSeries).ItemsSource = records;
                (lineChart.Series[1] as LineSeries).ItemsSource = records1;
                (lineChart.Series[2] as LineSeries).ItemsSource = records2;
                (lineChart.Series[3] as LineSeries).ItemsSource = records3;
                (lineChart.Series[4] as LineSeries).ItemsSource = torMax;
                (lineChart.Series[5] as LineSeries).ItemsSource = torMin;
                (lineChart.Series[6] as LineSeries).ItemsSource = max;
                (lineChart.Series[7] as LineSeries).ItemsSource = min;

                customChart();
            }
            catch (Exception ex)
            {
                
            }
        }
        private void customChart()
        {
            //overwrite default line series colour
            Style style2 = new Style(typeof(Control));
            style2.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Orange)));

            Style style3 = new Style(typeof(Control));
            style3.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Purple)));

            (lineChart.Series[1] as LineSeries).DataPointStyle = style2;
            (lineChart.Series[3] as LineSeries).DataPointStyle = style3;

            //display tolerance lines and hide its' legends
            Style style = new Style(typeof(Control));
            style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Red)));
            style.Setters.Add(new Setter(Control.HeightProperty, 4));
            style.Setters.Add(new Setter(Control.WidthProperty, 0));

            (lineChart.Series[4] as LineSeries).DataPointStyle = style;
            (lineChart.Series[5] as LineSeries).DataPointStyle = style;

            (lineChart.Series[4] as LineSeries).LegendItemStyle = style;
            (lineChart.Series[5] as LineSeries).LegendItemStyle = style;

            //set max and min for graph by create new series but hide their graph and its' legends
            Style style1 = new Style(typeof(Control));
            style1.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Transparent)));
            style1.Setters.Add(new Setter(Control.HeightProperty, 0));
            style1.Setters.Add(new Setter(Control.WidthProperty, 0));

            (lineChart.Series[6] as LineSeries).DataPointStyle = style1;
            (lineChart.Series[7] as LineSeries).DataPointStyle = style1;

            (lineChart.Series[6] as LineSeries).LegendItemStyle = style;
            (lineChart.Series[7] as LineSeries).LegendItemStyle = style;

        }
        private void btnSubmit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //delete old data
            deleteOldData();
            //clear error message
            lblError.Text = "";
            //if user entered all 4 data
            if (txtValue1.Text != "" && txtValue2.Text != "" && txtValue3.Text != "" && txtValue4.Text != "")
            {
                try
                {
                    //get values from textboxes
                    float v1 = float.Parse(txtValue1.Text);
                    float v2 = float.Parse(txtValue2.Text);
                    float v3 = float.Parse(txtValue3.Text);
                    float v4 = float.Parse(txtValue4.Text);
                    txtValue1.Text = "";
                    txtValue2.Text = "";
                    txtValue3.Text = "";
                    txtValue4.Text = "";

                    //construct SQL statement
                    string sSQL;
                    if (mode750 == true)
                    {
                        sSQL = @"INSERT INTO [Loadcell750]
                    ([Date_Measured],[Time_Measured],[Value1],[Value2],[Value3],[Value4]) 
                    VALUES ('" + DateTime.Today.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "'," + v1 + "," + v2 + "," + v3 + "," + v4 + ");";
                        //execute SQL statement
                        conn750.Prepare(sSQL).Step();
                    }
                    else
                    {
                        sSQL = @"INSERT INTO [Loadcell1500] 
                    ([Date_Measured],[Time_Measured],[Value1],[Value2],[Value3],[Value4]) 
                    VALUES ('" + DateTime.Today.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "'," + v1 + "," + v2 + "," + v3 + "," + v4 + ");";
                        //execute SQL statement
                        conn1500.Prepare(sSQL).Step();
                    }
                   
                    //reload graph view
                    LoadChartContents(DateTime.Today.ToString("yyyy-MM-dd"));
                }
                catch (Exception ex)
                {
                    lblError.Text = "*Invalid Input";
                }
            }
            else
            {
                //Notify user of error
                lblError.Text = "*Please Enter All Values Before Submitting";
            }
        }

        private void btnLookUp_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //get date that user chose and display graph
            string start = DatePickFrom.Date.ToString("yyyy-MM-dd");
            string end = DatePickTo.Date.ToString("yyyy-MM-dd");
            //reload graph view
            LoadChartContentRange(start, end);
            
        }

        private void btnUndo_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            string sSQL;
            //delete the last entered data
            if (mode750 == true)
            {
                sSQL = @"DELETE FROM Loadcell750 WHERE ID=(SELECT MAX(id) FROM Loadcell750)";
                //execute SQL statement
                conn750.Prepare(sSQL).Step();
            }
            else
            {
                sSQL = @"DELETE FROM Loadcell1500 WHERE ID=(SELECT MAX(id) FROM Loadcell1500)";
                //execute SQL statement
                conn1500.Prepare(sSQL).Step();
            }
            
            //reload graph view
            LoadChartContents(date);
        }

        private void pivotLoadCell_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if user select a different tab then change mode accordingly
            if(pivotLoadCell.SelectedItem == pivot750)
            {
                mode750 = true;
            }
            else
            {
                mode750 = false;
            }
            //display data
            LoadChartContents(DateTime.Today.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// Method get call everytime user click Submit. This is because user might not be shuting down program at the end of every day so MainPage() which was where this code was initially would not be call to delete data
        /// </summary>
        private void deleteOldData()
        {
            //delete old data (>3 months old)
            string sSQLDelete = @"DELETE FROM Loadcell750 WHERE Date_Measured >= '2017-01-01' AND Date_Measured < '" + DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd") + "'";
            ISQLiteStatement cnStatement = conn750.Prepare(sSQLDelete);
            cnStatement.Step();

            sSQLDelete = @"DELETE FROM Loadcell1500 WHERE Date_Measured >= '2017-01-01' AND Date_Measured < '" + DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd") + "'";
            cnStatement = conn1500.Prepare(sSQLDelete);
            cnStatement.Step();

            LoadChartContents(DateTime.Today.ToString("yyyy-MM-dd"));
        }
        //enable user to Submit when focus on textbox
        private void txtValue4_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                btnSubmit_Click(sender, e);
            }
        }

        private void txtValue3_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                btnSubmit_Click(sender, e);
            }
        }

        private void txtValue2_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                btnSubmit_Click(sender, e);
            }
        }

        private void txtValue1_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                btnSubmit_Click(sender, e);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Register for PrintTaskRequested event
            printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;

            // Build a PrintDocument and register for callbacks
            printDoc = new PrintDocument();
            printDocSource = printDoc.DocumentSource;
            printDoc.Paginate += Paginate;
            printDoc.GetPreviewPage += GetPreviewPage;
            
            printDoc.AddPages += AddPages;
            
        }
        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // Create the PrintTask.
            // Defines the title and delegate for PrintTaskSourceRequested
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequrested);

            // Handle PrintTask.Completed to catch failed print jobs
            printTask.Completed += PrintTaskCompleted;
        }

        private void PrintTaskSourceRequrested(PrintTaskSourceRequestedArgs args)
        {
            // Set the document source.
            args.SetSource(printDocSource);
        }

        private void Paginate(object sender, PaginateEventArgs e)
        {
            printDoc.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }
        
        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            // Provide a UIElement as the print preview.
            printDoc.SetPreviewPage(e.PageNumber, this.lineChart);
        }
        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, failed to print.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                });
            }
        }
        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (PrintManager.IsSupported())
            {
                try
                {
                    // Show print UI
                    await PrintManager.ShowPrintUIAsync();
                }
                catch
                {
                    // Printing cannot proceed at this time
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, printing can' t proceed at this time.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                }
            }
            else
            {
                // Printing is not supported on this device
                ContentDialog noPrintingDialog = new ContentDialog()
                {
                    Title = "Printing not supported",
                    Content = "\nSorry, printing is not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }
        }
        
        private void AddPages(object sender, AddPagesEventArgs e)
        {
           
            printDoc.AddPage(scrollGraph);
          
            scrollGraph.ChangeView(position += 1000, null, null);
            if (position >= lineChart.ActualWidth) position = 0;
            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
            
        }
    }
}

