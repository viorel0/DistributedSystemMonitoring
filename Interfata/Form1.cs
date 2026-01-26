using WebApplication1;
using System.Windows.Forms.DataVisualization.Charting;

namespace Interfata
{
    public partial class Form1 : Form
    {
        private Dictionary<string, Chart> _chartCache = new Dictionary<string, Chart>();
        private System.Windows.Forms.Timer _updateTimer;
        public Form1()
        {
            InitializeComponent();
            splitContainer2.Panel2.AutoScroll = true;
            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 2000;
            _updateTimer.Tick += UpdateActiveChart_Tick;
            _updateTimer.Start();
        }
        private Button CreateSensorButton(string text)
        {
            return new Button
            {
                Text = text,
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightSteelBlue,
                Margin = new Padding(5)
            };
        }

        private Button CreateNodeButton(string name, int id)
        {
            return new Button
            {
                Text = name,
                Height = 45,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
                Tag = id
            };
        }

        private Chart CreateEmptyChart(string sensorType)
        {
            Chart chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.BackColor = Color.FromArgb(245, 245, 245);

            ChartArea area = new ChartArea("MainArea");
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.Title = "Timp";
            area.AxisY.Title = sensorType;

            area.AxisX.IsMarginVisible = false;

            chart.ChartAreas.Add(area);

            Series series = new Series("DateSenzor");
            series.ChartType = SeriesChartType.Line;
            series.Color = Color.DodgerBlue;
            series.BorderWidth = 2;


            series.IsXValueIndexed = true;

            series.XValueType = ChartValueType.String;

            chart.Series.Add(series);

            Title title = new Title($"{sensorType}");
            title.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            chart.Titles.Add(title);

            return chart;

        }
        private async void UpdateActiveChart_Tick(object sender, EventArgs e)
        {
            if (_chartCache.Count == 0) return;

            var keys = _chartCache.Keys.ToList();

            foreach (var key in keys)
            {
                // Spargem cheia ca să știm ce să cerem de la API
                string[] parts = key.Split('|');
                string node = parts[0];
                string sensor = parts[1];

                try
                {
                    var data = await ApiRequests.GetNodeTypeValueLatest(node, sensor);
                    if (data != null)
                    {
                        Chart currentChart = _chartCache[key];
                        var serie = currentChart.Series["DateSenzor"];

                        string labelTimp = data.RecordedAt.ToString("HH:mm:ss");

                        serie.Points.AddXY(labelTimp, data.SensorValue);

                        if (serie.Points.Count > 12) serie.Points.RemoveAt(0);

                        if (currentChart.Visible)
                        {
                            currentChart.ChartAreas[0].RecalculateAxesScale();
                        }
                    }
                }
                catch { }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2.Controls.Clear();

            List<NodeSummary> nodes = await ApiRequests.GetNodes();

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    Button btn = CreateNodeButton(node.NodeName, node.NodeId);

                    btn.Click += NodeButton_Click;

                    splitContainer2.Panel2.Controls.Add(btn);

                    btn.BringToFront();
                }
            }
            //testbuttons
            //for (int i = 0; i < 20; i++)
            //{
            //    Button btn = CreateNodeButton("test", i);

            //    splitContainer2.Panel2.Controls.Add(btn);

            //    btn.BringToFront();
            //}
        }

        private async void NodeButton_Click(object sender, EventArgs e)
        {
            Button butonApasat = (Button)sender;

            string nume = butonApasat.Text;
            int id = (int)butonApasat.Tag;

            splitContainer1.Panel2.Controls.Clear();

            SplitContainer splitPaginaDetaliu = new SplitContainer();
            splitPaginaDetaliu.Dock = DockStyle.Fill;
            splitPaginaDetaliu.Orientation = Orientation.Horizontal;
            splitPaginaDetaliu.SplitterDistance = 55; 
            splitPaginaDetaliu.FixedPanel = FixedPanel.Panel1;
            splitPaginaDetaliu.BorderStyle = BorderStyle.FixedSingle;

            FlowLayoutPanel flowMeniuSenzori = new FlowLayoutPanel();
            flowMeniuSenzori.Dock = DockStyle.Fill;
            flowMeniuSenzori.FlowDirection = FlowDirection.LeftToRight;
            flowMeniuSenzori.WrapContents = false;
            flowMeniuSenzori.AutoScroll = true;

            splitPaginaDetaliu.Panel1.Controls.Add(flowMeniuSenzori);

            splitContainer1.Panel2.Controls.Add(splitPaginaDetaliu);
            
            List<string> tipuri = await ApiRequests.GetNodeTypes(nume);

            if (tipuri != null)
            {
                foreach (var tip in tipuri)
                {
                    Button btnTip = CreateSensorButton(tip);

                    btnTip.Click += (s, ev) => {
                        string chartKey = $"{nume}|{tip}";

                        foreach (Control c in splitPaginaDetaliu.Panel2.Controls)
                        {
                            c.Visible = false;
                        }

                        if (_chartCache.ContainsKey(chartKey))
                        {
                            Chart chart = _chartCache[chartKey];

                            if (!splitPaginaDetaliu.Panel2.Controls.Contains(chart))
                                splitPaginaDetaliu.Panel2.Controls.Add(chart);

                            chart.Visible = true;
                            chart.BringToFront();
                        }
                        else
                        {
                            Chart newChart = CreateEmptyChart(tip);
                            newChart.Name = chartKey;
                            _chartCache.Add(chartKey, newChart);
                            splitPaginaDetaliu.Panel2.Controls.Add(newChart);
                            newChart.Visible = true;
                        }
                    };

                    flowMeniuSenzori.Controls.Add(btnTip);

                }
                if (flowMeniuSenzori.Controls.Count > 0)
                {
                    Button primulButon = (Button)flowMeniuSenzori.Controls[0];
                    primulButon.PerformClick();
                }
            }
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer2_Panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void splitContainer3_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
