using System.Collections.Generic;
using UnityEngine;

public class BoneCombiner
{
    public readonly Dictionary<int , Transform> rootBones = new Dictionary<int , Transform>();
    private readonly Transform[] boneTransforms = new Transform[67];
    private readonly Transform rootTransform;

    public BoneCombiner(GameObject rootObject)
    {
        rootTransform = rootObject.transform;
        TraverseHierarchy(rootTransform);
    }

    private void TraverseHierarchy(Transform transform)
    {
        foreach (Transform child in transform)
        {
            rootBones.Add(child.name.GetHashCode(), child);
            TraverseHierarchy(child);
        }
    }

    public Transform AddLimb(GameObject boneObject)
    {
        Transform limb = ProcessBoneObject(boneObject.GetComponentInChildren<SkinnedMeshRenderer>());
        limb.SetParent(rootTransform);
        return limb;
    }

    private Transform ProcessBoneObject(SkinnedMeshRenderer renderer)
    {
        Transform boneObject = new GameObject().transform;
        SkinnedMeshRenderer boneMeshRenderer = boneObject.gameObject.AddComponent<SkinnedMeshRenderer>();
        Transform[] bones = renderer.bones;
        for (int i = 0; i < bones.Length; i++)
        {
            boneTransforms[i] = rootBones[bones[i].name.GetHashCode()];
        }
        boneMeshRenderer.bones = boneTransforms;
        boneMeshRenderer.sharedMesh = renderer.sharedMesh;
        boneMeshRenderer.materials = renderer.sharedMaterials;

        return boneObject;
    }
}
