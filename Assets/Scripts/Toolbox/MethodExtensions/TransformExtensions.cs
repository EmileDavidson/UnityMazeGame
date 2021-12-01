using UnityEngine;

namespace Toolbox.MethodExtensions
{
    public static class TransformExtensions
    {
        public static void ResetTransformation(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }
    }
}
