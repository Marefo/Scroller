using UnityEngine;

namespace _CodeBase.Extensions
{
  public static class Extensions
  {
    public static bool WorldSpaceOverlaps(this RectTransform overlaping, RectTransform overlaped)
    {
      Vector3[] aux = new Vector3[4];
      overlaping.GetWorldCorners(aux);
      Rect overlapingRect = new Rect(aux[0], (aux[2] - aux[0]));
 
      overlaped.GetWorldCorners(aux);
      Rect overlapedRect = new Rect(aux[0], (aux[2] - aux[0]));
 
      return (overlapedRect.Overlaps(overlapingRect, true));
    }
  }
}