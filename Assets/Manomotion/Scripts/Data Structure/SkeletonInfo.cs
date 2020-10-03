using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using System.Runtime.InteropServices;
/// <summary>
/// Contains ....
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct SkeletonInfo
{
    /// <summary>
    /// </summary>
    public float confidence;

    /// <summary>
    /// Position of the joints, to get a specific .....
    /// normalized values between 0..1
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
    public Vector3[] joints;
}
