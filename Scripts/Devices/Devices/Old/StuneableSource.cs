using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuneableSource : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform source;
    [SerializeField] private Transform stunnedSource;
    [Space]
    [SerializeField] private Transform activePos;
    [SerializeField] private Transform stunnedPos;

    private void Start()
    {
        EndStun();
    }

    public void EndStun()
    {
        source.position = activePos.position;
        stunnedSource.position = stunnedPos.position;
    }

    public void StunSource()
    {
        source.position = stunnedPos.position;
        stunnedSource.position = activePos.position;
    }
}
