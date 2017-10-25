using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using SQLitePCL;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Fault_Visualiser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //connect to database
        public SQLitePCL.SQLiteConnection conn = new SQLiteConnection("Loadcell.db");

        public MainPage()
        {
            this.InitializeComponent();
            //create table if it haven't been created
            string sSQL = @"CREATE TABLE IF NOT EXISTS Loadcell 
            (ID Integer Primary Key Autoincrement NOT NULL, Date_Measured DATE, Time_Measured TIME, Value1 float,Value2 float,Value3 float,Value4 float);";
            ISQLiteStatement cnStatement = conn.Prepare(sSQL);
            cnStatement.Step();
            LoadChartContents(DateTime.Today.ToString("yyyy-MM-dd"));
        }

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

        private void LoadChartContents(string d)
        {
            try
            {
                List<LoadCell> records = new List<LoadCell>();
                List<LoadCell1> records1 = new List<LoadCell1>();
                List<LoadCell2> records2 = new List<LoadCell2>();
                List<LoadCell3> records3 = new List<LoadCell3>();
                string sSQL;

                if(d == DateTime.Today.ToString("yyyy-MM-dd"))
                {
                    sSQL = @"select * from (SELECT * from Loadcell order by ID desc limit 8) T order by ID";
                }
                else
                {
                    sSQL = @"SELECT * FROM Loadcell where Date_Measured = '" + d + "'";
                }
                
                ISQLiteStatement dbState = conn.Prepare(sSQL);

                while (dbState.Step() == SQLiteResult.ROW)
                {
                    if (dbState["Date_Measured"].ToString() != null || dbState["Date_Measured"].ToString() != "")
                    {
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
                    }
                }
                (lineChart.Series[0] as LineSeries).ItemsSource = records;
                (lineChart.Series[1] as LineSeries).ItemsSource = records1;
                (lineChart.Series[2] as LineSeries).ItemsSource = records2;
                (lineChart.Series[3] as LineSeries).ItemsSource = records3;
                
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSubmit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
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
                    string sSQL = @"INSERT INTO [Loadcell] 
                    ([Date_Measured],[Time_Measured],[Value1],[Value2],[Value3],[Value4]) 
                    VALUES ('" + DateTime.Today.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "'," + v1 + "," + v2 + "," + v3 + "," + v4 + ");";
                    
                    //execute SQL statement
                    conn.Prepare(sSQL).Step();
                    LoadChartContents(DateTime.Today.ToString("yyyy-MM-dd"));
                }
                catch (Exception ex)
                {

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
            string date = DatePick.Date.ToString("yyyy-MM-dd");
            LoadChartContents(date);
        }

       
        private void btnUndo_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            
            string sSQL = @"DELETE FROM Loadcell WHERE ID=(SELECT MAX(id) FROM Loadcell)";

            //execute SQL statement
            conn.Prepare(sSQL).Step();

            LoadChartContents(date);
        }
    }
}
