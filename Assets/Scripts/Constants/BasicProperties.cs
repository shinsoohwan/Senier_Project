using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

class BasicProperties
{
    public static string ResourceDirectory
    {
        get
        {
            return "Assets" + Path.DirectorySeparatorChar + "Resources";
        }
    }

    public static string DatatablesDirectory
    {
        get
        {
            return ResourceDirectory + Path.DirectorySeparatorChar + "Datatables";
        }
    }

    public static bool IsPaused
    {
        get
        {
            return Time.timeScale.Equals(0.0f);
        }
    }
}
