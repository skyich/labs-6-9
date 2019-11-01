using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Task
{
    public partial class Form1 : Form
    {
        static public double len = 100.003198;

        static public double[,] displayMatrix = new double[4, 4] { { Math.Sqrt(0.5), 0, -Math.Sqrt(0.5), 0 },
                                                                { 1 / Math.Sqrt(6), 2 / Math.Sqrt(6), 1 / Math.Sqrt(6), 0 },
                                                                { 1 / Math.Sqrt(3), -1 / Math.Sqrt(3), 1 / Math.Sqrt(3), 0 },
                                                                { 0, 0, 0, 1 } };

        static public double[,] ortoX = new double[4, 4] { { 0, 0, 0, 0 },
                                                        { 0, 1, 0, 0 },
                                                        { 0, 0, 1, 0 },
                                                        { 0, 0, 0, 1 } };

        static public double[,] ortoY = new double[4, 4] { { 1, 0, 0, 0 },
                                                        { 0, 0, 0, 0 },
                                                        { 0, 0, 1, 0 },
                                                        { 0, 0, 0, 1 } };

        static public double[,] ortoZ = new double[4, 4] { { 1, 0, 0, 0 },
                                                        { 0, 1, 0, 0 },
                                                        { 0, 0, 0, 0 },
                                                        { 0, 0, 0, 1 } };

        static public double[,] identity = new double[4, 4] { { 1, 0, 0, 0 },
                                                           { 0, 1, 0, 0 },
                                                           { 0, 0, 1, 0 },
                                                           { 0, 0, 0, 1 } };

        static public double[,] perspective = new double[4, 4] { { 1, 0, 0, 0 },
                                                           { 0, 1, 0, 0 },
                                                           { 0, 0, 0, 0 },
                                                           { 0, 0, len/50000, 1 } };

        static public double[,] reflectionX = new double[4, 4] { { -1, 0, 0, 0 },
                                                                 { 0, 1, 0, 0 },
                                                                 { 0, 0, 1, 0 },
                                                                 { 0, 0, 0, 1 } };
        static public double[,] reflectionY = new double[4, 4] { { 1, 0, 0, 0 },
                                                                 { 0, -1, 0, 0 },
                                                                 { 0, 0, 1, 0 },
                                                                 { 0, 0, 0, 1 } };
        static public double[,] reflectionZ = new double[4, 4] { { 1, 0, 0, 0 },
                                                                 { 0, 1, 0, 0 },
                                                                 { 0, 0, -1, 0 },
                                                                 { 0, 0, 0, 1 } };

        Polyhedron pol;
        static Pen col;
        static int projection = 1;
        static public Graphics g;
        public Bitmap bmp;

        //для образующей фигуры вращения
        PointF lastPoint;
        bool isMouseDown;
        List<PointF> forming; // образующая


        [Serializable]
        public class PointPol
        {
            public int index;
            public double X, Y, Z, W;

            public PointPol(double x, double y, double z)
            {
                X = x; Y = y; Z = z; W = 1;
            }

            public PointPol(double x, double y, double z, double w)
            {
                X = x; Y = y; Z = z; W = w;
            }

            public PointPol(int ind, double x, double y, double z, double w)
            {
                index = ind; X = x; Y = y; Z = z; W = w;
            }


            //возвращает строку для перемножения матриц
            public double[,] getP()
            {
                return new double[1, 4] { { X, Y, Z, W } };
            }

            //возвращает столбец для перемножения матриц
            public double[,] getPol()
            {
                return new double[4, 1] { { X }, { Y }, { Z }, { W } };
            }

            //возвращает столбец из трех координат
            public double[,] getP1()
            {
                return new double[3, 1] { { X }, { Y }, { Z } };
            }

            //масштабирование
            public PointPol scale(double ind_scale)
            {
                double[,] transfer = new double[4, 4] { { ind_scale, 0, 0, 0 },
                                                        { 0, ind_scale, 0, 0 },
                                                        { 0, 0, ind_scale, 0 },
                                                        { 0, 0, 0, 1 } };

                return translatePol(matrix_mult(getP(), transfer));
            }

            //матрицу с одной строкой превращает в PointPol
            private PointPol translatePol(double[,] f)
            {
                return new PointPol(index, f[0, 0], f[0, 1], f[0, 2], f[0, 3]);
            }

            //матрицу с одним столбцом превращает в PoinPol
            private PointPol translatePol1(double[,] f)
            {
                return new PointPol(index, f[0, 0], f[1, 0], f[2, 0], f[3, 0]);
            }

            //сдвиг
            public PointPol shift(double x, double y, double z)
            {
                double[,] shiftMatrix = new double[4, 4] { { 1, 0, 0, x },
                                                           { 0, 1, 0, y },
                                                           { 0, 0, 1, z },
                                                           { 0, 0, 0, 1 } };
                return translatePol1(matrix_mult(shiftMatrix, getPol()));
            }

            //поворот
            public PointPol roration(int axis, double cos, double sin)
            {
                double[,] R_axis = new double[4, 4] { { 1, 0, 0, 0 },
                                                      { 0, 1, 0, 0 },
                                                      { 0, 0, 1, 0 },
                                                      { 0, 0, 0, 1 } };

                switch (axis)
                {
                    case 0: //вокруг оси Ox
                        R_axis = new double[4, 4] { { 1, 0,   0,    0 },
                                                    { 0, cos, -sin, 0 },
                                                    { 0, sin, cos,  0 },
                                                    { 0, 0,    0,   1 } };
                        break;
                    case 1: //вокруг оси Oy
                        R_axis = new double[4, 4] { { cos, 0, sin,  0 },
                                                    { 0,   1, 0,    0 },
                                                    { -sin, 0, cos, 0 },
                                                    { 0,    0, 0,   1 } };
                        break;
                    case 2: //вокруг оси Oz
                        R_axis = new double[4, 4] { { cos, -sin, 0, 0 },
                                                    { sin, cos,  0, 0 },
                                                    { 0,   0,    1, 0 },
                                                    { 0,   0,    0, 1 } };
                        break;
                    default:
                        break;
                }

                return translatePol(matrix_mult(getP(), R_axis));
            }

            //отражение
            public PointPol reflection(int reflec)
            {
                double[,] R_ref = new double[4, 4] { { 1, 0, 0, 0 },
                                                      { 0, 1, 0, 0 },
                                                      { 0, 0, 1, 0 },
                                                      { 0, 0, 0, 1 } };

                switch (reflec)
                {
                    case 0:
                        R_ref = reflectionX;
                        break;
                    case 1:
                        R_ref = reflectionY;
                        break;
                    case 2:
                        R_ref = reflectionZ;
                        break;
                    default:
                        break;
                }

                return translatePol(matrix_mult(getP(), R_ref));
            }
        }

        [Serializable]
        public class Edge
        {
            public int P1;
            public int P2;
            public Edge(int p1, int p2) { P1 = p1; P2 = p2; }
        }

        [Serializable]
        public class Polygon
        {

            public List<int> points = new List<int>();
            public List<Edge> edges = new List<Edge>();
            public Polygon(List<Edge> edg)
            {
                foreach (var el in edg)
                {
                    edges.Add(el);
                    if (!points.Contains(el.P1))
                        points.Add(el.P1);
                    if (!points.Contains(el.P2))
                        points.Add(el.P2);
                }
            }
            //грани
            public Polygon(List<PointPol> poins)
            {
                foreach (var v in poins)
                    points.Add(v.index);
            }
        }

        [Serializable]
        public class Polyhedron
        {
            [NonSerialized]
            public Form1 _form = new Form1();
            public PointPol center;
            //Список вершин многогранника
            public Dictionary<int, PointPol> vertices; //в классе координаты хранятся относительно центра, как векторы
            public List<Point> vertices2D;
            //по индексу точки возвращет список индексов точек, между которыми есть ребро
            public Dictionary<int, List<int>> neighbors = new Dictionary<int, List<int>>();
            public List<Tuple<Point, Point>> edges;
            public List<Edge> edges3D = new List<Edge>();
            public List<Polygon> polygons = new List<Polygon>();


            //по трем координатам точки и способу отображения возвращает точку с двумя координатами
            public Point GetPointIn2d(int proc, double[,] temp)
            {
                Point result = new Point();

                switch (proc)
                {
                    case 1: //изометрическая 
                        result = new Point((int)Math.Round(temp[0, 0] + center.X), (int)Math.Round(temp[1, 0] + center.Y));
                        break;
                    case 2: //ортографическая на YoZ
                        result = new Point((int)Math.Round(temp[1, 0] + center.Y), (int)Math.Round(temp[2, 0] + center.Z));
                        break;
                    case 3: //ортографическая на XoZ
                        result = new Point((int)Math.Round(temp[0, 0] + center.X), (int)Math.Round(temp[2, 0] + center.Z));
                        break;
                    case 4: //ортографическая на XoY
                        result = new Point((int)Math.Round(temp[0, 0] + center.X), (int)Math.Round(temp[1, 0] + center.Y));
                        break;
                    case 5:
                        result = new Point((int)Math.Round((temp[0, 0] + center.X) / (temp[3, 0] + center.Z * (len / 50000))), (int)Math.Round((temp[1, 0] + center.Y) / (temp[3, 0] + center.Z * (len / 50000))));
                        break;
                    default:
                        break;
                }

                return result;
            }


            public Polyhedron()
            {

            }


            public void draw()
            {
                Display(projection);

                foreach (var i in edges)
                    g.DrawLine(col, i.Item1, i.Item2);
            }

            //заполняет edges концами ребер на плоскости
            public void Display(int proc)
            {
                double[,] dM = new double[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };

                switch (proc)
                {
                    case 1:
                        dM = displayMatrix;
                        break;
                    case 2:
                        dM = ortoX;
                        break;
                    case 3:
                        dM = ortoY;
                        break;
                    case 4:
                        dM = ortoZ;
                        break;
                    case 5:
                        dM = perspective;
                        break;
                    default:
                        break;
                }
                edges = new List<Tuple<Point, Point>>();
                vertices2D = new List<Point>();

                foreach (var e in edges3D)
                {
                    var temp = matrix_mult(dM, vertices[e.P1].getPol());
                    Point temp2d = GetPointIn2d(proc, temp);

                    vertices2D.Add(temp2d);

                    temp = matrix_mult(dM, vertices[e.P2].getPol());
                    temp2d = GetPointIn2d(proc, temp);

                    edges.Add(new Tuple<Point, Point>(temp2d, vertices2D.Last()));
                    vertices2D.Add(temp2d);
                }
            }

            private PointPol translatePol(double[,] f)
            {
                return new PointPol(f[0, 0], f[1, 0], f[2, 0], f[3, 0]);
            }

            //смещение
            public void shift(double x, double y, double z)
            {
                center = center.shift(x, y, z);
            }

            //масштабирование
            public void scale(double ind_scale)
            {
                for (int i = 0; i < vertices.Count; i++)
                {
                    PointPol pp = vertices[i].scale(ind_scale);
                    vertices[i] = pp;
                }
            }

            //поворот
            //axis == 0 => поворот вокруг OX
            //axis == 1 => поворот вокруг OY
            //axis == 2 => поворот вокруг OZ
            public void rotation(int axis, double cos, double sin)
            {
                center = center.roration(axis, cos, sin);

                for (int i = 0; i < vertices.Count; i++)
                {
                    PointPol pp = vertices[i].roration(axis, cos, sin);
                    vertices[i] = pp;
                }
            }

            //отражение
            public void reflection(int reflec)
            {
                center = center.reflection(reflec);
            }

        }

        //гексаэдр
        [Serializable]
        public class cube : Polyhedron
        {
            public cube()
            {
                center = new PointPol(_form.pictureBox1.Image.Height / 2, _form.pictureBox1.Image.Height / 2, _form.pictureBox1.Image.Height / 2);
                vertices = new Dictionary<int, PointPol>();
                //у точки без индекса z меньше
                PointPol a = new PointPol(0, len / 2, len / 2, -len / 2, 1), a1 = new PointPol(1, len / 2, len / 2, len / 2, 1),
                     b = new PointPol(2, -len / 2, len / 2, -len / 2, 1), b1 = new PointPol(3, -len / 2, len / 2, len / 2, 1),
                     c = new PointPol(4, -len / 2, -len / 2, -len / 2, 1), c1 = new PointPol(5, -len / 2, -len / 2, len / 2, 1),
                     d = new PointPol(6, len / 2, -len / 2, -len / 2, 1), d1 = new PointPol(7, len / 2, -len / 2, len / 2, 1);

                for (int i = 0; i < 8; i++)
                {
                    neighbors[i] = new List<int>();
                }

                //добавляем вершины
                {
                    vertices[0] = a;
                    vertices[1] = a1;
                    vertices[2] = b;
                    vertices[3] = b1;
                    vertices[4] = c;
                    vertices[5] = c1;
                    vertices[6] = d;
                    vertices[7] = d1;
                }

                //добавляем ребра
                {
                    edges3D.Add(new Edge(a.index, a1.index));
                    edges3D.Add(new Edge(a.index, b.index));
                    edges3D.Add(new Edge(a.index, d.index));
                    edges3D.Add(new Edge(c.index, c1.index));
                    edges3D.Add(new Edge(c.index, b.index));
                    edges3D.Add(new Edge(c.index, d.index));
                    edges3D.Add(new Edge(b1.index, b.index));
                    edges3D.Add(new Edge(b1.index, c1.index));
                    edges3D.Add(new Edge(b1.index, a1.index));
                    edges3D.Add(new Edge(d1.index, d.index));
                    edges3D.Add(new Edge(d1.index, c1.index));
                    edges3D.Add(new Edge(d1.index, a1.index));
                }

                //заполняем списки смежности
                {
                    //A
                    neighbors[1].Add(0);
                    neighbors[2].Add(0);
                    neighbors[6].Add(0);

                    //A1
                    neighbors[0].Add(1);
                    neighbors[3].Add(1);
                    neighbors[7].Add(1);

                    // B
                    neighbors[0].Add(2);
                    neighbors[4].Add(2);
                    neighbors[3].Add(2);

                    //B1
                    neighbors[1].Add(3);
                    neighbors[5].Add(3);
                    neighbors[2].Add(3);

                    // C
                    neighbors[5].Add(4);
                    neighbors[6].Add(4);
                    neighbors[2].Add(4);

                    //C1
                    neighbors[4].Add(5);
                    neighbors[7].Add(5);
                    neighbors[3].Add(5);

                    //D
                    neighbors[4].Add(6);
                    neighbors[0].Add(6);
                    neighbors[7].Add(6);

                    //D1
                    neighbors[5].Add(7);
                    neighbors[1].Add(7);
                    neighbors[6].Add(7);
                }

                //заполняем полигоны
                {
                    List<Edge> e1 = new List<Edge>();
                    e1.Add(new Edge(a.index, b.index));
                    e1.Add(new Edge(a.index, d.index));
                    e1.Add(new Edge(c.index, b.index));
                    e1.Add(new Edge(c.index, d.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1 = new List<Edge>();
                    e1.Add(new Edge(a.index, b.index));
                    e1.Add(new Edge(a.index, a1.index));
                    e1.Add(new Edge(a1.index, b1.index));
                    e1.Add(new Edge(b.index, b1.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1 = new List<Edge>();
                    e1.Add(new Edge(a.index, d.index));
                    e1.Add(new Edge(a.index, a1.index));
                    e1.Add(new Edge(a1.index, d1.index));
                    e1.Add(new Edge(d.index, d1.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1 = new List<Edge>();
                    e1.Add(new Edge(c.index, d.index));
                    e1.Add(new Edge(c.index, c1.index));
                    e1.Add(new Edge(c1.index, d1.index));
                    e1.Add(new Edge(d.index, d1.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1 = new List<Edge>();
                    e1.Add(new Edge(c.index, b.index));
                    e1.Add(new Edge(c.index, c1.index));
                    e1.Add(new Edge(c1.index, b1.index));
                    e1.Add(new Edge(b.index, b1.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1 = new List<Edge>();
                    e1.Add(new Edge(a1.index, d1.index));
                    e1.Add(new Edge(c1.index, d1.index));
                    e1.Add(new Edge(b1.index, c1.index));
                    e1.Add(new Edge(a1.index, b1.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();
                }
            }
        }

        //тетраэдр
        [Serializable]
        public class Tetrahedron : Polyhedron
        {
            public Tetrahedron()
            {
                center = new PointPol(_form.pictureBox1.Image.Height / 2, _form.pictureBox1.Image.Height / 2, _form.pictureBox1.Image.Height / 2);
                vertices = new Dictionary<int, PointPol>();

                PointPol a = new PointPol(0, len / 2, len / 2, len / 2, 1),
                         b = new PointPol(1, -len / 2, -len / 2, len / 2, 1),
                         c = new PointPol(2, -len / 2, len / 2, -len / 2, 1),
                         d = new PointPol(3, len / 2, -len / 2, -len / 2, 1);

                //добавляем вершины
                {
                    vertices[0] = a;
                    vertices[1] = b;
                    vertices[2] = c;
                    vertices[3] = d;
                }

                for (int i = 0; i < 4; i++)
                {
                    neighbors[i] = new List<int>();
                }

                //заполняем списки смежности
                for (int i = 0; i < 4; ++i)
                {
                    neighbors[i].Add((i + 1) % 4);
                    neighbors[i].Add((i + 2) % 4);
                    neighbors[i].Add((i + 3) % 4);
                }

                //заполняем ребра
                {
                    edges3D.Add(new Edge(a.index, b.index));
                    edges3D.Add(new Edge(a.index, c.index));
                    edges3D.Add(new Edge(a.index, d.index));
                    edges3D.Add(new Edge(b.index, c.index));
                    edges3D.Add(new Edge(b.index, d.index));
                    edges3D.Add(new Edge(d.index, c.index));
                }

                //заполняем полигоны
                {
                    List<Edge> e1 = new List<Edge>();
                    e1.Add(new Edge(a.index, b.index));
                    e1.Add(new Edge(a.index, c.index));
                    e1.Add(new Edge(b.index, c.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1.Add(new Edge(a.index, c.index));
                    e1.Add(new Edge(a.index, d.index));
                    e1.Add(new Edge(c.index, d.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1.Add(new Edge(a.index, b.index));
                    e1.Add(new Edge(a.index, d.index));
                    e1.Add(new Edge(b.index, d.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1.Add(new Edge(b.index, c.index));
                    e1.Add(new Edge(b.index, d.index));
                    e1.Add(new Edge(c.index, d.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();
                }

            }
        }

        //октаэдр
        [Serializable]
        public class Octahedron : Polyhedron
        {
            public Octahedron()
            {
                center = new PointPol(_form.pictureBox1.Image.Height / 2, _form.pictureBox1.Image.Height / 2, _form.pictureBox1.Image.Height / 2);
                vertices = new Dictionary<int, PointPol>();

                PointPol a = new PointPol(0, 0, 0, len / 2, 1),
                         b = new PointPol(1, len / 2, 0, 0, 1),
                         c = new PointPol(2, 0, len / 2, 0, 1),
                         d = new PointPol(3, -len / 2, 0, 0, 1),
                         e = new PointPol(4, 0, -len / 2, 0, 1),
                         f = new PointPol(5, 0, 0, -len / 2, 1);

                //добовляем вершины
                {
                    vertices[a.index] = a;
                    vertices[b.index] = b;
                    vertices[c.index] = c;
                    vertices[d.index] = d;
                    vertices[e.index] = e;
                    vertices[f.index] = f;
                }

                //заполняем ребра
                {
                    edges3D.Add(new Edge(a.index, b.index));
                    edges3D.Add(new Edge(a.index, c.index));
                    edges3D.Add(new Edge(a.index, d.index));
                    edges3D.Add(new Edge(a.index, e.index));
                    edges3D.Add(new Edge(f.index, b.index));
                    edges3D.Add(new Edge(f.index, c.index));
                    edges3D.Add(new Edge(f.index, d.index));
                    edges3D.Add(new Edge(f.index, e.index));
                    edges3D.Add(new Edge(b.index, c.index));
                    edges3D.Add(new Edge(b.index, e.index));
                    edges3D.Add(new Edge(d.index, c.index));
                    edges3D.Add(new Edge(d.index, e.index));
                }

                for (int i = 0; i < 6; i++)
                {
                    neighbors[i] = new List<int>();
                }

                //заполяем списки смежности
                {
                    neighbors[a.index].Add(b.index);
                    neighbors[a.index].Add(c.index);
                    neighbors[a.index].Add(d.index);
                    neighbors[a.index].Add(e.index);

                    neighbors[b.index].Add(a.index);
                    neighbors[b.index].Add(f.index);
                    neighbors[b.index].Add(c.index);
                    neighbors[b.index].Add(e.index);

                    neighbors[c.index].Add(a.index);
                    neighbors[c.index].Add(f.index);
                    neighbors[c.index].Add(b.index);
                    neighbors[c.index].Add(d.index);

                    neighbors[d.index].Add(a.index);
                    neighbors[d.index].Add(f.index);
                    neighbors[d.index].Add(c.index);
                    neighbors[d.index].Add(e.index);

                    neighbors[e.index].Add(a.index);
                    neighbors[e.index].Add(f.index);
                    neighbors[e.index].Add(b.index);
                    neighbors[e.index].Add(d.index);

                    neighbors[f.index].Add(b.index);
                    neighbors[f.index].Add(c.index);
                    neighbors[f.index].Add(d.index);
                    neighbors[f.index].Add(e.index);
                }

                //заполняем полигоны
                {
                    List<Edge> e1 = new List<Edge>();
                    e1.Add(new Edge(a.index, b.index));
                    e1.Add(new Edge(a.index, c.index));
                    e1.Add(new Edge(b.index, c.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1.Add(new Edge(a.index, c.index));
                    e1.Add(new Edge(a.index, d.index));
                    e1.Add(new Edge(c.index, d.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1.Add(new Edge(a.index, d.index));
                    e1.Add(new Edge(a.index, e.index));
                    e1.Add(new Edge(d.index, e.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();

                    e1.Add(new Edge(a.index, b.index));
                    e1.Add(new Edge(a.index, e.index));
                    e1.Add(new Edge(b.index, e.index));

                    polygons.Add(new Polygon(e1));
                    e1.Clear();
                }

            }

        }

        static public double[,] matrix_mult(double[,] m1, double[,] m2)
        {
            double[,] res = new double[m1.GetLength(0), m2.GetLength(1)];
            for (int i = 0; i < m1.GetLength(0); ++i)
                for (int j = 0; j < m2.GetLength(1); ++j)
                    for (int k = 0; k < m2.GetLength(0); k++)
                        res[i, j] += m1[i, k] * m2[k, j];
            return res;
        }

        public Form1()
        {
            col = new Pen(Color.Black);
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp = (Bitmap)pictureBox1.Image;
            Clear();
            pictureBox1.Image = bmp;
            comboBox3.SelectedIndex = 0;
        }

        public void Clear()
        {
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = pictureBox1.Image;
            comboBox1.SelectedItem = "...";
        }

        public void ClearWithout()
        {
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Image = pictureBox1.Image;
        }

        //нарисовать
        private void button2_Click(object sender, EventArgs e)
        {
            pol = new Polyhedron();
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Гексаэдр":
                    pol = new cube();
                    break;
                case "Тетраэдр":
                    pol = new Tetrahedron();
                    break;
                case "Октаэдр":
                    pol = new Octahedron();
                    break;
                case "Загрузить из файла":
                    LoadFromFile();
                    break;
                case "Фигура вращения":
                    lastPoint = PointF.Empty;
                    pictureBox1.MouseDown += pictureBox1_MouseDown;
                    pictureBox1.MouseMove += pictureBox1_MouseMove;
                    pictureBox1.MouseUp += pictureBox1_MouseUp;
                    forming = new List<PointF>();
                    break;
                default:
                    return;
                    break;
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
                    break;
            }

            ClearWithout();

            pol.draw();

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

            pol.draw();

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

            pol.draw();

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
                    break;
            }

            pol.reflection(reflec);
            ClearWithout();

            pol.draw();

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

            rotation(start_point, line);
            ClearWithout();

            pol.draw();

            pictureBox1.Image = pictureBox1.Image;
        }

        //поворот вокруг прямой, проходящей через начало координат и центр объекта
        private void button7_Click(object sender, EventArgs e)
        {
            double[,] start_point = { { 0, 0, 0 } };
            double[,] line = { { pol.center.X, pol.center.Y, pol.center.Z, 1 } };
            rotation(start_point, line);
            ClearWithout();

            pol.draw();

            pictureBox1.Image = pictureBox1.Image;
        }

        //поворот
        private void rotation(double[,] start_point, double[,] line)
        {
            //перемещаем прямую в начало мировых координат, а также объект на ту же величину
            {
                double[,] T = { { 1,0,0,0},
                            {0,1,0,0 },
                            {0,0,1,0 },
                            {-start_point[0,0], -start_point[0,1], -start_point[0,2],1 } };

                line = matrix_mult(line, T);
                pol.shift(-start_point[0, 0], -start_point[0, 1], -start_point[0, 2]);
            }

            double d = Math.Sqrt(Math.Pow(line[0, 1], 2) + Math.Pow(line[0, 2], 2));

            double cos, sin;

            cos = line[0, 2] / d;
            sin = -line[0, 1] / d;

            double[,] R_x = { { 1, 0 , 0, 0} ,
                              {0, cos, -sin, 0 },
                              {0, sin, cos, 0},
                              {0, 0, 0, 1 } };
            //поворачиваем прямую вокруг Ox, чтобы она оказалась на плоскости XZ. На тот же угол поворачиваем объект
            {
                line = matrix_mult(line, R_x);

                pol.rotation(0, cos, sin);
            }

            d = Math.Sqrt(line[0, 0] * line[0, 0] + line[0, 2] * line[0, 2]);

            cos = line[0, 2] / d;
            sin = line[0, 0] / d;

            double[,] R_y = { { cos, 0 , sin, 0} ,
                              {0, 1, 0, 0 },
                              {-sin, 0, cos, 0},
                              {0, 0, 0, 1 } };

            //поворачиваем вокруг Oy, чтобы прямая совпала с Oz
            {
                pol.rotation(1, cos, sin);
            }

            //поворачиваем объект вокруг Oz
            double angle = (Convert.ToDouble(textBoxAngle.Text) / 180) * Math.PI;

            pol.rotation(2, Math.Cos(angle), Math.Sin(angle));


            Double[][] R_y_inv = new double[][] { new double[] { 7, 2, 1, 1 }, new double[] { 0, 3, -1, 1 }, new double[] { -3, 4, 2, 1 }, new double[] { -3, 4, 2, 1 } };

            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j)
                    R_y_inv[i][j] = R_y[i, j];

            R_y_inv = MatrixInverse(R_y_inv);

            Double[][] R_x_inv = new double[][] { new double[] { 7, 2, 1, 1 }, new double[] { 0, 3, -1, 1 }, new double[] { -3, 4, 2, 1 }, new double[] { -3, 4, 2, 1 } };

            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j)
                    R_x_inv[i][j] = R_x[i, j];

            R_x_inv = MatrixInverse(R_x_inv);

            pol.rotation(1, R_y_inv[0][0], R_y_inv[0][2]);

            pol.rotation(0, R_x_inv[1][1], R_x_inv[2][1]);

            pol.shift(start_point[0, 0], start_point[0, 1], start_point[0, 2]);
        }


        static double[][] MatrixCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        static double[][] MatrixDuplicate(double[][] matrix)
        {
            // allocates/creates a duplicate of a matrix.
            double[][] result = MatrixCreate(matrix.Length, matrix[0].Length);
            for (int i = 0; i < matrix.Length; ++i) // copy the values
                for (int j = 0; j < matrix[i].Length; ++j)
                    result[i][j] = matrix[i][j];
            return result;
        }

        static double[] HelperSolve(double[][] luMatrix, double[] b)
        {
            // before calling this helper, permute b using the perm array
            // from MatrixDecompose that generated luMatrix
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        }
        static double[][] MatrixDecompose(double[][] matrix, out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // rerturns: result is L (with 1s on diagonal) and U;
            // perm holds row permutations; toggle is +1 or -1 (even or odd)
            int rows = matrix.Length;
            int cols = matrix[0].Length; // assume square
            if (rows != cols)
                throw new Exception("Attempt to decompose a non-square m");

            int n = rows; // convenience

            double[][] result = MatrixDuplicate(matrix);

            perm = new int[n]; // set up row permutation result
            for (int i = 0; i < n; ++i) { perm[i] = i; }

            toggle = 1; // toggle tracks row swaps.
                        // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

            for (int j = 0; j < n - 1; ++j) // each column
            {
                double colMax = Math.Abs(result[j][j]); // find largest val in col
                int pRow = j;
                //for (int i = j + 1; i less-than n; ++i)
                //{
                //  if (result[i][j] greater-than colMax)
                //  {
                //    colMax = result[i][j];
                //    pRow = i;
                //  }
                //}

                // reader Matt V needed this:
                for (int i = j + 1; i < n; ++i)
                {
                    if (Math.Abs(result[i][j]) > colMax)
                    {
                        colMax = Math.Abs(result[i][j]);
                        pRow = i;
                    }
                }
                // Not sure if this approach is needed always, or not.

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    double[] rowPtr = result[pRow];
                    result[pRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                // --------------------------------------------------
                // This part added later (not in original)
                // and replaces the 'return null' below.
                // if there is a 0 on the diagonal, find a good row
                // from i = j+1 down that doesn't have
                // a 0 in column j, and swap that good row with row j
                // --------------------------------------------------

                if (result[j][j] == 0.0)
                {
                    // find a good row to swap
                    int goodRow = -1;
                    for (int row = j + 1; row < n; ++row)
                    {
                        if (result[row][j] != 0.0)
                            goodRow = row;
                    }

                    if (goodRow == -1)
                        throw new Exception("Cannot use Doolittle's method");

                    // swap rows so 0.0 no longer on diagonal
                    double[] rowPtr = result[goodRow];
                    result[goodRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[goodRow]; // and swap perm info
                    perm[goodRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }
                // --------------------------------------------------
                // if diagonal after swap is zero . .
                //if (Math.Abs(result[j][j]) less-than 1.0E-20) 
                //  return null; // consider a throw

                for (int i = j + 1; i < n; ++i)
                {
                    result[i][j] /= result[j][j];
                    for (int k = j + 1; k < n; ++k)
                    {
                        result[i][k] -= result[i][j] * result[j][k];
                    }
                }


            } // main j column loop

            return result;
        }

        static double[][] MatrixInverse(double[][] matrix)
        {
            int n = matrix.Length;
            double[][] result = MatrixDuplicate(matrix);

            int[] perm;
            int toggle;
            double[][] lum = MatrixDecompose(matrix, out perm,
              out toggle);
            if (lum == null)
                throw new Exception("Unable to compute inverse");

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                }

                double[] x = HelperSolve(lum, b);

                for (int j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        }

        // Загрузить модель из файла
        void LoadFromFile()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файл модели|*.model";
                openFileDialog.Title = "Открыть модель";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ReadFromFile(openFileDialog.FileName);
                }
            }
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
                        WriteToFile(saveFileDialog.FileName);
                        MessageBox.Show("Модель сохранена");
                    }
                }
            }
            else
            {
                MessageBox.Show("Модель не задана");
            }
        }

        public void WriteToFile(string filePath, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, pol);
            }
        }

        public void ReadFromFile(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                pol = (Polyhedron)binaryFormatter.Deserialize(stream);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = e.Location;
            isMouseDown = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown == true)
            {
                if (lastPoint != null)
                {
                    if (pictureBox1.Image == null)
                    {
                        Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                        pictureBox1.Image = bmp;
                    }
                    using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                    {
                        forming.Add(lastPoint);
                        g.DrawLine(new Pen(Color.Black, 2), lastPoint, e.Location);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                    }
                    pictureBox1.Invalidate();
                    lastPoint = e.Location;
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            lastPoint = Point.Empty;
        }

        private void ButtonDrawSolid_Click(object sender, EventArgs e)
        {

        }
    }
}