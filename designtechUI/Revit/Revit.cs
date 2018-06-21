using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using ProtoCore.AST.AssociativeAST;
using System.Linq;
using Autodesk.Revit.DB;
using Revit.Elements;


namespace dtRevit
{
    [NodeName("ViewTypes")]
    [NodeCategory("designtech.dtRevit.Collector")]
    [NodeDescription("A drop down list of levels based on their elevational height")]
    [IsDesignScriptCompatible]
    public class ViewTypes : DSDropDownBase
    {
        public ViewTypes() : base("Levels") { }

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

            List<DynamoDropDownItem> newItems = new List<DynamoDropDownItem>();

            foreach (KeyValuePair<Autodesk.Revit.DB.Level, double> i in items)
            {
                DynamoDropDownItem item = new DynamoDropDownItem(i.Key.Name, i);
                newItems.Add(item);
            }

            foreach (var e in newItems)
            {
                Items.Add(e);
            }
            // Set the selected index to something other
            // than -1, the default, so that your list
            // has a pre-selection.

            SelectedIndex = 0;
            return SelectionState.Done;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // Build an AST node for the type of object contained in your Items collection.

            var intNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
    }
}

