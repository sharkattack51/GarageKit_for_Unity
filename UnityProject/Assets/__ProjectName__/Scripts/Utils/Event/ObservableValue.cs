using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class ObservableValue<T>
    {
        private T internalValue;
        public T Value
        {
            get
            {
                return internalValue;
            }
            set
            {
                ValueChange(value);
                internalValue = value;
            }
        }

        public Action<T> OnValueChange;

        public ObservableValue()
        {
            internalValue = default(T);
        }

        public ObservableValue(T value)
        {
            this.internalValue = value;
        }

        private void ValueChange(T newValue)
        {
            if(internalValue.Equals(newValue))
                return;
            else
            {
                if(OnValueChange != null)
                    OnValueChange(newValue);
            }
        }
    }
}
