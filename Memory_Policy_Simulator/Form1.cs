using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Memory_Policy_Simulator
{
    public partial class Form1 : Form
    {
        Graphics g;
        PictureBox pbPlaceHolder;
        Bitmap bResultImage;
        List<char> datalist = new List<char>();
        int function;
        public Form1()
        {
            InitializeComponent();
            this.pbPlaceHolder = new PictureBox();
            this.bResultImage = new Bitmap(2048, 2048);
            this.pbPlaceHolder.Size = new Size(2048, 2048);
            g = Graphics.FromImage(this.bResultImage);
            pbPlaceHolder.Image = this.bResultImage;
            this.pImage.Controls.Add(this.pbPlaceHolder);
        }

        private void DrawBase(Core core, int windowSize, int dataLength)
        {
            /* parse window */
            var psudoList = new List<char>();
            int cursor = 0;


            List<char> store = new List<char>();
            List<int> countsave = new List<int>();
            List<bool> reference = new List<bool>();
            List<int> recountsave = new List<int>();
            g.Clear(Color.Black);


            for (int i = 0; i < dataLength; i++) // length
            {
                int psudoCursor = core.pageHistory[i].loc;
                char data = core.pageHistory[i].data;
                Page.STATUS status = core.pageHistory[i].status;
                Page.STATUS status1 = core.pageHistory[i].status;
                //fifo

                if (function == 0)
                {
                    switch (status)
                    {
                        case Page.STATUS.PAGEFAULT:
                            psudoList.Add(data);
                            break;
                        case Page.STATUS.MIGRATION:

                            psudoList.RemoveAt(cursor);
                            psudoList.Insert(cursor, data);
                            cursor++;
                            if (cursor == windowSize)
                                cursor = 0;
                            break; this.tbConsole.Clear();

                    }
                }
                //LRU
                if (function == 2)
                {
                    switch (status)
                    {
                        case Page.STATUS.PAGEFAULT:
                            psudoList.Add(data);
                            break;
                        case Page.STATUS.MIGRATION:
                            for (int a = 0; a < psudoList.Count; a++)
                            {
                                int count = 0;
                                for (int b = i; b >= 0; b--)
                                {
                                    if (core.pageHistory.ElementAt(b).data == psudoList.ElementAt(a))
                                    {
                                        countsave.Insert(a, count);
                                        char c = psudoList.ElementAt(a);
                                        break;
                                    }
                                    else
                                        count++;
                                }
                            }
                            int big = countsave.ElementAt(0);
                            int old = 0;
                            for (int d = 1; d < countsave.Count; d++)
                            {
                                if (big < countsave.ElementAt(d))
                                {
                                    big = countsave.ElementAt(d);
                                    old = d;
                                }
                            }
                            psudoList.RemoveAt(old);
                            psudoList.Insert(old, data);
                            countsave.Clear();
                            break;
                    }
                }

                //optimal
                if (function == 1)
                {
                    switch (status)
                    {
                        case Page.STATUS.PAGEFAULT:
                            psudoList.Add(data);
                            store.Add(data);
                            break;
                        case Page.STATUS.MIGRATION:
                            for (int a = 0; a < psudoList.Count; a++)
                            {
                                int count = 0;
                                for (int b = store.Count + 1; b < datalist.Count; b++)//store.Count+1
                                {
                                    if (psudoList.ElementAt(a).Equals(datalist.ElementAt(b)))
                                    {
                                        break;
                                    }
                                    else
                                    {

                                        count++;
                                    }
                                }
                                countsave.Insert(a, count);
                            }
                            int big = countsave.ElementAt(0);
                            int old = 0;
                            for (int a = 1; a < countsave.Count; a++)
                            {
                                if (big < countsave.ElementAt(a))
                                {
                                    big = countsave.ElementAt(a);
                                    old = a;
                                }
                            }
                            psudoList.RemoveAt(old);
                            psudoList.Insert(old, data);
                            store.Add(data);
                            countsave.Clear();
                            break;
                        case Page.STATUS.HIT:
                            store.Add(data);
                            break;
                    }
                }

                //second
                if (function == 3)
                {
                    switch (status)
                    {
                        case Page.STATUS.PAGEFAULT:
                            psudoList.Add(data);
                            reference.Add(false);
                            break;
                        case Page.STATUS.MIGRATION:
                            for (int re = 0; re < reference.Count; re++)
                            {
                                if (reference.ElementAt(cursor))
                                {
                                    reference[cursor] = false;
                                    cursor++;
                                    if (cursor == reference.Count)
                                        cursor = 0;
                                }
                                else
                                    break;
                            }

                            psudoList.RemoveAt(cursor);
                            reference[cursor] = false;
                            psudoList.Insert(cursor, data);

                            cursor++;
                            if (cursor == windowSize)
                                cursor = 0;
                            break;
                        case Page.STATUS.HIT:
                            int rhit;
                            for (rhit = 0; rhit < psudoList.Count; rhit++)
                                if (psudoList.ElementAt(rhit) == data)
                                    break;
                            reference[rhit] = true;
                            break;
                    }
                }

                //LFU
                if (function == 4)
                {
                    switch (status)
                    {
                        case Page.STATUS.PAGEFAULT:
                            psudoList.Add(data);
                            recountsave.Add(0);
                            break;
                        case Page.STATUS.MIGRATION:
                            int big = recountsave.ElementAt(0);
                            int old = 0;
                            for (int rec = 1; rec < recountsave.Count; rec++)
                            {
                                if (big > recountsave.ElementAt(rec))
                                {
                                    big = recountsave.ElementAt(rec);
                                    old = rec;
                                }
                            }
                            psudoList.RemoveAt(old);
                            recountsave.RemoveAt(old);
                            psudoList.Insert(old, data);
                            recountsave.Insert(old, 0);
                            break;
                        case Page.STATUS.HIT:
                            int rhit;
                            for (rhit = 0; rhit < psudoList.Count; rhit++)
                            {
                                if (psudoList.ElementAt(rhit) == data)
                                    break;
                            }
                            // recount++;
                            recountsave[rhit] = recountsave[rhit] + 1;
                            break;
                    }
                }
                //
                //MFU
                if (function == 5)
                {
                    switch (status)
                    {
                        case Page.STATUS.PAGEFAULT:
                            psudoList.Add(data);
                            recountsave.Add(0);
                            break;
                        case Page.STATUS.MIGRATION:
                            int big = recountsave.ElementAt(0);
                            int old = 0;
                            for (int rec = 1; rec < recountsave.Count; rec++)
                            {
                                if (big < recountsave.ElementAt(rec))
                                {
                                    big = recountsave.ElementAt(rec);
                                    old = rec;
                                }
                            }
                            psudoList.RemoveAt(old);
                            recountsave.RemoveAt(old);
                            psudoList.Insert(old, data);
                            recountsave.Insert(old, 0);
                            break;
                        case Page.STATUS.HIT:
                            int rhit;
                            for (rhit = 0; rhit < psudoList.Count; rhit++)
                            {
                                if (psudoList.ElementAt(rhit) == data)
                                    break;
                            }
                            // recount++;
                            recountsave[rhit] = recountsave[rhit] + 1;
                            break;
                    }
                }

                //

                for (int j = 0; j <= windowSize; j++) // height - STEP
                {
                    if (j == 0)
                    {
                        DrawGridText(i, j, data);
                    }
                    else
                    {
                        DrawGrid(i, j);
                    }
                }

                DrawGridHighlight(i, psudoCursor, status);
                int depth = 1;
                foreach (char t in psudoList)
                {
                    DrawGridText(i, depth++, t);
                }
            }
        }


        private void DrawGrid(int x, int y)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            g.DrawRectangle(new Pen(Color.White), new Rectangle(
                gridBaseX + (x * gridSpace),
                gridBaseY,
                gridSize,
                gridSize
                ));
        }

        private void DrawGridHighlight(int x, int y, Page.STATUS status)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            SolidBrush highlighter = new SolidBrush(Color.LimeGreen);

            switch (status)
            {
                case Page.STATUS.HIT:
                    break;
                case Page.STATUS.MIGRATION:
                    highlighter.Color = Color.Purple;
                    break;
                case Page.STATUS.PAGEFAULT:
                    highlighter.Color = Color.Red;
                    break;
            }

            g.FillRectangle(highlighter, new Rectangle(
                gridBaseX + (x * gridSpace),
                gridBaseY,
                gridSize,
                gridSize
                ));
        }

        private void DrawGridText(int x, int y, char value)
        {
            int gridSize = 30;
            int gridSpace = 5;
            int gridBaseX = x * gridSize;
            int gridBaseY = y * gridSize;

            g.DrawString(
                value.ToString(),
                new Font(FontFamily.GenericMonospace, 8),
                new SolidBrush(Color.White),
                new PointF(
                    gridBaseX + (x * gridSpace) + gridSize / 3,
                    gridBaseY + gridSize / 4));
        }

        private void btnOperate_Click(object sender, EventArgs e)
        {
            this.tbConsole.Clear();
            this.datalist.Clear();
            if (this.tbQueryString.Text != "" || this.tbWindowSize.Text != "")
            {
                string data = this.tbQueryString.Text;
                int windowSize = int.Parse(this.tbWindowSize.Text);
                /* initalize */
                foreach (char element in data)
                {
                    datalist.Add(element);
                }
                var window = new Core(windowSize);

                foreach (char element in data)
                {

                    if (function == 0)
                    {
                        var status = window.Operate(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                        ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                        + "\r\n";
                    }
                    else if (function == 1)
                    {
                        var status = window.OPTIMAL(element, datalist);
                        this.tbConsole.Text += "DATA " + element + " is " +
                        ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                        + "\r\n";
                    }
                    else if (function == 2)
                    {
                        var status = window.LRU(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                        ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                        + "\r\n";
                    }
                    else if (function == 3)
                    {
                        var status = window.SECOND(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                        ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                        + "\r\n";
                    }
                    else if (function == 4)
                    {
                        var status = window.LFU(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                        ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                        + "\r\n";
                    }
                    else if (function == 5)
                    {
                        var status = window.MFU(element);
                        this.tbConsole.Text += "DATA " + element + " is " +
                        ((status == Page.STATUS.PAGEFAULT) ? "Page Fault" : status == Page.STATUS.MIGRATION ? "Migrated" : "Hit")
                        + "\r\n";
                    }
                    ///여기가 진입점이네 단어 하나하나씩 보내주는군



                }

                
                DrawBase(window, windowSize, data.Length);
                this.pbPlaceHolder.Refresh();

                /* 차트 생성 */
                chart1.Series.Clear();
                Series resultChartContent = chart1.Series.Add("Statics");
                resultChartContent.ChartType = SeriesChartType.Pie;
                resultChartContent.IsVisibleInLegend = true;
                resultChartContent.Points.AddXY("Hit", window.hit);
                resultChartContent.Points.AddXY("Page Fault", window.fault - window.migration);
                resultChartContent.Points.AddXY("Migrated", window.migration);
                resultChartContent.Points[0].IsValueShownAsLabel = true;
                resultChartContent.Points[1].IsValueShownAsLabel = true;
                resultChartContent.Points[2].IsValueShownAsLabel = true;

                this.lbPageFaultRatio.Text = Math.Round(((float)window.fault / (window.fault + window.hit)), 2) * 100 + "%";
            }
            else
            {
            }

        }

        private void pbPlaceHolder_Paint(object sender, PaintEventArgs e)
        {
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void tbWindowSize_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void tbWindowSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void btnRand_Click(object sender, EventArgs e)
        {
            Random rd = new Random();

            int count = rd.Next(5, 50);
            StringBuilder sb = new StringBuilder();


            for (int i = 0; i < count; i++)
            {
                sb.Append((char)rd.Next(65, 90));
            }

            this.tbQueryString.Text = sb.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bResultImage.Save("./result.jpg");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string policy = comboBox1.SelectedItem.ToString();
            switch (policy)
            {
                case "FIFO":
                    function = 0;
                    break;
                case "OPTIMAL":
                    function = 1;
                    break;
                case "LRU":
                    function = 2;
                    break;
                case "SECOND":
                    function = 3;
                    break;
                case "LFU":
                    function = 4;
                    break;
                case "MFU":
                    function = 5;
                    break;

            }
        }
    }
}
