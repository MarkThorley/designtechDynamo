using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;

namespace dtGeometryPrimitives
{
    public abstract class GeometryPrimitives
    {
        Guid ID { get; set; }
    }

    public class Point : GeometryPrimitives
    {
        internal Point()
        {

        }

        //Fields
        private Guid id { get; set; }
        public Guid ID
        {
            get { return id; }
            set { id = value; }
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }


        #region Create
        /// <summary>
        /// Creates a primitive 3d point
        /// </summary>
        /// <param name="X">x value</param>
        /// <param name="Y">y value</param>
        /// <param name="Z">z value</param>
        /// <search>
        /// create, point, 3d, primitive
        /// </search>
        public static Point Create(double X = 0, double Y = 0, double Z = 0)
        {
            Point point = new Point();

            point.ID = Guid.NewGuid();
            point.X = X;
            point.Y = Y;
            point.Z = Z;

            return point;
        }
        #endregion
    }

    public class Line : GeometryPrimitives
    {
        internal Line()
        {

        }

        //Fields
        private Guid id { get; set; }
        public Guid ID
        {
            get { return id; }
            set { id = value; }
        }

        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public double Length { get; set; }
        public Vector Direction { get; set; }


        #region Create
        /// <summary>
        /// Creates a primitive 3d line
        /// </summary>
        /// <param name="startPoint">pt</param>
        /// <param name="endPoint">pt</param>
        /// <search>
        /// create, line, 3d, primitive
        /// </search>
        public static Line Create(Point startPoint, Point endPoint)
        {
            Line line = new Line();

            line.ID = Guid.NewGuid();
            line.StartPoint = startPoint;
            line.EndPoint = endPoint;

            Vector vec = Vector.Create(startPoint, endPoint);
            line.Direction = vec;
            line.Length = vec.Length;

            return line;
        }
        #endregion
    }

    public class Vector : GeometryPrimitives
    {
        internal Vector()
        {

        }

        //Fields
        private Guid id { get; set; }
        public Guid ID
        {
            get { return id; }
            set { id = value; }
        }

        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public double Length { get; set; }


        #region Create
        /// <summary>
        /// Creates a primitive 3d vector
        /// </summary>
        /// <param name="startPoint">pt</param>
        /// <param name="endPoint">pt</param>
        /// <search>
        /// create, vector, 3d, primitive
        /// </search>
        public static Vector Create(Point startPoint, Point endPoint)
        {
            Vector vector = new Vector();

            vector.ID = Guid.NewGuid();
            vector.StartPoint = startPoint;
            vector.EndPoint = endPoint;

            //Calculation of Length
            double stPtX = startPoint.X;
            double stPtY = startPoint.Y;
            double stPtZ = startPoint.Z;

            double endPtX = endPoint.X;
            double endPtY = endPoint.Y;
            double endPtZ = endPoint.Z;

            double length = Math.Sqrt((Math.Pow(stPtX - endPtX, 2)) + (Math.Pow(stPtY - endPtY, 2)) + (Math.Pow(stPtZ - endPtZ, 2)));

            vector.Length = length;

            return vector;
        }
        #endregion
    }

    public class DynamoGeometry
    {
        internal DynamoGeometry()
        {

        }

        #region Create
        /// <summary>
        /// Creates the corresponding Dynamo Geometry
        /// </summary>
        /// <param name="geometryPrimitive">type</param>
        /// <search>
        /// create, vector, line, point, dynamo, geometry 3d, primitive
        /// </search>
        public static object Create(object geometryPrimitive)
        {
            if (geometryPrimitive.GetType().ToString() == "dtGeometryPrimitives.Point")
            {
                Point p = geometryPrimitive as Point;
                Autodesk.DesignScript.Geometry.Point dynPoint = Autodesk.DesignScript.Geometry.Point.ByCoordinates(p.X, p.Y, p.Z);
                return dynPoint;
            }

            if (geometryPrimitive.GetType().ToString() == "dtGeometryPrimitives.Line")
            {
                Line l = geometryPrimitive as Line;
                Autodesk.DesignScript.Geometry.Point startPoint = Autodesk.DesignScript.Geometry.Point.ByCoordinates(l.StartPoint.X, l.StartPoint.Y, l.StartPoint.Z);
                Autodesk.DesignScript.Geometry.Point endPoint = Autodesk.DesignScript.Geometry.Point.ByCoordinates(l.EndPoint.X, l.EndPoint.Y, l.EndPoint.Z);
                Autodesk.DesignScript.Geometry.Line dynLine = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(startPoint, endPoint);
                startPoint.Dispose();
                endPoint.Dispose();
                return dynLine;
            }
            if (geometryPrimitive.GetType().ToString() == "dtGeometryPrimitives.Vector")
            {
                Vector v = geometryPrimitive as Vector;
                Autodesk.DesignScript.Geometry.Point startPoint = Autodesk.DesignScript.Geometry.Point.ByCoordinates(v.StartPoint.X, v.StartPoint.Y, v.StartPoint.Z);
                Autodesk.DesignScript.Geometry.Point endPoint = Autodesk.DesignScript.Geometry.Point.ByCoordinates(v.EndPoint.X, v.EndPoint.Y, v.EndPoint.Z);
                Autodesk.DesignScript.Geometry.Vector vector = Autodesk.DesignScript.Geometry.Vector.ByTwoPoints(startPoint, endPoint);
                startPoint.Dispose();
                endPoint.Dispose();
                return vector;
            }
            else
            {
                return "Primitive Type not found";
            }
        }
        #endregion
    }
}
