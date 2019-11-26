using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;

namespace Task
{
    class Camera
    {
        private static Camera instance;
        public double[,] perspective { get; set; }
        public Point3D position;

        private Camera()
        {
            Refresh();
        }

        public void Refresh()
        {
            perspective = new double[4, 4] { { 1, 0, 0, 0 },
                                                           { 0, 1, 0, 0 },
                                                           { 0, 0, 0, 0 },
                                                           { 0, 0, 100.003198/50000, 1 } };
            position = new Point3D ( 0, 0, 0 );
        }

        // движение камеры по осям
        public void Shift(int direction)
        {
            double[,] shiftMatrix = new double[4, 4] { { 1, 0, 0, 0 },
                                                           { 0, 1, 0, 0 },
                                                           { 0, 0, 1, 0 },
                                                           { 0, 0, 0, 1 } }; 
            int step = 5;
            switch (direction)
            {
                case 0: // влево по х
                    shiftMatrix = new double[4, 4] { { 1, 0, 0, -1 * step },
                                                           { 0, 1, 0, 0 },
                                                           { 0, 0, 1, 0 },
                                                           { 0, 0, 0, 1 } };
                    position.X -= step;
                    break;
                case 1: // вправо по x
                    shiftMatrix = new double[4, 4] { { 1, 0, 0, step },
                                                           { 0, 1, 0, 0 },
                                                           { 0, 0, 1, 0 },
                                                           { 0, 0, 0, 1 } };
                    position.X += step;
                    break;
                case 2: // влево по y
                    shiftMatrix = new double[4, 4] { { 1, 0, 0, 0 },
                                                           { 0, 1, 0, -1 * step },
                                                           { 0, 0, 1, 0 },
                                                           { 0, 0, 0, 1 } };
                    position.Y -= step;
                    break;
                case 3: // вправо по y
                    shiftMatrix = new double[4, 4] { { 1, 0, 0, 0 },
                                                           { 0, 1, 0, step },
                                                           { 0, 0, 1, 0 },
                                                           { 0, 0, 0, 1 } };
                    position.Y += step;
                    break;
                case 4: // вниз по z
                    shiftMatrix = new double[4, 4] { { 1, 0, 0, 0 },
                                                           { 0, 1, 0, 0 },
                                                           { 0, 0, 1, -1 * step },
                                                           { 0, 0, 0, 1 } };
                    position.Z -= step;
                    break;
                case 5: // вверх по z
                    shiftMatrix = new double[4, 4] { { 1, 0, 0, 0 },
                                                           { 0, 1, 0, 0 },
                                                           { 0, 0, 1, step },
                                                           { 0, 0, 0, 1 } };
                    position.Z += step;
                    break;
            }

            perspective = Utils.matrix_mult(perspective, shiftMatrix);
        }

        public void Rotate(int direction)
        {
            double angle = 0.05;
            double[,] rotationMatrix = new double[4, 4] { { 1, 0, 0, 0 },
                                                           { 0, 1, 0, 0 },
                                                           { 0, 0, 1, 0 },
                                                           { 0, 0, 0, 1 } };
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            var cos_neg = Math.Cos(-1 * angle);
            var sin_neg = Math.Sin(-1 * angle);

            switch (direction)
            {
                case 0: // влево по х
                    rotationMatrix = new double[4, 4] { { cos , 0, 0, 0 },
                                                           { 0, cos, -1*sin, 0 },
                                                           { 0, sin, cos, 0 },
                                                           { 0, 0, 0, 1 } };
                    break;
                case 1: // вправо по x
                    rotationMatrix = new double[4, 4] { { 1, 0, 0, 0 },
                                                           { 0, cos_neg, -1*sin_neg, 0 },
                                                           { 0, sin_neg, cos_neg, 0 },
                                                           { 0, 0, 0, 1 } };
                    break;
                case 2: // влево по y
                    rotationMatrix = new double[4, 4] { { cos, 0, sin, 0 },
                                                           { 0, 1, 0, 0 },
                                                           { -1 * sin, 0, cos, 0 },
                                                           { 0, 0, 0, 1 } };
                    break;
                case 3: // вправо по y
                    rotationMatrix = new double[4, 4] { { cos_neg, 0, sin_neg, 0 },
                                                           { 0, 1, 0, 0 },
                                                           { -1 * sin_neg, 0, cos_neg, 0 },
                                                           { 0, 0, 0, 1 } };
                    break;
                case 4: // вниз по z
                    rotationMatrix = new double[4, 4] { { cos, -1 * sin, 0, 0 },
                                                           { sin, cos, 0, 0 },
                                                           { 0, 0, 1, 0 },
                                                           { 0, 0, 0, 1 } };
                    break;
                case 5: // вверх по z
                    rotationMatrix = new double[4, 4] { { cos_neg, -1 * sin_neg, 0, 0 },
                                                           { sin_neg, cos_neg, 0, 0 },
                                                           { 0, 0, 1, 0 },
                                                           { 0, 0, 0, 1 } };
                    break;
            }
            perspective = Utils.matrix_mult(perspective, rotationMatrix);
        }

        public static Camera getInstance()
        {
            if (instance == null)
                instance = new Camera();
            return instance;
        }


    }
}
