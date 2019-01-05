using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HexVisualsData
{
    public float startHeight;
    public List<HexVisualsDataItem> items;
}

[Serializable]
public struct HexVisualsDataItem
{
    public Material sidesMaterial;
    public Material topMaterial;
}
