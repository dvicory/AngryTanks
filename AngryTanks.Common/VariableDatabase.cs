using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Common
{
    public class VariableStore
    {
        public readonly TypeCode TypeCode;
        public readonly String Name, Description;

        private Object defaultValue;
        public Object DefaultValue
        {
            get
            {
                return Convert.ChangeType(defaultValue, TypeCode);
            }
            set
            {
                this.defaultValue = value;
            }
        }

        private Object value = null;
        public Object Value
        {
            get
            {
                if (value != null)
                    return Convert.ChangeType(value, TypeCode);
                else
                    return DefaultValue;
            }
            set
            {
                this.value = value;
            }
        }

        public bool Changed
        {
            get
            {
                if (DefaultValue.Equals(Value) || value == null)
                    return false;
                else
                    return true;
            }
        }

        public VariableStore(String name, String description, Object defaultValue, TypeCode typeCode)
        {
            if (defaultValue == null)
                throw new ArgumentNullException("defaultValue", "Default value can not be null");

            this.Name         = name;
            this.Description  = description;
            this.TypeCode     = typeCode;
            this.defaultValue = defaultValue;
        }

        public void Reset()
        {
            Value = null;
        }
    }

    public class VariableDatabase
    {
        /*
         * Variables that should probably be done (straight from BZFlag):
         * explodeTime
         * flagPoleSize
         * flagPoleWidth
         * flagRadius
         * laserAdLife
         * laserAdRate
         * laserAdVel
         * obeseFactor
         * reloadTime
         * shockAdLife
         * shockInRadius
         * shockOutRadius
         * shotRadius
         * shotRange
         * shotSpeed
         * shotTailLength
         * tankAngVel
         * tankHeight
         * tankLength
         * tankSpeed
         * tankWidth
         * tinyFactor
         */

        // where we store all our variables. variables are case-sensitive.
        // (they preserve case once stored, but accessing them and trying to add more is case-insensitive)
        private Dictionary<String, VariableStore> variables = new Dictionary<string, VariableStore>(StringComparer.OrdinalIgnoreCase);

        public VariableStore this[String name]
        {
            get 
            {
                return variables[name];
            }
            set
            {
                variables[name] = value;
            }
        }

        public List<VariableStore> Changed
        {
            get
            {
                List<VariableStore> result = new List<VariableStore>();
                foreach (VariableStore variable in variables.Values)
                {
                    if (variable.Changed)
                        result.Add(variable);
                }

                return result;
            }
        }

        public VariableDatabase()
        {
            AddDefaultVariables();
        }

        private void AddDefaultVariables()
        {
            AddVariable("explodeTime",
                        "Time (in seconds) to respawn after being killed", 5f, typeof(Single));
            AddVariable("flagRadius",
                        "Determines how close a tank must be to a flag to pick it up", 2.5f, typeof(Single));
            AddVariable("reloadTime",
                        "Time (in seconds) between shot reloads", 3.5f, typeof(Single));
            AddVariable("shotRange",
                        "Range of shots", 350f, typeof(Single));
            AddVariable("shotSlots",
                        "Number of shot slots", 5, typeof(UInt16));
            AddVariable("shotSpeed",
                        "Speed of shots", 100f, typeof(Single));
            AddVariable("tankAngVel",
                        "Angular speed (radians/sec) of the tank", (Single)Math.PI / 2, typeof(Single));
            AddVariable("tankLength",
                        "Length of the tank", 6f, typeof(Single));
            AddVariable("tankSpeed",
                        "Speed of the tank", 25f, typeof(Single));
            AddVariable("tankWidth",
                        "Width of the tank", 4.86f, typeof(Single));
            AddVariable("updatesPerSecond",
                        "Number of network updates per second", 45, typeof(UInt16));
        }

        public VariableStore AddVariable(String name, String description, Object defaultValue, Type type)
        {
            VariableStore variable = new VariableStore(name, description, defaultValue, Type.GetTypeCode(type));
            variables.Add(name, variable);
            return variable;
        }

        public void ResetAllVariables()
        {
            foreach (VariableStore variable in variables.Values)
                ResetVariable(variable);
        }

        public void ResetVariables(List<String> variableNames)
        {
            foreach (String variableName in variableNames)
                ResetVariable(variables[variableName]);
        }

        public void ResetVariable(VariableStore variable)
        {
            variable.Reset();
        }
    }
}
