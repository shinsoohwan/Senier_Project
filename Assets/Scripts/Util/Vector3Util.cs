using System.Runtime.ConstrainedExecution;
using UnityEngine;

namespace Assets.Scripts.Util
{
    class Vector3Util
    {
        public const float APPROXIMATE_VALUE = 0.00001f;
        static public bool IsAlmostEquals(Vector3 lhs, Vector3 rhs)
        {
            return Mathf.Abs(lhs.x - rhs.x) < APPROXIMATE_VALUE
                   && Mathf.Abs(lhs.y - rhs.y) < APPROXIMATE_VALUE
                   && Mathf.Abs(lhs.z - rhs.z) < APPROXIMATE_VALUE;
        }

        static public bool IsXZAlmostEquals(Vector3 lhs, Vector3 rhs)
        {
            return Mathf.Abs(lhs.x - rhs.x) < APPROXIMATE_VALUE
                   && Mathf.Abs(lhs.z - rhs.z) < APPROXIMATE_VALUE;
        }

        static public Vector3 GridVector(Vector3 vec)
        {
            GridVectorInplace(ref vec);
            return vec;
        }

        static public Vector3 GridVectorInplace(ref Vector3 vec)
        {
            int intVecX = (int)(vec.x / 2);
            int intVecZ = (int)(vec.z / 2);

            if (vec.x < 0)
                intVecX -= 1;
            if (vec.z < 0)
                intVecZ -= 1;

            // -2.5 -> -1.25 -> -1 -> -2 => -3
            // -1.5 -> -0.75 -> 0 -> -1 => -1
            // -0.5 -> -0.25 -> 0 -> -1 => -1
            // 0.5 -> 0.25 -> 0 => 1
            // 1 -> 0.5 -> 0 => 1
            // 1.5 -> 0.75 -> 0 => 1
            // 2.5 -> 1.25 -> 1 => 3
            // 3 -> 1.5 -> 1 => 3
            // 3.5 -> 1.75 -> 1 => 3
            // 4.5 -> 2.25 -> 2 => 5
            vec.x = (1 + (intVecX * 2));
            vec.z = (1 + (intVecZ * 2));

            return vec;
        }
    }
}
