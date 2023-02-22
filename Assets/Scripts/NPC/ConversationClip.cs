using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConversationClip
{
    public Sprite speakerImage;
    public string speakerName;
    [Multiline(5)] public string contents;
    public int nextClipIndex; // -1 -> conservation terminate, -2 -> cook, -3 -> smithy, -4 -> allocate agent
    public int prevClipIndex;

    public int eventIndex;
    public int nextStartIndex;

    public int conservationKind; // 0: next-prev
}
