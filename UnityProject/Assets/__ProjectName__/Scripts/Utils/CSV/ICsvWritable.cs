using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit.CSV
{
    public interface ICsvWritable
    {
        string GetCsvHeader();
        string ToCsvRowString();
    }
}
