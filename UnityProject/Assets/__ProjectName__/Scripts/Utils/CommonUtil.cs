using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GarageKit
{
    public class CommonUtil
    {
        public static void OpenFolder(string path)
        {
            Process.Start(new ProcessStartInfo() {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
