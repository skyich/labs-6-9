using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Task
{
    class Models
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

        //скалярное произведение двух векторов
        static public PointPol normal_vector(PointPol a, PointPol b)
        {
            /*double x = a.Y * b.Z - a.Z * b.Y;
            double y = a.Z * b.X - a.X * b.Z;
            double z = a.X * b.Y - a.Y * b.X;*/ //правый ортонормированный базис

            double x = a.Z * b.Y - a.Y * b.Z;
            double y = a.X * b.Z - a.Z * b.X;
            double z = a.Y * b.X - a.X * b.Y; //левый ортонормированный базис

            PointPol result = new PointPol(x, y, z);
            return result;
        }

        //угол между векторами в радианах
        static public double angle_betw_vectors(PointPol a, PointPol b)
        {
            return Math.Acos((a.X * b.X + a.Y * b.Y + a.Z * b.Z) / (Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z) * Math.Sqrt(b.X * b.X + b.Y * b.Y + b.Z * b.Z)));
        }

        //класс для z-буфера. Для точки с указанным индексом можно узнать индексы соседних с ним точек
        public class Point_with_neighbors
        {
            public int index;
            public int left;
            public int right;
            
            public Point_with_neighbors(int ind, int l, int r)
            {
                index = ind;
                right = r;
                left = l;
            }
        }

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

                return translatePol(Utils.matrix_mult(getP(), transfer));
            }

            //матрицу с одной строкой превращает в PointPol
            public PointPol translatePol(double[,] f)
            {
                return new PointPol(index, f[0, 0], f[0, 1], f[0, 2], f[0, 3]);
            }

            //матрицу с одним столбцом превращает в PoinPol
            public PointPol translatePol1(double[,] f)
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
                return translatePol1(Utils.matrix_mult(shiftMatrix, getPol()));
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

                return translatePol(Utils.matrix_mult(getP(), R_axis));
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

                return translatePol(Utils.matrix_mult(getP(), R_ref));
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
            public List<int> edges = new List<int>();
            public PointPol normal;
            public Polygon(List<int> edg, PointPol norm)
            {
                normal = norm;
                foreach (var el in edg)
                {
                    edges.Add(el);
                   // if (!points.Contains(el.P1))
                   //     points.Add(el.P1);
                   // if (!points.Contains(el.P2))
                   //     points.Add(el.P2);
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
            public PointPol center;
            //Список вершин многогранника
            public Dictionary<int, PointPol> vertices; //в классе координаты хранятся относительно центра, как векторы
            public List<PointF> vertices2D;
            //по индексу точки возвращет список индексов точек, между которыми есть ребро
            public Dictionary<int, List<int>> neighbors = new Dictionary<int, List<int>>();
            public List<Tuple<PointF, PointF>> edges;
            public List<Edge> edges3D = new List<Edge>();
            public List<Polygon> polygons = new List<Polygon>();

            public Polyhedron DeepCopy()
            {
                Polyhedron other = (Polyhedron)this.MemberwiseClone();
                other.vertices = new Dictionary<int, PointPol>(vertices);
                other.vertices2D = new List<PointF>(vertices2D);
                other.neighbors = new Dictionary<int, List<int>>(neighbors);
                other.edges = new List<Tuple<PointF, PointF>>(edges);
                other.edges3D = new List<Edge>(edges3D);
                other.center = new PointPol(center.index, center.X, center.Y, center.Z, center.W);
                other.polygons = new List<Polygon>(polygons);
                return other;
            }


            //ищет индекс точки с наибольшим значением игрик в полигоне с указанным индексом
            public int find_top_point(int polyg_ind)
            {
                int result = edges3D[polygons[polyg_ind].edges[0]].P1;

                foreach (var edge in polygons[polyg_ind].edges)
                {
                    if (vertices[edges3D[edge].P1].Y > vertices[result].Y)
                        result = edges3D[edge].P1;

                    if (vertices[edges3D[edge].P2].Y > vertices[result].Y)
                        result = edges3D[edge].P2;
                }


                return result;
            }


            public Dictionary<int, Point_with_neighbors> new_view(int index_polyg)
            {
                Dictionary<int, Point_with_neighbors> result = new Dictionary<int, Point_with_neighbors>();

                foreach(int ed in polygons[index_polyg].edges)
                {
                    if (result.ContainsKey(edges3D[ed].P1))
                        result[edges3D[ed].P1].right = edges3D[ed].P2;
                    else
                        result[edges3D[ed].P1] = new Point_with_neighbors(edges3D[ed].P1, edges3D[ed].P2, -1);

                    if (result.ContainsKey(edges3D[ed].P2))
                        result[edges3D[ed].P2].right = edges3D[ed].P1;
                    else
                        result[edges3D[ed].P2] = new Point_with_neighbors(edges3D[ed].P2, edges3D[ed].P1, -1);
                }

                return result;
            }

            //по трем координатам точки и способу отображения возвращает точку с двумя координатами
            public PointF GetPointIn2d(int proc, double[,] temp)
            {
                PointF result = new PointF();

                switch (proc)
                {
                    case 1: //изометрическая 
                        result = new PointF((int)Math.Round(temp[0, 0] + center.X), (int)Math.Round(temp[1, 0] + center.Y));
                        break;
                    case 2: //ортографическая на YoZ
                        result = new PointF((int)Math.Round(temp[1, 0] + center.Y), (int)Math.Round(temp[2, 0] + center.Z));
                        break;
                    case 3: //ортографическая на XoZ
                        result = new PointF((int)Math.Round(temp[0, 0] + center.X), (int)Math.Round(temp[2, 0] + center.Z));
                        break;
                    case 4: //ортографическая на XoY
                        result = new PointF((int)Math.Round(temp[0, 0] + center.X), (int)Math.Round(temp[1, 0] + center.Y));
                        break;
                    case 5:
                        result = new PointF((int)Math.Round((temp[0, 0] + center.X) / (temp[3, 0] + center.Z * (len / 50000))), (int)Math.Round((temp[1, 0] + center.Y) / (temp[3, 0] + center.Z * (len / 50000))));
                        break;
                    default:
                        break;
                }

                return result;
            }


            public Polyhedron()
            {

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

                edges = new List<Tuple<PointF, PointF>>();
                vertices2D = new List<PointF>();

                foreach (var e in edges3D)
                {
                    var temp = Utils.matrix_mult(dM, vertices[e.P1].getPol());
                    PointF temp2d = GetPointIn2d(proc, temp);

                    vertices2D.Add(temp2d);

                    temp = Utils.matrix_mult(dM, vertices[e.P2].getPol());
                    temp2d = GetPointIn2d(proc, temp);

                    edges.Add(new Tuple<PointF, PointF>(temp2d, vertices2D.Last()));
                    vertices2D.Add(temp2d);
                }
            }

            // dM - матрица камеры
            public void Display(double[,] dM)
            {
                edges = new List<Tuple<PointF, PointF>>();
                vertices2D = new List<PointF>();

                foreach (var e in edges3D)
                {
                    var temp = Utils.matrix_mult(dM, vertices[e.P1].getPol());
                    PointF temp2d = GetPointIn2d(5, temp);

                    vertices2D.Add(temp2d);

                    temp = Utils.matrix_mult(dM, vertices[e.P2].getPol());
                    temp2d = GetPointIn2d(5, temp);

                    edges.Add(new Tuple<PointF, PointF>(temp2d, vertices2D.Last()));
                    vertices2D.Add(temp2d);
                }
            }

            public PointPol translatePol(double[,] f)
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
            public void rotate(int axis, double cos, double sin)
            {
                center = center.roration(axis, cos, sin);

                for (int i = 0; i < vertices.Count; i++)
                {
                    PointPol pp = vertices[i].roration(axis, cos, sin);
                    vertices[i] = pp;
                }
                for(int i = 0; i < polygons.Count; i++)
                {
                    polygons[i].normal = polygons[i].normal.roration(axis, cos, sin);
                }
            }

            //поворот
            public void rotation(double[,] start_point, double[,] line, double angle)
            {
                //перемещаем прямую в начало мировых координат, а также объект на ту же величину

                double[,] T = { { 1,0,0,0},
                            {0,1,0,0 },
                            {0,0,1,0 },
                            {-start_point[0,0], -start_point[0,1], -start_point[0,2],1 } };

                line = Utils.matrix_mult(line, T);
                shift(-start_point[0, 0], -start_point[0, 1], -start_point[0, 2]);


                double d = Math.Sqrt(Math.Pow(line[0, 1], 2) + Math.Pow(line[0, 2], 2));

                double cos, sin;

                if (d == 0)
                {
                    cos = 1;
                    sin = 0;
                }
                else
                {
                    cos = line[0, 2] / d;
                    sin = -line[0, 1] / d;
                }

                double[,] R_x = { { 1, 0 , 0, 0} ,
                              {0, cos, -sin, 0 },
                              {0, sin, cos, 0},
                              {0, 0, 0, 1 } };
                //поворачиваем прямую вокруг Ox, чтобы она оказалась на плоскости XZ. На тот же угол поворачиваем объект
                {
                    line = Utils.matrix_mult(line, R_x);

                    rotate(0, cos, sin);
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
                    rotate(1, cos, sin);
                }

                //поворачиваем объект вокруг Oz


                rotate(2, Math.Cos(angle), Math.Sin(angle));


                Double[][] R_y_inv = new double[][] { new double[] { 7, 2, 1, 1 }, new double[] { 0, 3, -1, 1 }, new double[] { -3, 4, 2, 1 }, new double[] { -3, 4, 2, 1 } };

                for (int i = 0; i < 4; ++i)
                    for (int j = 0; j < 4; ++j)
                        R_y_inv[i][j] = R_y[i, j];

                R_y_inv = Utils.MatrixInverse(R_y_inv);

                Double[][] R_x_inv = new double[][] { new double[] { 7, 2, 1, 1 }, new double[] { 0, 3, -1, 1 }, new double[] { -3, 4, 2, 1 }, new double[] { -3, 4, 2, 1 } };

                for (int i = 0; i < 4; ++i)
                    for (int j = 0; j < 4; ++j)
                        R_x_inv[i][j] = R_x[i, j];

                R_x_inv = Utils.MatrixInverse(R_x_inv);

                rotate(1, R_y_inv[0][0], R_y_inv[0][2]);

                rotate(0, R_x_inv[1][1], R_x_inv[2][1]);

                shift(start_point[0, 0], start_point[0, 1], start_point[0, 2]);
            }

            //отражение
            public void reflection(int reflec)
            {
                center = center.reflection(reflec);
            }

            // Запись в файл
            public void WriteToFile(string filePath, bool append = false)
            {
                using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    binaryFormatter.Serialize(stream, this);
                }
            }

            // Чтение из файла
            static public Models.Polyhedron ReadFromFile(string filePath)
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (Models.Polyhedron)binaryFormatter.Deserialize(stream);
                }
            }

            //true на i-ой позиции означает, что i-ый полигон является лицевым
            public List<bool> non_face_removal(PointPol review_vetor)
            {
                List<bool> result = new List<bool>();

                for (int i = 0; i < polygons.Count; i++)
                    result.Add(false);

                for (int i = 0; i < polygons.Count; i++)
                {
                    double x1, y1, z1, x2, y2, z2;
                    x1 = vertices[edges3D[polygons[i].edges[0]].P1].X - vertices[edges3D[polygons[i].edges[0]].P2].X;
                    y1 = vertices[edges3D[polygons[i].edges[0]].P1].Y - vertices[edges3D[polygons[i].edges[0]].P2].Y;
                    z1 = vertices[edges3D[polygons[i].edges[0]].P1].Z - vertices[edges3D[polygons[i].edges[0]].P2].Z;

                    x2 = vertices[edges3D[polygons[i].edges[1]].P1].X - vertices[edges3D[polygons[i].edges[1]].P2].X;
                    y2 = vertices[edges3D[polygons[i].edges[1]].P1].Y - vertices[edges3D[polygons[i].edges[1]].P2].Y;
                    z2 = vertices[edges3D[polygons[i].edges[1]].P1].Z - vertices[edges3D[polygons[i].edges[1]].P2].Z;

                    PointPol a = new PointPol(x1, y1, z1);
                    PointPol b = new PointPol(x2, y2, z2);

                    PointPol normal = normal_vector(a, b); //нормаль грани
                    //от начала функции и до сюда почти все фикция, потому что нормали граней заполняются при инициализации и не нужно ничего искать
                    if (angle_betw_vectors(polygons[i].normal, review_vetor) > Math.PI/2)
                    {
                        result[i] = true;
                    }
                }
                
                return result;
            }

        }

       
        public class Axis:Polyhedron
        {
            public Axis()
            {
                center = new PointPol(0, 0, 0);
                vertices = new Dictionary<int, PointPol>();

                PointPol O = new PointPol(0, 0, 0, 0, 1),
                         X = new PointPol(1, 100000, 0, 0, 1),
                         Y = new PointPol(2, 0, 100000, 0, 1),
                         Z = new PointPol(3, 0, 0, 100000, 1);

                for (int i = 0; i < 4; i++)
                {
                    neighbors[i] = new List<int>();
                }

                vertices[O.index] = O;
                vertices[X.index] = X;
                vertices[Y.index] = Y;
                vertices[Z.index] = Z;

                edges3D.Add(new Edge(O.index, X.index));
                edges3D.Add(new Edge(O.index, Y.index));
                edges3D.Add(new Edge(O.index, Z.index));

                neighbors[O.index].Add(X.index);
                neighbors[O.index].Add(Y.index);
                neighbors[O.index].Add(Z.index);
            }
        }

        //гексаэдр
        [Serializable]
        public class cube : Polyhedron
        {
            public cube(double p)
            {
                center = new PointPol(p, p, p);
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
                    edges3D.Add(new Edge(a.index, a1.index));//0
                    edges3D.Add(new Edge(a.index, b.index));//1
                    edges3D.Add(new Edge(a.index, d.index));//2
                    edges3D.Add(new Edge(c.index, c1.index));//3
                    edges3D.Add(new Edge(c.index, b.index));//4
                    edges3D.Add(new Edge(c.index, d.index));//5
                    edges3D.Add(new Edge(b1.index, b.index));//6
                    edges3D.Add(new Edge(b1.index, c1.index));//7
                    edges3D.Add(new Edge(b1.index, a1.index));//8
                    edges3D.Add(new Edge(d1.index, d.index));//9
                    edges3D.Add(new Edge(d1.index, c1.index));//10
                    edges3D.Add(new Edge(d1.index, a1.index));//11
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
                    PointPol normal;
                    List<int> e1 = new List<int>();
                    e1.Add(1);
                    e1.Add(2);
                    e1.Add(4);
                    e1.Add(5);
                    normal = new PointPol(0, 0, -1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1 = new List<int>();
                    e1.Add(1);
                    e1.Add(0);
                    e1.Add(8);
                    e1.Add(6);
                    normal = new PointPol(0, 1, 0);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1 = new List<int>();
                    e1.Add(2);
                    e1.Add(0);
                    e1.Add(11);
                    e1.Add(9);
                    normal = new PointPol(1, 0, 0);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1 = new List<int>();
                    e1.Add(5);
                    e1.Add(3);
                    e1.Add(10);
                    e1.Add(9);
                    normal = new PointPol(0, -1, 0);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1 = new List<int>();
                    e1.Add(4);
                    e1.Add(3);
                    e1.Add(7);
                    e1.Add(6);
                    normal = new PointPol(-1, 0, 0);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1 = new List<int>();
                    e1.Add(11);
                    e1.Add(10);
                    e1.Add(7);
                    e1.Add(8);
                    normal = new PointPol(0, 0, 1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();
                }
            }
        }

        //тетраэдр
        [Serializable]
        public class Tetrahedron : Polyhedron
        {
            public Tetrahedron(double p)
            {
                center = new PointPol(p, p, p);
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
                    edges3D.Add(new Edge(a.index, b.index));//0
                    edges3D.Add(new Edge(a.index, c.index));//1
                    edges3D.Add(new Edge(a.index, d.index));//2
                    edges3D.Add(new Edge(b.index, c.index));//3
                    edges3D.Add(new Edge(b.index, d.index));//4
                    edges3D.Add(new Edge(d.index, c.index));//5
                }

                //заполняем полигоны
                {
                    PointPol normal;
                    List<int> e1 = new List<int>();
                    e1.Add(0);
                    e1.Add(1);
                    e1.Add(3);
                    normal = new PointPol(-1, 1, 1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1.Add(1);
                    e1.Add(2);
                    e1.Add(5);
                    normal = new PointPol(1, 1, -1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1.Add(0);
                    e1.Add(2);
                    e1.Add(4);
                    normal = new PointPol(1, -1, 1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1.Add(3);
                    e1.Add(4);
                    e1.Add(5);
                    normal = new PointPol(-1, -1, -1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();
                }

            }
        }

        //октаэдр
        [Serializable]
        public class Octahedron : Polyhedron
        {
            public Octahedron(double p)
            {
                center = new PointPol(p, p, p);
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
                    edges3D.Add(new Edge(a.index, b.index));//0
                    edges3D.Add(new Edge(a.index, c.index));//1
                    edges3D.Add(new Edge(a.index, d.index));//2
                    edges3D.Add(new Edge(a.index, e.index));//3
                    edges3D.Add(new Edge(f.index, b.index));//4
                    edges3D.Add(new Edge(f.index, c.index));//5
                    edges3D.Add(new Edge(f.index, d.index));//6
                    edges3D.Add(new Edge(f.index, e.index));//7
                    edges3D.Add(new Edge(b.index, c.index));//8
                    edges3D.Add(new Edge(b.index, e.index));//9
                    edges3D.Add(new Edge(d.index, c.index));//10
                    edges3D.Add(new Edge(d.index, e.index));//11
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
                    PointPol normal;
                    List<int> e1 = new List<int>();
                    e1.Add(0);
                    e1.Add(1);
                    e1.Add(8);
                    normal = new PointPol(1, 1, 1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1.Add(1);
                    e1.Add(2);
                    e1.Add(10);
                    normal = new PointPol(-1, 1, 1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1.Add(2);
                    e1.Add(3);
                    e1.Add(11);
                    normal = new PointPol(-1, -1, 1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1.Add(0);
                    e1.Add(3);
                    e1.Add(9);
                    normal = new PointPol(1, -1, 1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    //=================

                    e1.Add(4);
                    e1.Add(5);
                    e1.Add(8);
                    normal = new PointPol(1, 1, -1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1.Add(5);
                    e1.Add(6);
                    e1.Add(10);
                    normal = new PointPol(-1, 1, -1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1.Add(6);
                    e1.Add(7);
                    e1.Add(11);
                    normal = new PointPol(-1, -1, -1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();

                    e1.Add(4);
                    e1.Add(7);
                    e1.Add(9);
                    normal = new PointPol(1, -1, -1);

                    polygons.Add(new Polygon(e1, normal));
                    e1.Clear();
                }
                
            }

        }

        [Serializable]
        public class SolidOfRevolution : Polyhedron
        {
            public SolidOfRevolution(List<System.Web.UI.DataVisualization.Charting.Point3D> forming, int splits, int axis, double p)
            {
                var c_x = p;
                center = new PointPol(c_x, c_x, c_x); // центр фигуры

                vertices = new Dictionary<int, PointPol>(); // точки 
                for (int i = 0; i < forming.Count; ++i)
                {
                    var t = new PointPol(i, forming[i].X, forming[i].Y, forming[i].Z, 1);
                    vertices[i] = t;
                }

                for (int i = 0; i < vertices.Count - 1; ++i)
                {
                    edges3D.Add(new Edge(vertices[i].index, vertices[i + 1].index)); // ребра
                }

                double angle = (double)360 / splits; // угол поворота
                var cos = Math.Cos(angle * Math.PI / 180);
                var sin = Math.Sin(angle * Math.PI / 180);
                var count = vertices.Count; // количество точек, которые поворачиваем

                for (int i = 0; i < splits - 1; ++i)
                {
                    for (int j = 0; j < count; j++)
                    {
                        PointPol pp = vertices[j + count * i].roration(axis, cos, sin);
                        pp.index = j + count * (i + 1);
                        vertices.Add(pp.index, pp);
                    }

                    for (int j = count * (i + 1); j < count * (i + 2) - 1; ++j)
                    {
                        edges3D.Add(new Edge(j, j + 1));
                    }

                    // соединяем все точки предыдущего ребра с соответсвующими точками текущего
                    for (int j = count * i; j < count * (i + 1); ++j)
                    {
                        edges3D.Add(new Edge(j, j + count));
                    }
                }
                // соединяем последнее ребро с начальным
                for (int i = 0; i < count; ++i)
                {
                    edges3D.Add(new Edge(count * (splits - 1) + i, i));
                }

            }

        }

        [Serializable]
        public class Plot3D : Polyhedron
        {
            public Plot3D(Func<double, double, double> func, double x1, double x2, int splits, double p)
            {
                double step = (double)(x2 - x1) / splits;
                var c_x = p;
                center = new PointPol(c_x, c_x, c_x); // центр фигуры
                vertices = new Dictionary<int, PointPol>(); // точки 

                // добавим все точки поверхности
                int i = 0;
                for (double x = x1; x < x2 + 0.001; x += step) // 0.001 - погрешность
                {
                   for (double y = x1; y < x2 + 0.001; y += step)
                    {
                        var t = new PointPol(i, x, y, func(x, y), 1);
                        vertices.Add(i, t);
                        i += 1;
                    }
                }
                int last_point = i - 1;

                //все ребра, кроме последнего слоя
                i = 0;
                for (double x = x1; x < x2 - step + 0.001; x += step)
                {
                    for (double y = x1; y < x2 - step + 0.001; y += step)
                    {
                        edges3D.Add(new Edge(i, i + 1));
                        edges3D.Add(new Edge(i, i + splits + 1));
                        i += 1;
                    }
                    i += 1;
                }

                // ребра для последнего слоя
                for (int j = 0; j < splits; ++j)
                {
                     edges3D.Add(new Edge(i, i + 1));
                     i += 1;
                }

                for (int j = splits; j < last_point; j += splits + 1)
                {
                   edges3D.Add(new Edge(j, j + splits + 1));
                }
            }

        }
    }
}
