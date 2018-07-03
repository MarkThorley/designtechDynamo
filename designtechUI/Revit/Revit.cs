using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using ProtoCore.AST.AssociativeAST;
using System.Linq;
using Autodesk.Revit.DB;
using System;
using Dynamo.Utilities;
using ElementSelector = Revit.Elements.ElementSelector;


namespace dtRevit
{

    #region Railings
    [NodeName("Railings")]
    [NodeCategory("designtech.dtRevit.Collector")]
    [NodeDescription("A drop down list of railings in the model")]
    [IsDesignScriptCompatible]
    public class Railings : DSDropDownBase
    {
        private const string NoRailings = "No railings were found.";

        public Railings() : base("Railings") { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {

            // The Items collection contains the elements
            // that appear in the list. For this example, we
            // clear the list before adding new items, but you
            // can also use the PopulateItems method to add items
            // to the list.

            Items.Clear();

            // Create a number of DynamoDropDownItem objects 
            // to store the items that we want to appear in our list.

            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            List<Autodesk.Revit.DB.Architecture.Railing> allRailings = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StairsRailing).WhereElementIsElementType().Cast<Autodesk.Revit.DB.Architecture.Railing>().ToList();

            if (allRailings.Count == 0)
            {
                Items.Add(new DynamoDropDownItem(NoRailings, null));
                SelectedIndex = 0;
                return SelectionState.Done;
            }

            foreach (Autodesk.Revit.DB.Architecture.Railing rail in allRailings)
            { 
                string str = rail.Name;
                Items.Add(new DynamoDropDownItem(str, rail));
            }
            return SelectionState.Restore;

        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            AssociativeNode node;

            if (SelectedIndex == -1)
            {
                node = AstFactory.BuildNullNode();
            }
            else
            {
                var rail = Items[SelectedIndex].Item as Autodesk.Revit.DB.Architecture.Railing;
                if (rail == null)
                {
                    node = AstFactory.BuildNullNode();
                }
                else
                {
                    var idNode = AstFactory.BuildStringNode(rail.UniqueId);
                    var falseNode = AstFactory.BuildBooleanNode(true);

                    node = AstFactory.BuildFunctionCall(
                        new Func<string, bool, object>(ElementSelector.ByUniqueId),
                        new List<AssociativeNode> { idNode, falseNode });
                }
            }

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }
    }
    #endregion

    #region SortedLevels
    [NodeName("SortedLevels")]
    [NodeCategory("designtech.dtRevit.Collector")]
    [NodeDescription("A drop down list of levels based on their elevational height")]
    [IsDesignScriptCompatible]
    public class SortedLevels : DSDropDownBase
    {
        private const string NoLevels = "No levels were found.";

        public SortedLevels() : base("SortedLevels") { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {

            // The Items collection contains the elements
            // that appear in the list. For this example, we
            // clear the list before adding new items, but you
            // can also use the PopulateItems method to add items
            // to the list.

            Items.Clear();

            // Create a number of DynamoDropDownItem objects 
            // to store the items that we want to appear in our list.

            Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            var allLevels = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().Cast<Autodesk.Revit.DB.Level>();
        
            Dictionary<Autodesk.Revit.DB.Level, double> dict = new Dictionary<Autodesk.Revit.DB.Level, double>();
            foreach (Autodesk.Revit.DB.Level level in allLevels)
            {
                double elevation = level.Elevation;
                dict.Add(level, elevation);
            }

            var items = from pair in dict orderby pair.Value ascending select pair;

            var sortedLevels = new List<Autodesk.Revit.DB.Level>();
            foreach (KeyValuePair<Autodesk.Revit.DB.Level, double> i in items)
            {
                sortedLevels.Add(i.Key);
            }
            
            if (sortedLevels.Count == 0)
            {
                Items.Add(new DynamoDropDownItem(NoLevels, null));
                SelectedIndex = 0;
                return SelectionState.Done;
            }

            DisplayUnitType units = doc.GetUnits().GetFormatOptions(UnitType.UT_Length).DisplayUnits;

            foreach (var lvl in sortedLevels)
            {
                string elevation = "";
                if (units == DisplayUnitType.DUT_DECIMAL_FEET)
                {
                    double unit = Math.Round(lvl.Elevation, 2);
                    elevation = unit.ToString() + "ft";
                }
                else if (units == DisplayUnitType.DUT_DECIMAL_INCHES)
                {
                    double unit = Math.Round(lvl.Elevation * 12, 2);
                    elevation = unit.ToString() + "\"";
                }
                else if (units == DisplayUnitType.DUT_CENTIMETERS)
                {
                    double unit = Math.Round(lvl.Elevation * 30.48, 2);
                    elevation = unit.ToString() + "cm";
                }
                else if (units == DisplayUnitType.DUT_METERS)
                {
                    double unit = Math.Round(lvl.Elevation * 0.3048, 2);
                    elevation = unit.ToString() + "m";
                }
                else
                {
                    double unit = Math.Round(lvl.Elevation * 304.8, 2);
                    elevation = unit.ToString() + "mm";
                }


                string str = lvl.Name + " | " + elevation;
                Items.Add(new DynamoDropDownItem(str, lvl));
            }
            return SelectionState.Restore;

        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            AssociativeNode node;

            if (SelectedIndex == -1)
            {
                node = AstFactory.BuildNullNode();
            }
            else
            {
                var level = Items[SelectedIndex].Item as Autodesk.Revit.DB.Level;
                if (level == null)
                {
                    node = AstFactory.BuildNullNode();
                }
                else
                {
                    var idNode = AstFactory.BuildStringNode(level.UniqueId);
                    var falseNode = AstFactory.BuildBooleanNode(true);

                    node = AstFactory.BuildFunctionCall(
                        new Func<string, bool, object>(ElementSelector.ByUniqueId),
                        new List<AssociativeNode> { idNode, falseNode });
                }
            }

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }
    }
    #endregion


}

