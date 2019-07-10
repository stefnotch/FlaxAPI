using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor.Surface.Elements;
using FlaxEngine;

namespace FlaxEditor.Surface
{
    public class NumberGraphSurface : VisjectSurface
    {
        public static readonly List<GroupArchetype> NumberGraphGroup = new List<GroupArchetype>(2)
        {
            new GroupArchetype
            {
                GroupID = 1,
                Name = "NumberGraph",
                Color = new Color(231, 231, 60),
                Archetypes = Archetypes.NumberGraph.Nodes
            },
            new GroupArchetype
            {
                GroupID = 3,
                Name = "Math",
                Color = new Color(52, 152, 219),
                Archetypes = Archetypes.Math.Nodes
            },
            new GroupArchetype
            {
                GroupID = 6,
                Name = "Parameters",
                Color = new Color(52, 73, 94),
                Archetypes = Archetypes.Parameters.Nodes
            }
        };

        /// <inheritdoc />
        public NumberGraphSurface(IVisjectSurfaceOwner owner, Action onSave)
        : base(owner, onSave, null, NumberGraphGroup) // Pass in our custom node archetype
        {
            //this.Nodes
            //this.Nodes[0].Values
            //this.Nodes[0].GetBoxes().DefaultType or .CurrentType
            //this.Nodes[0].GetBoxes().Connections
            //this.Nodes[0].GetBoxes().Connections[0].ParentNode


            //this.Parameters
            //this.Parameters[0].Value
        }


        private Dictionary<Int2, int> _connectionIds = new Dictionary<Int2, int>();
        private HashSet<int> _takenConnectionIds = new HashSet<int>();

        internal NumberGraphDefinition CompileToGraphDefinition()
        {
            _connectionIds = new Dictionary<Int2, int>();
            _takenConnectionIds = new HashSet<int>();
            var parameters = new List<NumberGraphDefinition.NumberGraphParameter>();

            GetParameterGetterNodeArchetype(out ushort ParameterGroupId);

            var debug = DepthFirstTraversal().ToArray();

            var actions =
                DepthFirstTraversal()
                .Select((node) =>
                {
                    // Initialize boxes
                    var boxes = node.GetBoxes();

                    // Get the input boxes and convert them to data
                    object[] data = boxes
                                .Where(box => !box.IsOutput)
                                .Select(box => default(object)) // TODO: Get the actual node data
                                .ToArray();

                    int[] inputBoxIds = boxes
                                        .Where(box => !box.IsOutput)
                                        .Select(box => box.HasAnyConnection ? GetOutputBoxId(box.Connections[0]) : -1)
                                        .ToArray();


                    // Output box ids
                    int[] outputBoxIds = boxes
                                            .Where(box => box.IsOutput)
                                            .Select(GetOutputBoxId)
                                            .ToArray();

                    // Special cases

                    // Special case for the main node
                    if (node.Type == MainNodeType)
                    {
                        // Generate an output box for each *input* box
                        // TODO: Think of something cleaner than this hack
                        outputBoxIds = boxes
                                           .Where(box => !box.IsOutput)
                                           .Select(box =>
                                           {
                                               for (int i = 0; true; i++)
                                               {
                                                   if (!_takenConnectionIds.Contains(i))
                                                   {
                                                       _takenConnectionIds.Add(i);

                                                       return i;
                                                   }
                                               }
                                           })
                                           .ToArray();
                    }

                    // Special case for the parameter node
                    if (node.GroupArchetype.GroupID == ParameterGroupId)
                    {
                        var parameter = GetParameter((Guid)node.Values[0]);
                        var numberGraphParameter = new NumberGraphDefinition.NumberGraphParameter(parameter.Name, parameter.Value);
                        parameters.Add(numberGraphParameter);
                        data = new object[] { parameter.Value };
                        inputBoxIds = new int[] { -1 };
                    }


                    /*var data = node
                                .Values
                                .Select((value, index) =>
                                {
                                    boxes[index].HasAnyConnection
                                    return value;
                                })
                                .ToArray();*/

                    // TODO: Remove unused boxes using RemoveOutputBoxId!

                    return new NumberGraphDefinition.NumberGraphAction(inputBoxIds, data, outputBoxIds, node.GroupArchetype.GroupID, node.Archetype.TypeID);
                })
                .ToList();

            return new NumberGraphDefinition(parameters, actions);
        }

        private List<SurfaceNode> DepthFirstTraversal()
        {
            // https://stackoverflow.com/a/56317289/3492994
            List<SurfaceNode> output = new List<SurfaceNode>(Nodes.Count);
            bool[] visited = new bool[Nodes.Count];
            bool[] onStack = new bool[Nodes.Count];
            Stack<int> toProcess = new Stack<int>(Nodes.Count);

            int mainNodeIndex = GetMainNodeIndex();

            // Start processing the nodes (backwards)
            toProcess.Push(mainNodeIndex);
            while (toProcess.Count > 0)
            {
                int nodeIndex = toProcess.Peek();
                // We have never seen this node before
                if (!visited[nodeIndex])
                {
                    visited[nodeIndex] = true;
                    onStack[nodeIndex] = true;
                }
                else
                {
                    // Otherwise, remove it from the stack
                    onStack[nodeIndex] = false;
                    toProcess.Pop();

                    // And add it to the output
                    output.Add(Nodes[nodeIndex]);
                }

                // For all children, push them onto the stack if they haven't been visited yet
                var boxes = Nodes[nodeIndex].GetBoxes();
                for (int i = 0; i < boxes.Count; i++)
                {
                    // All the input boxes can have children
                    if (!boxes[i].IsOutput && boxes[i].HasAnyConnection)
                    {
                        if (boxes[i].HasSingleConnection)
                        {
                            var parentNode = boxes[i].Connections[0].ParentNode;
                            // TODO: Optimize
                            int parentNodeIndex = Nodes.FindIndex((n) => n == parentNode);
                            if (!visited[parentNodeIndex])
                            {
                                toProcess.Push(parentNodeIndex);
                            }
                            else if (onStack[parentNodeIndex])
                            {
                                throw new Exception("Cycle detected!");
                            }
                        }
                        else
                        {
                            throw new Exception("Input box has more than one connection");
                        }
                    }
                }
            }

            return output;
        }

        private int GetOutputBoxId(Box box)
        {
            if (!box.IsOutput)
            {
                throw new ArgumentException("Not an output box", nameof(box));
            }

            // If this box already has been registered
            if (_connectionIds.TryGetValue(new Int2((int)box.ParentNode.ID, box.ID), out int id))
            {
                return id;
            }

            // Otherwise, register it
            for (int i = 0; true; i++)
            {
                if (!_takenConnectionIds.Contains(i))
                {
                    _connectionIds.Add(new Int2((int)box.ParentNode.ID, box.ID), i);
                    _takenConnectionIds.Add(i);

                    return i;
                }
            }
        }

        private void RemoveOutputBoxId(Box box)
        {
            if (_connectionIds.TryGetValue(new Int2((int)box.ParentNode.ID, box.ID), out int id))
            {
                _connectionIds.Remove(new Int2((int)box.ParentNode.ID, box.ID));
                _takenConnectionIds.Remove(id);
            }
        }

        private uint MainNodeType => ((uint)1 << 16) | 1;

        private int GetMainNodeIndex()
        {

            // Main node index
            int mainNodeIndex = -1;
            uint type = MainNodeType;
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Type == type)
                {
                    mainNodeIndex = i;
                    break;
                }
            }

            if (mainNodeIndex == -1)
            {
                Editor.LogError("Main node not found");
            }

            return mainNodeIndex;
        }
    }
}
