using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBaker : MonoBehaviour {

    public NavMeshSurface surfaces;

    IEnumerator Start() {
        yield return new WaitForSeconds(5f);
        //surfaces.BuildNavMesh();
    }
}