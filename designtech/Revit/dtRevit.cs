using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.Text;
using System.Threading.Tasks;
using Revit.Elements;
using Autodesk.DesignScript.Runtime;
using Revit.Elements.Views;
using Autodesk.DesignScript.Geometry;
using RevitServices.Persistence;
using Revit.GeometryConversion;
using DynamoConversions;
using DSCore;
using Revit.GeometryObjects;


namespace dtRevit
{

    class ListItem
    {
        public Revit.Elements.Element element { get; set; }
        public double elevationValue { get; set; }
        public object parameterValue { get; set; }
    }

    class LoadOpts : IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(Autodesk.Revit.DB.Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            source = FamilySource.Family;
            overwriteParameterValues = true;
            return true;
        }
    }

    public class Annotation
    {
        internal Annotation()
        {

        }

        #region DimensionWithReference
        /// <summary>
        /// Create a dimension in Revit based on a reference
        /// </summary>
        /// <param name="view">views</param>
        /// <param name="line">line</param>
        /// <param name="references">references</param>
        /// <returns name="view">view</returns>
        /// <search>
        /// revit, dim, dimension, annotate, distance, reference, line
        /// </search>
        public static List<Revit.Elements.Element> DimensionWithReference(Revit.Elements.Views.View view, Autodesk.DesignScript.Geometry.Line line, Reference[] references)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwViewEle = view.InternalElement;
            Autodesk.Revit.DB.View uwView = uwViewEle as Autodesk.Revit.DB.View;

            Autodesk.Revit.DB.Curve rvtCurve = line.ToRevitType();
            Autodesk.Revit.DB.Line rvtLine = rvtCurve as Autodesk.Revit.DB.Line;

            ReferenceArray referenceArray = new ReferenceArray();
            foreach (Reference r in references)
            {
                referenceArray.Append(r);
            }

            List<Revit.Elements.Element> newDims = new List<Revit.Elements.Element>();

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(doc);
            try
            {
                Autodesk.Revit.DB.Dimension newDim = doc.Create.NewDimension(uwView, rvtLine, referenceArray);
                newDims.Add(newDim.ToDSType(true));
            }
            catch (Exception ex)
            {

                throw new ArgumentException(ex.Message);
            }
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
            return newDims;


        }
        #endregion

        #region TagLinkedElement
        /// <summary>
        /// Tags an element from a linked model in a host view.
        /// </summary>
        /// <param name="views">views</param>
        /// <param name="location">location point</param>
        /// <param name="linkElement">linked element</param>
        /// <param name="linkReference">linked element reference</param>
        /// <returns name="view">view</returns>
        /// <search>
        /// revit, views, sheets, drawing, number, current, revision, add, separator
        /// </search>
        public static Revit.Elements.Element TagLinkedElement(Revit.Elements.Views.View views, Autodesk.DesignScript.Geometry.Point location, Revit.Elements.Element linkElement, Autodesk.Revit.DB.Reference linkReference)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwViewEle = views.InternalElement;
            Autodesk.Revit.DB.View uwView = uwViewEle as Autodesk.Revit.DB.View;

            Autodesk.Revit.DB.Element uwEle = linkElement.InternalElement;

            Autodesk.Revit.DB.Category revitCat = uwEle.Category;
            string revitCatName = revitCat.Name;

            int len = revitCatName.Length;
            char ch = revitCatName[len-1];
            char[] match = "s".ToCharArray();
            char match2 = match[0];

            string builtCat = "";

            if (ch == match2)
            {
                string rem = revitCatName.Remove(len - 1, 1);
                builtCat = "OST_" + rem + "Tags";
            }
            else
            {
                builtCat = "OST_" + revitCatName + "Tags";
            }

            double x = (location.X) / 304.8;
            double y = (location.Y) / 304.8;
            double z = (location.Z) / 304.8;

            XYZ pt = new XYZ(x, y, z);

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(doc);
            try
            {
                IndependentTag tag = IndependentTag.Create(doc, uwView.Id, linkReference, false, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, pt);
                RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            Array bicsValues = Enum.GetValues(typeof(BuiltInCategory));
            List<BuiltInCategory> bicsValuesList = bicsValues.OfType<BuiltInCategory>().ToList();

            Array bicsNames = Enum.GetNames(typeof(BuiltInCategory));

            List<bool> mask = new List<bool>();
            foreach (string name in bicsNames)
            {
                if (name == builtCat)
                {
                    mask.Add(true);
                }
                else
                {
                    mask.Add(false);
                }
            }

            List<BuiltInCategory> fecB = bicsValuesList.Zip(mask, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == true).Select(item => item.name).ToList();

            FilteredElementCollector fec = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(fecB[0]);

            var elems = new List<Revit.Elements.Element>();
            foreach (var e in fec)
            {
                var e1 = e as Autodesk.Revit.DB.Element;
                elems.Add(Revit.Elements.ElementWrapper.Wrap(e1, true));
            }

            Revit.Elements.Element output = elems.Last();
            return output;

        }
        #endregion

    }

    public class Collector
    {
        internal Collector()
        {

        }

        #region AllElements
        /// <summary>
        /// Retrieves all the elements in the model
        /// </summary>
        /// <returns name="elements">elements</returns>
        /// <search>
        /// revit, all, elements, model, placed
        /// </search>
        public static List<Revit.Elements.Element> AllElements()
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            FilteredElementCollector allele = new FilteredElementCollector(doc);
            allele.WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(false),new ElementIsElementTypeFilter(false)));

            List<Revit.Elements.Element> allEleList = new List<Revit.Elements.Element>();

            foreach (var e in allele)
            {
                allEleList.Add(e.ToDSType(true));
            }
            return allEleList;
        }
        #endregion

        #region CurtainWalls
        /// <summary>
        /// Retrieves all the curtain walls in the model
        /// </summary>
        /// <returns name="curtainWalls">levels</returns>
        /// <search>
        /// revit, curtain, walls, collector, model
        /// </search>
        public static object CurtainWalls()
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            var allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().Cast<Autodesk.Revit.DB.Wall>();

            List<Autodesk.Revit.DB.Element> allWallList = new List<Autodesk.Revit.DB.Element>();

            List<Revit.Elements.Element> curtainWalls = new List<Revit.Elements.Element>();
            foreach (Autodesk.Revit.DB.Wall wall in allWalls)
            {
                if (wall.CurtainGrid != null)
                {
                    curtainWalls.Add(wall.ToDSType(true));
                }
            }
            return curtainWalls;
        }
        #endregion

        #region ElementsInRvtLinkByCategory
        /// <summary>
        /// Returns all the elements in a Revit link instance by category
        /// </summary>
        /// <param name="linkDoc"></param>
        /// <param name="category"></param>
        /// <returns name="elements">output</returns>
        /// <search>
        /// revit, elements, category, link, document, instance, select, return, visible
        /// </search>
        public static List<Revit.Elements.Element> ElementsInRvtLinkByCategory(Autodesk.Revit.DB.Document linkDoc, Revit.Elements.Category category)
        {
            BuiltInCategory enumCategory = (BuiltInCategory)category.Id;

            List<Revit.Elements.Element> elementList = new List<Revit.Elements.Element>();
            ElementCategoryFilter filter = new ElementCategoryFilter(enumCategory);
            FilteredElementCollector allElements = new FilteredElementCollector(linkDoc).WherePasses(filter).WhereElementIsNotElementType();
            foreach (var ele in allElements)
            {
                Revit.Elements.Element e = ele.ToDSType(true);
                elementList.Add(e);
            }
            return elementList;
        }
        #endregion

        #region ElementsInSheetByCategory
        /// <summary>
        /// Returns all the elements in a sheet by category
        /// </summary>
        /// <param name="sheets"></param>
        /// <param name="category"></param>
        /// <returns name="elements">output</returns>
        /// <search>
        /// revit, elements, category, view, select, return, visible
        /// </search>
        public static List<List<Revit.Elements.Element>> ElementsInSheetByCategory(Revit.Elements.Views.Sheet sheets, Revit.Elements.Category category)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            Autodesk.Revit.DB.Element unwrappedSheet = sheets.InternalElement;

            //Get views on sheet
            Autodesk.Revit.DB.ViewSheet viewSheet = unwrappedSheet as Autodesk.Revit.DB.ViewSheet;
            ISet<ElementId> viewIds = viewSheet.GetAllPlacedViews();

            BuiltInCategory enumCategory = (BuiltInCategory)category.Id;

            List<List<Revit.Elements.Element>> elementList = new List<List<Revit.Elements.Element>>();
            foreach (ElementId id in viewIds)
            {
                FilteredElementCollector allElements = new FilteredElementCollector(doc, id).OfCategory(enumCategory).WhereElementIsNotElementType();
                List<Revit.Elements.Element> subList = new List<Revit.Elements.Element>();
                foreach (var ele in allElements)
                {
                    Revit.Elements.Element e = ele.ToDSType(true);
                    subList.Add(e);
                }
                elementList.Add(subList);
            }
            return elementList;
        }
        #endregion

        #region ElementsInViewByCategory
        /// <summary>
        /// Returns all the elements in a view by category
        /// </summary>
        /// <param name="views"></param>
        /// <param name="category"></param>
        /// <returns name="elements">output</returns>
        /// <search>
        /// revit, elements, category, view, select, return, visible
        /// </search>
        public static List<Revit.Elements.Element> ElementsInViewByCategory(Revit.Elements.Views.View views, Revit.Elements.Category category)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            Autodesk.Revit.DB.Element unwrappedView;
            unwrappedView = views.InternalElement;

            BuiltInCategory enumCategory = (BuiltInCategory)category.Id;
            List<Revit.Elements.Element> elementList = new List<Revit.Elements.Element>();
            FilteredElementCollector allElements = new FilteredElementCollector(doc, unwrappedView.Id).OfCategory(enumCategory).WhereElementIsNotElementType();
            foreach (var ele in allElements)
            {
                Revit.Elements.Element e = ele.ToDSType(true);
                elementList.Add(e);
            }
            return elementList;
        }
        #endregion

        #region ElementsInViewByWorkset
        /// <summary>
        /// Returns all the elements in a view by workset
        /// </summary>
        /// <param name="views"></param>
        /// <param name="workset"></param>
        /// <returns name="elements">output</returns>
        /// <search>
        /// revit, elements, category, view, select, return, visible
        /// </search>
        public static List<Revit.Elements.Element> ElementsInViewByWorkset(Revit.Elements.Views.View views, Autodesk.Revit.DB.Workset workset)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element unwrappedView;
            unwrappedView = views.InternalElement;

            // filter all elements that belong to the given workset
            FilteredElementCollector elementCollector = new FilteredElementCollector(doc, unwrappedView.Id);
            ElementWorksetFilter elementWorksetFilter = new ElementWorksetFilter(workset.Id);

            ICollection<Autodesk.Revit.DB.Element> worksetElemsfounds = elementCollector.WherePasses(elementWorksetFilter).ToElements();

            List<Revit.Elements.Element> elementList = new List<Revit.Elements.Element>();
            foreach (Autodesk.Revit.DB.Element ele in worksetElemsfounds)
            {
                Revit.Elements.Element e = ele.ToDSType(true);
                elementList.Add(e);
            }
            return elementList;
        }
        #endregion

        #region Levels
        /// <summary>
        /// Retrieves all the levels in the model
        /// </summary>
        /// <returns name="levels">levels</returns>
        /// <search>
        /// revit, levels, collector, model
        /// </search>
        public static List<Revit.Elements.Element> LevelsCollector()
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            var allLevels = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().Cast<Autodesk.Revit.DB.Level>();

            Dictionary<Revit.Elements.Element, double> dict = new Dictionary<Revit.Elements.Element, double>();
            foreach (Autodesk.Revit.DB.Level level in allLevels)
            {
                Revit.Elements.Element dynLevel = level.ToDSType(true);
                double elevation = level.Elevation;
                dict.Add(dynLevel, elevation);
            }

            var items = from pair in dict orderby pair.Value ascending select pair;

            List<Revit.Elements.Element> sortedList = new List<Revit.Elements.Element>();

            foreach (KeyValuePair<Revit.Elements.Element, double> i in items)
            {
                sortedList.Add(i.Key);
            }
            return sortedList;
        }
        #endregion

        #region RoomsByStatus
        /// <summary>
        /// Returns all the rooms in the model by their current status
        /// </summary>
        /// <search>rooms,unplaced,placed,unbounding,not,enclosed,redundant,status,collector</search>
        [MultiReturn(new[] { "placed", "unplaced", "notEnclosed", "redundant" })]
        public static Dictionary<string, object> RoomsByStatus()
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            var allRooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().Cast<Autodesk.Revit.DB.Architecture.Room>();

            List<Revit.Elements.Element> placedRooms = new List<Revit.Elements.Element>();
            List<Revit.Elements.Element> unplacedRooms = new List<Revit.Elements.Element>();
            List<Revit.Elements.Element> notEnclosedRooms = new List<Revit.Elements.Element>();
            List<Revit.Elements.Element> redundantRooms = new List<Revit.Elements.Element>();

            SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();

            foreach (Autodesk.Revit.DB.Architecture.Room room in allRooms)
            {
                if (room.Area > 0)
                {
                    placedRooms.Add(room.ToDSType(true));
                }
                else
                {
                    if (room.Location == null)
                    {
                        unplacedRooms.Add(room.ToDSType(true));
                    }
                    else
                    {
                        if (room.GetBoundarySegments(opt) == null || (room.GetBoundarySegments(opt)).Count == 0)
                        {
                            notEnclosedRooms.Add(room.ToDSType(true));
                        }
                        else
                        {
                            redundantRooms.Add(room.ToDSType(true));
                        }
                    }
                }
            }
            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
                {
                    {"placed",placedRooms},
                    {"unplaced",unplacedRooms},
                    {"notEnclosed",notEnclosedRooms},
                    {"redundant",redundantRooms}

                };
            return newOutput;
        }

        #endregion

        #region RvtLinks
        /// <summary>
        /// Returns all the Revit Link Instances
        /// </summary>
        /// <returns name="linkIns">output</returns>
        /// <search>
        /// revit, elements, category, view, select, return, visible
        /// </search>
        public static RevitLinkInstance RvtLinks()
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance));

            foreach (Autodesk.Revit.DB.Element e in collector)
            {
                RevitLinkInstance instance = e as RevitLinkInstance;
                return instance;
            }
            return null;
        }
        #endregion

        #region UserWorksets
        /// <summary>
        /// Returns all the worksets in the model
        /// </summary>
        /// <search>workset,collector,name,revit,id</search>
        [MultiReturn(new[] { "worksets", "names", "ids" })]
        public static Dictionary<string, object> UserWorksets()
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            WorksetKind wk = WorksetKind.UserWorkset;
            FilteredWorksetCollector allWorksets = new FilteredWorksetCollector(doc).OfKind(wk);
            IList<Autodesk.Revit.DB.Workset> worksets = allWorksets.ToWorksets();

            List<string> names = new List<string>();
            List<WorksetId> ids = new List<WorksetId>();
            foreach (var w in worksets)
            {
                string n = w.Name;
                names.Add(n);
                WorksetId i = w.Id;
                ids.Add(i);
            }

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
                {
                    {"worksets",worksets},
                    {"names",names},
                    {"ids",ids}
                };
            return newOutput;
        }
        #endregion

    }

    public class Element
    {
        internal Element()
        {

        }

        #region CreateLinkReference
        /// <summary>
        /// Creates a reference for an element from a reference in a RVT Link
        /// </summary>
        /// <param name="linkIns">rvt link instance with the element in</param>
        /// <param name="linkElement">the element in the linked file</param>
        /// <returns name="linkReference">linked element reference</returns>
        /// <search>
        /// revit, instance, document, link, reference, create, element
        /// </search>
        public static Reference CreateLinkReference(RevitLinkInstance linkIns, Revit.Elements.Element linkElement)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element unwrappedElements;
            unwrappedElements = linkElement.InternalElement;

            Reference elementRef = new Reference(unwrappedElements);
            Reference newHostDocRef = elementRef.CreateLinkReference(linkIns);

            return newHostDocRef;
        }
        #endregion

        #region CurtainGrid
        /// <summary>
        /// Retrieves all the curtain grids from the elements
        /// </summary>
        /// <param name="curtainWall"></param>
        /// <search>
        /// revit, curtain, walls, collector, model
        /// </search>
        [MultiReturn(new[] { "uCurtainGrids", "vCurtainGrids" })]
        public static Dictionary<string, List<Revit.Elements.Element>> CurtainGrid(Revit.Elements.Element curtainWall)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwCurtainWall = curtainWall.InternalElement;
            Autodesk.Revit.DB.Wall wall = uwCurtainWall as Autodesk.Revit.DB.Wall;

            Autodesk.Revit.DB.CurtainGrid grid = wall.CurtainGrid;

            List<ElementId> uIds = new List<ElementId>(grid.GetUGridLineIds());
            List<ElementId> vIds = new List<ElementId>(grid.GetVGridLineIds());

            List<Revit.Elements.Element> uElements = new List<Revit.Elements.Element>();
            List<Revit.Elements.Element> vElements = new List<Revit.Elements.Element>();

            foreach (ElementId id in uIds)
            {
                uElements.Add(doc.GetElement(id).ToDSType(true));
            }
            foreach (ElementId id in vIds)
            {
                vElements.Add(doc.GetElement(id).ToDSType(true));
            }
            Dictionary<string, List<Revit.Elements.Element>> newOutput;
            newOutput = new Dictionary<string, List<Revit.Elements.Element>>
                {
                    {"uCurtainGrids",uElements},
                    {"vCurtainGrids",vElements}
                };
            return newOutput;
        }
        #endregion

        #region CurtainGridExists
        /// <summary>
        /// Returns a true boolean if the wall has an active curtain grid else returns a false boolean
        /// </summary>
        /// <param name="curtainWall">element</param>
        /// <returns name="bool">boolean</returns>
        /// <search>
        /// revit, curtain, grid, wall, boolean, is, true, false
        /// </search>
        public static bool CurtainGridExists(Revit.Elements.Element curtainWall)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwWall = curtainWall.InternalElement;
            Autodesk.Revit.DB.Wall rvtWall = uwWall as Autodesk.Revit.DB.Wall;

            Autodesk.Revit.DB.CurtainGrid grid = rvtWall.CurtainGrid;

            List<ElementId> uIds = new List<ElementId>(grid.GetUGridLineIds());
            List<ElementId> vIds = new List<ElementId>(grid.GetVGridLineIds());

            if (uIds.Any() || vIds.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region CurtainGridLine
        /// <summary>
        /// Retrieves all the curtain grid lines from the curtain grid
        /// </summary>
        /// <param name="curtainGridLine"></param>
        /// <search>
        /// revit, curtain, walls, collector, model
        /// </search>
        [MultiReturn(new[] { "dynCurtainGridLine", "referenceLine" })]
        public static Dictionary<string, object> CurtainGridLine(Revit.Elements.Element curtainGridLine)
        {
            Autodesk.Revit.DB.Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwCurtainGridLine = curtainGridLine.InternalElement;
            Autodesk.Revit.DB.CurtainGridLine grid = uwCurtainGridLine as Autodesk.Revit.DB.CurtainGridLine;

            Autodesk.Revit.DB.Curve curve = grid.FullCurve;
            Autodesk.DesignScript.Geometry.Curve dynCurve = curve.ToProtoType();

            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.IncludeNonVisibleObjects = true;

            Reference geoElement = grid.get_Geometry(opt).OfType<Autodesk.Revit.DB.Line>().Select<Autodesk.Revit.DB.Line, Reference>(x => x.Reference).FirstOrDefault();

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
                {
                    {"dynCurtainGridLine",dynCurve},
                    {"referenceLine",geoElement}
                };
            return newOutput;
        }
        #endregion

        #region CurtainWallPerimeterCurves
        /// <summary>
        /// Retrieves the perimeter curves of the curtain wall
        /// </summary>
        /// <param name="curtainWall"></param>
        /// <search>
        /// revit, curtain, walls, collector, model, perimeter, boundary, boundaries, curves
        /// </search>
        [MultiReturn(new[] { "startCurve", "endCurve", "topCurve", "bottomCurve" })]
        public static Dictionary<string, object> CurtainWallPerimeterCurves(Revit.Elements.Element curtainWall)
        {
            // NEED TO DISPOSE GEOMETRY!!!

            Autodesk.Revit.DB.Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwCurtainWall = curtainWall.InternalElement;
            Autodesk.Revit.DB.Wall revitCurtainWall = uwCurtainWall as Autodesk.Revit.DB.Wall;

            Autodesk.Revit.DB.CurtainGrid grid = revitCurtainWall.CurtainGrid;
            ICollection<ElementId> panelID = grid.GetPanelIds();

            List<Revit.Elements.Element> panels = new List<Revit.Elements.Element>();
            List<Autodesk.DesignScript.Geometry.Surface> surfaces = new List<Autodesk.DesignScript.Geometry.Surface>();

            foreach (ElementId id in panelID)
            {
                Autodesk.Revit.DB.Element ele = doc.GetElement(id);
                Revit.Elements.Element dynEle = ele.ToDSType(true);
                Revit.Elements.CurtainPanel dynPan = dynEle as Revit.Elements.CurtainPanel;
                PolyCurve[] bound = dynPan.Boundaries;

                foreach (PolyCurve b in bound)
                {
                    Autodesk.DesignScript.Geometry.Curve closedCurve = b as Autodesk.DesignScript.Geometry.Curve;
                    Autodesk.DesignScript.Geometry.Surface surf = Autodesk.DesignScript.Geometry.Surface.ByPatch(closedCurve);
                    surfaces.Add(surf);
                }
            }
            PolySurface poly = PolySurface.ByJoinedSurfaces(surfaces);
            Autodesk.DesignScript.Geometry.Curve[] perimCurves = poly.PerimeterCurves();

            List<bool> boolList = new List<bool>();
            foreach (Autodesk.DesignScript.Geometry.Curve c in perimCurves)
            {
                Autodesk.DesignScript.Geometry.Point perimStart = c.StartPoint;
                Autodesk.DesignScript.Geometry.Point perimEnd = c.EndPoint;
                double startZ = perimStart.Z;
                double endZ = perimEnd.Z;

                if (startZ == endZ)
                {
                    boolList.Add(true);
                }
                else
                {
                    boolList.Add(false);
                }
            }

            List<Autodesk.DesignScript.Geometry.Curve> horizontalPerim = perimCurves.Zip(boolList, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == true).Select(item => item.name).ToList();
            List<Autodesk.DesignScript.Geometry.Curve> verticalPerim = perimCurves.Zip(boolList, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == false).Select(item => item.name).ToList();

            List<double> zPoints = new List<double>();
            foreach (Autodesk.DesignScript.Geometry.Curve crv in horizontalPerim)
            {
                Autodesk.DesignScript.Geometry.Point start = crv.StartPoint;
                double startZ = start.Z;
                zPoints.Add(startZ);
            }

            double max = zPoints.Max();
            List<bool> boolList2 = new List<bool>();
            foreach (double z in zPoints)
            {
                if (z == max)
                {
                    boolList2.Add(true);
                }
                else
                {
                    boolList2.Add(false);
                }
            }

            List<Autodesk.DesignScript.Geometry.Curve> topCurves = horizontalPerim.Zip(boolList2, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == true).Select(item => item.name).ToList();
            List<Autodesk.DesignScript.Geometry.Curve> bottomCurves = horizontalPerim.Zip(boolList2, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == false).Select(item => item.name).ToList();

            PolyCurve joinedTop = PolyCurve.ByJoinedCurves(topCurves);
            PolyCurve joinedBottom = PolyCurve.ByJoinedCurves(bottomCurves);

            Autodesk.DesignScript.Geometry.Point startTop = joinedTop.StartPoint;
            Autodesk.DesignScript.Geometry.Point endTop = joinedTop.EndPoint;
            Autodesk.DesignScript.Geometry.Line topCurve = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(endTop, startTop);

            Autodesk.DesignScript.Geometry.Point startBottom = joinedBottom.StartPoint;
            Autodesk.DesignScript.Geometry.Point endBottom = joinedBottom.EndPoint;
            Autodesk.DesignScript.Geometry.Line bottomCurve = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(startBottom, endBottom);
            
            List<string> stringValues = new List<string>();
            foreach (Autodesk.DesignScript.Geometry.Curve crv in verticalPerim)
            {
                Autodesk.DesignScript.Geometry.Point start = crv.StartPoint;
                Autodesk.DesignScript.Geometry.Vector vec = start.AsVector();
                string vecX = vec.X.ToString();
                string vecY = vec.Y.ToString();
                string newStr = vecX + " x " + vecY;
                stringValues.Add(newStr);
            }

            string first = stringValues.First();
            List<bool> boolList3 = new List<bool>();
            foreach (string str in stringValues)
            {
                if (str == first)
                {
                    boolList3.Add(true);
                }
                else
                {
                    boolList3.Add(false);
                }
            }

            List<Autodesk.DesignScript.Geometry.Curve> lastCurves = verticalPerim.Zip(boolList3, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == true).Select(item => item.name).ToList();
            List<Autodesk.DesignScript.Geometry.Curve> firstCurves = verticalPerim.Zip(boolList3, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == false).Select(item => item.name).ToList();

            PolyCurve joinedLast = PolyCurve.ByJoinedCurves(lastCurves);
            PolyCurve joinedFirst = PolyCurve.ByJoinedCurves(firstCurves);

            Autodesk.DesignScript.Geometry.Point startLast = joinedLast.StartPoint;
            Autodesk.DesignScript.Geometry.Point endLast = joinedLast.EndPoint;
            Autodesk.DesignScript.Geometry.Line lastCurve = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(startLast, endLast);

            Autodesk.DesignScript.Geometry.Point startFirst = joinedFirst.StartPoint;
            Autodesk.DesignScript.Geometry.Point endFirst = joinedFirst.EndPoint;
            Autodesk.DesignScript.Geometry.Line firstCurve = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(startFirst, endFirst);

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"startCurve",topCurve},
                {"endCurve",bottomCurve},
                {"topCurve",firstCurve},
                {"bottomCurve",lastCurve},
            };

            return newOutput;
        }
        #endregion

        #region CurtainWallReferences
        /// <summary>
        /// Retrieves the perimeter curves of the curtain wall
        /// </summary>
        /// <param name="curtainWall"></param>
        /// <search>
        /// revit, curtain, walls, collector, model, perimeter, boundary, boundaries, curves
        /// </search>
        [MultiReturn(new[] { "startRef", "endRef", "topRef", "bottomRef"})]
        public static object CurtainWallReferences(Revit.Elements.Element curtainWall)
        {
            // NEED TO DISPOSE GEOMETRY!!!

            Autodesk.Revit.DB.Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwCurtainWall = curtainWall.InternalElement;
            Autodesk.Revit.DB.Wall revitCurtainWall = uwCurtainWall as Autodesk.Revit.DB.Wall;

            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.IncludeNonVisibleObjects = true;

            List<Autodesk.Revit.DB.Solid> geomElem = revitCurtainWall.get_Geometry(opt).OfType<Autodesk.Revit.DB.Solid>().ToList();

            List<FaceArray> faceArrayList = new List<FaceArray>();
            foreach (Autodesk.Revit.DB.Solid g in geomElem)
            {
                FaceArray faces = g.Faces;
                {
                    faceArrayList.Add(faces);
                }
            }

            List<Autodesk.Revit.DB.Face> facesList = new List<Autodesk.Revit.DB.Face>();
            foreach (FaceArray fA in faceArrayList)
            {
                if (fA.Size == 1)
                {
                    facesList.Add(fA.get_Item(0));
                }
            }

            List<Reference> refList = new List<Reference>();
            List<Autodesk.DesignScript.Geometry.Curve> edgeList = new List<Autodesk.DesignScript.Geometry.Curve>();

            foreach (Autodesk.Revit.DB.Face f in facesList)
            {
                refList.Add(f.Reference);
                //IList<CurveLoop> crvL = f.GetEdgesAsCurveLoops();
                //foreach (CurveLoop loop in crvL)
                //{
                //   foreach (Autodesk.Revit.DB.Curve l in loop)
                //    {
                //        edgeList.Add(l.ToProtoType());
                //    }
                //}
            }

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
            {
                {"startRef",refList[0]},
                {"endRef",refList[2]},
                {"topRef",refList[1]},
                {"bottomRef",refList[3]}
            };

            return newOutput;
        }
        #endregion

        #region DimensionElements
        /// <summary>
        /// Returns all the elements associated to the given dimension
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns name="elements">output</returns>
        /// <search>
        /// revit, elements, dimension, associatied, select, return
        /// </search>
        public static List<Revit.Elements.Element> DimensionElements(Revit.Elements.Dimension dimension)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            Autodesk.Revit.DB.Element unwrappedDimension = dimension.InternalElement;

            Autodesk.Revit.DB.Dimension revitDimension = unwrappedDimension as Autodesk.Revit.DB.Dimension;

            ReferenceArray refArray = revitDimension.References;
            int size = refArray.Size;

            List<Revit.Elements.Element> eleList = new List<Revit.Elements.Element>();
            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(doc);
            try
            {
                for (int i = 0; i < size; i++)
                {
                    Reference _ref = refArray.get_Item(i);
                    ElementId id = _ref.ElementId;
                    Autodesk.Revit.DB.Element revitElement = doc.GetElement(id);
                    Revit.Elements.Element dynElement = revitElement.ToDSType(true);
                    eleList.Add(dynElement);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
            return eleList;
        }
        #endregion

        #region IsCurtainWall
        /// <summary>
        /// Returns a true boolean if the wall is a curtain wall else returns a false boolean
        /// </summary>
        /// <param name="wall">element</param>
        /// <returns name="bool">boolean</returns>
        /// <search>
        /// revit, curtain, walls, wall, boolean, is, true, false
        /// </search>
        public static bool IsCurtainWall(Revit.Elements.Element wall)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwWall = wall.InternalElement;
            Autodesk.Revit.DB.Wall rvtWall = uwWall as Autodesk.Revit.DB.Wall;

            if (rvtWall.CurtainGrid != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region RemoveParameter
        /// <summary>
        /// Removes a family parameter from a family.
        /// </summary>
        /// <param name="loadedFamily">the parent Revit family</param>
        /// <param name="paramName">the name of the parameter to remove</param>
        /// <returns name="element">the changed element</returns>
        /// <search>
        /// revit, remove, parameter, name, family, instance, type
        /// </search>
        public static object RemoveParameter(Revit.Elements.Family loadedFamily, string paramName)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwFam = loadedFamily.InternalElement;
            Autodesk.Revit.DB.Family fam = uwFam as Autodesk.Revit.DB.Family;

            RevitServices.Transactions.TransactionManager.Instance.ForceCloseTransaction();

            Document famDoc = doc.EditFamily(fam);
            FamilyManager familyManager = famDoc.FamilyManager;

            if (familyManager == null)
            {
                return "Could not open a family for edit";
            }

            else
            {
                using (Transaction RemoveParameters = new Transaction(famDoc, "Remove Parameter"))
                {
                    RemoveParameters.Start();
                    FamilyParameter param = familyManager.get_Parameter(paramName);

                    Definition def = param.Definition;
                    BuiltInParameterGroup bGroup = def.ParameterGroup;           

                    if (param != null)
                    {
                        familyManager.RemoveParameter(param);
                        RemoveParameters.Commit();
                    }
                    else
                    {
                        RemoveParameters.RollBack();
                    }

                    if (RemoveParameters.GetStatus() != TransactionStatus.Committed)
                    {
                        return "Could not make the changes in the family";
                    }

                    LoadOpts famLoadOptions = new LoadOpts();
                    Autodesk.Revit.DB.Family newFam = famDoc.LoadFamily(doc, famLoadOptions);

                    return newFam.ToDSType(true);
                }
            }
        }
        #endregion

        #region ReplaceFamilyParameter
        /// <summary>
        /// Replace a family parameter with a shared parameter.
        /// </summary>
        /// <param name="loadedFamily">the parent Revit family</param>
        /// <param name="oldParamName">the old parameter name as a string</param>
        /// <param name="sharedParamGroup">the parameter group the parameter is in within the shared parameter file as a string</param>
        /// <param name="sharedParamName">the name of the shared parameter as a string</param>
        /// <param name="isInstanceParam">boolean as to whether the new parameter is an instance or a type parameter</param>
        /// <returns name="element">the changed element</returns>
        /// <search>
        /// revit, replace, parameter, name, family, shared, group, instance, type
        /// </search>
        public static object ReplaceFamilyParameter(Revit.Elements.Family loadedFamily, string oldParamName, string sharedParamGroup, string sharedParamName, bool isInstanceParam = false)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwFam = loadedFamily.InternalElement;
            Autodesk.Revit.DB.Family fam = uwFam as Autodesk.Revit.DB.Family;

            RevitServices.Transactions.TransactionManager.Instance.ForceCloseTransaction();

            Document famDoc = doc.EditFamily(fam);
            FamilyManager familyManager = famDoc.FamilyManager;

            if (familyManager == null)
            {
                return "Could not open a family for edit";
            }

            else
            {
                using (Transaction newFamilyTypeTransaction = new Transaction(famDoc, "Change Parameter"))
                {
                    newFamilyTypeTransaction.Start();
                    FamilyParameter param = familyManager.get_Parameter(oldParamName);
                    BuiltInParameterGroup bG = param.Definition.ParameterGroup;

                    UIApplication uiapp = new UIApplication(doc.Application);
                    Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;

                    DefinitionFile defFile = app.OpenSharedParameterFile();
                    DefinitionGroups defGroups = defFile.Groups;
                    DefinitionGroup defGroup = defGroups.get_Item(sharedParamGroup);

                    if (defGroup != null)
                    {
                        ExternalDefinition newFamDef = defGroup.Definitions.get_Item(sharedParamName) as ExternalDefinition;
                        if (param != null)
                        {
                            FamilyParameter newParam = familyManager.ReplaceParameter(param, newFamDef, bG, isInstanceParam);
                            newFamilyTypeTransaction.Commit();
                        }
                        else
                        {
                            newFamilyTypeTransaction.RollBack();
                        }

                        if (newFamilyTypeTransaction.GetStatus() != TransactionStatus.Committed)
                        {
                            return "Could not make the changes in the family";
                        }
                    }

                    LoadOpts famLoadOptions = new LoadOpts();
                    Autodesk.Revit.DB.Family newFam = famDoc.LoadFamily(doc, famLoadOptions);

                    return newFam.ToDSType(true);
                }
            }
        }
        #endregion

        #region ReplaceSharedParameter
        /// <summary>
        /// Replace a shared family parameter with a new non-shared family parameter.
        /// </summary>
        /// <param name="loadedFamily">the parent Revit family</param>
        /// <param name="oldParamName">the old parameter name as a string</param>
        /// <param name="newParamName"></param>
        /// <param name="isInstanceParam">boolean as to whether the new parameter is an instance or a type parameter</param>
        /// <returns name="element">the changed element</returns>
        /// <search>
        /// revit, replace, parameter, name, family, shared, group, instance, type
        /// </search>
        public static object ReplaceSharedParameter(Revit.Elements.Family loadedFamily, string oldParamName, string newParamName, bool isInstanceParam = false)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwFam = loadedFamily.InternalElement;
            Autodesk.Revit.DB.Family fam = uwFam as Autodesk.Revit.DB.Family;

            RevitServices.Transactions.TransactionManager.Instance.ForceCloseTransaction();

            Document famDoc = doc.EditFamily(fam);
            FamilyManager familyManager = famDoc.FamilyManager;

            if (familyManager == null)
            {
                return "Could not open a family for edit";
            }

            else
            {
                using (Transaction newFamilyTypeTransaction = new Transaction(famDoc, "Change Parameter"))
                {
                    newFamilyTypeTransaction.Start();
                    FamilyParameter param = familyManager.get_Parameter(oldParamName);
                    BuiltInParameterGroup bG = param.Definition.ParameterGroup;

                    if (param != null)
                    {
                        FamilyParameter newParam = familyManager.ReplaceParameter(param, newParamName, bG, isInstanceParam);
                        newFamilyTypeTransaction.Commit();
                    }
                    else
                    {
                        newFamilyTypeTransaction.RollBack();
                    }

                    if (newFamilyTypeTransaction.GetStatus() != TransactionStatus.Committed)
                    {
                        return "Could not make the changes in the family";
                    }
                }
                
                LoadOpts famLoadOptions = new LoadOpts();
                Autodesk.Revit.DB.Family newFam = famDoc.LoadFamily(doc, famLoadOptions);

                return newFam.ToDSType(true);
             }
        }
        #endregion

        #region SortByLevel
        /// <summary>
        /// Sorts the elements by their level. This sorts based on the levels elevation not the inbuilt sort method using the level id.
        /// </summary>
        /// <param name="elements">the elements to be sorted</param>
        /// <returns name="elements">the sorted elements</returns>
        /// <search>
        /// revit, sort, elements, level, parameter
        /// </search>
        public static object SortByLevel(List<Revit.Elements.Element> elements)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Dictionary<Revit.Elements.Element, double> dict = new Dictionary<Revit.Elements.Element, double>();

            foreach (Revit.Elements.Element e in elements)
            {
                Autodesk.Revit.DB.Element unwrappedElement = e.InternalElement;
                ElementId levelId = unwrappedElement.LevelId;
                Autodesk.Revit.DB.Element levelElement = doc.GetElement(levelId);
                Autodesk.Revit.DB.Level level = levelElement as Autodesk.Revit.DB.Level;
                double elevation = level.Elevation;
                dict.Add(e, elevation);
            }

            var items = from pair in dict orderby pair.Value ascending select pair;

            List<Revit.Elements.Element> sortedList = new List<Revit.Elements.Element>();

            foreach (KeyValuePair<Revit.Elements.Element, double> i in items)
            {
                sortedList.Add(i.Key);
            }
            return sortedList;

        }
        #endregion

        #region SortByLevelAndParameter
        /// <summary>
        /// Sorts the elements by their level and then by the value of the given parameter. This sorts based on the levels elevation not the inbuilt sort method using the level id.
        /// </summary>
        /// <param name="category">the category of the elements</param>
        /// <param name="parameter">the name of the parameter to sort the elements by after their level</param>
        /// <returns name="elements">the sorted elements</returns>
        /// <search>
        /// revit, sort, elements, level, parameter
        /// </search>
        public static List<Revit.Elements.Element> SortByLevelAndParameter(Revit.Elements.Category category, string parameter)
        {
            //Initalise
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            FilteredElementCollector collLevels = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType();
            FilteredElementCollector collParameters = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType();
            FilteredElementCollector collCategories = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MatchAll).WhereElementIsNotElementType();
            List<Tuple<string, Revit.Elements.Element, double, object>> sortObjectsList = new List<Tuple<string, Revit.Elements.Element, double, object>>();

            string parameterInput = parameter;
            string errors = "None";
            bool parameterMatches = false;
            bool IsParameterANumber = false;

            //Make Parameter List
            collParameters.ToElements();
            var parameters = new List<Autodesk.Revit.DB.Parameter>();
            foreach (Autodesk.Revit.DB.Element element in collParameters)
            {
                //Create an IEnumerable from the set of parameters
                IEnumerable<Autodesk.Revit.DB.Parameter> lp = (from Autodesk.Revit.DB.Parameter p in element.Parameters select p).ToList();
                //Check if parameter is not shared and no exist in the final list   
                IEnumerable<Autodesk.Revit.DB.Parameter> query = lp.TakeWhile(p => p.IsShared == false && parameters.Contains(p).Equals(false));
                //Finally add it to the list
                parameters.AddRange(query);
            }
            List<string> paramaterListAsString = new List<string>();
            for (int x = 0; x < parameters.Count; x++)
            {
                paramaterListAsString.Add(parameters[x].Definition.Name);
            }
            foreach (string p in paramaterListAsString)
            {
                if (parameterInput == p)
                {
                    parameterMatches = true;
                }
            }
            //Make Object List   
            if (category == null)
            {
                collCategories = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MatchAll).WhereElementIsNotElementType();
            }
            else
            {
                BuiltInCategory enumCategory = (BuiltInCategory)category.Id;
                collCategories = new FilteredElementCollector(doc).OfCategory(enumCategory).WhereElementIsNotElementType();
            }
            //Add Elements to Object List
            foreach (Autodesk.Revit.DB.Element CurrentElement in collCategories)
            {
                Revit.Elements.Element e = CurrentElement.ToDSType(true);
                Autodesk.Revit.DB.Level level = CurrentElement.Document.GetElement(CurrentElement.LevelId) as Autodesk.Revit.DB.Level;
                try
                {
                    if (parameterMatches == true)
                    {
                        sortObjectsList.Add(new Tuple<string, Revit.Elements.Element, double, object>(level.Name, e, level.Elevation, e.GetParameterValueByName(parameterInput)));
                    }
                    else
                    {
                        sortObjectsList.Add(new Tuple<string, Revit.Elements.Element, double, object>(level.Name, e, level.Elevation, "No Parameter Found"));
                    }
                }
                catch (Exception ex)
                {
                    errors = ex.ToString();
                }
            }
            //Sort Lists
            //Check if object contains a number(if it does convert to an int and then sort)
            int input = 0;
            for (int x = 0; x < sortObjectsList.Count; x++)
            {
                if (int.TryParse(sortObjectsList[x].Item4.ToString(), out input))
                {
                    IsParameterANumber = true;
                }
                else
                {
                    IsParameterANumber = false;
                    break;
                }
            }
            //Sort By Level and then the Parameter
            if (IsParameterANumber)
            {
                try //Convert to Double and Order
                {
                    sortObjectsList = sortObjectsList.OrderBy(i => i.Item3).ThenBy(i => Convert.ToDouble(i.Item4)).ToList();
                }
                catch (Exception ex)
                {
                    errors = ex.ToString();
                }
            }
            else
            {
                try //Convert to String and Order
                {
                    sortObjectsList = sortObjectsList.OrderBy(i => i.Item3).ThenBy(i => i.Item4.ToString()).ToList();
                }
                catch (Exception ex)
                {
                    errors = ex.ToString();
                }
            }
            //Output objects in a new list
            List<Revit.Elements.Element> elements = new List<Revit.Elements.Element>();
            for (int x = 0; x < sortObjectsList.Count; x++)
            {
                elements.Add(sortObjectsList[x].Item2);
            }
            return elements;
        }
        #endregion

    }

    public class LinkDocument
    {
        internal LinkDocument()
        {

        }

        #region FileName
        /// <summary>
        /// Returns the name of the Rvt link document
        /// </summary>
        /// <param name="linkDoc">rvt link instance</param>
        /// <returns name="name">stringt</returns>
        /// <search>
        /// revit, document, link, name, rvt, string
        /// </search>
        public static string FileName(Document linkDoc)
        {
            string pathName = linkDoc.PathName;
            int ind = pathName.LastIndexOf("\\");
            string fN = pathName.Remove(0, (ind + 1));
            return fN;
        }
        #endregion

        #region FilePath
        /// <summary>
        /// Returns the name of the Rvt link document
        /// </summary>
        /// <param name="linkDoc">rvt link instance</param>
        /// <returns name="path">stringt</returns>
        /// <search>
        /// revit, document, link, name, rvt, string
        /// </search>
        public static string FilePath(Document linkDoc)
        {
            return linkDoc.PathName;
        }
        #endregion

    }

    public class Railing
    {
        internal Railing()
        {

        }

        #region Create
        /// <summary>
        /// Creates a rail in Revit based on a curve and railing type
        /// </summary>
        /// <param name="curveList[]">a list of placement curves</param>
        /// <param name="railingType">railing family type</param>
        /// <param name="hostLevel">the level to host the rail on</param>
        /// <returns name="element">railing instance</returns>
        /// <search>
        /// revit, instance, document, rail, railing, type, create, place, level, host, family, curve
        /// </search>
        public static Revit.Elements.Element Create(List<Autodesk.DesignScript.Geometry.Curve> curveList, Revit.Elements.Element railingType, Revit.Elements.Level hostLevel)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            CurveLoop curveLoop = new CurveLoop();
            foreach (Autodesk.DesignScript.Geometry.Curve curve in curveList)
            {
                curveLoop.Append(curve.ToRevitType());
            }

            Autodesk.Revit.DB.Element uwRailingType = railingType.InternalElement;
            Autodesk.Revit.DB.Architecture.RailingType rType = uwRailingType as Autodesk.Revit.DB.Architecture.RailingType;

            Autodesk.Revit.DB.ElementId railId = rType.Id;

            Autodesk.Revit.DB.Element uwHostLevel = hostLevel.InternalElement;
            Autodesk.Revit.DB.Level hostLvl = uwHostLevel as Autodesk.Revit.DB.Level;

            Autodesk.Revit.DB.ElementId lvlId = hostLvl.Id;

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(doc);
            try
            {
                Autodesk.Revit.DB.Architecture.Railing railInstance = Autodesk.Revit.DB.Architecture.Railing.Create(doc, curveLoop, railId, lvlId);
                RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
                return railInstance.ToDSType(true);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            
        }
        #endregion

    }

    public class RvtLink
    {
        internal RvtLink()
        {

        }

        #region LinkDocument
        /// <summary>
        /// Returns the link document from the Rvt link instance
        /// </summary>
        /// <param name="linkIns">rvt link instance</param>
        /// <returns name="linkDoc">revit linked document</returns>
        /// <search>
        /// revit, instance, document, link, returns, get
        /// </search>
        public static Document LinkDocument(RevitLinkInstance linkIns)
        {
            return linkIns.GetLinkDocument();

        }
        #endregion

        #region Name
        /// <summary>
        /// Returns the name of the Rvt link instance
        /// </summary>
        /// <param name="linkIns">rvt link instance</param>
        /// <returns name="name">stringt</returns>
        /// <search>
        /// revit, instance, link, name, rvt, string
        /// </search>
        public static string Name(RevitLinkInstance linkIns)
        {
            return linkIns.Name;

        }
        #endregion

    }

    public class Sheet
    {
        internal Sheet()
        {

        }

        #region AddCurrentRevisionToSheetNumber
        /// <summary>
        /// Adds the current revision to the sheet number of the selected sheet/s.
        /// </summary>
        /// <param name="sheets">sheets</param>
        /// <param name="separator">string</param>
        /// <returns name="view">view</returns>
        /// <search>
        /// revit, views, sheets, drawing, number, current, revision, add, separator
        /// </search>
        public static object AddCurrentRevisionToSheetNumber(Revit.Elements.Element sheets, string separator)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element unwrappedSheets;
            unwrappedSheets = sheets.InternalElement;

            Autodesk.Revit.DB.Parameter sheetNumberParam = unwrappedSheets.get_Parameter(BuiltInParameter.SHEET_NUMBER);
            string sheetNumberParamValue = sheetNumberParam.AsString();

            Autodesk.Revit.DB.Parameter currentRevisionParam = unwrappedSheets.get_Parameter(BuiltInParameter.SHEET_CURRENT_REVISION);
            string currentRevisionParamValue = currentRevisionParam.AsString();

            string newSheetNum = sheetNumberParamValue + separator + currentRevisionParamValue;

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(doc);
            try
            {
                Autodesk.Revit.DB.Parameter param = unwrappedSheets.get_Parameter(BuiltInParameter.SHEET_NUMBER);
                param.Set(newSheetNum);
            }
            catch (Exception ex)
            {

                throw new ArgumentException(ex.Message);
            }
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();

            return sheets;

        }
        #endregion

    }

    public class View
    {
        internal View()
        {

        }

        #region CreateCallout
        /// <summary>
        /// Creates a callout view
        /// </summary>
        /// <param name="hostView"></param>
        /// <param name="calloutType"></param>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns name="views">views</returns>
        /// <search>
        /// revit, views, callout, create, parent, point, type
        /// </search>
        public static object CreateCallout(Revit.Elements.Views.View hostView, Revit.Elements.Element calloutType, Autodesk.DesignScript.Geometry.Point pt1, Autodesk.DesignScript.Geometry.Point pt2)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            Autodesk.Revit.DB.Element uwHostView;
            uwHostView = hostView.InternalElement;
            ElementId parentViewId = uwHostView.Id;

            Autodesk.Revit.DB.Element uwCalloutType;
            uwCalloutType = calloutType.InternalElement;
            ElementId viewFamilyTypeId = uwCalloutType.Id;

            double pt1X = (pt1.X) / 304.8;
            double pt1Y = (pt1.Y) / 304.8;
            double pt1Z = (pt1.Z) / 304.8;

            double pt2X = (pt2.X) / 304.8;
            double pt2Y = (pt2.Y) / 304.8;
            double pt2Z = (pt2.Z) / 304.8;

            XYZ pt1XYZ = new XYZ(pt1X, pt1Y, pt1Z);
            XYZ pt2XYZ = new XYZ(pt2X, pt2Y, pt2Z);

            List<Revit.Elements.Element> newViews = new List<Revit.Elements.Element>();

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(doc);
            try
            {
                Autodesk.Revit.DB.View revitCallout = Autodesk.Revit.DB.ViewSection.CreateCallout(doc, parentViewId, viewFamilyTypeId, pt1XYZ, pt2XYZ);
                Revit.Elements.Element dynamoCallout = revitCallout.ToDSType(true);
                newViews.Add(dynamoCallout);
            }
            catch (Exception ex)
            {

                throw new ArgumentException(ex.Message);
            }
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();

            return newViews;

        }
        #endregion

        #region Current
        /// <summary>
        /// Gets the current/active view
        /// </summary>
        /// <returns name="view">view</returns>
        /// <search>
        /// revit, views, current, active, document, view, get
        /// </search>
        public static object Current()
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            return doc.ActiveView.ToDSType(true);

        }
        #endregion

        #region SetCategoryVisibility
        /// <summary>
        /// Sets the visibility of a category in a view or view template. To hide the elements select true.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="category"></param>
        /// <param name="boolean">select true to hide the category</param>
        /// <returns name="view">views</returns>
        /// <search>
        /// revit, views, category, hidden, visibility, bool, boolean
        /// </search>
        public static Revit.Elements.Views.View SetCategoryVisibility(Revit.Elements.Views.View view, Revit.Elements.Category category, bool boolean = false)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            Autodesk.Revit.DB.Element uwView;
            uwView = view.InternalElement;

            Autodesk.Revit.DB.View rvtView = uwView as Autodesk.Revit.DB.View;

            BuiltInCategory enumCategory = (BuiltInCategory)category.Id;
            ElementId eleId = new Autodesk.Revit.DB.ElementId(enumCategory);

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(doc);
            try
            {
                rvtView.SetCategoryHidden(eleId, boolean);
                RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
                return view;
            }
            catch (Exception ex)
            {

                throw new ArgumentException(ex.Message);
            }
        }
        #endregion

        #region SetViewTemplateParameter
        /// <summary>
        /// Sets the boolean whether to include a parameter in a view template
        /// </summary>
        /// <param name="viewTemplate">the view template</param>
        /// <param name="paramName">the name of the parameter to set</param>
        /// <returns name="viewTemplate">the changed element</returns>
        /// <search>
        /// revit, parameter, view, template, toggle
        /// </search>
        public static object SetViewTemplateParameter(Revit.Elements.Views.View viewTemplate, string paramName)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element uwView = viewTemplate.InternalElement;
            Autodesk.Revit.DB.View view = uwView as Autodesk.Revit.DB.View;

            RevitServices.Transactions.TransactionManager.Instance.ForceCloseTransaction();

            using (Transaction SetParam = new Transaction(doc, "SetParam"))
            {

                SetParam.Start();
                //creating a list so that I can use linq
                var viewparams = new List<Autodesk.Revit.DB.Parameter>();
                foreach (Autodesk.Revit.DB.Parameter p in view.Parameters)
                {
                    viewparams.Add(p);
                }

                //getting parameters by name (safety checks needed)
                var modelOverrideParam = viewparams.Where(p => p.Definition.Name == paramName).First();

                //setting includes
                view.SetNonControlledTemplateParameterIds(new List<ElementId> { modelOverrideParam.Id });

                SetParam.Commit();

                return viewTemplate;
            }
        }
        #endregion
    }
}
    


