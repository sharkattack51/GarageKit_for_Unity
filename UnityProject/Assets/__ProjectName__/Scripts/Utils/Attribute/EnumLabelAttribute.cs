using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GarageKit
{
    public static class EnumLabelAttribute
    {
        /*
        usage:
            enum MODE {
                [Label("モードA")]
                A = 0,

                [Label("モードB")]
                B,

                [Label("モードC")]
                C
            }

            string label = MODE.A.LabelAttribute();
            string[] labels = EnumLabelAttribute.LabelAttributes<MODE>();
        */

        public static string LabelAttribute(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());

            if(fieldInfo == null)
                return null;

            LabelAttribute[] atts = fieldInfo.GetCustomAttributes(typeof(LabelAttribute), false) as LabelAttribute[];
            if(atts.Length > 0)
                return atts[0].Label;
            else
                return null;
        }

        public static string[] LabelAttributes<T>() where T : Enum
        {
            T[] sortedValues = ((T[])Enum.GetValues(typeof(T))).OrderBy(v => v).ToArray();

            List<string> labels = new List<string>();
            foreach(T value in sortedValues)
                labels.Add(value.LabelAttribute());

            return labels.ToArray();
        }
    }

    public class LabelAttribute : Attribute
    {
        public string Label { get; protected set; }

        public LabelAttribute(string value)
        {
            this.Label = value;
        }
    }
}
