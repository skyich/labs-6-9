using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Forms;


namespace Task
{
    public partial class Form1 : Form
    {
        Models.Axis axis = new Models.Axis();
        Models.Polyhedron pol;
        static Pen col;
        static int projection = 1;
        static public Graphics g;
        public Bitmap bmp;

        public List<Point3D> forming; // образующая для фигру вращения
        public int lastModel = -1; // форма последней нарисованной фигуры

        public Form1()
        {
            col = new Pen(Color.Black);
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp = (Bitmap)pictureBox1.Image;
            Clear();
            pictureBox1.Image = bmp;
            comboBox3.SelectedIndex = 0;
            selectorAxis.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
        }

        public void Clear()
        {
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = pictureBox1.Image;
        }

        public void ClearWithout()
        {
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = pictureBox1.Image;
        }

        public void draw_model()
        {
            axis.shift(100, 100, 100);
            pol.shift(100, 100, 100);

            pol.Display(projection);
            axis.Display(projection);
            g.SmoothingMode = SmoothingMode.AntiAlias; // сглаживание
            foreach (var i in pol.edges)
                g.DrawLine(col, i.Item1, i.Item2);

            foreach (var i in axis.edges)
                g.DrawLine(col, i.Item1, i.Item2);

            axis.shift(-100, -100, -100);
            pol.shift(-100, -100, -100);
        }

        //нарисовать
        private void button2_Click(object sender, EventArgs e)
        {
            if (lastModel != comboBox1.SelectedIndex)
            {
                lastModel = comboBox1.SelectedIndex;
                pol = new Models.Polyhedron();
                switch (comboBox1.SelectedItem.ToString())
                {
                    case "Гексаэдр":
                        pol = new Models.cube(pictureBox1.Height/2);
                        break;
                    case "Тетраэдр":
                        pol = new Models.Tetrahedron(pictureBox1.Height / 2);
                        break;
                    case "Октаэдр":
                        pol = new Models.Octahedron(pictureBox1.Height / 2);
                        break;
                    case "Загрузить из файла":
                        LoadFromFile();
                        break;
                    case "Фигура вращения":
                        label5.Visible = true;
                        label6.Visible = true;
                        selectorAxis.Visible = true;
                        counterSplits.Visible = true;
                        buttonDrawSolid.Visible = true;
                        LoadSolid();
                        break;
                    case "Сегмент поверхности":
                        selectorFunc.Visible = true;
                        label8.Visible = true;
                        textBox_x1.Visible = true;
                        textBox_x2.Visible = true;
                        label7.Visible = true;
                        counterSplits2.Visible = true;
                        buttonDrawPlot3D.Visible = true;
                        break;
                    default:
                        return;
                }
            }


            switch (comboBox3.SelectedItem.ToString())
            {
                case "Изометрическая":
                    projection = 1;
                    break;
                case "Ортогональная на YoZ":
                    projection = 2;
                    break;
                case "Ортогональная на XoZ":
                    projection = 3;
                    break;
                case "Ортогональная на XoY":
                    projection = 4;
                    break;
                case "Перспективная":
                    projection = 5;
                    break;
                default:
                    return;
            }

            ClearWithout();

            draw_model();

            pictureBox1.Image = pictureBox1.Image;
        }

        //очистить
        private void button1_Click(object sender, EventArgs e)
        {
            Clear();
        }

        //масштабирование
        private void button3_Click(object sender, EventArgs e)
        {
            double ind_scale = Double.Parse(textBox5.Text);
            pol.scale(ind_scale);
            ClearWithout();

            draw_model();

            pictureBox1.Image = pictureBox1.Image;

            return;
        }

        //смещение
        private void button4_Click(object sender, EventArgs e)
        {
            double x = Double.Parse(textBox6.Text);
            double y = Double.Parse(textBox7.Text);
            double z = Double.Parse(textBox8.Text);

            pol.shift(x, y, z);
            ClearWithout();

            draw_model();

            pictureBox1.Image = pictureBox1.Image;
        }

        //отражение
        private void button5_Click(object sender, EventArgs e)
        {
            int reflec;

            switch (comboBox2.Text)
            {
                case "X":
                    reflec = 0;
                    break;
                case "Y":
                    reflec = 1;
                    break;
                case "Z":
                    reflec = 2;
                    break;
                default:
                    return;
            }

            pol.reflection(reflec);
            ClearWithout();

            draw_model();

            pictureBox1.Image = pictureBox1.Image;
        }

        //поворот вокруг произвольной прямой
        private void button6_Click(object sender, EventArgs e)
        {
            double x1 = Double.Parse(textBoxX1.Text);
            double y1 = Double.Parse(textBoxY1.Text);
            double z1 = Double.Parse(textBoxZ1.Text);

            double x2 = Double.Parse(textBoxX2.Text);
            double y2 = Double.Parse(textBoxY2.Text);
            double z2 = Double.Parse(textBoxZ2.Text);

            double[,] start_point = { { x1, y1, z1 } };
            double[,] line = { { x2 - x1, y2 - y1, z2 - z1, 1 } };

            double angle = (Convert.ToDouble(textBoxAngle.Text) / 180) * Math.PI;
            pol.rotation(start_point, line, angle);
            ClearWithout();

            draw_model();

            pictureBox1.Image = pictureBox1.Image;
        }

        //поворот вокруг прямой, проходящей через начало координат и центр объекта
        private void button7_Click(object sender, EventArgs e)
        {
            double[,] start_point = { { 0, 0, 0 } };
            double[,] line = { { pol.center.X, pol.center.Y, pol.center.Z, 1 } };
            double angle = (Convert.ToDouble(textBoxAngle.Text) / 180) * Math.PI;
            pol.rotation(start_point, line, angle);
            ClearWithout();

            draw_model();

            pictureBox1.Image = pictureBox1.Image;
        }

        // Сохранить модель в файл
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (pol != null)
            {
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Файл модели|*.model";
                    saveFileDialog.Title = "Сохранить модель";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pol.WriteToFile(saveFileDialog.FileName);
                        MessageBox.Show("Модель сохранена");
                    }
                }
            }
            else
            {
                MessageBox.Show("Модель не задана");
            }
        }

        // Загрузить модель из файла
        public void LoadFromFile()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файл модели|*.model";
                openFileDialog.Title = "Открыть модель";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pol = Models.Polyhedron.ReadFromFile(openFileDialog.FileName);
                }
            }
        }

        // Загрузить файл с координатами образующей для фигуры вращения
        public void LoadSolid()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файл образующей|*.txt";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    forming = new List<Point3D>();
                    string text;
                    using (TextReader reader = File.OpenText(openFileDialog.FileName))
                    {
                        while ((text = reader.ReadLine()) != null)
                        {
                            string[] bits = text.Split(',');
                            float x = float.Parse(bits[0]);
                            float y = float.Parse(bits[1]);
                            float z = float.Parse(bits[2]);
                            forming.Add(new Point3D(x, y, z));
                        }
                    }
                }
                else
                {
                    label5.Visible = false;
                    label6.Visible = false;
                    selectorAxis.Visible = false;
                    counterSplits.Visible = false;
                    buttonDrawSolid.Visible = false;
                }
            }
        }

        private void ButtonDrawSolid_Click(object sender, EventArgs e)
        {
            label5.Visible = false;
            label6.Visible = false;
            selectorAxis.Visible = false;
            counterSplits.Visible = false;
            buttonDrawSolid.Visible = false;

            var splits = Int32.Parse(counterSplits.Text);
            pol = new Models.SolidOfRevolution(forming, splits, selectorAxis.SelectedIndex, pictureBox1.Height / 2);
            ClearWithout();
            draw_model();
            pictureBox1.Image = pictureBox1.Image;

        }

        private void ButtonDrawPlot3D_Click(object sender, EventArgs e)
        {
            selectorFunc.Visible = false;
            label8.Visible = false;
            textBox_x1.Visible = false;
            textBox_x2.Visible = false;
            label7.Visible = false;
            counterSplits2.Visible = false;
            buttonDrawPlot3D.Visible = false;
            var x1 = Double.Parse(textBox_x1.Text);
            var x2 = Double.Parse(textBox_x2.Text);
            var splits = Int32.Parse(counterSplits2.Text);
            Func<double, double, double> f = (x, y) => x + y;
            switch (selectorFunc.SelectedIndex)
            {
                case 0:
                    f = (x, y) => Math.Cos(x) + Math.Sin(y);
                    break;
                case 1:
                    f = (x, y) => (x + y) * (x + y);
                    break;
                case 2:
                    f = (x, y) => Math.Cos(x + y);
                    break;
                case 3:
                    f = (x, y) => Math.Sqrt(x + y);
                    break;
            }


            pol = new Models.Plot3D(f, x1, x2, splits, pictureBox1.Height / 2);
            ClearWithout();
            draw_model();
            pictureBox1.Image = pictureBox1.Image;
        }
    }
}