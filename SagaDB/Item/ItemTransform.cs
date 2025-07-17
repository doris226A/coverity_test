using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaDB.Item
{
    public class ItemTransform
    {
        public uint ChangeID { get; set; }
        public uint Product { get; set; }
        public string TransformCondition { get; set; }
        public string Comment { get; set; }

        public Dictionary<uint, List<ItemTransformStuffAbility>> Stuffs = new Dictionary<uint, List<ItemTransformStuffAbility>>();
    }

    public class ItemTransformStuffAbility
    {
        public string Ability { get; set; }
        public uint Value { get; set; }
    }
}
