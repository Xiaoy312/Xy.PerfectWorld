using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis.Extension;
using Xy.DataAnalysis.Util;

namespace Xy.DataAnalysis
{
    public abstract class Entity
    {
        public virtual void DumpProperties()
        {
            var properties = this.GetType().GetProperties();

            //remember to tag [BaseAddress]
            var entityBase = properties.Single(p => p.HasAttribute<BaseAddressAttribute>());
            var entityPropertyies = properties.Where(p => !p.HasAttribute<BaseAddressAttribute>())
                .Where(p => typeof(PointerBase).IsAssignableFrom(p.PropertyType))
                .OrderBy(p => (p.GetValue(this, null) as PointerBase).GetOffset());

            Prettify(entityBase).Dump();

            using (Indentation.Indent())
            {
                int padding = entityPropertyies.Max(x => x.Name.Length);
                foreach (var property in entityPropertyies)
                {
                    Prettify(property, padding).Dump();
                }
            }

            Environment.NewLine.Dump();
        }
        public string Prettify(PropertyInfo property, int padding = 0)
        {
            var pointer = property.GetValue(this, null) as PointerBase;
            var type = property.PropertyType;
            var hexadecimal = pointer.GetValue().GetType() == typeof(int) &&
                (property.HasAttribute<BaseAddressAttribute>() || property.HasAttribute<HexadecimalAttribute>());

            var propertyName = property.Name.PadRight(padding);

            var isBaseAddress = property.HasAttribute<BaseAddressAttribute>();
            var isDataPointer = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Pointer<>);
            if (isBaseAddress || !isDataPointer)
            {
                return String.Format("{0} : {1:X8} -> {2:X8}",
                    propertyName, pointer.Address, pointer.GetValue());
            }
            else
            {
                return String.Format("[+{0:X3}] {1} : {2:X8} -> " + (hexadecimal ? "{3:X8}" : "{3}"),
                    pointer.GetOffset(), propertyName, pointer.Address, pointer.GetValue());
            }
        }
    }
}
