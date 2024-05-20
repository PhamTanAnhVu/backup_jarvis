using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class PropertyMessage
{
    public string PropertyName { get; set; }
    public object Value { get; set; }

    public PropertyMessage(string propertyName, object value)
    {
        PropertyName = propertyName;
        Value = value;
    }
}

