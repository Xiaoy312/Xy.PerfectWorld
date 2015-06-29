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
            Prettify(entityBase).Dump();
            var baseDepth = (entityBase.GetValue(this, null) as PointerBase).GetDepth();

            var entityPropertyies = properties.Where(p => !p.HasAttribute<BaseAddressAttribute>())
                .Where(p => typeof(PointerBase).IsAssignableFrom(p.PropertyType))
                /*.OrderBy(p =>
                {
                    var pointer = p.GetValue(this, null) as PointerBase;
                    return pointer.GetOffsets(pointer.GetDepth() - baseDepth);
                })*/;

            using (Indentation.Indent())
            {
                int padding = entityPropertyies.Max(x => x.Name.Length);
                foreach (var property in entityPropertyies)
                {
                    Prettify(property, baseDepth, padding).Dump();
                }
            }

            Environment.NewLine.Dump();
        }
        public string Prettify(PropertyInfo property, int depth = -1, int padding = 0)
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
                return string.Format("{0} : {1:X8} -> {2:X8}",
                    propertyName, pointer.Address, pointer.GetValue());
            }
            else
            {
                if (depth == -1 || pointer.GetDepth() - depth == 1)
                {
                    return string.Format("[+{0:X3}] {1} : {2:X8} -> " + (hexadecimal ? "{3:X8}" : "{3}"),
                    pointer.GetOffset(), propertyName, pointer.Address, pointer.GetValue());
                }
                {
                    return string.Format("[+{0}] {1} : {2:X8} -> " + (hexadecimal ? "{3:X8}" : "{3}"),
                    string.Join("+", pointer.GetOffsets(pointer.GetDepth() - depth).Select(x => x.ToString("X3"))),
                    propertyName, pointer.Address, pointer.GetValue());
                }
            }
        }
    }
}
