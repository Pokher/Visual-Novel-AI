using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class StorySO : ScriptableObject
{
    public List<string> StoryBlocks = new List<string>();
    public int storyBlockCount;
}
