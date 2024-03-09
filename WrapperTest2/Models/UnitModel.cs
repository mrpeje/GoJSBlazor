using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperTest2.Models
{
    internal class UnitModel
    {
        public string Name;
        public UnitType Type;
        public int Id;
        public List<Port> InputPorts;
        public List<Port> OutputPorts;        
    }
    internal class Port
    {
        public Port()
        {
            int Id;
            string Name;
            string Description;
            string Color;
        }
    }
    public enum UnitType
    {
        Type1,
        Type2,
        Type3
    }

}
