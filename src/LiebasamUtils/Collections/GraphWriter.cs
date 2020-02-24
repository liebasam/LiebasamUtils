using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LiebasamUtils.Collections
{
    public class GraphWriter<TVert, TEdge>
    {
        #region String Constants
        const string InvalidProperty = "Invalid property name \"{0}\".";

        const string Ext = ".png";
        const string Label = "label";
        const string Args = "-Tpng {0} -o {1}";
        const string StartFormat = "digraph {0} {{\n";
        const string GraphPropFormat = "{0} = \"{1}\";\n";
        const string PropFormat = "{0}=\"{1}\" ";
        const string VertFormat = "\"{0}\" [{1}];\n";
        const string EdgeFormat = "\"{0}\" -> \"{1}\" [{2}];\n";
        const string EndFormat = "}";
        #endregion

        string Name = string.Empty;
        readonly Dictionary<string, object> GraphProps;
        readonly Graph<Dictionary<string, object>, Dictionary<string, object>> Props;

        #region Constructors
        /// <summary>
        /// Creates a new <strong>GraphWriter</strong> from the provided graph.
        /// </summary>
        public GraphWriter(IGraph<TVert, TEdge> graph)
        {
            if (graph is null)
                throw new ArgumentNullException(nameof(graph));

            GraphProps = new Dictionary<string, object>();
            Props = new Graph<Dictionary<string, object>, Dictionary<string, object>>(
                graph.Vertices.Select(SelectVert), graph.Edges.Select(SelectEdge));
        }

        /// <summary>
        /// Creates a new <strong>GraphWriter</strong> from the provided graph
        /// and with the given name.
        /// </summary>
        public GraphWriter(IGraph<TVert, TEdge> graph, string name) : this(graph)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            Name = name;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the property of a particular vertex.
        /// </summary>
        public void SetVertexProperty(uint id, string property, object value)
        {
            property = ValidateKeyValue(property, value);
            Props[id][property] = value;
        }

        /// <summary>
        /// Gets the property of a particular vertex.
        /// </summary>
        public object GetVertexProperty(uint id, string property)
        {
            property = ValidateKey(property);
            return Props[id][property];
        }

        /// <summary>
        /// Sets the property of a particular edge.
        /// </summary>
        public void SetEdgeProperty(uint fromID, uint toID, string property, object value)
        {
            property = ValidateKeyValue(property, value);
            Props[fromID, toID][property] = value;
        }

        /// <summary>
        /// Gets the property of a particular edge.
        /// </summary>
        public object GetEdgeProperty(uint fromID, uint toID, string property)
        {
            property = ValidateKey(property);
            return Props[fromID, toID][property];
        }

        /// <summary>
        /// Sets the property of the entire graph.
        /// </summary>
        public void SetGraphProperty(string property, object value)
        {
            property = ValidateKeyValue(property, value);
            if (property.Equals("name"))
                Name = value.ToString();
            else
                GraphProps[property] = value;
        }

        /// <summary>
        /// Gets a property of the entire graph.
        /// </summary>
        /// <returns></returns>
        public object GetGraphProperty(string property)
        {
            property = ValidateKey(property);
            if (property.Equals("name"))
                return Name;
            else
                return GraphProps[property];
        }

        /// <summary>
        /// Compiles the graph to a PNG file specified by
        /// <paramref name="fullfile"/>.
        /// </summary>
        /// <returns>Exit code of the process.</returns>
        public int Compile(string fullfile)
        {
            fullfile = ValidateOutputFile(fullfile);

            // Write to temp file
            var tempfile = Path.GetTempFileName();
            File.WriteAllText(tempfile, ToString());

            // Compile temp file
            ProcessStartInfo psi = new ProcessStartInfo("dot")
            {
                Arguments = string.Format(Args, tempfile, fullfile),
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (Process p = Process.Start(psi))
            {
                p.WaitForExit(10000);
                int code = p.ExitCode;
                p.Close();
                return code;
            }

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var propsb = new StringBuilder();

            // Graph properties
            sb.AppendFormat(StartFormat, Name);
            foreach (var kv in GraphProps)
                sb.AppendFormat(GraphPropFormat, kv.Key, kv.Value);

            // Vertex properties
            foreach (var vert in Props.Vertices)
            {
                propsb.Clear();
                foreach (var kv in vert.Value)
                    propsb.AppendFormat(PropFormat, kv.Key, kv.Value);
                sb.AppendFormat(VertFormat, vert.ID, propsb.ToString());
            }

            // Edge properties
            foreach (var edge in Props.Edges)
            {
                propsb.Clear();
                foreach (var kv in edge.Value)
                    propsb.AppendFormat(PropFormat, kv.Key, kv.Value);
                sb.AppendFormat(EdgeFormat, edge.IDFrom, edge.IDTo, propsb.ToString());
            }

            sb.Append(EndFormat);

            return sb.ToString();
        }
        #endregion

        #region Helpers
        static string ValidateKey(string property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (string.IsNullOrWhiteSpace(property))
                throw new ArgumentException(string.Format(InvalidProperty, property));
            return property.ToLower();
        }

        static string ValidateKeyValue(string property, object value)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (string.IsNullOrWhiteSpace(property))
                throw new ArgumentException(string.Format(InvalidProperty, property));
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            return property.ToLower();
        }

        static string ValidateOutputFile(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException();

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return Path.ChangeExtension(path, Ext);
        }

        static Vertex<Dictionary<string, object>> SelectVert(Vertex<TVert> vert)
        {
            var dict = new Dictionary<string, object>();
            dict.Add(Label, vert.Value);
            return new Vertex<Dictionary<string, object>>(vert.ID, dict);
        }

        static Edge<Dictionary<string, object>> SelectEdge(Edge<TEdge> edge)
        {
            var dict = new Dictionary<string, object>();
            dict.Add(Label, edge.Value);
            return new Edge<Dictionary<string, object>>(edge.IDFrom, edge.IDTo, dict);
        }
        #endregion
    }
}
