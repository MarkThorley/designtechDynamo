using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dynamo.Core;
using Dynamo.Applications;
using Autodesk.DesignScript.Runtime;
using System.Windows.Media;

namespace dtDynamo
{
    public class NodeGraph
    {
        internal NodeGraph()
        {

        }
        #region GetFromFilePath
        /// <summary>
        /// Retrieves the Dynamo node graph from the dyn file path.
        /// </summary>
        /// <param name="dynFilePath">string</param>
        /// <returns name="nodeGraph">Dynamo.Graph.NodeGraph</returns>
        /// <search>
        /// dynamo, api, file, path, node, graph, retrieve, get, from
        /// </search>
        public static Dynamo.Graph.NodeGraph GetFromFilePath(string dynFilePath)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(dynFilePath);

            Dynamo.Graph.NodeGraph nodeGraph;
            Dynamo.Graph.Nodes.NodeLoaders.NodeFactory nodeFactory = new Dynamo.Graph.Nodes.NodeLoaders.NodeFactory();
            nodeGraph = Dynamo.Graph.NodeGraph.LoadGraphFromXml(doc, nodeFactory);

            return nodeGraph;
        }
        #endregion

        #region Groups
        /// <summary>
        /// Retrieves the groups/annotations from the Node Graph
        /// </summary>
        /// <param name="nodeGraph">Dynamo.Graph.NodeGraph</param>
        /// <returns name="groups">Dynamo.Graph.Annotations.AnnotationModel</returns>
        /// <search>
        /// dynamo, api, node, graph, annotation, model, groups
        /// </search>
        public static List<Dynamo.Graph.Annotations.AnnotationModel> Groups(Dynamo.Graph.NodeGraph nodeGraph)
        {
            return nodeGraph.Annotations;
        }
        #endregion

        #region Connectors
        /// <summary>
        /// Retrieves the current amount of connectors from the node graph
        /// </summary>
        /// <param name="nodeGraph">Dynamo.Graph.NodeGraph</param>
        /// <returns name="amount">int</returns>
        /// <search>
        /// dynamo, api, current, node, graph, wires, connectors, count
        /// </search>
        public static int Connectors(Dynamo.Graph.NodeGraph nodeGraph)
        {
            List<Dynamo.Graph.Connectors.ConnectorModel> con = new List<Dynamo.Graph.Connectors.ConnectorModel>(nodeGraph.Connectors);
            return con.Count;
        }
        #endregion

        #region Nodes
        /// <summary>
        /// Retrieves the nodes from the Node Graph
        /// </summary>
        /// <param name="nodeGraph">Dynamo.Graph.NodeGraphl</param>
        /// <returns name="nodes">Dynamo.Graph.Nodes.NodeModel</returns>
        /// <search>
        /// dynamo, api, node, graph, nodes, model
        /// </search>
        public static IEnumerable<Dynamo.Graph.Nodes.NodeModel> Nodes(Dynamo.Graph.NodeGraph nodeGraph)
        {
            return nodeGraph.Nodes;
        }
        #endregion

        #region Notes
        /// <summary>
        /// Retrieves the text from the notes of the supplied node graph
        /// </summary>
        /// <param name="nodeGraph">Dynamo.Graph.NodeGraph</param>
        /// <returns name="notes">Dynamo.Graph.Notes.NoteModel</returns>
        /// <search>
        /// dynamo, api, current, node, graph, notes, text, string
        /// </search>
        public static IEnumerable<Dynamo.Graph.Notes.NoteModel> Notes(Dynamo.Graph.NodeGraph nodeGraph)
        {
            //List<Dynamo.Graph.Notes.NoteModel> notes = new List<Dynamo.Graph.Notes.NoteModel>(nodeGraph.Notes);
            //
            //List<string> output = new List<string>();
            //foreach (var n in notes)
            //{
            //    output.Add(n.Text.ToString());
            //}
            return nodeGraph.Notes;
        }
        #endregion

    }

    public class Workspace
    {
        internal Workspace()
        {

        }

        #region Author
        /// <summary>
        /// Tells you the Dynamo Graphs Author.
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="author">string</returns>
        /// <search>
        /// dynamo, api, workspace, author, name, person
        /// </search>
        public static string Author(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            return workspace.Author;
        }
        #endregion

        #region Connectors
        /// <summary>
        /// Retrieves the amount of connectors from the workspace
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="amount">int</returns>
        /// <search>
        /// dynamo, api, workspace, wires, connectors, count
        /// </search>
        public static int Connectors(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            List<Dynamo.Graph.Connectors.ConnectorModel> con = new List<Dynamo.Graph.Connectors.ConnectorModel>(workspace.Connectors);
            return con.Count;
        }
        #endregion

        #region Current
        /// <summary>
        /// Retrieves the current Dynamo workspace model.
        /// </summary>
        /// <returns name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</returns>
        /// <search>
        /// dynamo, api, current, workspace, retrieve, get
        /// </search>
        public static Dynamo.Graph.Workspaces.WorkspaceModel Current()
        {
            var model = Dynamo.Applications.DynamoRevit.RevitDynamoModel;
            Dynamo.Graph.Workspaces.WorkspaceModel ws = model.CurrentWorkspace;
            return ws;
        }
        #endregion

        #region Description
        /// <summary>
        /// Tells you the Dynamo Graphs Description.
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="description">string</returns>
        /// <search>
        /// dynamo, api, workspace, description, notes, field
        /// </search>
        public static string Description(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            var message = workspace.Description;
            if (message == null)
            {
                return "Description field is empty.";
            }
            else
            {
                return message;
            }

        }
        #endregion

        #region FileName
        /// <summary>
        /// Tells you the Dynamo Graphs File Name.
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="fileName">string</returns>
        /// <search>
        /// dynamo, api, workspace, filename
        /// </search>
        public static string FileName(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            string f = workspace.FileName;
            if (f == "")
            {
                return "File has not been saved yet.";
            }
            int ind = f.LastIndexOf("\\");
            string fN = f.Remove(0, (ind + 1));
            return fN;
            //return workspace.FileName;
        }
        #endregion

        #region FilePath
        /// <summary>
        /// Tells you the Dynamo Graphs File Path.
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="filePath">string</returns>
        /// <search>
        /// dynamo, api, workspace, file, path
        /// </search>
        public static string FilePath(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            return workspace.FileName;
        }
        #endregion

        #region Groups
        /// <summary>
        /// Retrieves the groups/annotations from the workspace
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.NodeGraphl</param>
        /// <returns name="groups">Dynamo.Graph.Annotations.AnnotationModel</returns>
        /// <search>
        /// dynamo, api, node, graph, annotation, model, groups
        /// </search>
        public static IEnumerable<Dynamo.Graph.Annotations.AnnotationModel> Groups(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            return workspace.Annotations;
        }
        #endregion
        
        #region GUID
        /// <summary>
        /// Tells you the Dynamo Graphs GUID.
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="guid">Guid</returns>
        /// <search>
        /// dynamo, api, workspace, guid, global unique identifier, unique
        /// </search>
        public static Guid GUID(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            return workspace.Guid;
        }
        #endregion

        #region HasUnsavedChanges
        /// <summary>
        /// Tells you if the Dynamo Graph has any Unsaved Changes.
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="bool">bool</returns>
        /// <search>
        /// dynamo, api, workspace, unsaved, changes, 
        /// </search>
        public static bool UnsavedChanges(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            return workspace.HasUnsavedChanges;
        }
        #endregion

        #region IsReadOnly
        /// <summary>
        /// Tells you if the Dynamo Graph is Read Only.
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="bool">bool</returns>
        /// <search>
        /// dynamo, api, workspace, readonly, read only, is read only
        /// </search>
        public static bool IsReadOnly(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            return workspace.IsReadOnly;
        }
        #endregion

        #region LastSaved
        /// <summary>
        /// Tells you the Dynamo Graphs last Save Time.
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="lastSaved">DateTime</returns>
        /// <search>
        /// dynamo, api, workspace, save, last saved 
        /// </search>
        public static DateTime LastSaved(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            return workspace.LastSaved;
        }
        #endregion

        #region Name
        /// <summary>
        /// Tells you the Dynamo Graphs Name.
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="name">string</returns>
        /// <search>
        /// dynamo, api, workspace, name, title
        /// </search>
        public static string Name(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            return workspace.Name;
        }
        #endregion

        #region Nodes
        /// <summary>
        /// Retrieves the nodes from the workspace
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="nodes">Dynamo.Graph.Nodes.NodeModel</returns>
        /// <search>
        /// dynamo, api, node, workspace, nodes, model
        /// </search>
        public static IEnumerable<Dynamo.Graph.Nodes.NodeModel> Nodes(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            return workspace.Nodes;
        }
        #endregion

        #region Notes
        /// <summary>
        /// Retrieves all Notes from the workspace.
        /// </summary>
        /// <param name="workspace">Dynamo.Graph.Workspaces.WorkspaceModel</param>
        /// <returns name="notes">Dynamo.Graph.Notes.NoteModel</returns>
        /// <search>
        /// dynamo, api, workspace, notes, text
        /// </search>
        public static IEnumerable<Dynamo.Graph.Notes.NoteModel> Notes(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            return workspace.Notes;
        }
        #endregion

    }

    public class Node
    {
        internal Node()
        {

        }

        #region Category
        /// <summary>
        /// Retrieves the current amount of nodes from the workspace
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="category">string</returns>
        /// <search>
        /// dynamo, api, node, model, name, string
        /// </search>
        public static string Category(Dynamo.Graph.Nodes.NodeModel node)
        {
            return node.Category;
        }
        #endregion

        #region CreationName
        /// <summary>
        /// Retrieves the type of the node
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="name">string</returns>
        /// <search>
        /// dynamo, api, node, model, name, string, creation
        /// </search>
        public static string CreationName(Dynamo.Graph.Nodes.NodeModel node)
        {
            return node.CreationName;
        }
        #endregion

        #region Description
        /// <summary>
        /// Retrieves the description of the node
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="desc">string</returns>
        /// <search>
        /// dynamo, api, node, model, description, string
        /// </search>
        public static string Description(Dynamo.Graph.Nodes.NodeModel node)
        {
            return node.Description;
        }
        #endregion

        #region GUID
        /// <summary>
        /// Retrieves the Guid of the node
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="guid">Guid</returns>
        /// <search>
        /// dynamo, api, node, model, guid
        /// </search>
        public static Guid GUID(Dynamo.Graph.Nodes.NodeModel node)
        {
            return node.GUID;
        }
        #endregion

        #region IsFrozen
        /// <summary>
        /// Tells you if the node is frozen
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="bool">bool</returns>
        /// <search>
        /// dynamo, api, node, model, is, frozen, bool, yes, no, true, false
        /// </search>
        public static bool IsFrozen(Dynamo.Graph.Nodes.NodeModel node)
        {
            return node.IsFrozen;
        }
        #endregion

        #region IsInErrorState
        /// <summary>
        /// Tells you if the node has an error
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="bool">bool</returns>
        /// <search>
        /// dynamo, api, node, element, state, error, is, in, bool, yes, no, true, false
        /// </search>
        public static bool IsInErrorState(Dynamo.Graph.Nodes.NodeModel node)
        {
            return node.IsInErrorState;
        }
        #endregion

        #region Name
        /// <summary>
        /// Retrieves the name of the node
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="name">string</returns>
        /// <search>
        /// dynamo, api, node, model, name, string
        /// </search>
        public static string Name(Dynamo.Graph.Nodes.NodeModel node)
        {
            return node.Name;
        }
        #endregion

        #region Nickname
        /// <summary>
        /// Retrieves the nickname of the node
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="nickname">string</returns>
        /// <search>
        /// dynamo, api, node, nickname, name, string
        /// </search>
        public static string Nickname(Dynamo.Graph.Nodes.NodeModel node)
        {
            return node.NickName;
        }
        #endregion

        #region Position
        /// <summary>
        /// Retrieves the position of the node
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="position">string</returns>
        /// <search>
        /// dynamo, api, node, model, position, x, y
        /// </search>
        public static string Position(Dynamo.Graph.Nodes.NodeModel node)
        {
            double x = node.CenterX;
            double y = node.CenterY;

            string pos = "x = " + x.ToString() + ", y = " + y.ToString();
            return pos;
        }
        #endregion

        #region State
        /// <summary>
        /// Tells you current state of the node. Will return either Active, AstBuildBroken, Dead, Error, PersistentWarning or Warning
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="state">string</returns>
        /// <search>
        /// dynamo, api, node, element, state, error, warning, active, AstBuildBroken, Dead, Error, PersistentWarning
        /// </search>
        public static string State(Dynamo.Graph.Nodes.NodeModel node)
        {
            string eS = node.State.ToString();
            return eS;
        }
        #endregion

        #region Tags
        /// <summary>
        /// Retrieves the Tags of the node
        /// </summary>
        /// <param name="node">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="tags">string</returns>
        /// <search>
        /// dynamo, api, node, model, tags
        /// </search>
        public static List<string> Tags(Dynamo.Graph.Nodes.NodeModel node)
        {
            return node.Tags;
        }
        #endregion


    }

    public class Note
    {
        internal Note()
        {

        }

        /*#region Create
        /// <summary>
        /// Creates a new note
        /// </summary>
        /// <param name="x">double</param>
        /// <param name="y">double</param>
        /// <param name="text">string</param>
        /// <param name="fileName">string</param>
        /// <returns name="note">guid</returns>
        /// <search>
        /// dynamo, api, note, model, guid
        /// </search>
        public static Dynamo.Graph.Notes.NoteModel Create(double x, double y, string text, string fileName)
        {
            Guid guid = new Guid();
            Dynamo.Graph.Notes.NoteModel note = new Dynamo.Graph.Notes.NoteModel(x, y, text, guid);
            Dynamo.Graph.Workspaces.WorkspaceModel ws = Workspace.Current();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(fileName);
            Dynamo.Graph.SaveContext saveContext = Dynamo.Graph.SaveContext.Preset;
            note.Serialize(doc, saveContext);
            doc.Save(fileName);

            return note;
        }
        #endregion*/

        #region GUID
        /// <summary>
        /// Retrieves the guid of the note
        /// </summary>
        /// <param name="note">Dynamo.Graph.Notes.NoteModel</param>
        /// <returns name="guid">guid</returns>
        /// <search>
        /// dynamo, api, note, model, guid
        /// </search>
        public static Guid GUID(Dynamo.Graph.Notes.NoteModel note)
        {
            return note.GUID;
        }
        #endregion

        #region Position
        /// <summary>
        /// Retrieves the position of the node
        /// </summary>
        /// <param name="note">Dynamo.Graph.Nodes.NodeModel</param>
        /// <returns name="position">string</returns>
        /// <search>
        /// dynamo, api, note, model, position, x, y
        /// </search>
        public static string Position(Dynamo.Graph.Notes.NoteModel note)
        {
            double x = note.CenterX;
            double y = note.CenterY;

            string pos = "x = " + x.ToString() + ", y = " + y.ToString();
            return pos;
        }
        #endregion

        #region Text
        /// <summary>
        /// Retrieves the text of the note
        /// </summary>
        /// <param name="note">Dynamo.Graph.Notes.NoteModel</param>
        /// <returns name="text">string</returns>
        /// <search>
        /// dynamo, api, note, text, string
        /// </search>
        public static string Text(Dynamo.Graph.Notes.NoteModel note)
        {
            return note.Text;
        }
        #endregion

    }

    public class Group
    {
        internal Group()
        {

        }

        #region Colour
        /// <summary>
        /// Retrieves the colour of the group
        /// </summary>
        /// <param name="group">Dynamo.Graph.Annotations.AnnotationModel</param>
        /// <returns name="colour">Colour</returns>
        /// <search>
        /// dynamo, api, group, annotation, model, colour, color
        /// </search>
        public static DSCore.Color Colour(Dynamo.Graph.Annotations.AnnotationModel group)
        {
            string back = group.Background;

            Color colour = (Color)ColorConverter.ConvertFromString(back);

            int a = colour.A;
            int r = colour.R;
            int g = colour.G;
            int b = colour.B;

            DSCore.Color dynColour = DSCore.Color.ByARGB(a, r, g, b);

            return dynColour;
        }
        #endregion

        #region FontSize
        /// <summary>
        /// Retrieves the font size of the group
        /// </summary>
        /// <param name="group">Dynamo.Graph.Annotations.AnnotationModel</param>
        /// <returns name="fontSize">double</returns>
        /// <search>
        /// dynamo, api, group, annotation, model, font, size, text
        /// </search>
        public static double FontSize(Dynamo.Graph.Annotations.AnnotationModel group)
        {
            return group.FontSize;
        }
        #endregion

        #region GUID
        /// <summary>
        /// Retrieves the guid of the group
        /// </summary>
        /// <param name="group">Dynamo.Graph.Annotations.AnnotationModel</param>
        /// <returns name="guid">guid</returns>
        /// <search>
        /// dynamo, api, group, annotation, model, guid
        /// </search>
        public static Guid GUID(Dynamo.Graph.Annotations.AnnotationModel group)
        {
            return group.GUID;
        }
        #endregion

        #region Position
        /// <summary>
        /// Retrieves the position of the group
        /// </summary>
        /// <param name="group">Dynamo.Graph.Annotations.AnnotationModel</param>
        /// <returns name="position">string</returns>
        /// <search>
        /// dynamo, api, group, annotation, model, position, x, y
        /// </search>
        public static string Position(Dynamo.Graph.Annotations.AnnotationModel group)
        {
            double x = group.CenterX;
            double y = group.CenterY;

            string pos = "x = " + x.ToString() + ", y = " + y.ToString();
            return pos;
        }
        #endregion

        #region Title
        /// <summary>
        /// Retrieves the title of the group
        /// </summary>
        /// <param name="group">Dynamo.Graph.Annotations.AnnotationModel</param>
        /// <returns name="title">string</returns>
        /// <search>
        /// dynamo, api, group, annotation, model, title, string, text
        /// </search>
        public static string Title(Dynamo.Graph.Annotations.AnnotationModel group)
        {
            return group.AnnotationText;
        }
        #endregion

    }
}
