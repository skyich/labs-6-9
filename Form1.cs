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
        List<Models.Polyhedron> pols = new List<Models.Polyhedron>();
        static Pen col;
        static int projection = 1;
        static public Graphics g;
        public Bitmap bmp;

        public List<Point3D> forming; // образующая для фигру вращения
        public int lastModel = -1; // форма последней нарисованной фигуры
        private Camera camera;
        private List<Models.Polyhedron> pols_backup;
        Models.Polyhedron axis_backup;
        private bool PolsChanged = false;

        public List<Color> colors_of_pols = new List<Color>();
        public Bitmap image;

        public Form1()
        {
            camera = Camera.getInstance();
            col = new Pen(Color.Black);
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp = (Bitmap)pictureBox1.Image;
            Clear();
            pictureBox1.Image = bmp;
            comboBox3.SelectedIndex = 0;
            selectorAxis.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
        }

        public void Clear()
        {
            colors_of_pols.Clear();
            pols.Clear();
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);

            axis.shift(100, 100, 100);
            axis.Display(projection);

            g.DrawLine(new Pen(Color.Blue), axis.edges[0].Item1, axis.edges[0].Item2); // OX
            g.DrawLine(new Pen(Color.Red), axis.edges[1].Item1, axis.edges[1].Item2); // OY
            g.DrawLine(new Pen(Color.Green), axis.edges[2].Item1, axis.edges[2].Item2); // OZ

            axis.shift(-100, -100, -100);
            pictureBox1.Image = pictureBox1.Image;
        }

        public void ClearWithout()
        {
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = pictureBox1.Image;
        }

        public void draw_models()
        {
            if (radioButton5.Checked)
            {
                texture_overlay();
                return;
            }
            if (radioButton3.Checked)
            {
                shading();
                return;
            }
            if (radioButton1.Checked)
            {
                Z_buf();
                return;
            }
            axis.shift(100, 100, 100);
            foreach (var pol in pols)
                pol.shift(100, 100, 100);

            if (projection == 5)
            {
                foreach (var pol in pols)
                    pol.Display(camera.perspective);
                axis.Display(camera.perspective);
            }
            else
            {
                foreach (var pol in pols)
                    pol.Display(projection);
                axis.Display(projection);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias; // сглаживание
            foreach(var pol in pols)
                foreach (var i in pol.edges)
                    g.DrawLine(col, i.Item1, i.Item2);

            //foreach (var i in axis.edges)
               // g.DrawLine(col, i.Item1, i.Item2);

            g.DrawLine(new Pen(Color.Blue), axis.edges[0].Item1, axis.edges[0].Item2); // OX
            g.DrawLine(new Pen(Color.Red), axis.edges[1].Item1, axis.edges[1].Item2); // OY
            g.DrawLine(new Pen(Color.Green), axis.edges[2].Item1, axis.edges[2].Item2); // OZ

            axis.shift(-100, -100, -100);
            foreach(var pol in pols)
               pol.shift(-100, -100, -100);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
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
 
            draw_models();

            pictureBox1.Image = pictureBox1.Image;
        }

        // добавляет новую модель на picturebox
        private void draw_new_model(Models.Polyhedron p)
        {
            colors_of_pols.Add(pictureBox2.BackColor);
            ClearWithout();
            pols.Add(p);
            draw_models();
            pictureBox1.Image = pictureBox1.Image;
        }

        //добавить новый объект
        private void button2_Click(object sender, EventArgs e)
        {
            Models.Polyhedron pol = new Models.Polyhedron();
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Гексаэдр":
                    pol = new Models.cube(pictureBox1.Height / 2);
                    draw_new_model(pol);
                    break;
                case "Тетраэдр":
                    pol = new Models.Tetrahedron(pictureBox1.Height / 2);
                    draw_new_model(pol);
                    break;
                case "Октаэдр":
                    pol = new Models.Octahedron(pictureBox1.Height / 2);
                    draw_new_model(pol);
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

        //очистить
        private void button1_Click(object sender, EventArgs e)
        {
            Clear();
        }

        //масштабирование
        private void button3_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox4.Text) < 0 || Convert.ToInt32(textBox4.Text) >= pols.Count)
                return;

            Models.Polyhedron pol = pols[Convert.ToInt32(textBox4.Text)];

            double ind_scale = Double.Parse(textBox5.Text);
            pol.scale(ind_scale);
            ClearWithout();

            draw_models();

            pictureBox1.Image = pictureBox1.Image;

            return;
        }

        //смещение
        private void button4_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox4.Text) < 0 || Convert.ToInt32(textBox4.Text) >= pols.Count)
                return;

            Models.Polyhedron pol = pols[Convert.ToInt32(textBox4.Text)];

            double x = Double.Parse(textBox6.Text);
            double y = Double.Parse(textBox7.Text);
            double z = Double.Parse(textBox8.Text);

            pol.shift(x, y, z);
            ClearWithout();

            draw_models();

            pictureBox1.Image = pictureBox1.Image;
        }

        //отражение
        private void button5_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox4.Text) < 0 || Convert.ToInt32(textBox4.Text) >= pols.Count)
                return;

            Models.Polyhedron pol = pols[Convert.ToInt32(textBox4.Text)];

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

            draw_models();

            pictureBox1.Image = pictureBox1.Image;
        }

        //поворот вокруг произвольной прямой
        private void button6_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox4.Text) < 0 || Convert.ToInt32(textBox4.Text) >= pols.Count)
                return;

            Models.Polyhedron pol = pols[Convert.ToInt32(textBox4.Text)];

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

            draw_models();

            pictureBox1.Image = pictureBox1.Image;

            //textBox9.Text = pols[0].center.X + " " + pols[0].center.Y + " " + pols[0].center.Z;
        }

        //поворот вокруг прямой, проходящей через начало координат и центр объекта
        private void button7_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox4.Text) < 0 || Convert.ToInt32(textBox4.Text) >= pols.Count)
                return;

            Models.Polyhedron pol = pols[Convert.ToInt32(textBox4.Text)];

            double[,] start_point = { { 0, 0, 0 } };
            double[,] line = { { pol.center.X, pol.center.Y, pol.center.Z, 1 } };
            double angle = (Convert.ToDouble(textBoxAngle.Text) / 180) * Math.PI;
            pol.rotation(start_point, line, angle);
            ClearWithout();

            draw_models();

            pictureBox1.Image = pictureBox1.Image;
        }

        // Сохранить модель в файл
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox4.Text) < 0 || Convert.ToInt32(textBox4.Text) >= pols.Count)
                return;

            Models.Polyhedron pol = pols[Convert.ToInt32(textBox4.Text)];

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
            Models.Polyhedron pol = new Models.Polyhedron();

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файл модели|*.model";
                openFileDialog.Title = "Открыть модель";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pol = Models.Polyhedron.ReadFromFile(openFileDialog.FileName);
                    
                }
            }

            draw_new_model(pol);
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

            Models.Polyhedron pol;

            label5.Visible = false;
            label6.Visible = false;
            selectorAxis.Visible = false;
            counterSplits.Visible = false;
            buttonDrawSolid.Visible = false;

            var splits = Int32.Parse(counterSplits.Text);
            pol = new Models.SolidOfRevolution(forming, splits, selectorAxis.SelectedIndex, pictureBox1.Height / 2);

            draw_new_model(pol);

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


            Models.Polyhedron pol = new Models.Plot3D(f, x1, x2, splits, pictureBox1.Height / 2);

            draw_new_model(pol);
        }

        //удалить нелицевые грани
        private void button8_Click(object sender, EventArgs e)
        {
            Models.PointPol review_vector;

            if (comboBox4.SelectedItem.ToString() != "По проекции")
            {
                review_vector = new Models.PointPol(0, 0, 0);
                review_vector.X = Convert.ToDouble(textBox1.Text);
                review_vector.Y = Convert.ToDouble(textBox2.Text);
                review_vector.Z = Convert.ToDouble(textBox3.Text);

            }
            else
            {
                switch (comboBox3.SelectedItem.ToString())
                {
                    case "Изометрическая":
                        review_vector = new Models.PointPol(-1, 1, -1);
                        break;
                    case "Ортогональная на YoZ":
                        review_vector = new Models.PointPol(-1, 0, 0);
                        break;
                    case "Ортогональная на XoZ":
                        review_vector = new Models.PointPol(0, 1, 0);
                        break;
                    case "Ортогональная на XoY":
                        review_vector = new Models.PointPol(0, 0, -1);
                        break;
                    case "Перспективная":
                        return;
                    default:
                        return;
                }
            }

            if (Convert.ToInt32(textBox4.Text) < 0 || Convert.ToInt32(textBox4.Text) >= pols.Count)
                return;

            Models.Polyhedron pol = pols[Convert.ToInt32(textBox4.Text)];

            List<bool> visible_polyg = pol.non_face_removal(review_vector);//проверяем, какие грани лицевые
            List<bool> visible_edges = new List<bool>();

            //ребро видимо только тогда, когда есть хотя бы одна лицевая грань, которая ограничена этим ребром
            for (int i = 0; i < pol.edges.Count; i++)
                visible_edges.Add(false);

            for (int i = 0; i < pol.polygons.Count; i++)
                if (visible_polyg[i])
                    foreach (int j in pol.polygons[i].edges)
                        visible_edges[j] = true;

            ClearWithout();

            axis.shift(100, 100, 100);
            pol.shift(100, 100, 100);

            g.SmoothingMode = SmoothingMode.AntiAlias; // сглаживание
            for (int i = 0; i < pol.edges.Count; i++)
            {
                if (visible_edges[i])
                    g.DrawLine(col, pol.edges[i].Item1, pol.edges[i].Item2);
            }

            //foreach (var i in axis.edges)
            //g.DrawLine(col, i.Item1, i.Item2);

            g.DrawLine(new Pen(Color.Blue), axis.edges[0].Item1, axis.edges[0].Item2); // OX
            g.DrawLine(new Pen(Color.Red), axis.edges[1].Item1, axis.edges[1].Item2); // OY
            g.DrawLine(new Pen(Color.Green), axis.edges[2].Item1, axis.edges[2].Item2); // OZ

            axis.shift(-100, -100, -100);
            pol.shift(-100, -100, -100);

            pictureBox1.Image = pictureBox1.Image;
        }

        //Z-буфер
        private void Z_buf()
        {
            ClearWithout();

            axis.shift(100, 100, 100);
            axis.Display(projection);

            g.DrawLine(new Pen(Color.Blue), axis.edges[0].Item1, axis.edges[0].Item2); // OX
            g.DrawLine(new Pen(Color.Red), axis.edges[1].Item1, axis.edges[1].Item2); // OY
            g.DrawLine(new Pen(Color.Green), axis.edges[2].Item1, axis.edges[2].Item2); // OZ

            axis.shift(-100, -100, -100);

            //comboBox3.SelectedIndex = 0;
            comboBox3.SelectedIndex = 3;
            Models.PointPol review_vector = new Models.PointPol(0,0,-1);

            List<List<double>> Z_buf = new List<List<double>>();

            for (int i = 0; i < pictureBox1.Image.Width; i++)
            {
                Z_buf.Add(new List<double>());
                for (int j = 0; j < pictureBox1.Image.Height; j++)
                    Z_buf[i].Add(double.MinValue);
            }

            for (int i = 0; i < pols.Count; i++)
            {
                //обработка одного объекта
                Models.Polyhedron pol = pols[i];

                List<bool> visible_polyg = pol.non_face_removal(review_vector);


                for (int j = 0; j < pol.polygons.Count; j++)
                {
                    //обработка одного видимого полигона
                    if (visible_polyg[j] == false)
                        continue;

                    int left_up_point, right_up_point;

                    left_up_point = right_up_point = pol.find_top_point(j);

                    Dictionary<int, Models.Point_with_neighbors> view = pol.new_view(j);

                    int left_down_point = view[left_up_point].left, right_down_point = view[right_up_point].right;

                    if (pol.vertices[left_down_point].X > pol.vertices[right_down_point].X)
                    {
                        int x = left_down_point;
                        left_down_point = right_down_point;
                        right_down_point = x;
                    }

                    double current_y = pol.vertices[left_up_point].Y;
                    double step = 0.3;

                    while (true)
                    {
                        //обработка одной строки

                        if (current_y <= pol.vertices[left_down_point].Y)
                        {
                            int old_left = left_down_point;

                            if (view[left_down_point].right == left_up_point)
                                left_down_point = view[left_down_point].left;
                            else
                                left_down_point = view[left_down_point].right;

                            left_up_point = old_left;
                        }

                        if (current_y <= pol.vertices[right_down_point].Y)
                        {
                            int old_right = right_down_point;

                            if (view[right_down_point].right == right_up_point)
                                right_down_point = view[right_down_point].left;
                            else
                                right_down_point = view[right_down_point].right;

                            right_up_point = old_right;
                        }

                        if (current_y <= pol.vertices[right_down_point].Y || current_y <= pol.vertices[left_down_point].Y)
                            break;

                        double x1, x2, x3, x4, y1, y2, y3, y4, z1, z2, z3, z4;
                        x1 = pol.vertices[left_up_point].X;
                        y1 = pol.vertices[left_up_point].Y;
                        z1 = pol.vertices[left_up_point].Z;

                        x2 = pol.vertices[left_down_point].X;
                        y2 = pol.vertices[left_down_point].Y;
                        z2 = pol.vertices[left_down_point].Z;

                        x3 = pol.vertices[right_up_point].X;
                        y3 = pol.vertices[right_up_point].Y;
                        z3 = pol.vertices[right_up_point].Z;

                        x4 = pol.vertices[right_down_point].X;
                        y4 = pol.vertices[right_down_point].Y;
                        z4 = pol.vertices[right_down_point].Z;

                        double xa, za, xb, zb;

                        xa = x1 + (x2 - x1) * ((current_y - y1) / (y2 - y1));
                        xb = x3 + (x4 - x3) * ((current_y - y3) / (y4 - y3));
                        za = z1 + (z2 - z1) * ((current_y - y1) / (y2 - y1));
                        zb = z3 + (z4 - z3) * ((current_y - y3) / (y4 - y3));

                        if (xa > xb)
                        {
                            double old_xa = xa, old_za = za;
                            xa = xb;
                            za = zb;
                            xb = old_xa;
                            zb = old_za;

                        }

                        for (double current_x = xa; current_x <= xb; current_x += step)
                        {
                            double current_z = za + (zb - za) * ((current_x - xa) / (xb - xa)) + pol.center.Z;

                            int x, y;

                            x = Convert.ToInt32(current_x + pol.center.X) + 100;

                            y = Convert.ToInt32(current_y + pol.center.Y) + 100;

                            if (x < 0 ||
                                x >= pictureBox1.Image.Width ||
                                y < 0 ||
                                y >= pictureBox1.Image.Height)
                                continue;

                            if (current_z > Z_buf[x][y])
                            { //этот участок можно менять
                                Z_buf[x][y] = current_z;

                                Color col;

                                //в частности настройки цвета
                                col = colors_of_pols[i];

                                ((Bitmap)pictureBox1.Image).SetPixel(x, y, col);

                                //и вот тут возникают проблемы из-за ширины границ
                                if (Math.Abs(current_x - xa) < 1.2 || Math.Abs(xb - current_x) < 1.2)
                                  ((Bitmap)pictureBox1.Image).SetPixel(x, y, Color.Black);

                            }
                                    
                        }
                        current_y -= step;
                    }
                }
            }
            pictureBox1.Image = pictureBox1.Image;
        }

        private void CameraButton_Click(object sender, EventArgs e)
        {
            camera.Refresh();
            pols = pols_backup;
            axis = axis_backup as Models.Axis;
            PolsChanged = false;
            ClearWithout();
            draw_models();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (projection != 5)
                return;

            switch (e.KeyChar)
            {
                // смещения
                case 'a':
                    camera.Shift(0);
                    break;
                case 'd':
                    camera.Shift(1);
                    break;
                case 'q':
                    camera.Shift(2);
                    break;
                case 'e':
                    camera.Shift(3);
                    break;
                case 's':
                    camera.Shift(4);
                    break;
                case 'w':
                    camera.Shift(5);
                    break;

                // повороты
                case 't':
                    RotateAll(0);
                    break;
                case 'u':
                    RotateAll(1);
                    break;
                case 'g':
                    RotateAll(2);
                    break;
                case 'j':
                    RotateAll(3);
                    break;
                case 'h':
                    RotateAll(4);
                    break;
                case 'y':
                    RotateAll(5);
                    break;
            }
            ClearWithout();
            draw_models();
        }

        // повернуть все фигуры вокруг камеры
        public void RotateAll(int direction)
        {
            if (PolsChanged == false)
            {
                pols_backup = new List<Models.Polyhedron>();
                foreach (var x in pols) {
                    pols_backup.Add(x.DeepCopy());
                }
                axis_backup = axis.DeepCopy();
                PolsChanged = true;
            }
            double x1 = 0;
            double y1 = 0;
            double z1 = 0;

            double x2 = 0;
            double y2 = 0;
            double z2 = 0;
            double angle = 0.01;

            switch (direction)
            {
                case 0: // влево по х
                    x2 = camera.position.X + 1;
                    angle *= -1;
                    break;
                case 1: // вправо по x
                    x2 = camera.position.X + 1;
                    break;
                case 2: // влево по y
                    y2 = camera.position.Y + 1;
                    angle *= -1;
                    break;
                case 3: // вправо по y
                    y2 = camera.position.Y + 1;
                    break;
                case 4: // вниз по z
                    z2 = camera.position.Z + 1;
                    angle *= -1;
                    break;
                case 5: // вверх по z
                    z2 = camera.position.Z + 1;
                    break;
            }

            double[,] start_point = { { x1, y1, z1 } };
            double[,] line = { { x2 - x1, y2 - y1, z2 - z1, 1 } };

            foreach (var x in pols)
                x.rotation(start_point, line, angle);
            axis.rotation(start_point, line, angle);

        }

        //включение z-буфера
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            draw_models();
        }

        //выбор цвета
        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            // Keeps the user from selecting a custom color.
            MyDialog.AllowFullOpen = true;
            // Allows the user to get help. (The default is false.)
            MyDialog.ShowHelp = true;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.BackColor = MyDialog.Color;
            }
        }

        //затенение
        private void shading()
        {
            ClearWithout();

            axis.shift(100, 100, 100);
            axis.Display(projection);

            g.DrawLine(new Pen(Color.Blue), axis.edges[0].Item1, axis.edges[0].Item2); // OX
            g.DrawLine(new Pen(Color.Red), axis.edges[1].Item1, axis.edges[1].Item2); // OY
            g.DrawLine(new Pen(Color.Green), axis.edges[2].Item1, axis.edges[2].Item2); // OZ

            axis.shift(-100, -100, -100);

            //comboBox3.SelectedIndex = 0;
            comboBox3.SelectedIndex = 3;
            Models.PointPol review_vector = new Models.PointPol(0, 0, -1);

            List<List<double>> Z_buf = new List<List<double>>();

            for (int i = 0; i < pictureBox1.Image.Width; i++)
            {
                Z_buf.Add(new List<double>());
                for (int j = 0; j < pictureBox1.Image.Height; j++)
                    Z_buf[i].Add(double.MinValue);
            }

            for (int i = 0; i < pols.Count; i++)
            {
                //обработка одного объекта
                Models.Polyhedron pol = pols[i];

                List<bool> visible_polyg = pol.non_face_removal(review_vector);
                Dictionary<int, bool> lighted_vertices = pol.lighted_vertices(Convert.ToDouble(textBox9.Text), Convert.ToDouble(textBox10.Text), Convert.ToDouble(textBox11.Text));


                for (int j = 0; j < pol.polygons.Count; j++)
                {
                    //обработка одного видимого полигона
                    if (visible_polyg[j] == false)
                        continue;

                    int left_up_point, right_up_point;

                    left_up_point = right_up_point = pol.find_top_point(j);

                    Dictionary<int, Models.Point_with_neighbors> view = pol.new_view(j);

                    int left_down_point = view[left_up_point].left, right_down_point = view[right_up_point].right;

                    if (pol.vertices[left_down_point].X > pol.vertices[right_down_point].X)
                    {
                        int x = left_down_point;
                        left_down_point = right_down_point;
                        right_down_point = x;
                    }

                    double current_y = pol.vertices[left_up_point].Y;
                    double step = 0.3;

                    while (true)
                    {
                        //обработка одной строки

                        if (current_y <= pol.vertices[left_down_point].Y)
                        {
                            int old_left = left_down_point;

                            if (view[left_down_point].right == left_up_point)
                                left_down_point = view[left_down_point].left;
                            else
                                left_down_point = view[left_down_point].right;

                            left_up_point = old_left;
                        }

                        if (current_y <= pol.vertices[right_down_point].Y)
                        {
                            int old_right = right_down_point;

                            if (view[right_down_point].right == right_up_point)
                                right_down_point = view[right_down_point].left;
                            else
                                right_down_point = view[right_down_point].right;

                            right_up_point = old_right;
                        }

                        if (current_y <= pol.vertices[right_down_point].Y || current_y <= pol.vertices[left_down_point].Y)
                            break;

                        double x1, x2, x3, x4, y1, y2, y3, y4, z1, z2, z3, z4;
                        x1 = pol.vertices[left_up_point].X;
                        y1 = pol.vertices[left_up_point].Y;
                        z1 = pol.vertices[left_up_point].Z;

                        x2 = pol.vertices[left_down_point].X;
                        y2 = pol.vertices[left_down_point].Y;
                        z2 = pol.vertices[left_down_point].Z;

                        x3 = pol.vertices[right_up_point].X;
                        y3 = pol.vertices[right_up_point].Y;
                        z3 = pol.vertices[right_up_point].Z;

                        x4 = pol.vertices[right_down_point].X;
                        y4 = pol.vertices[right_down_point].Y;
                        z4 = pol.vertices[right_down_point].Z;

                        double xa, za, xb, zb;

                        xa = x1 + (x2 - x1) * ((current_y - y1) / (y2 - y1));
                        xb = x3 + (x4 - x3) * ((current_y - y3) / (y4 - y3));
                        za = z1 + (z2 - z1) * ((current_y - y1) / (y2 - y1));
                        zb = z3 + (z4 - z3) * ((current_y - y3) / (y4 - y3));

                        if (xa > xb)
                        {
                            double old_xa = xa, old_za = za;
                            xa = xb;
                            za = zb;
                            xb = old_xa;
                            zb = old_za;

                        }

                        for (double current_x = xa; current_x <= xb; current_x += step)
                        {
                            double current_z = za + (zb - za) * ((current_x - xa) / (xb - xa)) + pol.center.Z;

                            int x, y;

                            x = Convert.ToInt32(current_x + pol.center.X) + 100;

                            y = Convert.ToInt32(current_y + pol.center.Y) + 100;

                            if (x < 0 ||
                                x >= pictureBox1.Image.Width ||
                                y < 0 ||
                                y >= pictureBox1.Image.Height)
                                continue;

                            if (current_z > Z_buf[x][y])
                            { //этот участок можно менять
                                Z_buf[x][y] = current_z;

                                Color col;

                                //в частности настройки цвета
                                col = colors_of_pols[i];
                               
                                double length_edg = Models.length_btw_points(pol.vertices[0], pol.vertices[1]);
                                Models.PointPol current_p = new Models.PointPol(current_x, current_y, current_z);

                                List<int> including_vertices = pol.including_vertices(j);

                                foreach (int vert in including_vertices)
                                {
                                    int A_R, A_G, A_B, B_R, B_G, B_B;

                                    Models.PointPol p_a = new Models.PointPol(xa, current_y, za);
                                    Models.PointPol p_b = new Models.PointPol(xb, current_y, zb);

                                    double len_to_a1 = Models.length_btw_points(p_a, pol.vertices[left_up_point]);
                                    double len_to_a2 = Models.length_btw_points(p_a, pol.vertices[left_down_point]);

                                    double len_to_b1 = Models.length_btw_points(p_b, pol.vertices[right_up_point]);
                                    double len_to_b2 = Models.length_btw_points(p_b, pol.vertices[right_down_point]);

                                    int A1, G1, B1, A2, G2, B2;

                                    // левая точка ========================================

                                    if (lighted_vertices[left_up_point])
                                    {
                                        A1 = col.A; G1 = col.G; B1 = col.B;
                                    }
                                    else
                                    {
                                        A1 = 0; G1 = 0; B1 = 0;
                                    }

                                    if (lighted_vertices[left_down_point])
                                    {
                                        A2 = col.A; G2 = col.G; B2 = col.B;
                                    }
                                    else
                                    {
                                        A2 = 0; G2 = 0; B2 = 0;
                                    }

                                    double k;
                                    k = len_to_a1/ length_edg;
                                    

                                    
                                        A_R = Convert.ToInt32(A1 * (1-k) + A2 * (k));
                                        A_G = Convert.ToInt32(G1 * (1-k) + G2 * (k));
                                        A_B = Convert.ToInt32(B1 * (1-k) + B2 * (k));
                                    
                            
                                    //правая точка ========================================

                                    if (lighted_vertices[right_up_point])
                                    {
                                        A1 = col.A; G1 = col.G; B1 = col.B;
                                    }
                                    else
                                    {
                                        A1 = 0; G1 = 0; B1 = 0;
                                    }

                                    if (lighted_vertices[right_down_point])
                                    {
                                        A2 = col.A; G2 = col.G; B2 = col.B;
                                    }
                                    else
                                    {
                                        A2 = 0; G2 = 0; B2 = 0;
                                    }

                                    k = len_to_b1 / length_edg ;
                                    

                                    B_R = Convert.ToInt32(A1 * (1-k) + A2 * (k));
                                    B_G = Convert.ToInt32(G1 * (1-k) + G2 * (k));
                                    B_B = Convert.ToInt32(B1 * (1-k) + B2 * (k));


                                    double len_from_a_to_p = Models.length_btw_points(p_a, current_p);
                                    double len_from_b_to_p = Models.length_btw_points(p_b, current_p);

                                    double len_from_a_to_b = Models.length_btw_points(p_a, p_b);

                                    k = len_from_a_to_p / len_from_a_to_b;
                                    int R_P = Convert.ToInt32((A_R * (1 - k) + B_R * (k)));
                                    int G_P = Convert.ToInt32((A_G * (1 - k) + B_G * (k)));
                                    int B_P = Convert.ToInt32((A_B * (1 - k) + B_B * (k)));

                                    R_P = Math.Min(255, R_P);
                                    G_P = Math.Min(255, G_P);
                                    B_P = Math.Min(255, B_P);

                                    R_P = Math.Max(0, R_P);
                                    G_P = Math.Max(0, G_P);
                                    B_P = Math.Max(0, B_P);
                                    col = Color.FromArgb(R_P, G_P, B_P);

                                   // double length_to_vert = Models.length_btw_points(pol.vertices[vert], current_p);
                                    //if (lighted_vertices[vert])
                                   // {
                                        //A = A + (length_edg / length_to_vert)/10;
                                    //}
                                    //else
                                    //{
                                     //   A = A - (length_edg / length_to_vert)/4;
                                    //}
                                }

                               /* A = A * 255;

                                double old_r = col.R, old_g = col.G, old_b = col.B;
                                double R = col.R, G = col.G, B = col.B;

                                R += A;
                                G += A;
                                B += A;

                                R = Math.Max(0, R);
                                R = Math.Min(255, R);
                                G = Math.Max(0, G);
                                G = Math.Min(255, G);
                                B = Math.Max(0, B);
                                B = Math.Min(255, B);

                                if (R > old_r)
                                    R = old_r;
                                if (G > old_g)
                                    G = old_g;
                                if (B > old_b)
                                    B = old_b;*/

                               // col = Color.FromArgb(Convert.ToInt32(R), Convert.ToInt32(G), Convert.ToInt32(B));
                                //col = Color.FromArgb(Convert.ToInt32(A * 255), col);

                                ((Bitmap)pictureBox1.Image).SetPixel(x, y, col);

                                //и вот тут возникают проблемы из-за ширины границ
                                //if (Math.Abs(current_x - xa) < 1.2 || Math.Abs(xb - current_x) < 1.2)
                                  //  ((Bitmap)pictureBox1.Image).SetPixel(x, y, Color.Black);

                            }

                        }
                        current_y -= step;
                    }
                }
            }
            pictureBox1.Image = pictureBox1.Image;
        }

        //затенение
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            draw_models();
        }

        //загрузка текстуры
        private void button9_Click(object sender, EventArgs e)
        {
            // диалог для выбора файла
            OpenFileDialog ofd = new OpenFileDialog();
            // фильтр форматов файлов
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            // если в диалоге была нажата кнопка ОК
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // загружаем изображение
                    image = new Bitmap(ofd.FileName);
                }
                catch // в случае ошибки выводим MessageBox
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //наложение текстуры
        private void texture_overlay()
        {
            ClearWithout();

            axis.shift(100, 100, 100);
            axis.Display(projection);

            g.DrawLine(new Pen(Color.Blue), axis.edges[0].Item1, axis.edges[0].Item2); // OX
            g.DrawLine(new Pen(Color.Red), axis.edges[1].Item1, axis.edges[1].Item2); // OY
            g.DrawLine(new Pen(Color.Green), axis.edges[2].Item1, axis.edges[2].Item2); // OZ

            axis.shift(-100, -100, -100);

            //comboBox3.SelectedIndex = 0;
            comboBox3.SelectedIndex = 3;
            Models.PointPol review_vector = new Models.PointPol(0, 0, -1);

            List<List<double>> Z_buf = new List<List<double>>();

            for (int i = 0; i < pictureBox1.Image.Width; i++)
            {
                Z_buf.Add(new List<double>());
                for (int j = 0; j < pictureBox1.Image.Height; j++)
                    Z_buf[i].Add(double.MinValue);
            }

            for (int i = 0; i < pols.Count; i++)
            {
                //обработка одного объекта
                Models.Polyhedron pol = pols[i];

                List<bool> visible_polyg = pol.non_face_removal(review_vector);
                Dictionary<int, bool> lighted_vertices = pol.lighted_vertices(Convert.ToDouble(textBox9.Text), Convert.ToDouble(textBox10.Text), Convert.ToDouble(textBox11.Text));


                for (int j = 0; j < pol.polygons.Count; j++)
                {
                    //обработка одного видимого полигона
                    if (visible_polyg[j] == false)
                        continue;

                    List<int> including_vertices = pol.including_vertices(j);

                    int left_up_point, right_up_point;

                    left_up_point = right_up_point = pol.find_top_point(j);

                    Dictionary<int, Models.Point_with_neighbors> view = pol.new_view(j);

                    int left_down_point = view[left_up_point].left, right_down_point = view[right_up_point].right;

                    if (pol.vertices[left_down_point].X > pol.vertices[right_down_point].X)
                    {
                        int x = left_down_point;
                        left_down_point = right_down_point;
                        right_down_point = x;
                    }

                    double current_y = pol.vertices[left_up_point].Y;
                    double step = 0.3;

                    //==========================================================

                    //точки для нахождения точки пересечения перпендикуляра с прямой, проходящей через две верхние точки полигона
                    Point p1, p2, p3;
                    p1 = new Point(0, int.MaxValue);
                    p2 = new Point(0, int.MaxValue);

                    int p1_ind = -1, p2_ind = -1;

                    //ищем две верхние точки полигона на плоскости
                    foreach (int k in including_vertices)
                    {
                        if (pol.vertices[k].Y < p1.Y)
                        {
                            p2 = p1;
                            p1 = new Point(Convert.ToInt32(pol.vertices[k].X), Convert.ToInt32(pol.vertices[k].Y));
                            p2_ind = p1_ind;
                            p1_ind = k;
                        }
                        else if (pol.vertices[k].Y < p2.Y)
                        {
                            p2 = new Point(Convert.ToInt32(pol.vertices[k].X), Convert.ToInt32(pol.vertices[k].Y));
                            p2_ind = k;
                        }
                    }

                    double height_polyg = 1;

                    //ищем расстояние от самой верхней точки до смежной с ней, которая не является второй верхней
                    foreach (KeyValuePair<int, Models.Point_with_neighbors> q in view)
                    {
                        if (q.Key == p1_ind)
                        {
                            if (q.Value.left != p2_ind)
                            {
                                height_polyg = Math.Sqrt(
                                                        Math.Pow(p1.X - pol.vertices[q.Value.left].X, 2) +
                                                        Math.Pow(p1.Y - pol.vertices[q.Value.left].Y, 2));
                            }
                            else
                            {
                                height_polyg = Math.Sqrt(
                                                        Math.Pow(p1.X - pol.vertices[q.Value.right].X, 2) +
                                                        Math.Pow(p1.Y - pol.vertices[q.Value.right].Y, 2));
                            }
                        }
                    }

                    if (including_vertices.Count == 3)
                    {
                        p2 = new Point(int.MinValue, p1.Y);
                    }

                    while (true)
                    {
                        //обработка одной строки

                        if (current_y <= pol.vertices[left_down_point].Y)
                        {
                            int old_left = left_down_point;

                            if (view[left_down_point].right == left_up_point)
                                left_down_point = view[left_down_point].left;
                            else
                                left_down_point = view[left_down_point].right;

                            left_up_point = old_left;
                        }

                        if (current_y <= pol.vertices[right_down_point].Y)
                        {
                            int old_right = right_down_point;

                            if (view[right_down_point].right == right_up_point)
                                right_down_point = view[right_down_point].left;
                            else
                                right_down_point = view[right_down_point].right;

                            right_up_point = old_right;
                        }

                        if (current_y <= pol.vertices[right_down_point].Y || current_y <= pol.vertices[left_down_point].Y)
                            break;

                        double x1, x2, x3, x4, y1, y2, y3, y4, z1, z2, z3, z4;
                        x1 = pol.vertices[left_up_point].X;
                        y1 = pol.vertices[left_up_point].Y;
                        z1 = pol.vertices[left_up_point].Z;

                        x2 = pol.vertices[left_down_point].X;
                        y2 = pol.vertices[left_down_point].Y;
                        z2 = pol.vertices[left_down_point].Z;

                        x3 = pol.vertices[right_up_point].X;
                        y3 = pol.vertices[right_up_point].Y;
                        z3 = pol.vertices[right_up_point].Z;

                        x4 = pol.vertices[right_down_point].X;
                        y4 = pol.vertices[right_down_point].Y;
                        z4 = pol.vertices[right_down_point].Z;

                        double xa, za, xb, zb;

                        xa = x1 + (x2 - x1) * ((current_y - y1) / (y2 - y1));
                        xb = x3 + (x4 - x3) * ((current_y - y3) / (y4 - y3));
                        za = z1 + (z2 - z1) * ((current_y - y1) / (y2 - y1));
                        zb = z3 + (z4 - z3) * ((current_y - y3) / (y4 - y3));

                        if (xa > xb)
                        {
                            double old_xa = xa, old_za = za;
                            xa = xb;
                            za = zb;
                            xb = old_xa;
                            zb = old_za;

                        }

                        for (double current_x = xa; current_x <= xb; current_x += step)
                        {
                            double current_z = za + (zb - za) * ((current_x - xa) / (xb - xa)) + pol.center.Z;

                            int x, y;

                            x = Convert.ToInt32(current_x + pol.center.X) + 100;

                            y = Convert.ToInt32(current_y + pol.center.Y) + 100;

                            if (x < 0 ||
                                x >= pictureBox1.Image.Width ||
                                y < 0 ||
                                y >= pictureBox1.Image.Height)
                                continue;

                            if (current_z > Z_buf[x][y])
                            { //этот участок можно менять
                                Z_buf[x][y] = current_z;

                                Color col = Color.White;

                                double length_edg = Models.length_btw_points(pol.vertices[0], pol.vertices[1]);

                                p3 = new Point(Convert.ToInt32(current_x), Convert.ToInt32(current_y));


                                double p4_x = 0, p4_y = 0;

                                Utils.fff(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, ref p4_x, ref p4_y);

                                double len_to_up = Math.Sqrt(
                                                           Math.Pow(p4_x - current_x, 2) +
                                                         Math.Pow(p4_y - current_y, 2));

                                int x_in_im = Convert.ToInt32((image.Width / (xb - xa)) * (current_x - xa));
                                int y_in_im = 0;
                                if (!(p4_x == double.MinValue || p4_y == double.MinValue))
                                {
                                    y_in_im = Convert.ToInt32((image.Height / (height_polyg)) * (len_to_up));
                                }

                                y_in_im = Math.Min(image.Height - 1, y_in_im);

                                x_in_im = Math.Min(image.Width - 1, x_in_im);

                                col = image.GetPixel(x_in_im, y_in_im);

                                ((Bitmap)pictureBox1.Image).SetPixel(x, y, col);

                                if (Math.Abs(current_x - xa) < 1.2 || Math.Abs(xb - current_x) < 1.2)
                                    ((Bitmap)pictureBox1.Image).SetPixel(x, y, Color.Black);

                            }

                        }
                        current_y -= step;
                    }
                }
            }
            pictureBox1.Image = pictureBox1.Image;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            draw_models();
        }
    }
}