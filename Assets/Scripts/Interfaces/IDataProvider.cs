using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataProvider
{
    string GetDataLabel();
    Dictionary<string, object> GetData();
}
