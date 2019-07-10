using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaxEngine
{
    [Serializable]
    internal class NumberGraphDefinition
    {
        /// <summary>
        /// Takes an input and returns some output
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public delegate void ExecuteAction(object[] input, ICollection<object> output);

        /// <summary>
        /// Types that a number graph understands
        /// </summary>
        [NoSerialize]
        public static readonly Type[] KnownTypes = new Type[] { typeof(float), typeof(Vector2), typeof(Vector3), typeof(Vector4) };

        // NumberGraph Nodes
        // 1,1 => Output
        // 1,2 => Random Vector3 (changes every second)
        // 3,x => Maffs
        // 6,x => Parameters

        [NoSerialize]
        // Action<Inputs, Outputs>
        public static readonly Dictionary<int, Dictionary<int, ExecuteAction>> ActionMap = new Dictionary<int, Dictionary<int, ExecuteAction>>()
        {
            [1] = new Dictionary<int, ExecuteAction>()
            {
                // Main output node
                [1] = (input, output) =>
                {
                    // TODO: casting like float <--> Vector2

                    output.Add(As<float>(input[0]));
                    var x = As<double>(input[0]);
                    output.Add(As<Vector2>(input[1]));
                    output.Add(As<Vector3>(input[2]));

                }
            },
            [3] = new Dictionary<int, ExecuteAction>()
            {
                // Multiply
                [3] = (input, output) =>
                {
                    output.Add(As<float>(input[0]) * As<float>(input[1]));
                }
            },
            [6] = new Dictionary<int, ExecuteAction>()
            {
                // Parameter Node
                [1] = (input, output) =>
                {
                    output.Add(input[0]);
                }
            }
        };

        // Note: I'm not using a type enum, because the "is" check is fast enough anyways

        public class NumberGraphParameter
        {
            public readonly string Name;
            public readonly object Value;


            public NumberGraphParameter() : this("")
            {

            }

            public NumberGraphParameter(string name) : this(name, null)
            {
            }

            public NumberGraphParameter(string name, object value)
            {
                Name = name;
                Value = value;
            }
        }

        public class NumberGraphAction
        {
            public readonly int[] DataInput;
            public readonly object[] Data;
            public readonly int[] DataOutput;
            public readonly int ActionType;
            public readonly int ActionSubtype;

            public NumberGraphAction(int[] dataInput, object[] data, int[] dataOutput, int actionType, int actionSubtype)
            {
                DataInput = dataInput;
                Data = data;
                DataOutput = dataOutput;
                ActionType = actionType;
                ActionSubtype = actionSubtype;
            }

            public void Execute(List<object> variables)
            {
                // TODO: Cache and reuse this list
                List<object> outputData = new List<object>();

                // Execute the action
                ActionMap[ActionType][ActionSubtype].Invoke(GetData(variables), outputData);

                // Copy the output data to the variables array
                for (int i = 0; i < DataOutput.Length; i++)
                {
                    if (i < outputData.Count)
                    {
                        variables[DataOutput[i]] = outputData[i];
                    }
                    else
                    {
                        // TODO: What if the node doesn't output anything?
                    }
                }
            }

            private object[] GetData(List<object> variables)
            {
                for (int i = 0; i < Data.Length; i++)
                {
                    if (DataInput[i] != -1)
                    {
                        Data[i] = variables[DataInput[i]];
                    }
                }
                return Data;
            }
        }

        public static T As<T>(object Value)
        {

            if (Value == null)
            {
                return default(T);
            }
            else if (typeof(T) == typeof(float))
            {
                // Special handling for numbers
                // TODO: Replace this with something more efficient and/or better
                return (T)Convert.ChangeType(Value, typeof(T));
            }
            else if (Value is T castedValue)
            {
                return castedValue;

            }
            // TODO: Special cases for float --> Vector2 and whatnot
            else
            {
                return default(T);
            }
        }


        [Serialize]
        private int _maxVariableIndex;
        [Serialize]
        private List<NumberGraphParameter> _parameters;
        [Serialize]
        private List<NumberGraphAction> _actions;

        public NumberGraphDefinition(List<NumberGraphParameter> parameters, List<NumberGraphAction> actions)
        {
            Parameters = parameters;
            Actions = actions;

            if (Actions != null)
            {
                // Maximum variable index
                foreach (var action in Actions)
                {
                    foreach (var outputIndex in action.DataOutput)
                    {
                        if (outputIndex > _maxVariableIndex)
                        {
                            _maxVariableIndex = outputIndex;
                        }
                    }
                }
            }
        }

        [NoSerialize]
        public List<NumberGraphParameter> Parameters
        {
            get { return _parameters; }
            private set { _parameters = value; }
        }

        [NoSerialize]
        public List<NumberGraphAction> Actions
        {
            get { return _actions; }
            private set { _actions = value; }
        }

        [NoSerialize]
        public float OutputFloat { get; private set; }

        [NoSerialize]
        public Vector2 OutputVector2 { get; private set; }

        [NoSerialize]
        public Vector3 OutputVector3 { get; private set; }


        public void Update()
        {
            // Action outputs and inputs
            // TODO: Expand when there are more variables?
            List<object> variables = new List<object>(new object[_maxVariableIndex + 1]);
            // Execute the actions
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Execute(variables);
            }

            if (Actions.Count >= 1)
            {
                var mainAction = Actions[Actions.Count - 1];

                // Set the outputs
                OutputFloat = As<float>(variables[mainAction.DataOutput[0]]);
                OutputVector2 = As<Vector2>(variables[mainAction.DataOutput[1]]);
                OutputVector3 = As<Vector3>(variables[mainAction.DataOutput[2]]);
            }
        }
    }
}
