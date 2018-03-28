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
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml;
using Dynamo.Engine.CodeGeneration;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using ProtoCore.AST.AssociativeAST;
using VMDataBridge;
using dtRevit;

namespace dtMace
{
    class Mace
    {
        internal Mace()
        {

        }

        #region NumberOfElementsInEachDrawing
        /// <summary>
        /// Counts the number of datums in each drawing
        /// </summary>
        /// <param name="sheets">sheets to check</param>
        /// <param name="category">category of elements to count</param>
        /// <returns name="results">count of datums in each sheet</returns>
        /// <search>
        /// mace, structural, general, arrangement, drawing, datum, count, number
        /// </search>
        public static string NumberOfElementsInEachDrawing(Revit.Elements.Views.Sheet sheets, Revit.Elements.Category category)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            List<List<Revit.Elements.Element>> elements = Collector.ElementsInSheetByCategory(sheets, category);

            List<int> counts = new List<int>();
            foreach (List<Revit.Elements.Element> e in elements)
            {
                counts.Add(e.Count);
            }

            string output = sheets.SheetNumber.ToString() + " - " + sheets.SheetName.ToString() + " = " + counts[0].ToString();

            return output;
        }
        #endregion

        #region StructuralGeneralArrangementDrawings
        /// <summary>
        /// Do structural general arrangement drawings for each level exist?
        /// </summary>
        /// <param name="dirPath">the directory path where the drawings are stored</param>
        /// <returns name="results">true and false statements for each level</returns>
        /// <search>
        /// mace, structural, general, arrangement, drawing, exist, true, false, boolean
        /// </search>
        public static List<string> StructuralGeneralArrangementDrawings(string dirPath)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            DirectoryInfo dir = Directory.CreateDirectory(dirPath);
            FileInfo[] files = dir.GetFiles();

            List<object> drawings = new List<object>();
            foreach (var f in files)
            {
                if (f.Name.Contains(".pdf"))
                {
                    drawings.Add(f.Name);
                }
            }

            List<Revit.Elements.Element> levels = dtRevit.Collector.LevelsCollector();
            List<string> levelNames = new List<string>();
            foreach (Revit.Elements.Element l in levels)
            {
                levelNames.Add(l.Name);
            }

            List<List<bool>> boolMatch = new List<List<bool>>();
            foreach (string lN in levelNames)
            {
                List<bool> subMatch = new List<bool>();
                foreach (string d in drawings)
                {
                    if (d.Contains(lN))
                    {
                        subMatch.Add(true);
                    }
                    else
                    {
                        subMatch.Add(false);
                    }
                }
                boolMatch.Add(subMatch);
            }

            List<string> output = new List<string>();
            for (int i = 0; i < boolMatch.Count; i++)
            {
                if (boolMatch[i].All(a => a == false))
                {
                    output.Add(levelNames[i] + " = " + "False");
                }
                else
                {
                    output.Add(levelNames[i] + " = " + "True");
                }
            }

            return output;
        }
        #endregion

        #region PercentageOfDimensionedGridlines
        /// <summary>
        /// Returns the percentage number of gridlines dimensioned in the drawing. If all gridlines are dimensioned the value should be 100%.
        /// </summary>
        /// <param name="sheets">sheets to check</param>
        /// <returns name="results">count of datums in each sheet</returns>
        /// <search>
        /// mace, structural, general, arrangement, drawing, datum, count, number
        /// </search>
        public static object PercentageOfDimensionedGridlines(Revit.Elements.Views.Sheet sheets)
        {
            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;

            Autodesk.Revit.DB.Element unwrappedSheet = sheets.InternalElement;

            BuiltInCategory dimCat = BuiltInCategory.OST_Dimensions;
            BuiltInCategory gridCat = BuiltInCategory.OST_Grids;

            //Get views on sheet
            Autodesk.Revit.DB.ViewSheet viewSheet = unwrappedSheet as Autodesk.Revit.DB.ViewSheet;
            List<ElementId> viewIds = viewSheet.GetAllPlacedViews().ToList();
            ElementId viewId = viewIds[0];

            FilteredElementCollector dimElements = new FilteredElementCollector(doc, viewId).OfCategory(dimCat).WhereElementIsNotElementType();
            FilteredElementCollector gridElements = new FilteredElementCollector(doc, viewId).OfCategory(gridCat).WhereElementIsNotElementType();

            List<Autodesk.Revit.DB.Dimension> dimList = new List<Autodesk.Revit.DB.Dimension>();

            foreach (Autodesk.Revit.DB.Dimension dim in dimElements)
            {
                dimList.Add(dim as Autodesk.Revit.DB.Dimension);
            }

            List<int> gridList = new List<int>();
            foreach (Autodesk.Revit.DB.Element grid in gridElements)
            {
                gridList.Add(grid.Id.IntegerValue);
            }

            List<int> dimRefs = new List<int>();
            foreach (Autodesk.Revit.DB.Dimension d in dimList)
            {
                ReferenceArray refArray = d.References;
                int size = refArray.Size;
                RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(doc);

                for (int i = 0; i < size; i++)
                {
                    Reference _ref = refArray.get_Item(i);
                    ElementId id = _ref.ElementId;
                    //Autodesk.Revit.DB.Element revitElement = doc.GetElement(id);
                    dimRefs.Add(id.IntegerValue);
                }
            }

            List<int> uniqueIds = new HashSet<int>(dimRefs).ToList();

            List<bool> bools = new List<bool>();

            foreach (int id in gridList)
            {
                if (uniqueIds.Contains(id))
                {
                    bools.Add(true);
                }
                else
                {
                    bools.Add(false);
                }
            }

            double countTrue = bools.Where(c => c).Count();
            double countTotal = bools.Count();

            double num = (countTrue / countTotal) * 100;
            decimal dec = System.Convert.ToDecimal(num);
            decimal round = System.Math.Round(dec, 1);

            string output = sheets.SheetNumber.ToString() + " - " + sheets.SheetName.ToString() + " = " + round.ToString() + "%";
            return output;

        }
        #endregion
    }
}
