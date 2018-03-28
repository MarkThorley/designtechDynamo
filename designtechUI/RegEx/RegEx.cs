using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;

namespace designtechUI
{
    [NodeName("ClassTypes")]
    [NodeCategory("designtech.dtRegularExpression.RegEx")]
    [NodeDescription("RegEx class types selection")]
    [IsDesignScriptCompatible]
    public class ClassTypes : DSDropDownBase
    {
        public ClassTypes() : base("RegExClassType") { }

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

            var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Fixed", "ClassType.Fixed"),
                new DynamoDropDownItem("List", "ClassType.List"),
                new DynamoDropDownItem("Varies", "ClassType.Varies"),
                new DynamoDropDownItem("Minimum", "ClassType.Minimum"),
                new DynamoDropDownItem("Maximum", "ClassType.Maximum"),
                new DynamoDropDownItem("Range", "ClassType.Range"),
                new DynamoDropDownItem("Count", "ClassType.Count")
            };

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
