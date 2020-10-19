using System.Collections.Specialized;

namespace RightPoint.Web.Environment
{
    /// <summary>
    /// Summary description for ConnectionDictionary.
    /// </summary>
    public class EnvironmentDictionary : HybridDictionary
    {
        public void Add(Environment value)
        {
            base.Add(value.MachineType, value);
        }

        private new void Add(object key, object value)
        {
            base.Add(key, value);
        }

        public Environment this[MachineType machineType]
        {
            get { return ((Environment)base[machineType]); }
            set { base[machineType] = value; }
        }

        private new object this[object key]
        {
            get { return (base[key]); }
            set { base[key] = value; }
        }

        public bool Contains(MachineType machineType)
        {
            return (base.Contains(machineType));
        }

        private new bool Contains(object key)
        {
            return (base.Contains(key));
        }
    }
}