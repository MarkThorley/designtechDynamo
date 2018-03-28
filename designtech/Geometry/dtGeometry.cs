using System;
using System.Collections.Generic;
using System.Linq;
using DSCore;
using DSCore.Properties;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace dtGeometry
{
    public class Deconstruct
    {
        internal Deconstruct()
        {

        }

        #region CoordinateSystem
        /// <summary>
        /// Deconstruct a coordinate system element to return the origin, x axis, y axis, z axis. 
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <search>geometry,deconstruct,coordinate,system,origin,point,axis,x,y,z,vector</search>
        [MultiReturn(new[] { "origin", "x", "y", "z" })]
        public static Dictionary<string, object> CoordinateSystem(CoordinateSystem coordinateSystem)
        {
            Point origin;
            origin = coordinateSystem.Origin;
            Vector x;
            x = coordinateSystem.XAxis;
            Vector y;
            y = coordinateSystem.YAxis;
            Vector z;
            z = coordinateSystem.ZAxis;

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"origin",origin},
                {"x",x},
                {"y",y},
                {"z",z}
            };
            return newOutput;
        }
        #endregion

        #region Curve
        /// <summary>
        /// Deconstruct a curve element to return the start, mid and endpoint as well as the direction and length. 
        /// </summary>
        /// <param name="curve">curve</param>
        /// <search>geometry,deconstruct,curve,line,start,mid,end,point,direction,length</search>
        [MultiReturn(new[] { "startPt", "midPt", "endPt", "direction", "len" })]
        public static Dictionary<string, object> Curve(Line curve)
        {
            Point startPoint;
            startPoint = curve.PointAtParameter(0);
            Point midPoint;
            midPoint = curve.PointAtParameter(0.5);
            Point endPoint;
            endPoint = curve.PointAtParameter(1);
            Vector direction;
            direction = curve.Direction;
            double length;
            length = curve.SegmentLengthBetweenParameters(0, 1);

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"startPt",startPoint},
                {"midPt",midPoint},
                {"endPt",endPoint},
                {"direction",direction},
                {"len",length}
            };
            return newOutput;
        }
        #endregion

        #region Face
        /// <summary>
        /// Deconstruct a face element to return the origin, x, y and z vectors 
        /// </summary>
        /// <param name="face"></param>
        /// <search>geometry,deconstruct,face,edges,vertices</search>
        [MultiReturn(new[] { "edges", "vertices" })]
        public static Dictionary<string, object> Face(Face face)
        {
            Edge[] edges;
            edges = face.Edges;
            Vertex[] vertices;
            vertices = face.Vertices;

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"edges",edges},
                {"vertices",vertices}
            };
            return newOutput;
        }
        #endregion

        #region Edge
        /// <summary>
        /// Deconstruct an edge element to return the adjacent faces, the curve geometry, and the start and end vertex. 
        /// </summary>
        /// <param name="edge"></param>
        /// <search>geometry,deconstruct,edge,adjacent,faces,curve,start,end,vertex</search>
        [MultiReturn(new[] { "adjacentFaces", "curve", "startVertex", "endVertex" })]
        public static Dictionary<string, object> Edge(Edge edge)
        {
            Face[] faces;
            faces = edge.AdjacentFaces;
            Curve curve;
            curve = edge.CurveGeometry;
            Vertex startVertex;
            startVertex = edge.StartVertex;
            Vertex endVertex;
            endVertex = edge.EndVertex;

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"faces",faces},
                {"curve",curve},
                {"startVertex",startVertex},
                {"endVertex",endVertex}
            };
            return newOutput;
        }
        #endregion

        #region Mesh
        /// <summary>
        /// Deconstruct a mesh element to return the face indices, vertex normals and vertex positions.
        /// </summary>
        /// <param name="mesh"></param>
        /// <search>geometry,deconstruct,mesh,faces,indices,vertices,vertex,normals,vector,points</search>
        [MultiReturn(new[] { "indices", "vectors", "points" })]
        public static Dictionary<string, object> Mesh(Mesh mesh)
        {
            IndexGroup[] indices;
            indices = mesh.FaceIndices;
            Vector[] vectors;
            vectors = mesh.VertexNormals;
            Point[] points;
            points = mesh.VertexPositions;

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"indices",indices},
                { "vectors",vectors},
                {"points",points}
            };
            return newOutput;
        }
        #endregion

        #region Plane
        /// <summary>
        /// Deconstruct a plane element to return the origin, x, y and z vectors 
        /// </summary>
        /// <param name="plane"></param>
        /// <search>geometry,deconstruct,plane,origin,x,y,z,vector</search>
        [MultiReturn(new[] { "origin", "x", "y", "z" })]
        public static Dictionary<string, object> Plane(Plane plane)
        {
            Point origin;
            origin = plane.Origin;
            Vector x;
            x = plane.XAxis;
            Vector y;
            y = plane.YAxis;
            Vector z;
            z = plane.Normal;

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"origin",origin},
                {"x",x},
                {"y",y},
                {"z",z}
            };
            return newOutput;
        }
        #endregion

        #region Point
        /// <summary>
        /// Deconstruct a point element to return the x, y and z values 
        /// </summary>
        /// <param name="point"></param>
        /// <search>geometry,deconstruct,point,x,y,z</search>
        [MultiReturn(new[] { "x", "y", "z" })]
        public static Dictionary<string, double> Point(Point point)
        {
            double test = point.X;

            double x;
            x = point.X;
            double y;
            y = point.Y;
            double z;
            z = point.Z;

            Dictionary<string, double> newOutput;
            newOutput = new Dictionary<string, double>
            {
                {"x",x},
                {"y",y},
                {"z",z}
            };
            return newOutput;
        }
        #endregion

        #region Solid
        /// <summary>
        /// Deconstruct a solid element to return the edges, faces, vertices, area and volume.
        /// </summary>
        /// <param name="solid"></param>
        /// <search>geometry,deconstruct,solid,edges,faces,vertices,area,volume</search>
        [MultiReturn(new[] { "edges", "faces", "vertices", "area", "volume" })]
        public static Dictionary<string, object> Solid(Solid solid)
        {
            Edge[] edges;
            edges = solid.Edges;
            Face[] faces;
            faces = solid.Faces;
            Vertex[] vertices;
            vertices = solid.Vertices;
            double area;
            area = solid.Area;
            double volume;
            volume = solid.Volume;

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"edges",edges},
                {"faces",faces},
                {"vertices",vertices},
                {"area",area},
                {"volume",volume}
            };
            return newOutput;
        }
        #endregion

        #region Surface
        /// <summary>
        /// Deconstruct a surface element to return the area, edges, faces, perimeter, perimeter curves and vertices.
        /// </summary>
        /// <param name="surface"></param>
        /// <search>geometry,deconstruct,surface,edges,faces,vertices,area,perimeter,curves</search>
        [MultiReturn(new[] { "area", "edges", "faces", "perimeter", "perimeterCurves", "vertices" })]
        public static Dictionary<string, object> Surface(Surface surface)
        {
            double area;
            area = surface.Area;
            Edge[] edges;
            edges = surface.Edges;
            Face[] faces;
            faces = surface.Faces;
            double perimeter;
            perimeter = surface.Perimeter;
            Curve[] perimeterCurves;
            perimeterCurves = surface.PerimeterCurves();
            Vertex[] vertices;
            vertices = surface.Vertices;

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"area",area},
                { "edges",edges},
                {"faces",faces},
                {"perimeter",perimeter},
                {"perimeterCurves",perimeterCurves},
                {"vertices",vertices}
            };
            return newOutput;
        }
        #endregion

        #region UV
        /// <summary>
        /// Deconstruct a UV element to return the U and V values
        /// </summary>
        /// <param name="UV"></param>
        /// <search>geometry,deconstruct,UV,U,V</search>
        [MultiReturn(new[] { "U", "V" })]
        public static Dictionary<string, object> UV(UV UV)
        {
            double U;
            U = UV.U;
            double V;
            V = UV.V;

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"U",U},
                {"V",V}
            };
            return newOutput;
        }
        #endregion

        #region Vector
        /// <summary>
        /// Deconstruct a vector element to return the x, y and z values 
        /// </summary>
        /// <param name="vector"></param>
        /// <search>geometry,deconstruct,vector,x,y,z</search>
        [MultiReturn(new[] { "x", "y", "z" })]
        public static Dictionary<string, double> Vector(Vector vector)
        {
            double x;
            x = vector.X;
            double y;
            y = vector.Y;
            double z;
            z = vector.Z;

            Dictionary<string, double> newOutput;
            newOutput = new Dictionary<string, double>
            {
                {"x",x},
                {"y",y},
                {"z",z}
            };
            return newOutput;
        }
        #endregion

        #region Vertex
        /// <summary>
        /// Deconstruct a vertex element to return the adjacent edges, adjacent faces and point geometry.
        /// </summary>
        /// <param name="vertex"></param>
        /// <search>geometry,deconstruct,vertex,,adjacent,edges,faces,point</search>
        [MultiReturn(new[] { "adjacentEdges", "adjacentFaces", "point" })]
        public static Dictionary<string, object> Vertex(Vertex vertex)
        {
            Edge[] edges;
            edges = vertex.AdjacentEdges;
            Face[] faces;
            faces = vertex.AdjacentFaces;
            Point point;
            point = vertex.PointGeometry;

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"edges",edges},
                {"faces",faces},
                {"point",point}

            };
            return newOutput;
        }
        #endregion

    }

    public class Surfaces
    {
        internal Surfaces()
        {

        }
        
        #region PlanarBase
        /// <summary>
        /// Extract the planar base surface/s from a list of surfaces.
        /// </summary>
        /// <param name="surfaces"></param>
        /// <search>geometry,base,planar,surface,surfaces,bottom</search>
        [MultiReturn(new[] { "baseSurfaces", "otherSurfaces" })]
        public static Dictionary<string, object> PlanarBase(List<Surface> surfaces)
        {
            List<Double> zValues = new List<Double>();
            foreach (var srf in surfaces)
            {
                Double z = DSCore.Math.Round(srf.PointAtParameter(0.5).Z);
                zValues.Add(z);
            }
            Double min = zValues.Min();

            List<bool> boolList = new List<bool>();
            foreach (Double z in zValues)
            {
                if (z == min)
                {
                    boolList.Add(true);
                }
                else
                {
                    boolList.Add(false);
                }
            }
            List<Surface> baseSrfs = surfaces.Zip(boolList, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == true).Select(item => item.name).ToList();
            List<Surface> otherSrfs = surfaces.Zip(boolList, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == false).Select(item => item.name).ToList();

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"baseSurfaces",baseSrfs},
                {"otherSurfaces",otherSrfs}

            };
            return newOutput;
        }
        #endregion

        #region PlanarTop
        /// <summary>
        /// Extract the planar top surface/s from a list of surfaces.
        /// </summary>
        /// <param name="surfaces"></param>
        /// <search>geometry,top,planar,surface,surfaces,ceiling</search>
        [MultiReturn(new[] { "baseSurfaces", "otherSurfaces" })]
        public static Dictionary<string, object> PlanarTop(List<Surface> surfaces)
        {
            List<Double> zValues = new List<Double>();
            foreach (var srf in surfaces)
            {
                Double z = DSCore.Math.Round(srf.PointAtParameter(0.5).Z);
                zValues.Add(z);
            }
            Double max = zValues.Max();

            List<bool> boolList = new List<bool>();
            foreach (Double z in zValues)
            {
                if (z == max)
                {
                    boolList.Add(true);
                }
                else
                {
                    boolList.Add(false);
                }
            }
            List<Surface> topSrfs = surfaces.Zip(boolList, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == true).Select(item => item.name).ToList();
            List<Surface> otherSrfs = surfaces.Zip(boolList, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == false).Select(item => item.name).ToList();

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"baseSurfaces",topSrfs},
                {"otherSurfaces",otherSrfs}

            };
            return newOutput;
        }
        #endregion
        
    }
}

